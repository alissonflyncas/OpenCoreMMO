﻿using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Helpers;
using NeoServer.Networking.Packets.Outgoing.Item;
using NeoServer.Networking.Packets.Outgoing.Npc;
using NeoServer.Server.Common.Contracts;

namespace NeoServer.Server.Events.Items;

public class ContentModifiedOnContainerEventHandler
{
    private readonly ICoinTypeStore _coinTypeStore;
    private readonly IGameServer game;

    public ContentModifiedOnContainerEventHandler(IGameServer game, ICoinTypeStore coinTypeStore)
    {
        this.game = game;
        _coinTypeStore = coinTypeStore;
    }

    public void Execute(IPlayer player, ContainerOperation operation, byte containerId, byte slotIndex, IItem item)
    {
        if (Guard.IsNull(player)) return;
        if (!game.CreatureManager.GetPlayerConnection(player.CreatureId, out var connection)) return;
        
        switch (operation)
        {
            case ContainerOperation.ItemRemoved:
                connection.OutgoingPackets.Enqueue(new RemoveItemContainerPacket(containerId, slotIndex, item));
                break;
            case ContainerOperation.ItemAdded:
                connection.OutgoingPackets.Enqueue(new AddItemContainerPacket(containerId, item));
                break;
            case ContainerOperation.ItemUpdated:
                connection.OutgoingPackets.Enqueue(new UpdateItemContainerPacket(containerId, slotIndex, item));
                break;
        }

        if (Equals(player.Containers[containerId]?.Parent, player) && player.Shopping)
            connection.OutgoingPackets.Enqueue(new SaleItemListPacket(player,
                player.TradingWithNpc?.ShopItems?.Values, _coinTypeStore));

        connection.Send();
    }
}

public enum ContainerOperation
{
    ItemRemoved,
    ItemAdded,
    ItemUpdated
}