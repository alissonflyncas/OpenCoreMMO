﻿using Autofac;
using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Events.Combat;
using NeoServer.Server.Events.Creature;
using NeoServer.Server.Events.Player;
using NeoServer.Server.Events.Server;
using NeoServer.Server.Events.Tiles;

namespace NeoServer.Server.Events.Subscribers;

public sealed class EventSubscriber
{
    private readonly IComponentContext _container;
    private readonly IGameServer _gameServer;

    private readonly IMap _map;

    public EventSubscriber(IMap map, IGameServer gameServer, IComponentContext container)
    {
        _map = map;
        _gameServer = gameServer;
        _container = container;
   
    }

    public void AttachEvents()
    {
        _map.OnCreatureAddedOnMap += (creature, cylinder) =>
            _container.Resolve<PlayerAddedOnMapEventHandler>().Execute(creature, cylinder);
        _map.OnThingRemovedFromTile += _container.Resolve<ThingRemovedFromTileEventHandler>().Execute;
        _map.OnCreatureMoved += _container.Resolve<CreatureMovedEventHandler>().Execute;
        _map.OnThingMovedFailed += _container.Resolve<InvalidOperationEventHandler>().Execute;
        _map.OnThingAddedToTile += _container.Resolve<ThingAddedToTileEventHandler>().Execute;
        _map.OnThingUpdatedOnTile += _container.Resolve<ThingUpdatedOnTileEventHandler>().Execute;
        BaseSpell.OnSpellInvoked += _container.Resolve<SpellInvokedEventHandler>().Execute;

        OperationFailService.OnOperationFailed += _container.Resolve<PlayerOperationFailedEventHandler>().Execute;

        _gameServer.OnOpened += _container.Resolve<ServerOpenedEventHandler>().Execute;
    }
}