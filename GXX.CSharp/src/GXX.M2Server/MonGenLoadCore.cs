using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// `TFrmDB.LoadMonGen` **文件加载决策层** 与 `TUserEngine.DoInitSortMapMonGenList` /
/// `FindFirstSortMapMonGenListIndex` 1:1 移植（批次J109）。
/// 主源：LocalDB.pas 3359-3527（`LoadMonGen` 外层 + 嵌套 `LoadMapGen` + `AddEmptyMonGenInfo`）、
/// UsrEngn.pas 11179-11259（排序与二分查找）。
///
/// J108 移植了 `MonGen.txt` 的**逐行解析**，本批次补上**文本从哪来**与**解析后如何排序**：
/// ① `loadgen` 指令把二级文件的行**插入**主列表（3523）；
/// ② 三级兜底 `AddEmptyMonGenInfo`（3428-3450，原文注释"加这个是为了让系统其他地方
///    配置的怪物有地方可以挂靠(火龙守护，沙巴克城墙等)"）；
/// ③ 解析完成后 `DoInitSortMapMonGenList` 按**地图名**排序（3698）。
///
/// **`loadgen` 的插入是"就地展开"而非"追加"**（3517-3525）：
/// 主循环遇到 `loadgen X.txt` 时，先 `GetValidStr3` 取文件名（**取自返回值**）、
/// **随即 `LoadList.Delete(I)` 删掉该行**，然后 `LoadMapGen` 把二级文件的行 `Add` 到**主列表末尾**。
/// 由于 3526 的 `Inc(I)` 在删除后仍执行，**下一个元素落到当前下标 `I`**——即被访问的是
/// "原 `I+1` 位置的行"。
///
/// **关键结论：`loadgen` 不会递归展开**。因二级行**追加在末尾**，而 `Delete(I)` + `Inc(I)`
/// 使游标越过它们；当主列表原内容处理完时 `I >= Count` 即 Break，
/// 故**末尾的二级行不会被再次扫描**。若二级文件里含 `loadgen`，
/// 该行会被当作**普通数据行**交给 J108 的解析器（多半因字段不合格被丢弃），
/// **而不是**再次触发加载。已用 `ExpandDoesNotRecurseIntoAppendedLines` 固化这一反直觉点——
/// 直觉上"展开后继续扫描"会得到递归，实际不会。
///
/// **三处文件来源的优先级（3467-3509 与 3372-3424 同构）**：
/// ① 磁盘文件存在 → 读盘（`LoadList.LoadFromFile` + `DeCodeStringList`）；
/// ② 磁盘文件不存在**且**插件管理器存在且 `IsCheckLoadScriptFileHook` → 走插件钩子
///    `HookLoadScriptFile('MonGen\文件名', MemoryStream)`；
/// ③ 钩子返回 false 或**未启用钩子** → 三级兜底。
/// **但主文件与二级文件在"钩子返回 false"时的后果不同，这是本批次最容易写错的一点**：
/// - **主文件**（3478-3493）：钩子返回 false → 3492 **`AddEmptyMonGenInfo`** 兜底；
/// - **二级文件**（3395-3399）：钩子返回 false → 3398 仅 `Exit`，**不兜底**。
/// 另有 `AddEmptyMonGenInfo` 被调用的另外两处（均在主文件路径）：钩子抛异常（3500）、
/// 钩子未启用（3507）。二级文件在钩子抛异常时（3415-3421）同样只 `Exit`。
///
/// **`AddEmptyMonGenInfo`（3428-3450）创建一个"空挂靠点"**：
/// `sMapName`/`sMonName` 均为空串、`CertList` 已建、`Envir = nil`，
/// 并直接 `Add` 进 `m_MonGenList`。原文 3426 的注释说明其用途——
/// 供火龙守护、沙巴克城墙等**由系统其它配置产生**的怪物挂靠。
/// 它在三处被调用：主文件不存在且无钩子（3507）、钩子抛异常（3500）、钩子返回空流（3492）。
///
/// **`DoInitSortMapMonGenList`（11179-11227）是"就地快速排序"**：
/// `m_SortMapMonGenList.Assign(m_MonGenList)` 先整体拷贝，再对 `Count > 0` 时执行 `QuickSort(0, Count-1)`。
/// 比较函数是 `AnsiCompareText(sMapName, sMapName)`——**大小写不敏感的字符串比较**。
/// 排序算法是经典 Hoare 分区**非递归尾递归消除**写法：外层 `repeat`、内层 `repeat`，
/// **`P` 会随交换更新**（11207-11210，即"枢轴跟随"技巧），
/// 故 `P` 处的元素始终是原枢轴值——这避免了枢轴被交换走后比较基准错位。
///
/// **`FindFirstSortMapMonGenListIndex`（11229-11259）找"第一个"匹配下标**：
/// 标准二分，但**命中后继续向左收缩**（`H := I - 1`）而非立即返回，
/// 结束时 `L` 即第一个匹配位置。**注意 `IsFound` 与 `Result` 分离**——
/// 未命中返回 **-1**，命中返回 `L`。
/// **`AnsiCompareText` 在本移植中用 `string.Compare(a, b, StringComparison.OrdinalIgnoreCase)`
/// 近似**；原文为 ANSI 代码页比较，对中文地图名等价，对纯 ASCII 亦等价。
/// </summary>
public static class MonGenLoadCore
{
    /// <summary>3395/3517：二级文件路径前缀（3372 的 `g_Config.sEnvirDir + 'MonGen\'`）。</summary>
    public const string MonGenSubDir = "MonGen\\";

