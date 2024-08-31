using System;
using System.Collections.Generic;
using UnityEngine;

namespace Console
{
    public abstract class GameComponent
    {

        public delegate void RemoveCard(Player player, Cards card);
        public delegate void UpdateInterface(Player player);
        public System.Random random = new System.Random();
        public Player owner;
        public List<Cards> cards = new List<Cards>();
        public abstract void Push(Cards card);
        public abstract Cards Pop();
        public abstract void SendBottom(Cards card);
        public abstract void Remove(Cards card);
        public abstract List<Cards> Find(Predicate<Cards> predicate);
        public abstract void Shuffle();
    }
}