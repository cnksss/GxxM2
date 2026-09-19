namespace GXX.M2Server.Engine;

/// <summary>
/// Castle.pas / Guild.pas 全局管理器实例（Delphi g_CastleManager / g_GuildManager 单例 1:1）。
/// </summary>
public static class CastleState
{
    /// <summary>M2Share：g_CastleManager: TCastleManager。</summary>
    public static readonly TCastleManager g_CastleManager = new();

    /// <summary>M2Share：g_GuildManager: TGuildManager。</summary>
    public static readonly GuildManager g_GuildManager = new();
}
