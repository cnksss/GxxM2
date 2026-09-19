using System;
using System.Collections.Generic;
using System.IO;
using GXX.Core;
using GXX.Core.IO;
using GXX.Core.Protocol;
using GXX.Core.Rtl;
using GXX.Core.Util;

namespace GXX.M2Server.Engine;

/// <summary>TMapHeader（packed，52 字节）。</summary>
[Serializable]
[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1)]
public unsafe struct TMapHeader
{
    public ushort wWidth;
    public ushort wHeight;
    public byte sTitle0;   // string[15]：长度字节 + 15 字节
    public fixed byte sTitle[15];
    public double UpdateDate;
    public byte btVersion;
    public byte Reserved0;
    public fixed byte Reserved[22];
}

/// <summary>TENMapHeader（'Map 2010 Ver 1.0' 加密地图）。</summary>
[Serializable]
[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1)]
public unsafe struct TENMapHeader
{
    public fixed byte Title[17];   // string[16]
    public uint Reserved;
    public ushort Width;
    public ushort Not1;
    public ushort Height;
    public ushort Not2;
    public fixed byte Reserved2[25];
}

/// <summary>TMapUnitInfo（12 字节/格）。</summary>
[Serializable]
public struct TMapUnitInfo
{
    public ushort wBkImg;      // $8000 = 禁止移动区域
    public ushort wMidImg;
    public ushort wFrImg;      // $8000 = 禁止移动
    public byte btDoorIndex;   // $80 | 门号
    public byte btDoorOffset;
    public byte btAniFrame;
    public byte btAniTick;
    public byte btArea;
    public byte btLight;
}

/// <summary>格子对象槽（chFlag: 位含义 1=不可走 2=不可站）。</summary>
public class TMapCellinfo
{
    public byte chFlag;
    public List<object> ObjList = new();

    public bool CanWalk() => (chFlag & 1) == 0;
    public bool CanHold() => (chFlag & 2) == 0;
}

/// <summary>TDoorInfo：地图门。</summary>
public class TDoorInfo
{
    public byte n08;            // 门号
    public int nMapX;
    public int nMapY;
    public bool boOpened;
    public uint dwOpenTick;
    public int nRefCount = 1;
}

/// <summary>
/// Envir.pas TEnvirnoment 1:1 核心转换：地图加载（经典/EN 加密/归来格式）+ 格子管理 + 门窗。
/// </summary>
public class TEnvirnoment
{
    public const ushort XORWORD = 0xAA38;
    public const string NEWMAPTITLE = "Map 2010 Ver 1.0";

    public string sMapName = "";
    public string sMapDesc = "";
    public int nWidth;
    public int nHeight;
    public int nServerIndex;
    public bool boMainMap;

    public TMapCellinfo[,] MapCellArray = new TMapCellinfo[0, 0];
    public TMapUnitInfo[,] MapData = new TMapUnitInfo[0, 0];
    public List<TDoorInfo> DoorList = new();
    public List<string> QuestNpcList = new();

    // ---- 地图加载 ----

