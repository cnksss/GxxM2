using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GXX.M2Server.Engine;

namespace GXX.M2Server.Engine;

/// <summary>ItemEffects.pas TItemEffect record 1:1（批次J57）：五套图库索引（内观/外观/包裹/外观附加/地面特效）。</summary>
public sealed class TItemEffect
{
    public ushort Index;

    public short FileIndex1;   // 内观
    public short FileIndex2;   // 外观
    public short FileIndex3;   // 包裹
    public short FileIndex4;   // 外观附加
    public short FileIndex5;   // 地面特效

    public ushort StartIndex1;
    public ushort StartIndex2;
    public ushort StartIndex3;
    public ushort StartIndex4;
    public ushort StartIndex5;

    public ushort ImageCount1;
    public ushort ImageCount2;
    public ushort ImageCount3;
    public ushort ImageCount4;
    public ushort ImageCount5;

    public ushort Time1;
    public ushort Time2;
    public ushort Time3;
    public ushort Time4;
    public ushort Time5;

    public short OffSetX1;
    public short OffSetY1;
    public short OffSetX2;   // 外观 - 废字段
    public short OffSetY2;   // 外观 - 废字段
    public short OffSetX3;
    public short OffSetY3;
    public short OffSetX5;
    public short OffSetY5;

    public bool NoBlendMode2;      // 外观 - 不透明模式
    public bool NoSex2;            // 外观 - 不分男女
    public bool DrawCenter1;       // 居中对齐 内观
    public bool DrawCenter3;       // 居中对齐 包裹
    public bool DrawCenter5;       // 居中对齐 地面特效
    public bool NoBlendMode1;
    public bool NoBlendMode3;
    public bool NoBlendMode5;
    public bool boEfectBelowItem1; // 物效在物品底层播放 内观
    public bool boEfectBelowItem5; // 物效在物品底层播放 地面特效

    public int AddEffectDrawOrder; // 附加特效绘制顺序
    public bool AddEffectNoBlendMode;
    public bool AddEffectDrawCenter;

    public string EffectDesc = "";
}

/// <summary>
/// ItemEffects.pas TItemEffects 1:1（批次J57）：EffectList.txt 五段图库特效表。
/// LoadFromFile 逐字段 GetValidStr3 取值（缺省 -1/-1/-1、Time 缺省 1、FileIndex5 起缺省 0/1），
/// 仅 Index &gt; 0 收录；Add 以 Index 去重（重复丢弃）。SaveToFile 37 列 TAB 行格式。
/// </summary>
public class TItemEffects
{
    private readonly List<TItemEffect> _list = new();
    private int _recordCount;

    public int Count => _list.Count;
    public int RecordCount => _recordCount;
    public TItemEffect GetItems(int index) => _list[index];
    public TItemEffect? Get(int index)
    {
        foreach (var e in _list)
            if (e.Index == index) return e;
        return null;
    }

    public bool Find(int index) => Get(index) != null;

    /// <summary>Add：Index 重复则拒绝（结果 false，调用方 Dispose）。</summary>
    public bool Add(TItemEffect itemEffect)
    {
        foreach (var e in _list)
            if (e.Index == itemEffect.Index) return false;
        _list.Add(itemEffect);
        _recordCount++;
        return true;
    }

    public bool Delete(TItemEffect itemEffect)
    {
        int i = _list.IndexOf(itemEffect);
        if (i < 0) return false;
        _list.RemoveAt(i);
        _recordCount--;
        return true;
    }

    /// <summary>DeleteIndex：按列表下标（0..Count-1）。</summary>
    public bool DeleteIndex(int index)
    {
        if (index >= 0 && index <= _list.Count - 1)
        {
            _list.RemoveAt(index);
            _recordCount--;
            return true;
        }
        return false;
    }

    public void Clear()
    {
        _list.Clear();
        _recordCount = 0;
    }

