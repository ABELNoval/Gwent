using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

namespace Console
{
    [Serializable]
    public static class Store
    {
        public const string file = "dataGame.json";
        public static List<Deck> decks { get; private set; }
        public static List<Effect> effects { get; private set; }
        public delegate void ChangeDecks();
        public static event ChangeDecks changeDecks;

        static Store()
        {
            LoadFromJSON();
        }

        public static void AddOrEditDeck(Deck deck)
        {
            Deck deck1 = GetDeck(deck.id);
            if (deck1 != null)
            {
                deck1 = deck;
            }
            else
            {
                decks.Add(deck);
            }
            SaveFromJSON();
            LoadFromJSON();
            changeDecks();
        }

        public static void AddEffect(Effect effect)
        {
            throw new NotImplementedException();
        }

        private static void LoadFromJSON()
        {
            string path = Path.Combine(Application.dataPath, file);
            Debug.Log($"{path}");
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                StoreData storeData = JsonConvert.DeserializeObject<StoreData>(json);
                decks = storeData.decks;
                effects = storeData.effects;
                Debug.Log("Datos cargados correctamente desde el archivo JSON");
            }
            else
            {
                Debug.LogWarning("El archivo JSON no existe. Inicializando datos vacíos");
                decks = new List<Deck>();
                effects = new List<Effect>();
            }
        }

        private static void SaveFromJSON()
        {
            string path = Path.Combine(Application.dataPath, file);
            StoreData storeData = new StoreData
            {
                decks = decks,
                effects = effects
            };
            string json = JsonConvert.SerializeObject(storeData, Formatting.Indented);
            File.WriteAllText(path, json);
            Debug.Log("Datos guardados correctamente en el archivo JSON");
        }

        public static Deck GetDeck(Guid id)
        {
            return decks.Find(deck => deck.id == id);
        }
    }

    [Serializable]
    class StoreData
    {
        public List<Deck> decks;
        public List<Effect> effects;
    }
}