    public bool LoadMapData(string mapFile)
    {
        using var stream = new TMapStream();
        if (!stream.LoadFromFile(mapFile)) return false;

        // 先按 TENMapHeader 读取（对应原 FileRead(nHandle, ENMapHeader, ...)）
        byte[] headBuf = new byte[64];
        stream.Position = 0;
        int got = stream.Read(headBuf, 0, headBuf.Length);
        if (got < 54) return false;

        string enTitle = ReadShortString(headBuf, 0, 16);
        bool boENMap = enTitle == NEWMAPTITLE;

        ushort wWidth, wHeight;
        long dataOffset;
        if (boENMap)
        {
            var en = StructBytes.FromBytes<TENMapHeader>(headBuf);
            wWidth = (ushort)(en.Width ^ XORWORD);
            wHeight = (ushort)(en.Height ^ XORWORD);
            dataOffset = StructBytes.SizeOf<TENMapHeader>();
        }
        else
        {
            var header = StructBytes.FromBytes<TMapHeader>(headBuf);
            wWidth = header.wWidth;
            wHeight = header.wHeight;
            dataOffset = StructBytes.SizeOf<TMapHeader>();
        }

        nWidth = wWidth;
        nHeight = wHeight;
        MapCellArray = new TMapCellinfo[nWidth, nHeight];
        MapData = new TMapUnitInfo[nWidth, nHeight];
        for (int x = 0; x < nWidth; x++)
            for (int y = 0; y < nHeight; y++)
                MapCellArray[x, y] = new TMapCellinfo();

        byte[] mapBuf = new byte[nWidth * nHeight * 12];
        stream.Position = (int)dataOffset;
        int read = stream.Read(mapBuf, 0, mapBuf.Length);
        if (read < mapBuf.Length) return boENMap ? true : false; // 允许尾部截断

        // 列主序：n24 = nW * nHeight + nH（与原实现一致）
        for (int nW = 0; nW < nWidth; nW++)
        {
            int n24 = nW * nHeight;
            for (int nH = 0; nH < nHeight; nH++)
            {
                int off = (n24 + nH) * 12;
                var unit = new TMapUnitInfo
                {
                    wBkImg = BitConverter.ToUInt16(mapBuf, off),
                    wMidImg = BitConverter.ToUInt16(mapBuf, off + 2),
                    wFrImg = BitConverter.ToUInt16(mapBuf, off + 4),
                    btDoorIndex = mapBuf[off + 6],
                    btDoorOffset = mapBuf[off + 7],
                    btAniFrame = mapBuf[off + 8],
                    btAniTick = mapBuf[off + 9],
                    btArea = mapBuf[off + 10],
                    btLight = mapBuf[off + 11]
                };
                if (boENMap)
                {
                    // 对应原实现：BkImg/MidImg/FrImg 逐字段 XOR $AA38 解密；$8000 位 = 禁走/禁站
                    unit.wBkImg ^= XORWORD;
                    if ((unit.wBkImg & 0x8000) != 0)
                        MapCellArray[nW, nH].chFlag |= 1;
                    unit.wMidImg ^= XORWORD;
                    unit.wFrImg ^= XORWORD;
                    if ((unit.wFrImg & 0x8000) != 0)
                        MapCellArray[nW, nH].chFlag |= 2;
                }
                else
                {
                    if ((unit.wBkImg & 0x8000) != 0)
                        MapCellArray[nW, nH].chFlag |= 1;
                    if ((unit.wFrImg & 0x8000) != 0)
                        MapCellArray[nW, nH].chFlag |= 2;
                }

                // 门
                if ((unit.btDoorIndex & 0x80) != 0)
                {
                    int point = unit.btDoorIndex & 0x7F;
                    if (point > 0)
                    {
                        var door = new TDoorInfo { n08 = (byte)point, nMapX = nW, nMapY = nH };
                        foreach (var existing in DoorList)
                        {
                            if (Math.Abs(existing.nMapX - door.nMapX) <= 10 &&
                                Math.Abs(existing.nMapY - door.nMapY) <= 10 &&
                                existing.n08 == point)
                            {
                                door.boOpened = existing.boOpened;
                                door.dwOpenTick = existing.dwOpenTick;
                                door.nRefCount = existing.nRefCount + 1;
                                break;
                            }
                        }
                        DoorList.Add(door);
                    }
                }
                MapData[nW, nH] = unit;
            }
        }
        return true;
    }

    private static string ReadShortString(byte[] buf, int offset, int capacity)
    {
        int len = buf[offset];
        if (len > capacity) len = capacity;
        return EncodingInit.GBK.GetString(buf, offset + 1, Math.Max(0, Math.Min(len, buf.Length - offset - 1)));
    }

    // ---- 格子管理 ----

    public bool CanWalk(int nX, int nY)
    {
        if (nX < 0 || nX >= nWidth || nY < 0 || nY >= nHeight) return false;
        var cell = MapCellArray[nX, nY];
        if (!cell.CanWalk() || !cell.CanHold()) return false;
        return true;
    }

    public bool CanAddToMapPosition(int nX, int nY)
    {
        if (nX < 0 || nX >= nWidth || nY < 0 || nY >= nHeight) return false;
        return MapCellArray[nX, nY].CanHold();
    }

    public bool GetMapCellInfo(int nX, int nY, out TMapCellinfo? cell)
    {
        cell = null;
        if (nX < 0 || nX >= nWidth || nY < 0 || nY >= nHeight) return false;
        cell = MapCellArray[nX, nY];
        return true;
    }

    public bool AddToMap(int nX, int nY, object obj)
    {
        if (!GetMapCellInfo(nX, nY, out var cell) || cell == null) return false;
        lock (cell.ObjList)
        {
            cell.ObjList.Add(obj);
        }
        return true;
    }

    public void DeleteFromMap(int nX, int nY, object obj)
    {
        if (!GetMapCellInfo(nX, nY, out var cell) || cell == null) return;
        lock (cell.ObjList)
        {
            cell.ObjList.Remove(obj);
        }
    }

    /// <summary>获取指定坐标的对象列表（发送视野消息用）。</summary>
    public List<object> GetObjects(int nX, int nY)
    {
        if (!GetMapCellInfo(nX, nY, out var cell) || cell == null)
            return new List<object>();
        lock (cell.ObjList)
        {
            return new List<object>(cell.ObjList);
        }
    }

    /// <summary>开门（对应 OpenDoor）。</summary>
    public void OpenDoor(int nX, int nY)
    {
        foreach (var door in DoorList)
        {
            if (door.nMapX == nX && door.nMapY == nY)
            {
                door.boOpened = true;
                door.dwOpenTick = DelphiRTL.GetTickCount();
                return;
            }
        }
    }

    public bool DoorOpened(int nX, int nY)
    {
        foreach (var door in DoorList)
        {
            if (door.nMapX == nX && door.nMapY == nY)
                return door.boOpened;
        }
        return true; // 无门视为通
    }
}
