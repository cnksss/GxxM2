using System;

namespace GXX.Client.Scenes;

/// <summary>
/// Actor.pas 2294-2754 图片偏移计算 1:1。
/// GetOffset：怪物外观（Appr）→ WIL 图片组基址（nrace=Appr div 10、npos=Appr mod 10；
/// Appr>=1000 修正为 (Appr mod 10)×360）；GetNpcOffset：NPC 外观 → 图片基址（含 >=2000 减 2000 修正）。
/// 分表中的 -1 为原文“保留”项，按原义返回。
/// </summary>
public static class ActorOffsets
{
    public static int GetOffset(int appr)
    {
        if (appr >= 1000)
            return appr % 10 * 360;

        int nrace = appr / 10;
        int npos = appr % 10;
        switch (nrace)
        {
            case 0: return npos * 280;
            case 1: return npos * 230;
            case 2:
            case 3:
            case 7:
            case 8:
            case 9:
            case 10:
            case 11:
            case 12: return npos * 360;
            case 4:
                {
                    int result = npos * 360;
                    if (npos == 1) result = 600;
                    return result;
                }
            case 5: return npos * 430;
            case 6: return npos * 440;
            case 13:
                switch (npos)
                {
                    case 0: return 0;
                    case 1: return 360;
                    case 2: return 440;
                    case 3: return 550;
                    default: return npos * 360;
                }
            case 14: return npos * 360;
            case 15: return npos * 360;
            case 17: // Mon18
                switch (npos)
                {
                    case 2: return 920;
                    case 3: return 1280;
                    default: return npos * 350;
                }
            case 18: // Mon19
                switch (npos)
                {
                    case 0: return 0;
                    case 1: return 520;
                    case 2: return 950;
                    case 3: return 1574;
                    case 4: return 1934;
                    case 5: return 2294;
                    case 6: return 2654;
                    case 7: return 3014;
                    default: return 0; // Delphi 嵌套 case 无 else → 保持初值 0
                }
            case 19: // Mon20
                switch (npos)
                {
                    case 0: return 0;
                    case 1: return 370;
                    case 2: return 810;
                    case 3: return 1250;
                    case 4: return 1630;
                    case 5: return 2010;
                    case 6: return 2390;
                    default: return 0;
                }
            case 20: // Mon21
                switch (npos)
                {
                    case 0: return 0;
                    case 1: return 360;
                    case 2: return 720;
                    case 3: return 1080;
                    case 4: return 1440;
                    case 5: return 1800;
                    case 6: return 2350;
                    case 7: return 3060;
                    default: return 0;
                }
            case 21: // Mon22（npos 5 无分支 → 0；6: 2440 覆盖注释行 2260 原文形态）
                switch (npos)
                {
                    case 0: return 0;
                    case 1: return 460;
                    case 2: return 820;
                    case 3: return 1180;
                    case 4: return 1540;
                    case 6: return 2440;
                    case 7: return 2570;
                    case 8: return 2700;
                    default: return 0;
                }
            case 22: // Mon23
                switch (npos)
                {
                    case 0: return 0;
                    case 1: return 430;
                    case 2: return 1290;
                    case 3: return 1810;
                    default: return 0;
                }
            case 23: // Mon24
                switch (npos)
                {
                    case 0: return 0;
                    case 1: return 340;
                    case 2: return 680;
                    case 3: return 1180;
                    case 4: return 1770;
                    case 5: return 2610;
                    case 6: return 2950;
                    case 7: return 3290;
                    case 8: return 3750;
                    case 9: return 4460;
                    default: return 0;
                }
            case 24: // Mon25
                switch (npos)
                {
                    case 0: return 0;
                    case 1: return 510;
                    case 2: return 1090;
                    default: return 0;
                }
            case 25: // Mon26
                switch (npos)
                {
                    case 0: return 0;
                    case 1: return 510;
                    case 2: return 1020;
                    case 3: return 1370;
                    case 4: return 1720;
                    case 5: return 2070;
                    case 6: return 2740;
                    case 7: return 3780;
                    case 8: return 3820;
                    case 9: return 4170;
                    default: return 0;
                }
            case 26: // Mon27
                switch (npos)
                {
                    case 0: return 0;
                    case 1: return 340;
                    case 2: return 680;
                    case 3: return 1190;
                    case 4: return 2100;
                    case 5: return 2440;
                    case 6: return 2540;
                    case 7: return 3570;
                    default: return 0;
                }
            case 27: // 圣兽 Mon28（基址重新定义 piaoyun 2013-11-23）
                switch (npos)
                {
                    case 0: return 0;
                    case 1: return 350;
                    case 2: return 780;
                    case 3: return 1130;
                    case 4: return 1560;
                    case 5: return 1910;
                    default: return 0;
                }
            case 28: // Mon29
                switch (npos)
                {
                    case 0: return 0;
                    case 1: return 600;
                    default: return 0;
                }
            case 29: // Mon30
                switch (npos)
                {
                    case 0: return 0;
                    case 1: return 360;
                    case 2: return 720;
                    case 3: return 1070;
                    default: return 0;
                }
            case 32: // Mon33
                switch (npos)
                {
                    case 0: return 0;
                    case 1: return 440;
                    case 2: return 820;
                    case 3: return 1360;
                    case 4: return 2650;
                    case 5: return 2680;
                    case 6: return 2790;
                    case 7: return 2900;
                    case 8: return 3500;
                    case 9: return 3930;
                    default: return 0;
                }
            case 33: // Mon34
                switch (npos)
                {
                    case 0: return 20;
                    case 1: return 720;
                    case 2: return 1160;
                    case 3: return 1840;
                    case 4: return 2540;
                    case 5: return 2900;
                    case 6: return 3250;
                    case 7: return 3310;
                    default: return 0;
                }
            case 34: // Mon35
                switch (npos)
                {
                    case 0: return 0;
                    case 1: return 680;
                    case 2: return 1030;
                    default: return 0;
                }
            case 35: // Mon36（2/13 为原文 -1 保留项）
                switch (npos)
                {
                    case 0: return 0;
                    case 1: return 810;
                    case 2: return -1;
                    case 3: return 1800;
                    case 4: return 2610;
                    case 5: return 3420;
                    case 6: return 4390;
                    case 7: return 5200;
                    case 8: return 6170;
                    case 9: return 6980;
                    case 10: return 7790;
                    case 11: return 8760;
                    case 12: return 9570;
                    case 13: return -1;
                    case 14: return 11030;
                    case 15: return 12000;
                    case 16: return 13800;
                    case 17: return 14770;
                    case 18: return 15580;
                    case 19: return 16390;
                    case 20: return 17360;
                    case 21: return 18330;
                    case 22: return 19300;
                    case 23: return 20270;
                    case 24: return 21240;
                    case 25: return 22050;
                    case 26: return 22860;
                    case 27: return 23990;
                    case 28: return 24800;
                    case 29: return 25930;
                    default: return 0;
                }
            case 36: // Mon37
                switch (npos)
                {
                    case 0: return 0;
                    case 1: return 400;
                    case 2: return 960;
                    case 3: return 1360;
                    case 4: return 1440;
                    case 5: return 1840;
                    case 6: return 2240;
                    case 7: return 2840;
                    case 8: return 3320;
                    default: return 0;
                }
            case 40: // 新神兽 Mon41-2/3
                switch (npos)
                {
                    case 0: return 0;
                    case 1: return 352;
                    case 2: return 480;
                    default: return 0;
                }
            case 49:
            case 50:
            case 51:
            case 52:
            case 53: return npos * 360;
            case 60: // Mon36
                switch (npos)
                {
                    case 0: return 0;
                    case 1: return 810;
                    case 2: return -1;
                    case 3: return 1800;
                    case 4: return 2610;
                    case 5: return 3420;
                    case 6: return 4390;
                    case 7: return 5200;
                    case 8: return 6170;
                    case 9: return 6980;
                    default: return 0;
                }
            case 61:
                switch (npos)
                {
                    case 0: return 7790;
                    case 1: return 8760;
                    case 2: return 9570;
                    case 3: return -1;
                    case 4: return 11030;
                    case 5: return 12000;
                    case 6: return 13800;
                    case 7: return 14770;
                    case 8: return 15580;
                    case 9: return 16390;
                    default: return 0;
                }
            case 62:
                switch (npos)
                {
                    case 0: return 17360;
                    case 1: return 18330;
                    case 2: return 19300;
                    case 3: return 20270;
                    case 4: return 21240;
                    case 5: return 22050;
                    case 6: return 22860;
                    case 7: return 23990;
                    case 8: return 24800;
                    case 9: return 25930;
                    default: return 0;
                }
            case 63: // Mon32
                switch (npos)
                {
                    case 0: return 0;
                    case 1: return 610;
                    case 2: return 1050;
                    case 3: return 1420;
                    case 4: return 1860;
                    case 5: return 2230;
                    case 6: return 2670;
                    case 7: return 3190;
                    case 8: return 3710;
                    case 9: return 4390;
                    default: return 0;
                }
            case 64: // Mon32
                switch (npos)
                {
                    case 0: return 4750;
                    case 1: return 5270;
                    case 2: return 5870;
                    case 3: return 6680;
                    default: return 0;
                }
            case 95:
                if (npos == 3) return 3008; // 血灵教主
                return npos * 360;          // 新骷髅 Mon-kulou
            case 80:
                switch (npos)
                {
                    case 0: return 0;
                    case 1: return 80;
                    case 2: return 300;
                    case 3: return 301;
                    case 4: return 302;
                    case 5: return 320;
                    case 6: return 321;
                    case 7: return 322;
                    case 8: return 321;
                    default: return 0;
                }
            case 90:
                switch (npos)
                {
                    case 0: return 80;
                    case 1: return 168;
                    case 2: return 184;
                    case 3: return 200;
                    case 4: return 1770;
                    case 5: return 1780;
                    case 6: return 1790;
                    default: return 0;
                }
            default: return npos * 360;
        }
    }

