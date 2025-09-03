using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace SaveSystem
{
    /// <summary>
    /// Handles saving and loading of persistent game data for the application.
    /// Provides binary serialization for improved data security.
    /// </summary>
    public static class SaveHandler
    {
        /// <summary>
        /// Default file name used for storing game data.
        /// </summary>
        public const string DEFAULT_DATA_FILE_NAME = "gamedata.dat";

        private static GameData _currentGameData;

        public static bool GameDataExists()
        {
            if (_currentGameData == null)
            {
                _currentGameData = LoadOrCreate();

                return _currentGameData.username != string.Empty;
            }
            else
            {
                return _currentGameData.username != string.Empty;
            }
        }

        /// <summary>
        /// Saves the current game data to disk. 
        /// If no data exists, it will create a new one before saving.
        /// </summary>
        public static void Save()
        {
            if (_currentGameData == null)
            {
                // Try to load
                _currentGameData = LoadOrCreate();
            }

            string path = GetFilePath();
            try
            {
                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, _currentGameData);
                }
#if UNITY_EDITOR
                Debug.Log($"[SaveHandler] Game data saved at: {path}");
#endif
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveHandler] Failed to save data: {e.Message}");
            }
        }

        /// <summary>
        /// Loads game data from disk into memory. 
        /// If no file is found, a new data file will be created.
        /// </summary>
        public static void Load()
        {
            _currentGameData = LoadOrCreate();
        }

        /// <summary>
        /// Gets the current game data in memory. 
        /// If none is loaded, it will load or create it first.
        /// </summary>
        /// <returns>The current GameData instance.</returns>
        public static GameData GetGameData()
        {
            if (_currentGameData == null)
            {
                _currentGameData = LoadOrCreate();
            }

            return _currentGameData;
        }

        /// <summary>
        /// Deletes the saved game data from disk and clears it from memory.
        /// </summary>
        public static void Delete()
        {
            string path = GetFilePath();
            if (File.Exists(path))
            {
                File.Delete(path);
#if UNITY_EDITOR
                Debug.Log("[SaveHandler] Game data deleted.");
#endif
            }
            _currentGameData = null;
        }

        /// <summary>
        /// Loads the game data if it exists, otherwise creates and saves a new one.
        /// </summary>
        /// <returns>A valid GameData instance.</returns>
        private static GameData LoadOrCreate()
        {
            string path = GetFilePath();

            if (File.Exists(path))
            {
                try
                {
                    using (FileStream stream = new FileStream(path, FileMode.Open))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        var data = (GameData)formatter.Deserialize(stream);
                        return data;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"[SaveHandler] Failed to load data. Creating new one. Error: {e.Message}");
                }
            }
            
            return CreateNewGameData(string.Empty, -1);
        }


        /// <summary>
        /// Create and save new data, this functions will override exiting data.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="age"></param>
        /// <returns></returns>
        public static GameData CreateNewGameData(string username, int age)
        {
            GameData newData = new GameData(username, age);
            _currentGameData = newData;
            Save(); 

            return newData;
        }

        /// <summary>
        /// Returns the full path to the save file based on the persistent data directory.
        /// </summary>
        private static string GetFilePath()
        {
            return Path.Combine(Application.persistentDataPath, DEFAULT_DATA_FILE_NAME);
        }
    }
}
