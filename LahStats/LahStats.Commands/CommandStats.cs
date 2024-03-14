using System;
using System.Collections.Generic;
using LahStats.Models;
using Rocket.API;
using Rocket.Core.Plugins;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Rocket.Core.Utils;

namespace LahStats.Commands
{
    internal class CommandStats : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "stats";
        public string Help => "Show stats";
        public string Syntax => "[<name>]"; // El nombre es opcional
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string> { "LahStats.stats" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            LahStatsPlugin instance = LahStatsPlugin.Instance;
            UnturnedPlayer player = (UnturnedPlayer)caller;
            TaskDispatcher.QueueOnMainThread(() =>
            {
                string playerName = (command.Length > 0) ? string.Join(" ", command) : player.DisplayName;
                UnturnedPlayer targetPlayer = UnturnedPlayer.FromName(playerName);

                if (targetPlayer == null)
                {
                    instance.SendMessage(player, $"Player '{playerName}' not found!");
                    return;
                }

                Stat stats = instance.DatabaseManager.GetStats(targetPlayer.CSteamID);
                if (stats != null)
                {
                    float kdr = (stats.Deaths > 0) ? ((float)stats.Kills / (float)stats.Deaths) : ((float)stats.Kills);
                    instance.SendMessage(player, string.Format(((RocketPlugin)instance).Translate("stats_of", new object[6] { playerName, stats.Kills, stats.Deaths, kdr, stats.Headshots, instance.DatabaseManager.GetRankOnStats(targetPlayer.CSteamID) })));
                }
                else
                {
                    instance.SendMessage(player, $"Stats for player '{playerName}' not found!");
                }
            });
        }
    }
}
