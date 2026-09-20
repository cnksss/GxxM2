// ============================================================================
// 车道 p6-test-isolation：静态全局状态快照/还原器
//
// 设计要点（对应任务书「快照/还原的实现要求」）：
//   * 反射枚举，**不手写成员清单**：覆盖每个受管类型的全部静态字段（Public+NonPublic）
//     与全部「编译器生成的自动属性」（= 有实际存储的静态属性），
//     所以将来往 M2Config 里加字段/属性会自动被纳入。
//   * 逐项复位（含 null 与默认值）：快照存的是「每成员一份值」，还原时逐成员比较、
//     只写不同项，null ↔ 非 null 双向都能复位。
//   * 数组做元素级快照（浅克隆）+ 引用级复位：既能吃掉「改了元素」，也能吃掉「换了实例」；
//     对 static readonly 的数组（引用不可变）同样能吃掉元素级污染。
//   * 幂等且可重入：Capture/Restore 是纯函数 + 不可变快照，重复 Restore 是 no-op。
//   * 异常安全：调用方（执行器）在 finally 里 Restore，测试抛异常也还原。
//
// 明确**不做**的（见报告「未覆盖的部分」）：
//   * 非数组的只读引用成员（static readonly 的 TStringList / Dictionary / TFastIniFile /
//     g_CastleManager 等）只做**引用比对**，不做内容深拷贝：没有通用安全深拷贝，
//     且深拷贝大表会拖垮 7,300 例的运行时间。**数组是唯一例外**（元素级）。
//   * 属性只纳入「编译器生成 getter」的自动属性；手写 getter 的属性可能带副作用
//     （例如 M2ShareState.ConfigIni 会 `??=` 建对象、M2Config.ModuleListPath 会拼路径），
//     读写它们等于主动制造污染，故一律不碰。它们的存储由字段口径（后备字段）覆盖。
// ============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace GXX.M2Server.Tests;

/// <summary>静态全局状态快照/还原器（反射口径，逐成员复位）。</summary>
public static class M2ConfigIsolationState
{
    /// <summary>单个数组成员的一次快照长度上限（防御性：避免把巨型表整份克隆 7,300 次）。</summary>
    private const int MaxArraySnapshotLength = 65536;

    /// <summary>一次静态全局快照（不可变；与内部 Slots 下标一一对应）。</summary>
    public sealed class Snapshot
    {
        internal Snapshot(object[] values) => Values = values;

        internal object[] Values { get; }

        /// <summary>本次快照覆盖的成员数。</summary>
        public int MemberCount => Values.Length;
    }

    private static readonly Slot[] Slots;
    private static readonly string[] CoverageDiagnosticsBacking;

    static M2ConfigIsolationState()
    {
        var slots = new List<Slot>();
        var diags = new List<string>();
        foreach (var type in M2ConfigIsolationCoverage.Types)
            CollectSlots(type, slots, diags);
        Slots = slots.ToArray();
        CoverageDiagnosticsBacking = diags.ToArray();
    }

    /// <summary>被覆盖的静态全局类型（<see cref="M2ConfigIsolationCoverage"/>）。</summary>
    public static IReadOnlyList<Type> CoveredTypes => M2ConfigIsolationCoverage.Types;

    /// <summary>被纳入快照的成员总数。</summary>
    public static int SlotCount => Slots.Length;

    /// <summary>被纳入快照的可赋值成员数（不含只读数组这类「只能回填内容」的成员）。</summary>
    public static int AssignableSlotCount => Slots.Count(s => s.Assignable);

    /// <summary>构建期诊断：未能纳入的成员及原因（正常只应包含两类「明确排除」）。</summary>
    public static IReadOnlyList<string> CoverageDiagnostics => CoverageDiagnosticsBacking;

    /// <summary>快照成员键（"类型.成员"），供覆盖度自检使用。</summary>
    public static IReadOnlyCollection<string> SlotKeys => Slots.Select(s => s.Key).ToArray();

    /// <summary>
    /// 是否是「能做元素级快照」的成员类型：一维数组（任何元素类型）。
    /// 元素是引用类型时元素级 = 元素引用数组的浅拷贝 + 逐元素引用比较，这正好能吃掉
    /// 「往 static readonly 槽位数组里登记了一个对象」这一类污染（如 InterServerState.SrvArray）；
    /// 元素是基元/字符串/结构体时则是真正的值级快照。
    /// 交错数组（int[][]）只回填外层引用，内层内容不在口径内。
    /// </summary>
    internal static bool IsSnapshotArray(Type type) => type.IsArray && type.GetArrayRank() == 1;

