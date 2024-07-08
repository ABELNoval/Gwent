using System;
using System.Collections;
using System.Collections.Generic;
using Jujutsu_Kaisen_Game_Proyect.Assets.BackEnd;
using UnityEngine;

namespace Console
{
    [Serializable]
    public class Cards
    {
        public Guid owner { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string description { get; set; }
        public string faction { get; set; }
        public List<string> range { get; set; }
        public int attack { get; set; }
        public List<string> effectsName { get; set; }
        public string img { get; set; }

        public Cards(string name, string type, string description, string faction, List<string> range, int attack, List<string> effectsName, string img)
        {
            this.name = name;
            this.type = type;
            this.description = description;
            this.range = range;
            this.faction = faction;
            this.attack = attack;
            this.effectsName = effectsName;
            this.img = img;
        }
        public void OnActivation(List<(Effect, Selector)> effects, Context context)
        {
            /*if (effects[0].Item2.IsAPosActivation() || effects[0].Item2 == null)
            {
                throw new Exception("Ese efecto anda como tu, sin padres");
            }
            for (int i = 0; i < effects.Count; i++)
            {
                effects[i].Item1.Activation(effects[i].Item2, context);
            }*/
        }
    }
}