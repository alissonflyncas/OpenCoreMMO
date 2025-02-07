﻿using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Commands.Player.UseItem;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;
using NeoServer.Server.Tasks;

namespace NeoServer.Networking.Handlers.Player;

public class PlayerUseItemHandler : PacketHandler
{
    private readonly IGameServer game;
    private readonly PlayerUseItemCommand playerUseItemCommand;

    public PlayerUseItemHandler(IGameServer game, PlayerUseItemCommand playerUseItemCommand)
    {
        this.game = game;
        this.playerUseItemCommand = playerUseItemCommand;
    }

    public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        var useItemPacket = new UseItemPacket(message);
        if (game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player))
            game.Dispatcher.AddEvent(new Event(2000,
                () => playerUseItemCommand.Execute(player,
                    useItemPacket))); //todo create a const for 2000 expiration time
    }
}