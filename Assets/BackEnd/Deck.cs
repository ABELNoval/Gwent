using System;
using UnityEngine;
using System.Collections.Generic;

namespace Jujutsu_Kaisen_Game_Proyect.Assets.BackEnd
{
    public enum DeckType
    {
        Sorcers,
        Curses
    }

    public class BossEffect
    {
        public string description;
        public TypeOfEffects typeOfEffect;

        public BossEffect(string description, TypeOfEffects typeOfEffect)
        {
            this.description = description;
            this.typeOfEffect = typeOfEffect;
        }
    }
    public class Deck
    {
        public List<BossEffect> bossEffects = new List<BossEffect>()
        {
            new BossEffect("Mantener una carta aleatoria en el campo entre cada ronda", TypeOfEffects.SafeCard),
            new BossEffect("Un robo extra en la siguiente ronda", TypeOfEffects.DrawOneMore),
            new BossEffect("Ganar en caso de empate y evitar que el rival obtenga punto", TypeOfEffects.TheVictoryInTheDraw)
        };
        Card card;
        public List<Card> cards = new List<Card>();
        System.Random random = new System.Random();
        public Deck(DeckType deckType)
        {
            cards = (deckType == DeckType.Sorcers) ? CreateSorcererDeck() : CreateCurseDeck();
        }
        public List<Card> CreateHand()
        {
            List<Card> hand = new List<Card>();
            int a;
            for (int i = 0; i < 10; i++)
            {
                a = random.Next(0, cards.Count);
                hand.Add(cards[a]);
                cards.Remove(cards[a]);
            }
            return hand;
        }

        public Card DrawCard()
        {
            int a = 0;
            a = random.Next(0, cards.Count);
            card = cards[a];
            cards.Remove(cards[a]);
            return card;
        }