    /// <summary>LoadFromFile：EffectList.txt（Envir 目录）。</summary>
    public void LoadFromFile()
    {
        _recordCount = 0;
        _list.Clear();

        string sFileName = Path.Combine(M2Config.sEnvirDir, "EffectList.txt");
        if (!File.Exists(sFileName))
            return;

        // GBK 一律经 GXX.Core.EncodingInit.GBK 获取：其内部先 Ensure() 注册 CodePagesEncodingProvider，消除加载顺序依赖（CP936 实例等价）。
        foreach (var line in File.ReadLines(sFileName, GXX.Core.EncodingInit.GBK))
        {
            string sLineText = line;
            if (sLineText.Length == 0 || sLineText[0] == ';')
                continue;
            string sIndex = "", sFileIndex1 = "", sFileIndex2 = "", sFileIndex3 = "";
            string sOffSet1 = "", sOffSet2 = "", sOffSet3 = "";
            string sImageCount1 = "", sImageCount2 = "", sImageCount3 = "";
            string sTime1 = "", sTime2 = "", sTime3 = "";
            string sOffSetX1 = "", sOffSetY1 = "", sOffSetX2 = "", sOffSetY2 = "", sOffSetX3 = "", sOffSetY3 = "";
            string sNoBlendMode2 = "", sNoSex2 = "";
            string sDrawCenter1 = "", sDrawCenter3 = "";
            string sFileIndex4 = "", sAddEffectDrawOrder = "", sOffset4 = "", sImageCount4 = "", sTime4 = "";
            string sAddEffectNoBlendMode = "", sAddEffectDrawCenter = "";
            string sNoBlendMode1 = "", sNoBlendMode3 = "", sEfectBelowItem1 = "";
            string sFileIndex5 = "", sOffset5 = "", sImageCount5 = "", sTime5 = "";
            string sOffSetX5 = "", sOffSetY5 = "", sDrawCenter5 = "", sNoBlendMode5 = "", sEfectBelowItem5 = "";

            sLineText = TSndaShopList.GetValidStr3(sLineText, ref sIndex, ' ', '\t');
            sLineText = TSndaShopList.GetValidStr3(sLineText, ref sFileIndex1, ' ', '\t');
            sLineText = TSndaShopList.GetValidStr3(sLineText, ref sFileIndex2, ' ', '\t');
            sLineText = TSndaShopList.GetValidStr3(sLineText, ref sFileIndex3, ' ', '\t');

            sLineText = TSndaShopList.GetValidStr3(sLineText, ref sOffSet1, ' ', '\t');
            sLineText = TSndaShopList.GetValidStr3(sLineText, ref sOffSet2, ' ', '\t');
            sLineText = TSndaShopList.GetValidStr3(sLineText, ref sOffSet3, ' ', '\t');

            sLineText = TSndaShopList.GetValidStr3(sLineText, ref sImageCount1, ' ', '\t');
            sLineText = TSndaShopList.GetValidStr3(sLineText, ref sImageCount2, ' ', '\t');
            sLineText = TSndaShopList.GetValidStr3(sLineText, ref sImageCount3, ' ', '\t');

            sLineText = TSndaShopList.GetValidStr3(sLineText, ref sTime1, ' ', '\t');
            sLineText = TSndaShopList.GetValidStr3(sLineText, ref sTime2, ' ', '\t');
            sLineText = TSndaShopList.GetValidStr3(sLineText, ref sTime3, ' ', '\t');

            sLineText = TSndaShopList.GetValidStr3(sLineText, ref sOffSetX1, ' ', '\t');
            sLineText = TSndaShopList.GetValidStr3(sLineText, ref sOffSetY1, ' ', '\t');
            sLineText = TSndaShopList.GetValidStr3(sLineText, ref sOffSetX2, ' ', '\t');
            sLineText = TSndaShopList.GetValidStr3(sLineText, ref sOffSetY2, ' ', '\t');
            sLineText = TSndaShopList.GetValidStr3(sLineText, ref sOffSetX3, ' ', '\t');
            sLineText = TSndaShopList.GetValidStr3(sLineText, ref sOffSetY3, ' ', '\t');

            sLineText = TSndaShopList.GetValidStr3(sLineText, ref sNoBlendMode2, ' ', '\t');
            sLineText = TSndaShopList.GetValidStr3(sLineText, ref sNoSex2, ' ', '\t');

            sLineText = TSndaShopList.GetValidStr3(sLineText, ref sDrawCenter1, ' ', '\t');
            sLineText = TSndaShopList.GetValidStr3(sLineText, ref sDrawCenter3, ' ', '\t');

            sLineText = TSndaShopList.GetValidStr3(sLineText, ref sFileIndex4, ' ', '\t');
            sLineText = TSndaShopList.GetValidStr3(sLineText, ref sAddEffectDrawOrder, ' ', '\t');
            sLineText = TSndaShopList.GetValidStr3(sLineText, ref sOffset4, ' ', '\t');
            sLineText = TSndaShopList.GetValidStr3(sLineText, ref sImageCount4, ' ', '\t');
            sLineText = TSndaShopList.GetValidStr3(sLineText, ref sTime4, ' ', '\t');
            sLineText = TSndaShopList.GetValidStr3(sLineText, ref sAddEffectNoBlendMode, ' ', '\t');
            sLineText = TSndaShopList.GetValidStr3(sLineText, ref sAddEffectDrawCenter, ' ', '\t');

            sLineText = TSndaShopList.GetValidStr3(sLineText, ref sNoBlendMode1, ' ', '\t');
            sLineText = TSndaShopList.GetValidStr3(sLineText, ref sNoBlendMode3, ' ', '\t');

            sLineText = TSndaShopList.GetValidStr3(sLineText, ref sEfectBelowItem1, ' ', '\t');

            sLineText = TSndaShopList.GetValidStr3(sLineText, ref sFileIndex5, ' ', '\t');
            sLineText = TSndaShopList.GetValidStr3(sLineText, ref sOffset5, ' ', '\t');
            sLineText = TSndaShopList.GetValidStr3(sLineText, ref sImageCount5, ' ', '\t');
            sLineText = TSndaShopList.GetValidStr3(sLineText, ref sTime5, ' ', '\t');
            sLineText = TSndaShopList.GetValidStr3(sLineText, ref sOffSetX5, ' ', '\t');
            sLineText = TSndaShopList.GetValidStr3(sLineText, ref sOffSetY5, ' ', '\t');
            sLineText = TSndaShopList.GetValidStr3(sLineText, ref sDrawCenter5, ' ', '\t');
            sLineText = TSndaShopList.GetValidStr3(sLineText, ref sNoBlendMode5, ' ', '\t');
            sLineText = TSndaShopList.GetValidStr3(sLineText, ref sEfectBelowItem5, ' ', '\t');

            int nIndex = TSndaShopList.StrToIntDef(sIndex, -1);
            int FileIndex1 = TSndaShopList.StrToIntDef(sFileIndex1, -1);
            int FileIndex2 = TSndaShopList.StrToIntDef(sFileIndex2, -1);
            int FileIndex3 = TSndaShopList.StrToIntDef(sFileIndex3, -1);

            int OffSet1 = TSndaShopList.StrToIntDef(sOffSet1, -1);
            int OffSet2 = TSndaShopList.StrToIntDef(sOffSet2, -1);
            int OffSet3 = TSndaShopList.StrToIntDef(sOffSet3, -1);

            int ImageCount1 = TSndaShopList.StrToIntDef(sImageCount1, -1);
            int ImageCount2 = TSndaShopList.StrToIntDef(sImageCount2, -1);
            int ImageCount3 = TSndaShopList.StrToIntDef(sImageCount3, -1);

            int Time1 = TSndaShopList.StrToIntDef(sTime1, 1);
            int Time2 = TSndaShopList.StrToIntDef(sTime2, 1);
            int Time3 = TSndaShopList.StrToIntDef(sTime3, 1);

            int OffSetX1 = TSndaShopList.StrToIntDef(sOffSetX1, -1);
            int OffSetY1 = TSndaShopList.StrToIntDef(sOffSetY1, -1);
            int OffSetX2 = TSndaShopList.StrToIntDef(sOffSetX2, -1);
            int OffSetY2 = TSndaShopList.StrToIntDef(sOffSetY2, -1);
            int OffSetX3 = TSndaShopList.StrToIntDef(sOffSetX3, -1);
            int OffSetY3 = TSndaShopList.StrToIntDef(sOffSetY3, -1);

            int FileIndex4 = TSndaShopList.StrToIntDef(sFileIndex4, -1);
            int Offset4 = TSndaShopList.StrToIntDef(sOffset4, -1);
            int ImageCount4 = TSndaShopList.StrToIntDef(sImageCount4, 0);
            int Time4 = TSndaShopList.StrToIntDef(sTime4, 1);

            int FileIndex5 = TSndaShopList.StrToIntDef(sFileIndex5, -1);

            // 修正添加特效物品后，什么都没有指定，重启M2后没有了 2020-03-14 22:27:47
            if (nIndex > 0)
            {
                var itemEffect = new TItemEffect
                {
                    Index = (ushort)nIndex,
                    FileIndex1 = (short)FileIndex1,
                    FileIndex2 = (short)FileIndex2,
                    FileIndex3 = (short)FileIndex3,
                    FileIndex4 = (short)FileIndex4,
                    StartIndex1 = (ushort)OffSet1,
                    StartIndex2 = (ushort)OffSet2,
                    StartIndex3 = (ushort)OffSet3,
                    StartIndex4 = (ushort)Offset4,
                    ImageCount1 = (ushort)ImageCount1,
                    ImageCount2 = (ushort)ImageCount2,
                    ImageCount3 = (ushort)ImageCount3,
                    ImageCount4 = (ushort)ImageCount4,
                    Time1 = (ushort)Time1,
                    Time2 = (ushort)Time2,
                    Time3 = (ushort)Time3,
                    Time4 = (ushort)Time4,
                    OffSetX1 = (short)OffSetX1,
                    OffSetY1 = (short)OffSetY1,
                    OffSetX2 = (short)OffSetX2,
                    OffSetY2 = (short)OffSetY2,
                    OffSetX3 = (short)OffSetX3,
                    OffSetY3 = (short)OffSetY3,
                    NoBlendMode2 = TSndaShopList.StrToIntDef(sNoBlendMode2, 0) != 0,
                    NoSex2 = TSndaShopList.StrToIntDef(sNoSex2, 0) != 0,
                    DrawCenter1 = TSndaShopList.StrToIntDef(sDrawCenter1, 0) != 0,
                    DrawCenter3 = TSndaShopList.StrToIntDef(sDrawCenter3, 0) != 0,
                    NoBlendMode1 = TSndaShopList.StrToIntDef(sNoBlendMode1, 0) != 0,
                    NoBlendMode3 = TSndaShopList.StrToIntDef(sNoBlendMode3, 0) != 0,
                    boEfectBelowItem1 = TSndaShopList.StrToIntDef(sEfectBelowItem1, 0) != 0,
                    boEfectBelowItem5 = TSndaShopList.StrToIntDef(sEfectBelowItem5, 0) != 0,
                    AddEffectDrawOrder = TSndaShopList.StrToIntDef(sAddEffectDrawOrder, 0),
                    AddEffectNoBlendMode = TSndaShopList.StrToIntDef(sAddEffectNoBlendMode, 0) != 0,
                    AddEffectDrawCenter = TSndaShopList.StrToIntDef(sAddEffectDrawCenter, 0) != 0,
                    EffectDesc = sLineText,
                    FileIndex5 = (short)FileIndex5,
                    StartIndex5 = (ushort)TSndaShopList.StrToIntDef(sOffset5, 0),
                    ImageCount5 = (ushort)TSndaShopList.StrToIntDef(sImageCount5, 1),
                    Time5 = (ushort)TSndaShopList.StrToIntDef(sTime5, 1),
                    OffSetX5 = (short)TSndaShopList.StrToIntDef(sOffSetX5, 0),
                    OffSetY5 = (short)TSndaShopList.StrToIntDef(sOffSetY5, 0),
                    DrawCenter5 = TSndaShopList.StrToIntDef(sDrawCenter5, 0) != 0,
                    NoBlendMode5 = TSndaShopList.StrToIntDef(sNoBlendMode5, 0) != 0,
                };
                Add(itemEffect);
            }
        }
    }

