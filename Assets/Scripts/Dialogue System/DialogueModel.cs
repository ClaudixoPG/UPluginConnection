using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem
{
    /// <summary>
    /// ScriptableObject that defines a dialogue model for the Dialogue System.
    /// Stores an ID and a collection of dialogue entries, each with text,
    /// background, character, and possible options.
    /// </summary>
    [CreateAssetMenu(menuName = "Dialogue/Dialogue Model")]
    public class DialogueModel : ScriptableObject
    {
        /// <summary>
        /// Represents a single dialogue entry, including the message text,
        /// optional background, optional character sprite, and a list of dialogue options.
        /// </summary>
        [System.Serializable]
        public struct DialogueData
        {
            /// <summary>
            /// The message text shown in this dialogue entry.
            /// </summary>
            public string message;

            /// <summary>
            /// The background sprite displayed during this dialogue.
            /// </summary>
            public Sprite background;

            /// <summary>
            /// The character sprite displayed with this dialogue.
            /// </summary>
            public Sprite character;

            /// <summary>
            /// List of dialogue options that the player can choose from at this point.
            /// </summary>
            public List<DialogueOptionData> dalogueOptions;
        }

        /// <summary>
        /// Represents a simplified dialogue option, containing only the message text.
        /// Can be used for lightweight option handling.
        /// </summary>
        [System.Serializable]
        public struct DialogueOptionData
        {
            /// <summary>
            /// The message text for this option.
            /// </summary>
            public string message;
        }

        /// <summary>
        /// Unique identifier for this dialogue model.
        /// </summary>
        public string dialogueID;

        /// <summary>
        /// The list of dialogues that make up this dialogue model.
        /// </summary>
        public List<DialogueData> dialogues = new List<DialogueData>();
    }
}