    /// <summary>抓取当前静态全局状态。永不抛异常（无法读取的成员被跳过并记入运行期诊断）。</summary>
    public static Snapshot Capture()
    {
        var values = new object[Slots.Length];
        for (var i = 0; i < Slots.Length; i++)
        {
            try
            {
                values[i] = Slots[i].Capture();
            }
            catch (Exception ex)
            {
                values[i] = Skipped;
                NoteDiagnostic($"{Slots[i].Key}: capture failed: {ex.GetType().Name}");
            }
        }

        return new Snapshot(values);
    }

    /// <summary>把静态全局状态复位到快照值；返回实际写回的成员数。逐成员独立容错。</summary>
    public static int Restore(Snapshot snapshot)
    {
        if (snapshot == null) throw new ArgumentNullException(nameof(snapshot));

        var restored = 0;
        var values = snapshot.Values;
        for (var i = 0; i < Slots.Length; i++)
        {
            var slot = Slots[i];
            var captured = values[i];
            if (ReferenceEquals(captured, Skipped)) continue;

            try
            {
                var current = slot.Read();
                if (slot.Same(captured, current)) continue;
                if (slot.Restore(captured, current)) restored++;
            }
            catch (Exception ex)
            {
                NoteDiagnostic($"{slot.Key}: restore failed: {ex.GetType().Name}");
            }
        }

        return restored;
    }

    // ---------------------------------------------------------------- 内部实现

    private static void CollectSlots(Type type, List<Slot> slots, List<string> diags)
    {
        const BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic
                                 | BindingFlags.Static | BindingFlags.DeclaredOnly;

        foreach (var field in type.GetFields(Flags).OrderBy(f => f.Name, StringComparer.Ordinal))
        {
            if (field.IsLiteral)
            {
                diags.Add($"{type.Name}.{field.Name}: skipped (const/literal)");
                continue;
            }

            // 自动属性的后备字段由属性槽接管，避免同一存储被快照两次。
            if (IsAutoPropertyBackingField(field)) continue;

            if (field.IsInitOnly && !IsSnapshotArray(field.FieldType))
            {
                diags.Add($"{type.Name}.{field.Name}: skipped (static readonly, non-array)");
                continue;
            }

            slots.Add(new FieldSlot(type, field, assignable: !field.IsInitOnly));
        }

        foreach (var property in type.GetProperties(Flags).OrderBy(p => p.Name, StringComparer.Ordinal))
        {
            if (property.GetIndexParameters().Length != 0) continue;              // 索引器
            if (property.GetMethod == null) continue;
            if (!property.GetMethod.IsDefined(typeof(CompilerGeneratedAttribute), false))
                continue;                                                          // 手写 getter：可能有副作用，不碰

            var setter = property.SetMethod;
            var assignable = setter != null && !setter.ReturnParameter
                .GetRequiredCustomModifiers()
                .Any(m => m.FullName == "System.Runtime.CompilerServices.IsExternalInit");

            if (!assignable && !IsSnapshotArray(property.PropertyType)) continue;

            slots.Add(new PropertySlot(type, property, assignable));
        }
    }

    private static bool IsAutoPropertyBackingField(FieldInfo field)
        => field.IsDefined(typeof(CompilerGeneratedAttribute), false)
           && field.Name.EndsWith("k__BackingField", StringComparison.Ordinal);

    private static readonly HashSet<string> ReportedDiagnostics = new(StringComparer.Ordinal);

    private static void NoteDiagnostic(string message)
    {
        lock (ReportedDiagnostics)
        {
            if (ReportedDiagnostics.Count > 64) return;
            ReportedDiagnostics.Add(message);
        }
    }

    /// <summary>运行期诊断（快照/还原过程中出现的异常，最多保留 64 条）。</summary>
    public static IReadOnlyCollection<string> RuntimeDiagnostics
    {
        get { lock (ReportedDiagnostics) return ReportedDiagnostics.ToArray(); }
    }

    private static readonly object Skipped = new();

    private abstract class Slot
    {
        protected Slot(Type owner, string name, Type memberType, bool assignable, bool arraySlot)
        {
            Owner = owner;
            Name = name;
            MemberType = memberType;
            Assignable = assignable;
            ArraySlot = arraySlot;
        }

        protected Type Owner { get; }

        protected string Name { get; }

        protected Type MemberType { get; }

        /// <summary>成员是否可赋值（static readonly / 只读属性为 false）。</summary>
        public bool Assignable { get; }

        /// <summary>是否走元素级数组快照。</summary>
        protected bool ArraySlot { get; }

        public string Key => Owner.Name + "." + Name;

        public abstract object Read();

        public abstract object Capture();

        public abstract bool Same(object captured, object current);

        /// <summary>把 captured 复位到当前值；返回 true 表示确实写回了。</summary>
        public abstract bool Restore(object captured, object current);

