﻿using KompasClient.GameCore;
using KompasCore.Networking;

namespace KompasCore.Networking
{
    public class SetFirstPlayerPacket : Packet
    {
        public int playerIndex;

        public SetFirstPlayerPacket() : base(SetFirstTurnPlayer) { }

        public SetFirstPlayerPacket(int playerIndex) : this()
        {
            this.playerIndex = playerIndex;
        }

        public override Packet Copy() => new SetFirstPlayerPacket();
    }
}

namespace KompasClient.Networking
{
    public class SetFirstPlayerClientPacket : SetFirstPlayerPacket, IClientOrderPacket
    {
        public void Execute(ClientGame clientGame) => clientGame.SetFirstTurnPlayer(playerIndex);
    }
}