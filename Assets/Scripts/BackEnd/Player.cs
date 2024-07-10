using System;

namespace Console
{
    public class Player
    {
        public bool isPlaying;
        public Graveyard graveyard;
        public Deck deck;
        public Hand hand;
        public Guid id;
        public Field field;
        public Player(Deck deck)
        {
            graveyard = new Graveyard();
            isPlaying = true;
            this.deck = deck;
            id = Guid.NewGuid();
            field = new Field();
            SaveIdInTheCards();
        }

        public void GenerateHand()
        {
            hand = deck.GenerateHand();
        }

        private void SaveIdInTheCards()
        {
            deck.SaveID(id);
        }
    }
}