    /// <summary>3467：主文件名。</summary>
    public const string MainFileName = "MonGen.txt";

    /// <summary>3395：插件钩子收到的路径形如 `MonGen\文件名`。</summary>
    public static string HookPath(string fileName) => MonGenSubDir + fileName;

    /// <summary>`AnsiCompareText` 近似：大小写不敏感比较。</summary>
    public static int AnsiCompareText(string a, string b)
        => string.Compare(a, b, StringComparison.OrdinalIgnoreCase);

    /// <summary>文本来源的三种情形（3368-3424 / 3468-3509 同构）。</summary>
    public enum TextSource
    {
        /// <summary>磁盘文件存在 → 直接读盘。</summary>
        DiskFile,

        /// <summary>磁盘无文件但钩子启用 → 由插件提供（返回 false / 抛异常 / 成功但空流各不相同）。</summary>
        PluginHook,

        /// <summary>磁盘无文件且钩子未启用 → 无内容（主文件走兜底；二级文件不处理）。</summary>
        NoSource,
    }

    /// <summary>
    /// 3378-3424 / 3468-3474：决定文本来源。
    /// **优先级：磁盘 &gt; 钩子**；钩子未启用时 `NoSource`。
    /// </summary>
    public static TextSource SelectTextSource(bool fileExists, bool pluginManagerPresent, bool hookEnabled)
    {
        if (fileExists)
            return TextSource.DiskFile;

        if (pluginManagerPresent && hookEnabled)
            return TextSource.PluginHook;

        return TextSource.NoSource;
    }

    /// <summary>
    /// 3490-3509：主文件路径下的**兜底触发**判定。
    /// **主文件与二级文件在此处行为不同，不可统一**：
    /// - **主文件（3478-3493）**：钩子返回 false → 3492 **`AddEmptyMonGenInfo`** 兜底；
    /// - **二级文件（3395-3399）**：钩子返回 false → 3398 仅 `Exit`，**不兜底**。
    /// 另有 3492（流为空**不**再兜底——见下）、3500（钩子抛异常 → 兜底并 `Exit`）、
    /// 3507（钩子未启用 → 兜底并 `Exit`）。
    /// **注意 3481-3488：钩子返回 true 但流为空时不走 else**，此时 `LoadList` 已被
    /// 3480 创建为非 nil（只是内容为空），故 3510 的 `if LoadList = nil` 不成立、
    /// **继续解析一个空列表**——即"钩子成功但无内容"与"钩子拒绝"后果不同。
    /// </summary>
    public static bool ShouldAddEmptyMonGenInfo(TextSource source, bool hookReturnedFalse, bool hookThrew, bool streamEmpty)
    {
        if (source == TextSource.DiskFile)
            return false;

        // 3505-3508：钩子未启用（NoSource）→ 兜底并 Exit
        if (source == TextSource.NoSource)
            return true;

        // 3490-3492：钩子返回 false → **兜底**（主文件特有；二级文件此处只 Exit）
        if (hookReturnedFalse)
            return true;

        // 3495-3501：钩子抛异常 → 兜底并 Exit
        if (hookThrew)
            return true;

        // 3481-3488：钩子成功但流为空 → LoadList 非 nil，**不兜底**，继续解析空列表
        return false;
    }

    /// <summary>
    /// 3395-3399：**二级文件**在钩子返回 false 时仅 `Exit`，**不兜底**。
    /// 返回 `true` 表示"应放弃且不兜底"。
    /// </summary>
    public static bool SubFileExitsOnHookFalse(bool hookReturnedFalse) => hookReturnedFalse;

    /// <summary>
    /// 3510：`LoadList = nil` 时**整体放弃**（`Exit`），不继续解析。
    /// 这覆盖了"钩子返回 false"与"主文件不存在且无钩子但已兜底"之外的所有空情形。
    /// </summary>
    public static bool ShouldAbortLoad(bool loadListIsNull) => loadListIsNull;