    public static int GetNpcOffset(int nAppr)
    {
        if (nAppr >= 2000)
            nAppr -= 2000;

        switch (nAppr)
        {
            case 24:
            case 25: return (nAppr - 24) * 60 + 1470;
            case int v when v >= 0 && v <= 22: return nAppr * 60;
            case 23: return 1380;
            case 27:
            case 32: return (nAppr - 26) * 60 + 1620 - 30;
            case 26:
            case 28:
            case 29:
            case 30:
            case 31:
            case int v when v >= 33 && v <= 41: return (nAppr - 26) * 60 + 1620;
            case 42:
            case 43: return 2580;
            case int v when v >= 44 && v <= 47: return 2640;
            case int v when v >= 48 && v <= 50: return (nAppr - 48) * 60 + 2700;
            case 51: return 2880;
            case 52: return 2960;
            case int v when v >= 54 && v <= 58: return 4490 + (nAppr - 54) * 10;
            case int v when v >= 94 && v <= 98: return 4490 + (nAppr - 94) * 10;
            case 59: return 4540;
            case int v when v >= 60 && v <= 67: return 3060 + (nAppr - 60) * 60;
            case 68: return 3600;
            case int v when v >= 70 && v <= 75: return 3780 + (nAppr - 70) * 10;
            case 76:
            case 77: return 3840 + (nAppr - 76) * 60;
            case int v when v >= 78 && v <= 80: return 4060 + (nAppr - 78) * 60;
            case int v when v >= 81 && v <= 83: return 3960 + (nAppr - 81) * 20;
            case 84: return 4030;
            case int v when v >= 90 && v <= 92: return 3750 + (nAppr - 90) * 10;
            case 99: return 4240;
            case 100: return 4560;
            case 101: return 4770;
            case 102: return 4810;
            case int v when v >= 200 && v <= 207: return (nAppr - 200) * 70;
            case 208: return 630;
            case 209: return 700;
            case 210: return 740;
            case 211: return 810;
            case 212: return 820;
            case 213: return 830;
            case 214: return 840;
            case 215: return 850;
            case 216: return 860;
            case 217: return 870;
            case 218: return 900;
            case 219: return 930;
            case 220: return 970;
            case 221: return 980;
            case 222: return 990;
            case 223: return 1020;
            case 224: return 1030;
            case 225: return 1060;
            case 226: return 0;
            case 227: return 40;
            case 228: return 80;
            case 229: return 120;
            case 230: return 160;
            case 231: return 200;
            case 232: return 240;
            case 233: return 280;
            case 234: return 320;
            case 235: return 360;
            case 236: return 400;
            case 237: return 470;
            case 238: return 540;
            case 239: return 610;
            case 240: return 680;
            case 241: return 750;
            case 242: return 820;
            case 243: return 890;
            case 244: return 950;
            case 245: return 1010;
            case 246: return 0;
            case 247: return 90;
            case 248: return 180;
            case 249: return 270;
            case 250: return 360;
            case 251: return 450;
            case 252: return 540;
            case 253: return 710;
            case 254: return 800;
            case 255: return 890;
            case 256: return 980;
            case 257: return 1070;
            case 258: return 1160;
            case 259: return 1330;
            case 260: return 1420;
            case 261: return 1510;
            case 262: return 1600;
            case 263: return 1690;
            case 264: return 1860;
            case 265: return 2030;
            case 266: return 2120;
            case 267: return 2210;
            case 268: return 2300;
            case 269: return 2390;
            case 270: return 2560;
            case 271: return 2730;
            case 272: return 2820;
            default:
                if (nAppr >= 1000)
                    return (nAppr - 1000) * 60;
                if (nAppr > 200)
                    return (nAppr - 200) * 70;
                return (nAppr - 200) * 60;
        }
    }
}
