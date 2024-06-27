using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Jujutsu_Kaisen_Game_Proyect.Assets.BackEnd
{
    public class Game
    {
        public List<TypeOfEffects> player1ActivesEffeccts;
        public List<TypeOfEffects> player2ActivesEffeccts;
        public Player winnerOfTheCurrentRound;
        public Board player1Board;
        public Board player2Board;
        public bool isPlayer1Sorcer;
        public Player activePlayer;
        public Player player1;
        public Player player2;
        public int player1Wins;
        public int player2Wins;
        public Deck sorcererDeck;
        public Deck curseDeck;
        public Game()
        {
            player1ActivesEffeccts = new List<TypeOfEffects>();
            player2ActivesEffeccts = new List<TypeOfEffects>();
            winnerOfTheCurrentRound = null;
            player1Wins = 0;
            player2Wins = 0;
            player1Board = new Board();
            player2Board = new Board();
            isPlayer1Sorcer = true;
            sorcererDeck = new Deck(DeckType.Sorcers);
            curseDeck = new Deck(DeckType.Curses);

            player1 = new Player(sorcererDeck.CreateHand());
            player2 = new Player(curseDeck.CreateHand());
            player1.isPlaying = true;
            player2.isPlaying = true;
            activePlayer = player1;
        }

        public void ChangePlayerDeck()
        {
            List<Card> handAux = player1.hand;
            player1.hand = player2.hand;
            player2.hand = handAux;
            isPlayer1Sorcer = !isPlayer1Sorcer;
        }

        public void Play(Guid cardId, BoardPosition boardPosition)
        {
            Card card = activePlayer.hand.Find(c => c.id == cardId);
            activePlayer.hand.Remove(card);
            if (boardPosition == BoardPosition.Mele)
            {
                PlayMeleCard(card);
            }

            if (boardPosition == BoardPosition.Range)
            {
                PlayRangeCard(card);
            }

            if (boardPosition == BoardPosition.Siege)
            {
                PlaySiegeCard(card);
            }

            if (boardPosition == BoardPosition.Expansion)
            {
                PlayExpansionCard(card);
            }

            if (boardPosition == BoardPosition.BuffMele)
            {
                PlayBuffMeleCard(card);
            }

            if (boardPosition == BoardPosition.BuffRange)
            {
                PlayBuffRangeCard(card);
            }

            if (boardPosition == BoardPosition.BuffSiege)
            {
                PlayBuffSiegeCard(card);
            }

            if (boardPosition == BoardPosition.Cementery)
            {
                PlayCementeryCard(card);
            }
        }

        public void ChangesActivePLayer()
        {
            activePlayer = (activePlayer == player1) ? player2 : player1;
        }

        public void PlayMeleCard(Card card)
        {
            if (activePlayer == player1)
            {
                player1Board.AddMeleCard(card);
                return;
            }
            player2Board.AddMeleCard(card);
        }

        public void PlayRangeCard(Card card)
        {
            if (activePlayer == player1)
            {
                player1Board.AddRangeCard(card);
                return;
            }
            player2Board.AddRangeCard(card);
        }

        public void PlaySiegeCard(Card card)
        {
            if (activePlayer == player1)
            {
                player1Board.AddSiegeCard(card);
                return;
            }
            player2Board.AddSiegeCard(card);
        }

        public void PlayExpansionCard(Card card)
        {
            if (activePlayer == player1)
            {
                player1Board.AddExpansionCard(card);
                return;
            }
            player2Board.AddExpansionCard(card);
        }

        public void PlayBuffMeleCard(Card card)
        {
            if (activePlayer == player1)
            {
                player1Board.AddBuffMeleCard(card);
                return;
            }
            player2Board.AddBuffMeleCard(card);
        }

        public void PlayBuffRangeCard(Card card)
        {
            if (activePlayer == player1)
            {
                player1Board.AddBuffRangeCard(card);
                return;
            }
            player2Board.AddBuffRangeCard(card);
        }

        public void PlayBuffSiegeCard(Card card)
        {
            if (activePlayer == player1)
            {
                player1Board.AddBuffSiegeCard(card);
                return;
            }
            player2Board.AddBuffSiegeCard(card);
        }

        public void PlayCementeryCard(Card card)
        {
            if (activePlayer == player1)
            {
                player1Board.AddCementeryCard(card);
                return;
            }
            player2Board.AddCementeryCard(card);
        }

        public int GetPointsInPlayer1Board()
        {
            return player1Board.GetBoardPoints();
        }

        public int GetPointsInPlayer2Board()
        {
            return player2Board.GetBoardPoints();
        }

        public int GetCardPower(Guid cardId)
        {
            Card foundCard = activePlayer.hand.Find(c => c.id == cardId);
            if (foundCard != null)
            {
                return foundCard.powerBase;
            }
            foundCard = player1Board.GetCard(cardId);
            if (foundCard != null)
            {
                return foundCard.currentPower;
            }
            foundCard = player2Board.GetCard(cardId);
            if (foundCard != null)
            {
                return foundCard.currentPower;
            }
            return 0;
        }

        public void StopPlaying()
        {
            if (activePlayer == player1)
            {
                player1.isPlaying = false;
                return;
            }
            player2.isPlaying = false;
        }

        public void RefrechBoard()
        {
            DrawCard();
            DeleteCard();
        }

        public void DeleteCard()
        {
            player1Board.RemoveCards();
            player2Board.RemoveCards();
        }

        public void DrawCard()
        {
            if (isPlayer1Sorcer)
            {
                player1.hand.Add(sorcererDeck.DrawCard());
                Swap(player1.hand);
                player1.hand.Add(sorcererDeck.DrawCard());
                Swap(player1.hand);
                player2.hand.Add(curseDeck.DrawCard());
                Swap(player2.hand);
                player2.hand.Add(curseDeck.DrawCard());
                Swap(player2.hand);
                return;
            }
            player1.hand.Add(curseDeck.DrawCard());
            Swap(player1.hand);
            player1.hand.Add(curseDeck.DrawCard());
            Swap(player1.hand);
            player2.hand.Add(sorcererDeck.DrawCard());
            Swap(player2.hand);
            player2.hand.Add(sorcererDeck.DrawCard());
            Swap(player2.hand);
        }

        public string GetTheWinnerOfTheRound()
        {
            int player1Points = player1Board.GetBoardPoints();
            int player2Points = player2Board.GetBoardPoints();
            if (player1Points == player2Points)
            {
                if (IsThisEffectActive(TypeOfEffects.TheVictoryInTheDraw, player1ActivesEffeccts) && !IsThisEffectActive(TypeOfEffects.TheVictoryInTheDraw, player2ActivesEffeccts))
                {
                    winnerOfTheCurrentRound = player1;
                    player1Wins++;
                    return "Player1 wins the round";
                }
                if (IsThisEffectActive(TypeOfEffects.TheVictoryInTheDraw, player2ActivesEffeccts) && !IsThisEffectActive(TypeOfEffects.TheVictoryInTheDraw, player1ActivesEffeccts))
                {
                    winnerOfTheCurrentRound = player2;
                    player2Wins++;
                    return "Player2 wins the round";
                }
                winnerOfTheCurrentRound = null;
                player1Wins++;
                player2Wins++;
                return "Draw";
            }
            if (player1Points > player2Points)
            {
                winnerOfTheCurrentRound = player1;
                player1Wins++;
                return "Player1 wins the round";
            }
            winnerOfTheCurrentRound = player2;
            player2Wins++;
            return "Player2 wins the round";
        }

        public bool IsThisEffectActive(TypeOfEffects typeOfEffects, List<TypeOfEffects> playerActivesEffeccts)
        {
            int i = 0;
            while (i < playerActivesEffeccts.Count && typeOfEffects != playerActivesEffeccts[i])
            {
                i++;
            }
            if (i < playerActivesEffeccts.Count)
            {
                playerActivesEffeccts.RemoveAt(i);
                return true;
            }
            return false;
        }

        public void AddEffect(TypeOfEffects typeOfEffects)
        {
            if (activePlayer == player1)
            {
                player1ActivesEffeccts.Add(typeOfEffects);
                return;
            }
            player2ActivesEffeccts.Add(typeOfEffects);
        }

        public void Swap(List<Card> hand)
        {
            (hand[^1], hand[0]) = (hand[0], hand[^1]);
        }

        public void ActiveBuffEffect(EffectPosition effectPosition)
        {
            Board board = activePlayer == player1 ? player1Board : player2Board;
            board.IncreasePowerBy(effectPosition, 2);
            board.effects.Add(new BoardEffect(BoardEffectType.Buff, effectPosition));
        }

        public void ActiveYutaEffect(EffectPosition effectPosition)
        {
            Board board = activePlayer == player1 ? player1Board : player2Board;
            board.IncreasePowerBy(effectPosition, 1);
        }

        public Card ActiveHigurumaEffect()
        {
            Board board = activePlayer == player1 ? player2Board : player1Board;
            Card card = board.GetWeakCard();
            if (card == null)
            {
                return null;
            }
            board.RemoveCard(card.id);
            return card;
        }

        public void ActiveSukunaEffect(EffectPosition effectPosition)
        {
        }

        public void ActiveHakariEffect(int cant, Card card)
        {
            card.currentPower += cant;
        }

        public void ActiveBuggiBuggiEffect(Card cardInBoard)
        {
            SendCardToHand(cardInBoard);
            if (activePlayer == player1)
            {
                player1Board.ChangesCards(cardInBoard.id);
                return;
            }
            player2Board.ChangesCards(cardInBoard.id);
            return;
        }

        public List<Card> ActiveKashimoEffect()
        {
            List<Card> result = new List<Card>();
            Card player1Card = player1Board.GetMaxCard();
            if (player1Card != null)
            {
                result.Add(player1Card);
                player1Board.RemoveCard(player1Card.id);
            }

            Card player2Card = player2Board.GetMaxCard();
            if (player2Card != null)
            {
                result.Add(player2Card);
                player2Board.RemoveCard(player2Card.id);
            }
            return result;
        }

        public Card ActiveMahitoEffect()
        {
            if (activePlayer == player1)
            {
                if (isPlayer1Sorcer)
                {
                    sorcererDeck.DrawCard();
                    return player1.hand[player1.hand.Count - 1];
                }
                curseDeck.DrawCard();
                return player1.hand[player1.hand.Count - 1];
            }

            if (isPlayer1Sorcer)
            {
                curseDeck.DrawCard();
                return player2.hand[player2.hand.Count - 1];
            }
            sorcererDeck.DrawCard();
            return player2.hand[player2.hand.Count - 1];
        }

        public void ActiveKenjakuEffect(Card card)
        {
            int cant = (activePlayer == player1) ? player1Board.GetCountSameCard(card) : player2Board.GetCountSameCard(card);
            card.currentPower += card.currentPower * (cant - 1);
        }

        public void SendCardToHand(Card card)
        {
            card.currentPower = card.powerBase;
            activePlayer.hand.Add(card);
        }

        public void ActiveExpansionHakariEffect(EffectPosition effectPosition)
        {
            player1Board.IncreasePowerBy(effectPosition, -1);
            player2Board.IncreasePowerBy(effectPosition, -1);
            player1Board.effects.Add(new BoardEffect(BoardEffectType.ExpansionHakari, effectPosition));
            player2Board.effects.Add(new BoardEffect(BoardEffectType.ExpansionHakari, effectPosition));
        }

        public void ActiveExpansionMahitoEffect(EffectPosition effectPosition)
        {
            player1Board.IncreasePowerBy(effectPosition, -1);
            player2Board.IncreasePowerBy(effectPosition, -1);
            player1Board.effects.Add(new BoardEffect(BoardEffectType.ExpansionMahito, effectPosition));
            player2Board.effects.Add(new BoardEffect(BoardEffectType.ExpansionMahito, effectPosition));
        }

        public void ActiveExpansionJogoEffect(EffectPosition effectPosition)
        {
            player1Board.IncreasePowerBy(effectPosition, -1);
            player2Board.IncreasePowerBy(effectPosition, -1);
            player1Board.effects.Add(new BoardEffect(BoardEffectType.ExpansionJogo, effectPosition));
            player2Board.effects.Add(new BoardEffect(BoardEffectType.ExpansionJogo, effectPosition));
        }

        public void ActiveExpansionYutaEffect(EffectPosition effectPosition)
        {
            player1Board.IncreasePowerBy(effectPosition, -2);
            player2Board.IncreasePowerBy(effectPosition, -2);
            player1Board.effects.Add(new BoardEffect(BoardEffectType.ExpansionYuta, effectPosition));
            player2Board.effects.Add(new BoardEffect(BoardEffectType.ExpansionYuta, effectPosition));
        }

        public void ActiveExpansionSukunaEffect(EffectPosition effectPosition)
        {
            player1Board.IncreasePowerBy(effectPosition, -2);
            player2Board.IncreasePowerBy(effectPosition, -2);
            player1Board.effects.Add(new BoardEffect(BoardEffectType.ExpansionSukuna, effectPosition));
            player2Board.effects.Add(new BoardEffect(BoardEffectType.ExpansionSukuna, effectPosition));
        }

        public void DesactiveExpansionEffect()
        {
            player1Board.RemoveExpansionEffects();
            player2Board.RemoveExpansionEffects();
        }
    }
}