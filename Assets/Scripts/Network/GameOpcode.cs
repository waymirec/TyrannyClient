using CommonLib;

namespace Network
{
    public enum GameOpcode
    {
        NoOp=0,

        GameIdent=10,
        GameReady=11,
        Ping=21,
        Pong=22,

        Hello=50,

        EnterWorld=100,
        SpawnWorldEntity=101,
        DestroyWorldEntity=102,
        MoveWorldEntity=103,
        WorldEntityDisco=104
    }
}