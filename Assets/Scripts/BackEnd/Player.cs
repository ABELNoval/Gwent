using System;

namespace Console
{
    public class Player
    {
        public delegate void RemoveCard(Player player, Cards card);
        public delegate void UpdateHand(Player player);
        public event UpdateHand updateHand;
        public event RemoveCard removeCard;
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
            field.removeCard += RemoveCardOnField;
            SaveIdInTheCards();
        }

        public void GenerateHand()
        {
            hand = deck.GenerateHand();
            hand.updateInterface += Update;
        }

        private void SaveIdInTheCards()
        {
            deck.SaveID(id);
        }

        private void RemoveCardOnField(Player player, Cards card)
        {
            removeCard(player, card);
        }

        private void Update(Player player)
        {
            updateHand(player);
        }
    }
}