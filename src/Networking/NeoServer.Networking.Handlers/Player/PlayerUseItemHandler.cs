﻿using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Commands.Player;
using NeoServer.Server.Contracts;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Tasks;

namespace NeoServer.Server.Handlers.Player
{
    public class PlayerUseItemHandler : PacketHandler
    {
        private readonly IGameServer game;
        public PlayerUseItemHandler(IGameServer game)
        {
            this.game = game;
        }
        public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            var useItemPacket = new UseItemPacket(message);
            if (game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player))
            {
                game.Dispatcher.AddEvent(new Event(2000, new PlayerUseItemCommand(player, game, useItemPacket).Execute)); //todo create a const for 2000 expiration time
            }
        }
    }
}
