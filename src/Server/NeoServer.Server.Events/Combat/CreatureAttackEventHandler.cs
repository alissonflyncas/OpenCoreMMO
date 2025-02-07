﻿using System.Linq;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Effects.Parsers;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Networking.Packets.Outgoing.Effect;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;

namespace NeoServer.Server.Events.Combat;

public class CreatureAttackEventHandler
{
    private readonly IGameServer game;

    public CreatureAttackEventHandler(IGameServer game)
    {
        this.game = game;
    }

    public void Execute(ICreature creature, ICreature victim, CombatAttackType[] attacks)
    {
        var spectators = game.Map.GetPlayersAtPositionZone(creature.Location);
        
        foreach (var spectator in spectators)
        {
            if (!game.CreatureManager.GetPlayerConnection(spectator.CreatureId, out var connection)) continue;

            SendAttack(creature, victim, attacks, connection);
            
            connection.Send();
        }
    }

    private static void SendAttack(ICreature creature, ICreature victim, CombatAttackType[] attacks, IConnection connection)
    {
        foreach (var attack in attacks)
        {
            if (attack.Missed)
            {
                SendMissedAttack(creature, victim, attack, connection);
            }
            else
            {
                if (attack.ShootType != default && victim?.Location is not null)
                    connection.OutgoingPackets.Enqueue(new DistanceEffectPacket(creature.Location, victim.Location,
                        (byte)attack.ShootType));
            }

            if (attack.Area?.Any() ?? false)
                SpreadAreaEffect(attack, connection);

            else if (!attack.Missed && victim is not null) SendEffect(attack, connection, victim.Location);
        }
    }

    private static void SpreadAreaEffect(CombatAttackType attack, IConnection connection)
    {
        foreach (var coordinate in attack.Area)
        {
            if (coordinate.Missed) continue;
            SendEffect(attack, connection, coordinate.Point.Location);
        }
    }

    private static void SendEffect(CombatAttackType attack, IConnection connection, Location location)
    {
        attack.EffectT = attack.EffectT == 0 ? EffectT.None : attack.EffectT;
        var effect = attack.EffectT == EffectT.None ? DamageEffectParser.Parse(attack.DamageType) : attack.EffectT;

        if (attack.EffectT == EffectT.None && attack.DamageType == DamageType.Melee) return;
        if (effect == EffectT.None) return;
        
        connection.OutgoingPackets.Enqueue(new MagicEffectPacket(location, effect));
    }

    private static void SendMissedAttack(ICreature creature, ICreature victim, CombatAttackType attack,
        IConnection connection)
    {
        Location destLocation;
        do
        {
            var index = GameRandom.Random.Next(0, maxValue: victim.Location.Neighbours.Length);
            destLocation = victim.Location.Neighbours[index];
        } while (destLocation == creature.Location);

        if (attack.ShootType != default)
            connection.OutgoingPackets.Enqueue(new DistanceEffectPacket(creature.Location, destLocation,
                (byte)attack.ShootType));
        connection.OutgoingPackets.Enqueue(new MagicEffectPacket(destLocation, EffectT.Puff));
    }
}