        public Card CreateBossCard()
        {
            int i = random.Next(0, bossEffects.Count);
            return new BossCard("Tengen", "", Resources.Load<Sprite>("Art/Images/Tengen"), CardPos.BossPosition, TypeOfCard.BossCard, bossEffects[i].description, bossEffects[i].typeOfEffect);
        }
        public static List<Card> CreateSorcererDeck()
        {
            List<Card> deck = new List<Card>();

            //Hechiceros
            deck.Add(new ComunCard("Itadori Yuji", "Hechicero de primer grado de la escuela de Jujutsu en Tokio, es el recipiente del Rey de las Maldiciones, Ryomen Sukuna. Es conocido por su gran dominio del Black Flash y sus capacidades fisicas", Resources.Load<Sprite>("Art/Images/Itadori"), CardPos.Mele, TypeOfCard.ComunCard, "Sorcer", 5));
            deck.Add(new ComunCard("Itadori Yuji", "Hechicero de primer grado de la escuela de Jujutsu en Tokio, es el recipiente del Rey de las Maldiciones, Ryomen Sukuna. Es conocido por su gran dominio del Black Flash y sus capacidades fisicas", Resources.Load<Sprite>("Art/Images/Itadori"), CardPos.Mele, TypeOfCard.ComunCard, "Sorcer", 5));
            deck.Add(new ComunCard("Itadori Yuji", "Hechicero de primer grado de la escuela de Jujutsu en Tokio, es el recipiente del Rey de las Maldiciones, Ryomen Sukuna. Es conocido por su gran dominio del Black Flash y sus capacidades fisicas", Resources.Load<Sprite>("Art/Images/Itadori"), CardPos.Mele, TypeOfCard.ComunCard, "Sorcer", 5));
            deck.Add(new GoldCard("Gojo", "El hechicero mas fuerte de la epoca actual, es maestro de la escuela de Jujutsu, es portador de los seis ojos y el infinito, heredados de su lineaje y es considerado un prodigio sin igual", Resources.Load<Sprite>("Art/Images/Gojo"), CardPos.MeleAndRangeAndSiege, TypeOfCard.GoldCard, "Sorcer", "Limpia la fila del campo no vacia del rival con menos unidades", 10, TypeOfEffects.Gojo));
            deck.Add(new ComunCard("Fushiguro", "Hechicero de primer grado de la escuela de Jujutsu en Tokio, es el portador de Las Diez Sombras, una hablidad heredada de su clan que le permite invocar varios shikigamis a voluntad", Resources.Load<Sprite>("Art/Images/Megumi"), CardPos.Range, TypeOfCard.ComunCard, "Sorcer", 6));
            deck.Add(new ComunCard("Fushiguro", "Hechicero de primer grado de la escuela de Jujutsu en Tokio, es el portador de Las Diez Sombras, una hablidad heredada de su clan que le permite invocar varios shikigamis a voluntad", Resources.Load<Sprite>("Art/Images/Megumi"), CardPos.Range, TypeOfCard.ComunCard, "Sorcer", 6));
            deck.Add(new ComunCard("Nanami", "Hechicero de primer grado de la ecuela de JuJutsu en Tokio, es un gran profesor aunque odia el trbajo e incumplir sus horarios, es un hechicero de gran nivel cuya tecnica herradica en golpear los puntos criticos de sus oponentes", Resources.Load<Sprite>("Art/Images/Nanami"), CardPos.Mele, TypeOfCard.ComunCard, "Sorcer", 5));
            deck.Add(new ComunCard("Nanami", "Hechicero de primer grado de la ecuela de JuJutsu en Tokio, es un gran profesor aunque odia el trbajo e incumplir sus horarios, es un hechicero de gran nivel cuya tecnica herradica en golpear los puntos criticos de sus oponentes", Resources.Load<Sprite>("Art/Images/Nanami"), CardPos.Mele, TypeOfCard.ComunCard, "Sorcer", 5));
            deck.Add(new PlatCard("Yuta", "Hechicero de grado especial de la escuela de Jujutsu en Tokio, es considerado el mas fuerte despues de Satoru Gojo, es un gran prodigio con la capacidad de copiar tecnicas de otros hechiceros y posee como su shikigami personal a la llamda Reina de las Maldiciones de grado especial", Resources.Load<Sprite>("Art/Images/Yuta"), CardPos.MeleAndSiege, TypeOfCard.PlatCard, "Sorcer", "Aumenta en 1 las cartas de la fila donde se coloca", 8, TypeOfEffects.Yuta));
            deck.Add(new PlatCard("Yuta", "Hechicero de grado especial de la escuela de Jujutsu en Tokio, es considerado el mas fuerte despues de Satoru Gojo, es un gran prodigio con la capacidad de copiar tecnicas de otros hechiceros y posee como su shikigami personal a la llamda Reina de las Maldiciones de grado especial", Resources.Load<Sprite>("Art/Images/Yuta"), CardPos.MeleAndSiege, TypeOfCard.PlatCard, "Sorcer", "Aumenta en 1 las cartas de la fila donde se coloca", 8, TypeOfEffects.Yuta));
            deck.Add(new PlatCard("Hakari", "Hechicero de grado especial de la escuela de Jujutsu en Tokio, le gustan las apuestas y es conocido por su gran suerte y capacidad fisica", Resources.Load<Sprite>("Art/Images/Hakari"), CardPos.Mele, TypeOfCard.PlatCard, "Sorcer", "Cuando es invocado Hakari tirara un dado que, en dependencia del nurmeo que caiga este recibira ese ataque extra", 4, TypeOfEffects.Hakari));
            deck.Add(new PlatCard("Hakari", "Hechicero de grado especial de la escuela de Jujutsu en Tokio, le gustan las apuestas y es conocido por su gran suerte y capacidad fisica", Resources.Load<Sprite>("Art/Images/Hakari"), CardPos.Mele, TypeOfCard.PlatCard, "Sorcer", "Cuando es invocado Hakari tirara un dado que, en dependencia del nurmeo que caiga este recibira ese ataque extra", 4, TypeOfEffects.Hakari));
            deck.Add(new PlatCard("Higuruma", "Tiene dos meses de experiencia como hechicero y es considerado un genio debido al gran avance que consiguio en tan poco timpo. Es un abogado cansado de las injusticias de este mundo", Resources.Load<Sprite>("Art/Images/Higuruma"), CardPos.Mele, TypeOfCard.PlatCard, "Sorcer", "Eliminar la carta con menos poder del rival", 5, TypeOfEffects.Higuruma));
            deck.Add(new PlatCard("Higuruma", "Tiene dos meses de experiencia como hechicero y es considerado un genio debido al gran avance que consiguio en tan poco timpo. Es un abogado cansado de las injusticias de este mundo", Resources.Load<Sprite>("Art/Images/Higuruma"), CardPos.Mele, TypeOfCard.PlatCard, "Sorcer", "Eliminar la carta con menos poder del rival", 5, TypeOfEffects.Higuruma));
            deck.Add(new ComunCard("Toji Fushiguro", "Su trabajo radica en la caza de Hechiceros y en varias ocaciones se le ha visto enfrentarse contra maldiciones. Posee una restriccion celestial que le permite hacer frente a los pinaculos de la hechiceria sin poseer energia maldita ", Resources.Load<Sprite>("Art/Images/Toji"), CardPos.Mele, TypeOfCard.ComunCard, "Sorcer Hunter", 7));
            deck.Add(new ComunCard("Maki", "Hechicera de primera clase de la escuela de Jujutsu en Tokio. Posee una restriccion celestial que le da unas capacidades sobre humanas y posee una gran destreza con las armas malditas", Resources.Load<Sprite>("Art/Images/Maki"), CardPos.MeleAndSiege, TypeOfCard.ComunCard, "Sorcer", 7));
            deck.Add(new ComunCard("Maki", "Hechicera de primera clase de la escuela de Jujutsu en Tokio. Posee una restriccion celestial que le da unas capacidades sobre humanas y posee una gran destreza con las armas malditas", Resources.Load<Sprite>("Art/Images/Maki"), CardPos.MeleAndSiege, TypeOfCard.ComunCard, "Sorcer", 7));

            //Cartas especiales
            deck.Add(new SpecialCard("Vacio Infinito", "Expansion de dominio de Gojo Satoru", Resources.Load<Sprite>("Art/Images/Expansion de Gojo"), CardPos.Expansion, TypeOfCard.SpecialCard, "Hace que la ronda termine en dos turnos y evita que el usuario de esta expansion juegue en ese tiempo", TypeOfEffects.Gojo));
            deck.Add(new SpecialCard("Amor mutuo y verdadero", "Expansion de Yuta", Resources.Load<Sprite>("Art/Images/Expansion de Yuta"), CardPos.Expansion, TypeOfCard.SpecialCard, "Disminuya en 2 el ataque de las cartas que se encuentran en una fila en especifico para ambos jugadores", TypeOfEffects.ExpansionYuta));
            deck.Add(new SpecialCard("RondaMortal", "Expansion de Hakari", Resources.Load<Sprite>("Art/Images/Expansion de Hakari"), CardPos.Expansion, TypeOfCard.SpecialCard, "Disminuya en 1 el ataque de las cartas que se encuentran en una fila en especifico para ambos jugadores", TypeOfEffects.ExpansionHakari));
            deck.Add(new SpecialCard("Buff de energia maldita", "Potencia la energia maldita de los hechiceros", Resources.Load<Sprite>("Art/Images/Energia Maldita(potenciador)"), CardPos.Buff, TypeOfCard.SpecialCard, "Aumenta en 2 el ataque de las tropas que se encuentren en la fila seleccionada", TypeOfEffects.Buff));
            deck.Add(new SpecialCard("Buff de energia maldita", "Potencia la energia maldita de los hechiceros", Resources.Load<Sprite>("Art/Images/Energia Maldita(potenciador)"), CardPos.Buff, TypeOfCard.SpecialCard, "Aumenta en 2 el ataque de las tropas que se encuentren en la fila seleccionada", TypeOfEffects.Buff));
            deck.Add(new SpecialCard("Buff de energia maldita", "Potencia la energia maldita de los hechiceros", Resources.Load<Sprite>("Art/Images/Energia Maldita(potenciador)"), CardPos.Buff, TypeOfCard.SpecialCard, "Aumenta en 2 el ataque de las tropas que se encuentren en la fila seleccionada", TypeOfEffects.Buff));
            deck.Add(new SpecialCard("Buggi Buggi", "Ante los sonidos de aplausos, dos hechiceros cambian de lugar", Resources.Load<Sprite>("Art/Images/Buggi Buggi"), CardPos.MeleAndRangeAndSiege, TypeOfCard.SpecialCard, "Cambia una carta del campo por ella misma teniendo 0 de ataque", TypeOfEffects.Lure));
            deck.Add(new SpecialCard("Impacto externo", "Los dominios son debiles por fuera, esta carta aprovecha esao", Resources.Load<Sprite>("Art/Images/Impacto externo"), CardPos.Clearance, TypeOfCard.SpecialCard, "Destruye todo dominio activo", TypeOfEffects.Despeje));
            deck.Add(new SpecialCard("Impacto externo", "Los dominios son debiles por fuera, esta carta aprovecha esao", Resources.Load<Sprite>("Art/Images/Impacto externo"), CardPos.Clearance, TypeOfCard.SpecialCard, "Destruye todo dominio activo", TypeOfEffects.Despeje));

            return deck;
        }

