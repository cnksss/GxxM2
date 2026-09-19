// 共享池模式，所有的TIOCPObject共享 IODataPool, TIOCPClientContextPool
{$DEFINE SHARE_POOL_MODE}
//是否使用原子锁；经长时间测试，原子锁可能导致界面死
{.$DEFINE USE_SPINLOCK}
