using System;
using System.Collections.Generic;
using System.Threading;
using GXX.Core.Rtl;
using GXX.Core.Util;

namespace GXX.Core.Protocol;

/// <summary>SDK.pas 1:1 转换：线程安全列表 + 值列表 + 插件回调委托。</summary>
public static class SDK
{
    public const int MAXPULGCOUNT = 32;

    // ---- 插件回调（原 stdcall 函数指针 → 托管委托）----
    public delegate void TMsgProc(string msg, int nMsgLen, int nMode);
    public delegate IntPtr TFindProc(string procName, int nNameLen);
    public delegate bool TSetProc(IntPtr procAddr, string procName, int nNameLen);
    public delegate object TFindObj(string objName, int nNameLen);
    public delegate bool TStartPlug();
    public delegate bool TSetStartPlug(TStartPlug startPlug);
    public delegate IntPtr TPlugInit(IntPtr appHandle, TMsgProc msgProc, TFindProc findProc, TSetProc setProc, TFindObj findObj);
    public delegate void TStartProc();
    public delegate bool TStartRegister(string sRegisterInfo);
    public delegate bool TGameDataLog(string procName, int nNameLen);
    public delegate int TGetProcInt();

    public struct TPlugFile
    {
        public string FileName;
        public string EntryPoint;
    }

    public struct TProcArrayInfo
    {
        public string ProcName;
        public IntPtr ProcAddr;
    }

    public struct TObjectArrayInfo
    {
        public string ObjName;
        public object ObjAddr;
    }

    public struct TPlugBuffer
    {
        public IntPtr AppHandle;
        public uint ServerGetHandle;
        public uint ServerGetCrc;
        public uint PlugGetHandle;
        public uint PlugGetCrc;
    }

    /// <summary>TGList：线程安全对象列表（Lock/UnLock/TryLock/Up/Down）。</summary>
    public class TGList : IDisposable
    {
        protected readonly object Critical = new();
        protected readonly List<object> Items = new();

        public int Count
        {
            get { lock (Critical) return Items.Count; }
        }

        public object this[int index]
        {
            get { lock (Critical) return Items[index]; }
        }

        public void Add(object item) { lock (Critical) Items.Add(item); }
        public void Remove(object item) { lock (Critical) Items.Remove(item); }
        public void RemoveAt(int index) { lock (Critical) Items.RemoveAt(index); }
        public void Insert(int index, object item) { lock (Critical) Items.Insert(index, item); }
        public void Clear() { lock (Critical) Items.Clear(); }
        public int IndexOf(object item) { lock (Critical) return Items.IndexOf(item); }

        public bool TryLock() => Monitor.TryEnter(Critical);
        public void Lock() => Monitor.Enter(Critical);
        public void UnLock() => Monitor.Exit(Critical);

        public void Up(object item)
        {
            lock (Critical)
            {
                int i = Items.IndexOf(item);
                if (i > 0) { Items.RemoveAt(i); Items.Insert(i - 1, item); }
            }
        }

        public void Down(object item)
        {
            lock (Critical)
            {
                int i = Items.IndexOf(item);
                if (i >= 0 && i < Items.Count - 1) { Items.RemoveAt(i); Items.Insert(i + 1, item); }
            }
        }

        public void Dispose() { }
    }

    /// <summary>TGStringList：线程安全字符串列表。</summary>
    public class TGStringList : IDisposable
    {
        private readonly TStringList _list = new();
        private readonly object _critical = new();

        public int Count
        {
            get { lock (_critical) return _list.Count; }
        }

        public string this[int index]
        {
            get { lock (_critical) return _list[index]; }
        }

        public object GetObject(int index)
        {
            lock (_critical) return _list.GetObject(index);
        }

        public void PutObject(int index, object obj)
        {
            lock (_critical) _list.PutObject(index, obj);
        }

        public int Add(string s) { lock (_critical) return _list.Add(s); }
        public int AddObject(string s, object obj) { lock (_critical) return _list.AddObject(s, obj); }
        public void Delete(int index) { lock (_critical) _list.Delete(index); }
        public void Clear() { lock (_critical) _list.Clear(); }
        public int IndexOf(string s) { lock (_critical) return _list.IndexOf(s); }

