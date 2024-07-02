using System.Collections.Generic;
using UnityEngine;

namespace Console
{
    public delegate void PlayCard(GameObject card);
    public enum FieldPosition
    {
        Melee,
        Range,
        Siege,
        Graveyard,
        BuffMelee,
        BuffRange,
        BuffSiege,
        Climate
    }
    public class Field
    {

        List<GameObject> Melee = new List<GameObject>();
        List<GameObject> range = new List<GameObject>();
        List<GameObject> siege = new List<GameObject>();
        List<GameObject> graveyard = new List<GameObject>();
        List<GameObject> climate = new List<GameObject>();
        List<GameObject> buffMelee = new List<GameObject>();
        List<GameObject> buffRange = new List<GameObject>();
        List<GameObject> buffSiege = new List<GameObject>();

        public void AddCard(GameObject card, FieldPosition fieldPosition)
        {
            if (fieldPosition == FieldPosition.Melee)
            {
                AddMeleeCard(card);
                return;
            }
            if (fieldPosition == FieldPosition.Range)
            {
                AddRangeCard(card);
                return;
            }
            if (fieldPosition == FieldPosition.Siege)
            {
                AddSiegeCard(card);
                return;
            }
            if (fieldPosition == FieldPosition.Climate)
            {
                AddClimateCard(card);
                return;
            }
            if (fieldPosition == FieldPosition.Graveyard)
            {
                SendToGraveyard(card);
                return;
            }
        }
        public void AddMeleeCard(GameObject card)
        {
            Melee.Add(card);
        }
        public void AddSiegeCard(GameObject card)
        {
            siege.Add(card);
        }
        public void AddRangeCard(GameObject card)
        {
            range.Add(card);
        }
        public void AddClimateCard(GameObject card)
        {
            climate.Add(card);
        }
        public void SendToGraveyard(GameObject card)
        {
            graveyard.Add(card);
        }
    }
}