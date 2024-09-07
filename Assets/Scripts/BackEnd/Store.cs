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
        public static List<EffectNode> effectsNode { get; private set; }
        public static List<string> effects { get; private set; }
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
            changeDecks();
        }

        public static void AddEffectNode(EffectNode effect)
        {
            effectsNode.Add(effect);
        }

        public static void AddEffect(string effect)
        {
            effects.Add(effect);
        }

        public static EffectNode GetEffectNode(string name)
        {
            foreach (var effect in effectsNode)
            {
                if ((string)effect.Name.Evaluate(null, null, null) == name)
                {
                    return effect;
                }
            }
            throw new Exception($"{name} efecto no definido");
        }

        public static bool ConteinsCard(string name)
        {
            foreach (var deck in decks)
            {
                foreach (var card in deck.cards)
                {
                    if (card.name == name)
                        return true;
                }
            }
            return false;
        }

        public static bool ConteinsEffect(string name)
        {
            foreach (var effect in effectsNode)
            {
                if ((string)effect.Name.Evaluate(null, null, null) == name)
                    return true;
            }
            return false;
        }

        public static EffectNode FindEffect(string name)
        {
            foreach (var effect in effectsNode)
            {
                if ((string)effect.Name.Evaluate(null, null, null) == name)
                    return effect;
            }
            throw new Exception($"El effecto de nombre {name} no esta creado");
        }

        private static void LoadFromJSON()
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            };
            string path = Path.Combine(Application.dataPath, file);
            Debug.Log($"{path}");
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                StoreData storeData = JsonConvert.DeserializeObject<StoreData>(json, settings);
                decks = storeData.decks;
                effects = storeData.effects;
                ParseEffect();
                Debug.Log("Datos cargados correctamente desde el archivo JSON");
            }
            else
            {
                Debug.LogWarning("El archivo JSON no existe. Inicializando datos vacíos");
                decks = new List<Deck>();
                effects = new List<string>();
            }
        }

        private static void SaveFromJSON()
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            };
            string path = Path.Combine(Application.dataPath, file);
            StoreData storeData = new StoreData
            {
                decks = decks,
                effects = effects
            };
            string json = JsonConvert.SerializeObject(storeData, Formatting.Indented, settings);
            File.WriteAllText(path, json);
            Debug.Log("Datos guardados correctamente en el archivo JSON");
        }

        public static Deck GetDeck(Guid id)
        {
            return decks.Find(deck => deck.id == id);
        }

        private static void ParseEffect()
        {
            effectsNode = new List<EffectNode>();
            foreach (var effect in effects)
            {
                Lexer lexer = new Lexer(effect);
                Parser parser = new Parser(lexer.Analyze());
                EffectNode effectNode = parser.Parse() as EffectNode;
                effectsNode.Add(effectNode);
            }
        }
    }

    [Serializable]
    class StoreData
    {
        public List<Deck> decks;
        public List<string> effects;
    }
}