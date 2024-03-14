using System;
using System.Collections.Generic;
using System.Linq;
using DataStorage;
using LahStats;
using LahStats.Models;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Unturned.Player;
using Steamworks;

namespace Basededatos.Database
{
    public class DatabaseManager
    {
        private List<Stat> Data;
        private DataStorage<List<Stat>> DataStorage { get; set; }

        public DatabaseManager()
        {
            DataStorage = new DataStorage<List<Stat>>(((RocketPlugin)LahStatsPlugin.Instance).Directory, "Database.json");
            Reload();
            Logger.Log("Conexion a la base de datos... OK", ConsoleColor.White);
        }

        public void Reload()
        {
            Data = DataStorage.Read();
            if (Data == null)
            {
                Data = new List<Stat>();
                DataStorage.Save(Data);
            }
        }

        private List<Stat> OrdenarPorKills()
        {
            return Data.OrderByDescending((Stat x) => x.Kills).ToList();
        }

        public List<Stat> GetTop()
        {
            List<Stat> list = OrdenarPorKills();
            List<Stat> list2 = new List<Stat>();
            int num = 0;
            foreach (Stat item in list)
            {
                if (num > 3)
                {
                    break;
                }
                num++;
                list2.Add(item);
            }
            return list2;
        }

        public Stat GetStats(CSteamID iD)
        {
            return Data.Find((Stat x) => x.Id == iD);
        }

        public int GetRankOnStats(CSteamID iD)
        {
            List<Stat> list = OrdenarPorKills();
            return list.FindLastIndex((Stat x) => x.Id == iD) + 1;
        }

        public void SumarHeadshot(CSteamID iD)
        {
            Stat stat = Data.Find((Stat x) => x.Id == iD);
            if (stat != null)
            {
                stat.Headshots++;
                DataStorage.Save(Data);
            }
        }

        public void SumarKill(CSteamID iD)
        {
            Stat stat = Data.Find((Stat x) => x.Id == iD);
            if (stat != null)
            {
                stat.Kills++;
                DataStorage.Save(Data);
            }
        }

        public void SumarMuerte(CSteamID iD, bool isZombieKill = false)
        {
            Stat stat = Data.Find((Stat x) => x.Id == iD);
            if (stat != null && !isZombieKill)
            {
                stat.Deaths++;
                DataStorage.Save(Data);
            }
        }

        public void Create(CSteamID id)
        {
            try
            {
                Stat stat = Data.Find((Stat x) => x.Id == id);
                if (stat == null)
                {
                    throw new Exception();
                }
                UnturnedPlayer val = UnturnedPlayer.FromCSteamID(id);
                stat.Name = val.CharacterName;
                DataStorage.Save(Data);
            }
            catch
            {
                UnturnedPlayer val2 = UnturnedPlayer.FromCSteamID(id);
                Stat item = new Stat
                {
                    Deaths = 0,
                    Headshots = 0,
                    Id = id,
                    Kills = 0,
                    Name = val2.CharacterName
                };
                Data.Add(item);
                DataStorage.Save(Data);
            }
        }
    }
}