    /// <summary>SaveToFile：37 列 TAB 行格式（原字段顺序逐项对应）。</summary>
    public void SaveToFile()
    {
        var saveList = new List<string>();
        foreach (var e in _list)
        {
            var sb = new StringBuilder();
            sb.Append(e.Index).Append('\t').Append(e.FileIndex1).Append('\t').Append(e.FileIndex2).Append('\t')
              .Append(e.FileIndex3).Append('\t').Append(e.StartIndex1).Append('\t').Append(e.StartIndex2).Append('\t')
              .Append(e.StartIndex3).Append('\t').Append(e.ImageCount1).Append('\t').Append(e.ImageCount2).Append('\t')
              .Append(e.ImageCount3).Append('\t').Append(e.Time1).Append('\t').Append(e.Time2).Append('\t')
              .Append(e.Time3).Append('\t').Append(e.OffSetX1).Append('\t').Append(e.OffSetY1).Append('\t')
              .Append(e.OffSetX2).Append('\t').Append(e.OffSetY2).Append('\t').Append(e.OffSetX3).Append('\t')
              .Append(e.OffSetY3).Append('\t').Append(e.NoBlendMode2 ? 1 : 0).Append('\t').Append(e.NoSex2 ? 1 : 0)
              .Append('\t').Append(e.DrawCenter1 ? 1 : 0).Append('\t').Append(e.DrawCenter3 ? 1 : 0).Append('\t')
              .Append(e.FileIndex4).Append('\t').Append(e.AddEffectDrawOrder).Append('\t').Append(e.StartIndex4)
              .Append('\t').Append(e.ImageCount4).Append('\t').Append(e.Time4).Append('\t')
              .Append(e.AddEffectNoBlendMode ? 1 : 0).Append('\t').Append(e.AddEffectDrawCenter ? 1 : 0).Append('\t')
              .Append(e.NoBlendMode1 ? 1 : 0).Append('\t').Append(e.NoBlendMode3 ? 1 : 0).Append('\t')
              .Append(e.boEfectBelowItem1 ? 1 : 0).Append('\t').Append(e.FileIndex5).Append('\t')
              .Append(e.StartIndex5).Append('\t').Append(e.ImageCount5).Append('\t').Append(e.Time5).Append('\t')
              .Append(e.OffSetX5).Append('\t').Append(e.OffSetY5).Append('\t').Append(e.DrawCenter5 ? 1 : 0).Append('\t')
              .Append(e.NoBlendMode5 ? 1 : 0).Append('\t').Append(e.boEfectBelowItem5 ? 1 : 0).Append('\t')
              .Append(e.EffectDesc);
            saveList.Add(sb.ToString());
        }

        string sFileName = Path.Combine(M2Config.sEnvirDir, "EffectList.txt");
        try
        {
            File.WriteAllLines(sFileName, saveList, GXX.Core.EncodingInit.GBK);
        }
        catch
        {
            // Delphi try-except 吞异常
        }
    }

    public IEnumerable<TItemEffect> All()
    {
        foreach (var e in _list) yield return e;
    }
}
