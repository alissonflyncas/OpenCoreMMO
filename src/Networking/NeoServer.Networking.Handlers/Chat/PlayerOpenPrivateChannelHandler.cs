﻿using NeoServer.Data.Interfaces;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;
using NeoServer.Server.Tasks;

namespace NeoServer.Server.Handlers.Player
{
    public class PlayerOpenPrivateChannelHandler : PacketHandler
    {
        private readonly Game game;
        private readonly IAccountRepository accountRepository;
        public PlayerOpenPrivateChannelHandler(Game game, IAccountRepository accountRepository)
        {
            this.game = game;
            this.accountRepository = accountRepository;
        }
        public override async void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            var channel = new OpenPrivateChannelPacket(message);
            if (!game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;

            IPlayer receiver = null;

            if (string.IsNullOrWhiteSpace(channel.Receiver) || (await accountRepository.GetPlayer(channel.Receiver)) is null)
            {
                connection.Send(new TextMessagePacket("A player with this name does not exist.", TextMessageOutgoingType.Small));
                return;
            }

            if (string.IsNullOrWhiteSpace(channel.Receiver) || !game.CreatureManager.TryGetPlayer(channel.Receiver, out receiver))
            {
                connection.Send(new TextMessagePacket("A player with this name is not online.", TextMessageOutgoingType.Small));
                return;
            }

            if (channel.Receiver.Trim().Equals(player.Name.Trim(), System.StringComparison.InvariantCultureIgnoreCase))
            {
                connection.Send(new TextMessagePacket("You cannot set up a private message channel with yourself.", TextMessageOutgoingType.Small));
                return;
            }

            if (receiver is null) return;

            connection.OutgoingPackets.Enqueue(new PlayerOpenPrivateChannelPacket(receiver.Name));
            connection.Send();
        }
    }
}
