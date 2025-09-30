using Firebase.Extensions;
using Firebase.Firestore;
using Firebase;
using Firebase.Database;
using SaveSystem;
using System.Collections.Generic;
using UnityEngine;

public static class FirebaseHandler
{
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

    public static void StorePlayer(string playerId, StadisticsLog[] logs)
    {
        List<Dictionary<string, object>> statisticsList = new List<Dictionary<string, object>>();
        
        foreach (var log in logs)
        {
            statisticsList.Add(new Dictionary<string, object>
        {
            { "stadisticName", log.stadisticName },
            { "log", log.log },
            { "percentage", log.percentage }
        });
        }

        Dictionary<string, object> data = new Dictionary<string, object>
        {
            { "username", playerId },
            { "statistics", statisticsList }
        };

        DatabaseReference reference = FirebaseDatabase.GetInstance("https://doctoralthesis-4ddda-default-rtdb.firebaseio.com/").RootReference;

        reference.Child("players_V1").Child(playerId).SetValueAsync(data);
    }
}