        protected bool SameCore(object captured, object current)
            => ArraySlot && captured is ArraySnapshot snap
                ? snap.ValueEquals(current)
                : Equals(captured, current);

        protected object CaptureCore()
        {
            var value = Read();
            return ArraySlot && value is Array array ? new ArraySnapshot(array, MaxArraySnapshotLength) : value;
        }

        protected bool RestoreCore(object captured, object current)
        {
            if (ArraySlot && captured is ArraySnapshot snap)
                return snap.RestoreInto(current, WriteValue, Assignable);
            if (!Assignable) return false;
            WriteValue(captured);
            return true;
        }

        protected abstract void WriteValue(object value);
    }

    private sealed class FieldSlot : Slot
    {
        private readonly FieldInfo _field;

        public FieldSlot(Type owner, FieldInfo field, bool assignable)
            : base(owner, field.Name, field.FieldType, assignable, IsSnapshotArray(field.FieldType))
        {
            _field = field;
        }

        public override object Read() => _field.GetValue(null);

        public override object Capture() => CaptureCore();

        public override bool Same(object captured, object current) => SameCore(captured, current);

        public override bool Restore(object captured, object current) => RestoreCore(captured, current);

        protected override void WriteValue(object value) => _field.SetValue(null, value);
    }

    private sealed class PropertySlot : Slot
    {
        private readonly PropertyInfo _property;

        public PropertySlot(Type owner, PropertyInfo property, bool assignable)
            : base(owner, property.Name, property.PropertyType, assignable, IsSnapshotArray(property.PropertyType))
        {
            _property = property;
        }

        public override object Read() => _property.GetValue(null);

        public override object Capture() => CaptureCore();

        public override bool Same(object captured, object current) => SameCore(captured, current);

        public override bool Restore(object captured, object current) => RestoreCore(captured, current);

        protected override void WriteValue(object value) => _property.SetValue(null, value);
    }

    /// <summary>一维数组的元素级快照：保留原实例引用 + 一份内容浅克隆。</summary>
    private sealed class ArraySnapshot
    {
        private static readonly Dictionary<Type, Func<Array, Array, bool>> ElementComparers = new();

        private readonly Array _original;
        private readonly Array _copy;

        public ArraySnapshot(Array original, int maxLength)
        {
            _original = original;
            _copy = original.Length <= maxLength ? (Array)original.Clone() : original;
            Truncated = original.Length > maxLength;
        }

        public bool Truncated { get; }

        public bool ValueEquals(object current)
        {
            if (Truncated) return ReferenceEquals(_original, current);
            if (current is not Array other) return false;
            if (other.GetType() != _copy.GetType() || other.Length != _copy.Length) return false;
            return GetElementComparer(_copy.GetType().GetElementType()!)(_copy, other);
        }

        /// <summary>用泛型实例化做逐元素比较（无装箱；纯反射 GetValue 在 1001 长表上要慢约 50 倍）。</summary>
        private static Func<Array, Array, bool> GetElementComparer(Type elementType)
        {
            lock (ElementComparers)
            {
                if (ElementComparers.TryGetValue(elementType, out var cached)) return cached;
                var method = typeof(ArraySnapshot)
                    .GetMethod(nameof(GenericEquals), BindingFlags.NonPublic | BindingFlags.Static)!
                    .MakeGenericMethod(elementType);
                var comparer = (Func<Array, Array, bool>)method.CreateDelegate(typeof(Func<Array, Array, bool>));
                ElementComparers[elementType] = comparer;
                return comparer;
            }
        }

        private static bool GenericEquals<T>(Array left, Array right)
        {
            var a = (T[])left;
            var b = (T[])right;
            var comparer = EqualityComparer<T>.Default;
            for (var i = 0; i < a.Length; i++)
                if (!comparer.Equals(a[i], b[i]))
                    return false;
            return true;
        }

        /// <summary>
        /// 优先原实例原地回填（保持引用恒等）；引用被换过时先把原实例内容修回再整实例赋值。
        /// </summary>
        public bool RestoreInto(object current, Action<object> writeValue, bool assignable)
        {
            if (Truncated)
            {
                if (ReferenceEquals(current, _original)) return false;
                if (!assignable) return false;
                writeValue(_original);
                return true;
            }

            // 原实例可能既被改了元素、又被整个换掉过，所以先把内容修回去。
            Array.Copy(_copy, _original, _copy.Length);

            if (ReferenceEquals(current, _original)) return true;

            if (assignable)
            {
                writeValue(_original);
                return true;
            }

            // static readonly 且引用被换过（理论不可达）：只能尽力回填内容。
            if (current is Array other && other.GetType() == _copy.GetType() && other.Length == _copy.Length)
            {
                Array.Copy(_copy, other, _copy.Length);
                return true;
            }

            return false;
        }
    }
}