    /// <summary>
    /// 3517-3526：`loadgen` 行处理——取文件名后**删除该行**。
    /// 返回 `(fileName, shouldLoad)`；`shouldLoad` 为假表示文件名为空（3521）。
    /// **注意 3519 的赋值方向**：`sMapGenFile := GetValidStr3(line, sLineText, ...)`——
    /// **文件名取自 `GetValidStr3` 的返回值（即"剩余串"）**，而 `sLineText` 拿到的是
    /// 第一个 token（字面量 `loadgen`）。故文件名**可以含空格**（直到行尾），
    /// 而非"第二个 token"。这一处极易写反。
    /// </summary>
    public static (string FileName, bool ShouldLoad) ParseLoadGenLine(string line)
    {
        string rest = MonGenParseCore.GetValidStr3(line, out string _firstToken, ' ', '\t');
        return (rest, rest != "");
    }

    /// <summary>
    /// 3513-3527 的整体语义：把 `loadgen` 行替换为其文件内容（追加到末尾）。
    /// 本方法模拟主列表的**就地展开**结果，返回展开后的完整行序列。
    /// `readFile` 为读取二级文件（返回 null 表示文件不存在且钩子也拿不到）。
    /// </summary>
    public static List<string> ExpandLoadGen(IReadOnlyList<string> mainLines, Func<string, IReadOnlyList<string>?> readFile)
    {
        var list = new List<string>(mainLines);
        int i = 0;

        while (true)
        {
            if (i >= list.Count)
                break;

            if (MonGenParseCore.IsLoadGenLine(list[i]))
            {
                var (fileName, shouldLoad) = ParseLoadGenLine(list[i]);

                // 3520：先删该行
                list.RemoveAt(i);

                if (shouldLoad)
                {
                    // 3523：LoadMapGen 把二级行**追加到末尾**
                    var extra = readFile(fileName);
                    if (extra is not null)
                        list.AddRange(extra);
                }
            }

            // 3526：删除后仍执行 Inc(I)
            i++;
        }

        return list;
    }

    // ===================== DoInitSortMapMonGenList（11179-11227） =====================

    /// <summary>
    /// 11221-11227：`Assign` 后按地图名排序。
    /// 使用与原文同构的**就地 QuickSort**（11200-11218），含枢轴跟随技巧。
    /// </summary>
    public static void SortByMapName<T>(IList<T> list, Func<T, string> getMapName)
    {
        if (list.Count <= 0)
            return;

        QuickSort(list, 0, list.Count - 1, getMapName);
    }

    /// <summary>11191-11219：`QuickSort` 1:1（含 11207-11210 的枢轴跟随）。</summary>
    private static void QuickSort<T>(IList<T> list, int l, int r, Func<T, string> getMapName)
    {
        // 11195：repeat … until I >= R
        while (true)
        {
            int i = l;
            int j = r;
            int p = (l + r) >> 1;   // 11198：shr 1

            // 11199：内层 repeat … until I > J
            do
            {
                while (AnsiCompareText(getMapName(list[i]), getMapName(list[p])) < 0)
                    i++;

                while (AnsiCompareText(getMapName(list[j]), getMapName(list[p])) > 0)
                    j--;

                if (i <= j)
                {
                    (list[i], list[j]) = (list[j], list[i]);

                    // 11207-11210：枢轴跟随
                    if (p == i)
                        p = j;
                    else if (p == j)
                        p = i;

                    i++;
                    j--;
                }
            } while (i <= j);

            // 11215-11217
            if (l < j)
                QuickSort(list, l, j, getMapName);

            l = i;

            if (i >= r)
                break;
        }
    }

    // ===================== FindFirstSortMapMonGenListIndex（11229-11259） =====================

    /// <summary>
    /// 11229-11259：二分查找**第一个** `sMapName` 匹配项。
    /// 命中后继续向左收缩（`H := I - 1`），返回 `L`；未命中返回 **-1**。
    /// 要求列表已按 `AnsiCompareText` 升序排列。
    /// </summary>
    public static int FindFirstSortMapMonGenListIndex<T>(IList<T> sortedList, Func<T, string> getMapName, string mapName)
    {
        bool isFound = false;
        int result = -1;
        int l = 0;
        int h = sortedList.Count - 1;

        while (l <= h)
        {
            int i = (l + h) >> 1;
            int c = AnsiCompareText(getMapName(sortedList[i]), mapName);

            if (c < 0)
            {
                l = i + 1;
            }
            else
            {
                h = i - 1;

                if (c == 0)
                    isFound = true;
            }
        }

        if (isFound)
            result = l;

        return result;
    }

    /// <summary>
    /// 本移植的便捷包装：按地图名查找第一个匹配下标（内部按需排序）。
    /// </summary>
    public static int FindFirstByMapName<T>(IList<T> sortedList, Func<T, string> getMapName, string mapName)
        => FindFirstSortMapMonGenListIndex(sortedList, getMapName, mapName);
}