        public static List<Card> CreateCurseDeck()
        {
            List<Card> deck = new List<Card>();

            deck.Add(new ComunCard("Jogo", "Maldicion de grado especial. Jogo cree que las maldiciones eran los verdaderos humanos y deseaba un mundo donde los de su especie dominaran la tierra", Resources.Load<Sprite>("Art/Images/Jogo"), CardPos.RangeAndSiege, TypeOfCard.ComunCard, "Curse", 6));
            deck.Add(new ComunCard("Jogo", "Maldicion de grado especial. Jogo cree que las maldiciones eran los verdaderos humanos y deseaba un mundo donde los de su especie dominaran la tierra", Resources.Load<Sprite>("Art/Images/Jogo"), CardPos.RangeAndSiege, TypeOfCard.PlatCard, "Curse", 6));
            deck.Add(new GoldCard("Ryomen Sukuna", "...", Resources.Load<Sprite>("Art/Images/Ryomen Sukuna"), CardPos.MeleAndRangeAndSiege, TypeOfCard.GoldCard, "Curse", "Caclula el promedio de poder entre todas las cartas del campo (propio o del rival). Luego iguala el poder de todas las cartas del campo(propias o del rival) a ese promedio", 10, TypeOfEffects.Sukuna));
            deck.Add(new ComunCard("Hanami", "...", Resources.Load<Sprite>("Art/Images/Hanami"), CardPos.Range, TypeOfCard.ComunCard, "Curse", 5));
            deck.Add(new ComunCard("Hanami", "...", Resources.Load<Sprite>("Art/Images/Hanami"), CardPos.Range, TypeOfCard.ComunCard, "Curse", 5));
            deck.Add(new ComunCard("Hanami", "...", Resources.Load<Sprite>("Art/Images/Hanami"), CardPos.Range, TypeOfCard.ComunCard, "Curse", 5));
            deck.Add(new ComunCard("Dagon", "...", Resources.Load<Sprite>("Art/Images/Dagon"), CardPos.Mele, TypeOfCard.ComunCard, "Curse", 5));
            deck.Add(new ComunCard("Dagon", "...", Resources.Load<Sprite>("Art/Images/Dagon"), CardPos.Mele, TypeOfCard.ComunCard, "Curse", 5));
            deck.Add(new ComunCard("Dagon", "...", Resources.Load<Sprite>("Art/Images/Dagon"), CardPos.Mele, TypeOfCard.ComunCard, "Curse", 5));
            deck.Add(new PlatCard("Kenjaku", "...", Resources.Load<Sprite>("Art/Images/Kenjaku"), CardPos.RangeAndSiege, TypeOfCard.PlatCard, "Curse", "Multiplica por n su ataque, siendo n la cantidad de cartas iguales a ella en el campo", 6, TypeOfEffects.Kenjaku));
            deck.Add(new PlatCard("Kenjaku", "...", Resources.Load<Sprite>("Art/Images/Kenjaku"), CardPos.RangeAndSiege, TypeOfCard.PlatCard, "Curse", "Multiplica por n su ataque, siendo n la cantidad de cartas iguales a ella en el campo", 6, TypeOfEffects.Kenjaku));
            deck.Add(new ComunCard("Toji Fushiguro", "Su trabajo radica en la caza de Hechiceros y en varias ocaciones se le ha visto enfrentarse contra maldiciones. Posee una restriccion celestial que le permite hacer frente a los pinaculos de la hechiceria sin poseer energia maldita ", Resources.Load<Sprite>("Art/Images/Toji"), CardPos.Mele, TypeOfCard.ComunCard, "Sorcer Hunter", 7));
            deck.Add(new PlatCard("Mahito", "...", Resources.Load<Sprite>("Art/Images/Mahito"), CardPos.MeleAndRange, TypeOfCard.PlatCard, "Curse", "Robar una carta", 7, TypeOfEffects.Mahito));
            deck.Add(new PlatCard("Mahito", "...", Resources.Load<Sprite>("Art/Images/Mahito"), CardPos.MeleAndRange, TypeOfCard.PlatCard, "Curse", "Robar una carta", 7, TypeOfEffects.Mahito));
            deck.Add(new ComunCard("Categoria Especial", "...", Resources.Load<Sprite>("Art/Images/Categoria Especial"), CardPos.Mele, TypeOfCard.ComunCard, "Curse", 3));
            deck.Add(new ComunCard("Categoria Especial", "...", Resources.Load<Sprite>("Art/Images/Categoria Especial"), CardPos.Mele, TypeOfCard.ComunCard, "Curse", 3));
            deck.Add(new PlatCard("Kashimo", "...", Resources.Load<Sprite>("Art/Images/Kashimo"), CardPos.Siege, TypeOfCard.PlatCard, "Curse", "Elimina la carta con mas poder de ambos jugadores", 7, TypeOfEffects.Kashimo));
            deck.Add(new PlatCard("Kashimo", "...", Resources.Load<Sprite>("Art/Images/Kashimo"), CardPos.Siege, TypeOfCard.PlatCard, "Curse", "Elimina la carta con mas poder de ambos jugadores", 7, TypeOfEffects.Kashimo));

            //Cartas especiales
            deck.Add(new SpecialCard("Relicario Demoniaco", "Expansion de dominio de Ryomen Sukuna", Resources.Load<Sprite>("Art/Images/Expansion de Sukuna"), CardPos.Expansion, TypeOfCard.SpecialCard, "Disminuye en dos el ataque de las cartas de una fila seleccionada de ambos jugadores", TypeOfEffects.ExpansionSukuna));
            deck.Add(new SpecialCard("Montaña Ataúd de Hierro", "Expansion de Jogo", Resources.Load<Sprite>("Art/Images/Expansion de Jogo"), CardPos.Expansion, TypeOfCard.SpecialCard, "Causa quemaduras en una fila seleccionada propia y del rival, haciendo que cada carta en dicha fila pierda uno de ataque", TypeOfEffects.ExpansionJogo));
            deck.Add(new SpecialCard("Prisión Mahayana", "Expansion de Mahito", Resources.Load<Sprite>("Art/Images/Expansion de Mahito"), CardPos.Expansion, TypeOfCard.SpecialCard, "Disminuye en 1 el ataque de una fila propia y del rival", TypeOfEffects.ExpansionMahito));
            deck.Add(new SpecialCard("Dedo de Sukuna", "Potencia la energia maldita de las maldiciones", Resources.Load<Sprite>("Art/Images/Dedo de Sukuna(potenciador)"), CardPos.Buff, TypeOfCard.SpecialCard, "Aumenta en 2 el ataque de las cartas que se encuentren en la fila seleccionada", TypeOfEffects.Buff));
            deck.Add(new SpecialCard("Dedo de Sukuna", "Potencia la energia maldita de las maldiciones", Resources.Load<Sprite>("Art/Images/Dedo de Sukuna(potenciador)"), CardPos.Buff, TypeOfCard.SpecialCard, "Aumenta en 2 el ataque de las cartas que se encuentren en la fila seleccionada", TypeOfEffects.Buff));
            deck.Add(new SpecialCard("Dedo de Sukuna", "Potencia la energia maldita de las maldiciones", Resources.Load<Sprite>("Art/Images/Dedo de Sukuna(potenciador)"), CardPos.Buff, TypeOfCard.SpecialCard, "Aumenta en 2 el ataque de las cartas que se encuentren en la fila seleccionada", TypeOfEffects.Buff));
            deck.Add(new SpecialCard("Cambio de forma", "La forma de una tropa cambia a la de otra, habia sido una alteracion de Mahito todo el tiempo", Resources.Load<Sprite>("Art/Images/Buggi Buggi"), CardPos.MeleAndRangeAndSiege, TypeOfCard.SpecialCard, "Cambia una carta de la mano por una del campo", TypeOfEffects.Lure));
            deck.Add(new SpecialCard("Impacto externo", "Los dominios son debiles por fuera, esta carta aprovecha esao", Resources.Load<Sprite>("Art/Images/Impacto externo"), CardPos.Clearance, TypeOfCard.SpecialCard, "Destruye un dominio rival que este activo", TypeOfEffects.Despeje));
            deck.Add(new SpecialCard("Impacto externo", "Los dominios son debiles por fuera, esta carta aprovecha esao", Resources.Load<Sprite>("Art/Images/Impacto externo"), CardPos.Clearance, TypeOfCard.SpecialCard, "Destruye un dominio rival que este activo", TypeOfEffects.Despeje));

            return deck;
        }
    }
}
