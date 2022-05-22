﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Common.Contracts.Spells;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Configurations;
using NeoServer.Server.Helpers.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;

namespace NeoServer.Loaders.Spells;

public class SpellLoader
{
    private readonly ILogger logger;
    private readonly ServerConfiguration serverConfiguration;
    private readonly IGameCreatureManager gameCreatureManager;

    public SpellLoader(ServerConfiguration serverConfiguration, IGameCreatureManager gameCreatureManager, ILogger logger)
    {
        this.serverConfiguration = serverConfiguration;
        this.gameCreatureManager = gameCreatureManager;
        this.logger = logger;
    }

    public void Load()
    {
        LoadSpells();
    }

    private void LoadSpells()
    {
        logger.Step("Loading spells...", "{n} spells loaded", () =>
        {
            var path = Path.Combine(serverConfiguration.Data, "spells", "spells.json");
            var jsonString = File.ReadAllText(path);
            var spells = JsonConvert.DeserializeObject<List<IDictionary<string, object>>>(jsonString)?.ToList() ??
                         new List<IDictionary<string, object>>(0);

            var types = ScriptSearch.All.Where(x => typeof(ISpell).IsAssignableFrom(x)).ToList();

            foreach (var spell in spells)
            {
                if (spell is null) continue;

                var type = types.FirstOrDefault(x => x.Name == spell["script"].ToString());
                if (type is null) continue;

                if (Activator.CreateInstance(type, true) is not ISpell spellInstance) continue;

                spellInstance.Name = spell["name"].ToString();
                spellInstance.Description = spell["description"].ToString();
                spellInstance.Cooldown = Convert.ToUInt32(spell["cooldown"]);
                spellInstance.Mana = Convert.ToUInt16(spell["mana"]);
                spellInstance.MinLevel = Convert.ToUInt16(spell["level"]);
                spellInstance.Vocations = spell.ContainsKey("vocations")
                    ? (spell["vocations"] as JArray)?.Select(jv => (byte)jv).ToArray()
                    : null;

                SpellList.Add(spell["words"].ToString(), spellInstance);
            }

            return new object[] { spells.Count };
        });
    }
}