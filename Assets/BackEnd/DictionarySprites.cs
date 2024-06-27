using UnityEngine;
using System.Collections.Generic;
using Sprite = UnityEngine.Sprite;
using Jujutsu_Kaisen_Game_Proyect.Assets.BackEnd;


namespace Dictionary
{
    public class DictionarySprites
    {
        public Dictionary<string, Sprite> spritesDisponibles;

        public void SafeResources()
        {
            spritesDisponibles = new Dictionary<string, Sprite>
            {
                { "Itadori", Resources.Load<Sprite>("Itadori") },
                { "Gojo", Resources.Load<Sprite>("Gojo") },
                { "Sukuna", Resources.Load<Sprite>("Sukuna") },
                { "Todou", Resources.Load<Sprite>("Todou") },
                { "Nanami", Resources.Load<Sprite>("Nanami") },
                { "Fushiguro", Resources.Load<Sprite>("Fushiguro") },
                { "Shoko", Resources.Load<Sprite>("Shoko") },
                { "Jogo", Resources.Load<Sprite>("Jogo") },
                { "Hanami", Resources.Load<Sprite>("Hanami") }
            };
        }
    }
}