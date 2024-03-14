using System;
using System.Collections.Generic;
using LahStats.Models;
using Rocket.API;
using Rocket.Core.Plugins;
using Rocket.Core.Utils;
using Rocket.Unturned.Player;

namespace LahStats.Commands
{
    internal class CommandLahStats : IRocketCommand
    {
        public AllowedCaller AllowedCaller => (AllowedCaller)2;
        public string Name => "ranking";
        public string Help => "show top 3";
        public string Syntax => string.Empty;
        public List<string> Aliases => new List<string> { "top" };
        public List<string> Permissions => new List<string> { "LahStats.ranking" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            LahStatsPlugin instance = LahStatsPlugin.Instance;
            UnturnedPlayer player = (UnturnedPlayer)caller;
            TaskDispatcher.QueueOnMainThread(() =>
            {
                List<Stat> top = instance.DatabaseManager.GetTop();
                int num = 1;
                foreach (Stat item in top)
                {
                    instance.SendMessage(player, ((RocketPlugin)instance).Translate("ranking", new object[5] { num, item.Name, item.Kills, item.Deaths, item.Headshots }));
                    num++;
                }
            });
        }
    }
}
