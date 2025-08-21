using UnityEngine;

namespace QuestSystem
{
    /// <summary>
    /// Defines the core data of a quest, including its identifier, name, description, 
    /// and the list of associated points of interest (POIs) the player must visit.
    /// </summary>
    [CreateAssetMenu]
    public class QuestData : ScriptableObject
    {
        public string questID;
        public string questName;
        public string questDescription;

        public string[] POI_Ids;
    }
}