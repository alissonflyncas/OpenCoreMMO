﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoServer.Server.Model.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Data.Seeds
{
    public class PlayerModelSeed
    {
        public static void Seed(EntityTypeBuilder<PlayerModel> builder)
        {
            builder.HasData
                (
                new PlayerModel
                {
                    PlayerId = 1,
                    PlayerType = 3,
                    AccountId = 1,
                    Name = "GOD",
                    ChaseMode = NeoServer.Game.Common.Players.ChaseMode.Follow,
                    Capacity = 90000,
                    Level = 1000,
                    Health = 4440,
                    MaxHealth = 4440,
                    Vocation = 11,
                    Gender = NeoServer.Game.Common.Players.Gender.Male,
                    Speed = 800,
                    Online = false,
                    Mana = 1750,
                    MaxMana = 1750,
                    Soul = 100,
                    MaxSoul = 100,
                    PosX = 1020,
                    PosY = 1022,
                    PosZ = 7,
                    StaminaMinutes = 2520,
                    LookType = 75,
                    SkillAxe = byte.MaxValue,
                    SkillSword = byte.MaxValue,
                    SkillClub = byte.MaxValue,
                    SkillDist = byte.MaxValue,
                    SkillFishing = byte.MaxValue,
                    SkillFist = byte.MaxValue,
                    MagicLevel = byte.MaxValue,
                    SkillShielding = byte.MaxValue,
                    Experience = 0,
                    FightMode = Game.Common.Players.FightMode.Attack
                }
              );
        }
    }
}
