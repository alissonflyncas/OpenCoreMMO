using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Inspection;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Items.Factories;
using NeoServer.Game.Items.Items;
using NeoServer.Game.World.Map;
using NeoServer.Networking.Handlers;
using NeoServer.Networking.Handlers.Player;
using NeoServer.Server.Common.Contracts.Network.Enums;

namespace NeoServer.Extensions.Spells.Commands
{
    public class ListCommandsCommand : CommandSpell
    {
        public override bool OnCast(ICombatActor actor, string words, out InvalidOperation error)
        {
            error = InvalidOperation.NotEnoughRoom;

            StringBuilder text = new StringBuilder();
            int maxSizeWord = SpellList.Spells.Keys.Where(key => !string.IsNullOrEmpty(SpellList.Spells[key].Description)).Max(key => key.Length);
            int maxSizeDescription = SpellList.Spells.Values.Where(value => !string.IsNullOrEmpty(value.Description)).Max(value => value.Description.Length);

            foreach (var item in SpellList.Spells)
            {
                if (string.IsNullOrEmpty(item.Value.Description))
                    continue;

                string word = item.Key.PadRight(maxSizeWord, ' ');
                string description = item.Value.Description.PadRight(maxSizeDescription, ' ');
                text.AppendLine($"{word} = {description}");
            }

            if (actor is not IPlayer player)
                return false;

            Paper paper = BuildPaper(actor);
            paper.Write(text.ToString(), player);
            player.Read(paper);

            return true;
        }

        private Paper BuildPaper(ICombatActor actor)
        {
            IItem item = ItemFactory.Instance.Create(2597, actor.Location,
             new Dictionary<ItemAttribute, IConvertible> { { ItemAttribute.Count, 1 } });

            return item as Paper;
        }
    }
}