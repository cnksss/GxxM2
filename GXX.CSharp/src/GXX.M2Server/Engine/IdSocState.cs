namespace GXX.M2Server.Engine;

/// <summary>M2Definition.pas TSessInfo（全局会话，ViewSession/网关链路使用子集）。</summary>
public class TSessInfo
{
    public string sAccount = "";
    public string sIPaddr = "";
    public int nSessionID;
    public int nPayMent;
    public int nPayMode;
    public int nSessionStatus;
    public bool boStartPlay;
    public bool boLoadRcd;
    public uint dwStartTick;
    public uint dwActiveTick;
    public int nRefCount;
    public bool boClose;
    public uint dwCloseTick;
}

/// <summary>
/// IdSrvClient.pas FrmIDSoc 会话表子集（批次J4a：ViewSession 依赖）。
/// Delphi m_SessionList: TMTList → Lock/UnLock + Items 计数访问 1:1。
/// </summary>
public static class IdSocState
{
    /// <summary>Delphi 全局 FrmIDSoc（窗体单例，此处仅承载会话表状态）。</summary>
    public static readonly IdSocForm FrmIDSoc = new();

    public sealed class IdSocForm
    {
        public readonly IdSocSessionList m_SessionList = new();
    }

    public class IdSocSessionList
    {
        private readonly object _lock = new();
        private readonly List<TSessInfo> _items = new();

        public int Count { get { lock (_lock) return _items.Count; } }

        public TSessInfo this[int index] { get { lock (_lock) return _items[index]; } }

        public void Add(TSessInfo info) { lock (_lock) _items.Add(info); }

        public void RemoveAt(int index) { lock (_lock) _items.RemoveAt(index); }

        public void Clear() { lock (_lock) _items.Clear(); }

        public void Lock() => System.Threading.Monitor.Enter(_lock);
        public void UnLock() => System.Threading.Monitor.Exit(_lock);
    }
}