        public bool TryLock() => Monitor.TryEnter(_critical);
        public void Lock() => Monitor.Enter(_critical);
        public void UnLock() => Monitor.Exit(_critical);

        public void Up(object obj) { lock (_critical) UpDown(_list, obj, -1); }
        public void Down(object obj) { lock (_critical) UpDown(_list, obj, 1); }

        private static void UpDown(TStringList list, object obj, int dir)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (ReferenceEquals(list.GetObject(i), obj))
                {
                    int j = i + dir;
                    if (j >= 0 && j < list.Count)
                    {
                        string s1 = list[i]; object o1 = list.GetObject(i);
                        list[i] = list[j]; list.PutObject(i, list.GetObject(j));
                        list[j] = s1; list.PutObject(j, o1);
                    }
                    break;
                }
            }
        }

        public void Dispose() { }
    }

    /// <summary>TSortStringList：数字/时间/字符串排序。</summary>
    public class TSortStringList : TStringList
    {
        public void NumberSort(bool ascending)
        {
            Sort();
            if (ascending) Reverse();
        }

        public void DateTimeSort(bool ascending) => NumberSort(ascending);
        public void StringSort(bool ascending) => NumberSort(ascending);

        private void Reverse()
        {
            // 保 Objects 同步的逆序
            var list = new List<(string, object)>();
            for (int i = Count - 1; i >= 0; i--)
                list.Add((this[i], GetObject(i)));
            Clear();
            foreach (var (s, o) in list)
                AddObject(s, o);
        }
    }

    /// <summary>TValueList：Name=Value 键值列表。</summary>
    public class TValueList
    {
        private readonly List<(string Name, string Value, object Obj)> _items = new();

        public int Count => _items.Count;
        public bool Sorted { get; set; }
        public bool CaseSensitive { get; set; }

        public string GetName(int index) => _items[index].Name;
        public string GetValue(int index) => _items[index].Value;

        public int Add(string name, string value)
        {
            _items.Add((name, value, null));
            return _items.Count - 1;
        }

        public int AddObject(string name, string value, object obj)
        {
            _items.Add((name, value, obj));
            return _items.Count - 1;
        }

        public bool AddRecord(string name, string s)
        {
            if (GetIndex(name) >= 0) return false;
            Add(name, s);
            return true;
        }

        public bool AddRecord(string name, int value) => AddRecord(name, value.ToString());
        public bool AddRecord(string name, string s, int value) => AddRecord(name, s + "=" + value);

        public int GetIndex(string name)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (CaseSensitive
                    ? string.Equals(_items[i].Name, name, StringComparison.Ordinal)
                    : string.Equals(_items[i].Name, name, StringComparison.OrdinalIgnoreCase))
                    return i;
            }
            return -1;
        }

        public void Clear() => _items.Clear();
        public void Delete(int index) => _items.RemoveAt(index);
    }

    // ---- 排序辅助函数 ----

    public static int NumberSort_1(string a, string b)
    {
        int va = DelphiRTL.StrToIntDef(a, 0), vb = DelphiRTL.StrToIntDef(b, 0);
        return va > vb ? -1 : va < vb ? 1 : 0;
    }

    public static int NumberSort_2(string a, string b)
    {
        int va = DelphiRTL.StrToIntDef(a, 0), vb = DelphiRTL.StrToIntDef(b, 0);
        return va > vb ? 1 : va < vb ? -1 : 0;
    }

    public static int ObjIntegerSort_1(int a, int b) => a > b ? -1 : a < b ? 1 : 0;
    public static int ObjIntegerSort_2(int a, int b) => a > b ? 1 : a < b ? -1 : 0;
    public static int ObjLongWordSort_1(uint a, uint b) => a > b ? -1 : a < b ? 1 : 0;
    public static int ObjLongWordSort_2(uint a, uint b) => a > b ? 1 : a < b ? -1 : 0;
}
