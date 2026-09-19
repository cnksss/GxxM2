namespace GXX.M2Server.Engine;

/// <summary>M2Definition.pas TMsgType（Delphi 枚举序 1:1）。</summary>
public enum TMsgType
{
    t_Notice = 0,
    t_Hint = 1,
    t_System = 2,
    t_Say = 3,
    t_Mon = 4,
    t_GM = 5,
    t_Cust = 6,
    t_Castle = 7,
    t_Char = 8,
}

/// <summary>M2Definition.pas TMsgColor（Delphi 枚举序 1:1）。</summary>
public enum TMsgColor
{
    c_Red = 0,
    c_Green = 1,
    c_Blue = 2,
    c_White = 3,
}

/// <summary>Grobal2.pas TMagicAttr（Delphi 枚举序 1:1）。</summary>
public enum TMagicAttr
{
    mtHum = 0,
    mtHero = 1,
    mtContinuous = 2,
    mtDefense = 3,
    mtAttack = 4,
}
