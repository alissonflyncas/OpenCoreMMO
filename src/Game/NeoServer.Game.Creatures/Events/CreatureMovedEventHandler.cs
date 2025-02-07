﻿using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Creatures.Events;

public class CreatureMovedEventHandler : IGameEventHandler
{
    public void Execute(ICreature creature, Location fromLocation, Location toLocation,
        ICylinderSpectator[] spectators)
    {
        foreach (var cylinderSpectator in spectators)
        {
            var spectator = cylinderSpectator.Spectator;
            if (creature == spectator) continue;

            if (spectator is ICombatActor actor) actor.SetAsEnemy(creature);
            if (creature is ICombatActor a) a.SetAsEnemy(spectator);
        }

        if (creature is ICombatActor combatActor) combatActor.Tile.MagicField?.CauseDamage(combatActor);
    }
}