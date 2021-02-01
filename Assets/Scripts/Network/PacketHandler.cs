using System;
using Tyranny.Networking;

namespace Network
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class GamePacketHandler : Attribute, IPacketHandler<GameOpcode>
    {
        public GamePacketHandler(GameOpcode opcode)
        {
            Opcode = opcode;
        }

        public GameOpcode Opcode { get; }
    }
}