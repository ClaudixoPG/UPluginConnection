using Firebase.Extensions;
using Firebase.Firestore;
using Firebase;
using Firebase.Database;
using SaveSystem;
using System.Collections.Generic;
using UnityEngine;

namespace FirebaseSystem
{
    public static class FirebaseHandler
    {
        private const string GAME_VERSION = "v1";

        [System.Serializable]
        public class PlayerData
        {
            public string username;
            public List<StadisticsLog> statistics;

            public PlayerData(string username, List<StadisticsLog> statistics)
            {
                this.username = username;
                this.statistics = statistics;
            }
        }

        public static void StorePlayer(string uniqueID, string username, StadisticsLog[] logs)
        {
            List<Dictionary<string, object>> statisticsList = new List<Dictionary<string, object>>();

            foreach (var log in logs)
            {
                statisticsList.Add(new Dictionary<string, object>
                {
                    { "category", log.stadisticName },
                    { "log", log.log },
                    { "percentage", Mathf.Round(log.percentage * 10f) / 10f}
                });
            }

            Dictionary<string, object> data = new Dictionary<string, object>
            {
                { "username", username },
                { "gameHistory", statisticsList }
            };

            DatabaseReference reference = FirebaseDatabase.GetInstance("https://doctoralthesis-4ddda-default-rtdb.firebaseio.com/").RootReference;

            var dataEntry = $"players_{GAME_VERSION}_{uniqueID}";

            reference.Child(dataEntry).SetValueAsync(data);
        }
    }
}
