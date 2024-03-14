using System;
using System.Collections.Generic;
using LahStats.Configuration;
using LahStats.Models;
using Basededatos.Database;
using Rocket.API.Collections;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace LahStats
{
    public class LahStatsPlugin : RocketPlugin<LahStatsConfiguration>
    {
        private readonly Dictionary<CSteamID, bool> isPlayerDead = new Dictionary<CSteamID, bool>();

        public static LahStatsPlugin Instance { get; private set; }

        public DatabaseManager DatabaseManager { get; private set; }
        
        
      




        public override TranslationList DefaultTranslations => new TranslationList
{
    { "your_stats", "<color=#59FF00>Kills: {0}</color> <color=#FF0000>Deaths: {1}</color> <color=#FFFF00>K/D: {2:F2}</color> <color=#00FFFF>Headshots: {3}</color> <color=#800080>Rank: {4}</color>" },
    { "stats_of", "<color=#FF0000>Stats {0} - Kills: {1}</color> <color=#59FF00>Deaths: {2}</color> <color=#FFFF00>K/D: {3:F2}</color> <color=#00FFFF>Headshots: {4}</color> <color=#800080>Rank: {5}</color>" },
    { "ranking", "<color=#FF0000>{0} - [{1}]</color> <color=#59FF00>[{2} Kills]</color> <color=#FF0000>[{3} Deaths]</color> <color=#FFFF00>[{4} Headshots]</color>" }
};

        protected override void Load()
        {



            Logger.Log(@"
░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░
░░░░░████░░░░░░░░░░░░░░░████░░░░░
░░░░███░░░░░░░░░░░░░░░░░░░███░░░░
░░░███░░░░░░░░░░░░░░░░░░░░░███░░░
░░███░░░░░░░░░░░░░░░░░░░░░░░███░░
░███░░░░░░░░░░░░░░░░░░░░░░░░░███░
████░░░░░░░░░░░░░░░░░░░░░░░░░████
████░░░░░░░░░░░░░░░░░░░░░░░░░████
██████░░░░░░░███████░░░░░░░██████
█████████████████████████████████
█████████████████████████████████
░███████████████████████████████░
░░████░███████████████████░████░░
░░░░░░░███▀███████████▀███░░░░░░░
░░░░░░████──▀███████▀──████░░░░░░
░░░░░░█████───█████───█████░░░░░░
░░░░░░███████▄█████▄███████░░░░░░
░░░░░░░███████████████████░░░░░░░
░░░░░░░░█████████████████░░░░░░░░
░░░░░░░░░███████████████░░░░░░░░░
░░░░░░░░░░█████████████░░░░░░░░░░
░░░░░░░░░░░███████████░░░░░░░░░░░
░░░░░░░░░░███──▀▀▀──███░░░░░░░░░░
░░░░░░░░░░███─█████─███░░░░░░░░░░
░░░░░░░░░░░███─███─███░░░░░░░░░░░
░░░░░░░░░░░░█████████░░░░░░░░░░░░
░░░░░░Plugin Mejorado by lah░░░░░
░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░", ConsoleColor.Green);


            Instance = this;
            DatabaseManager = new DatabaseManager();
            U.Events.OnPlayerConnected += Events_OnPlayerConnected;
            UnturnedPlayerEvents.OnPlayerDeath += UnturnedPlayerEvents_OnPlayerDeath;
        }

        public void SendMessage(UnturnedPlayer player, string message)
        {
            ChatManager.serverSendMessage(message, Color.white, (SteamPlayer)null, player.SteamPlayer(), (EChatMode)0, base.Configuration.Instance.Image, true);
        }

        private void Events_OnPlayerConnected(UnturnedPlayer player)
        {
            DatabaseManager.Create(player.CSteamID);
            SendPlayerStatsUI(player.CSteamID);
            isPlayerDead[player.CSteamID] = false;
        }

        private void UnturnedPlayerEvents_OnPlayerDeath(UnturnedPlayer player, EDeathCause cause, ELimb limb, CSteamID murderer)
        {
            if (player == null || (int)cause == 10)
            {
                return;
            }
            if ((int)cause == 8)
            {
                DatabaseManager.SumarMuerte(player.CSteamID);
                SendPlayerStatsUI(player.CSteamID);
                isPlayerDead[player.CSteamID] = true;
            }
            else if (murderer != CSteamID.Nil && murderer != player.CSteamID)
            {
                UnturnedPlayer val = UnturnedPlayer.FromCSteamID(murderer);
                if (val != null)
                {
                    if ((int)limb == 13)
                    {
                        DatabaseManager.SumarHeadshot(val.CSteamID);
                    }
                    DatabaseManager.SumarKill(val.CSteamID);
                    SendPlayerStatsUI(val.CSteamID);
                }
            }
            if (!isPlayerDead[player.CSteamID])
            {
                DatabaseManager.SumarMuerte(player.CSteamID);
                SendPlayerStatsUI(player.CSteamID);
            }
            isPlayerDead[player.CSteamID] = false;
        }

        public void Update()
        {
            foreach (SteamPlayer client in Provider.clients)
            {
                if (!client.player.life.isDead)
                {
                    isPlayerDead[client.playerID.steamID] = false;
                }
            }
        }

        private void SendPlayerStatsUI(CSteamID iD)
        {
            Stat stats = DatabaseManager.GetStats(iD);
            if (stats != null)
            {
                UnturnedPlayer player = UnturnedPlayer.FromCSteamID(iD);
                float num = ((stats.Deaths > 0) ? ((float)stats.Kills / (float)stats.Deaths) : ((float)stats.Kills));
                SendMessage(player, string.Format(((RocketPlugin)this).Translate("your_stats", Array.Empty<object>()), stats.Kills, stats.Deaths, num, stats.Headshots, DatabaseManager.GetRankOnStats(iD)));
                EffectManager.sendUIEffect(base.Configuration.Instance.UIEffectID, (short)432, iD, true, stats.Kills.ToString(), stats.Deaths.ToString(), num.ToString("F2"), stats.Headshots.ToString());
            }
        }

        protected override void Unload()
        {
            U.Events.OnPlayerConnected -= Events_OnPlayerConnected;
            UnturnedPlayerEvents.OnPlayerDeath -= UnturnedPlayerEvents_OnPlayerDeath;
            DatabaseManager = null;
            Instance = null;
        }
    }
}
