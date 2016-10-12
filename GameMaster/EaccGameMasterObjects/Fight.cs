using com.eurosport.logging;
using eacc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eacc.gamemaster
{
    
    public class Fighter
    {
        public Vehicle Vehicle { get; set; }

        public int VehicleId { get { return Vehicle.Id; } }
        public int TeamId { get { return Vehicle.TeamId; } }

        /// <summary>
        /// Liste des coups joués, 0 pour une attaque
        /// </summary>
        public List<int> MoveList { get; private set; }

        private List<int> cards { get; set; }

        public int NbCards { get { return cards.Count; } }

        public bool IsWinner { get; set; }

        public string IAUrl { get; set; }

        public Fighter(Vehicle vehicle, List<int> availableCards)
        {
            this.cards = new List<int>();
            this.Vehicle = vehicle;
            MoveList = new List<int>();
            for (int i = 0; i < vehicle.NbWeapons; i++)
            {
                var card = availableCards.RandomElement();
                cards.Add(card);
                availableCards.Remove(card);
            }
        }

        public bool IsValid()
        {
            return Vehicle.CanFight() && Vehicle.CanBeDriven();
        }

        public void addCard(int i)
        {
            cards.Add(i);
        }

        public void removeCard(int i)
        {
            cards.Remove(i);
        }

        public bool hasCard(int i)
        {
            return cards.Contains(i);
        }

        public bool hasCard()
        {
            return cards.Any();
        }

        public int howMany(int i)
        {
            return cards.Count(c => (c == i));
        }

        public int howManymissingCards()
        {
            int nbCards = cards.Count();
            int normalNbCards = Vehicle.NbWeapons;
            return normalNbCards - nbCards;
        }

        public int GetRandomCard()
        {
            return cards.RandomElement();
        }

        public bool IsPirate()
        {
            return this.Vehicle.TeamId == 42;
        }

        public string PrintCards()
        {
            return "[" + String.Join(",", cards) + "]";
        }

        public string PrintMoves()
        {
            return "[" + String.Join(",", MoveList) + "]";
        }

        public void FillIAUrl()
        {
            this.IAUrl = Vehicle.RequestIAUrl();
        }
    }
    public class Fight
    {
        public List<Fighter> Fighters { get; set; }

        public const int DISTANCE_START = 21;

        /// <summary>
        /// Nombre de cartes classées par force
        /// </summary>
        private static readonly int[] INITIAL_NB_CARDS = new int[6] { 0, 5, 5, 5, 5, 5 };

        private List<int> remainingCards { get; set; }

        private void initializeCards()
        {
            remainingCards = new List<int>();
            for (int puissance = 1; puissance < INITIAL_NB_CARDS.Length; puissance++)
            {
                for (int j = 0; j < INITIAL_NB_CARDS[puissance]; j++)
                {
                    remainingCards.Add(puissance);
                }
            }
        }

        public Fight(Vehicle firstVehicle, Vehicle secondVehicle)
        {
            initializeCards();
            Fighters = new List<Fighter>()
            {
                new Fighter(firstVehicle, remainingCards),
                new Fighter(secondVehicle,remainingCards)
            };
        }

        public List<Vehicle> GetInvalidFighters()
        {
            List<Vehicle> res = new List<Vehicle>();
            foreach (var fighter in Fighters)
            {
                if (!fighter.IsValid())
                {
                    res.Add(fighter.Vehicle);
                }
            }
            return res;
        }

        public bool IsMoveValid(Vehicle vehicle, int[] move)
        {
            var fighter = getFighter(vehicle);
            if (move.Length < 1)
                return false;
            if (move.Length == 1)
            {
                //coup "normal" ou attaque simple
                int card = move[0];
                //le joueur a la carte
                if (!fighter.hasCard(card))
                    return false;
                //la carte est une carte normale
                if (card < 1)
                    return false;
                //le coup est valide
                if (Fighters.Sum(f => f.MoveList.Sum()) + card > (DISTANCE_START + 1))
                    return false;
            }
            else
            {
                //attaque directe ou indirecte : 2 types de carte max
                if (move.Distinct().Count() > 2)
                    return false;
                    //attaque multiple
                if (move.Distinct().Count() == 2)
                {
                    //attaque indirecte

                    //la première carte est unique
                    if (move.Count(card => card == move[0]) != 1)
                        return false;
                    
                    //1ere carte = coup valide
                    //coup "normal" ou attaque simple
                    int movement = move[0];
                    //le joueur a la carte
                    if (!fighter.hasCard(movement))
                        return false;
                    //la carte est une carte normale
                    if (movement < 1)
                        return false;
                    //le coup est valide
                    if (Fighters.Sum(f => f.MoveList.Sum()) + movement > (DISTANCE_START + 1))
                        return false;

                    //autres cartes : attaque directe
                    //le joueur a les cartes
                    if (fighter.howMany(move[1]) > move.Length-1)
                        return false;
                    //c'est une attaque
                    if (!DoesMoveAttack(move[1]+move[0]))
                        return false;
                }
                else
                {
                    //un seul type de carte : attaque directe (ou attaque indirecte avec la même carte)
                    //le joueur a les cartes
                    if (fighter.howMany(move[0]) > move.Length)
                        return false;
                    //c'est une attaque
                    if (!DoesMoveAttack(move[0]))
                    {
                        //attaque indirecte?
                        if (!DoesMoveAttack(move[0]+move[1]))
                            return false;
                    }
                }
            }
            return true;
        }

        public bool DoesMoveAttack(int move)
        {
            return (Fighters.Sum(f => f.MoveList.Sum()) + move) == (DISTANCE_START + 1);
        }

        public bool Move(Vehicle vehicle, int[] move)
        {
            var fighter = getFighter(vehicle);
            Log.Info("[Team #{3}] Fighter {0} got cards {1}, trying to play {2}", vehicle.Id, fighter.PrintCards(), "[" + String.Join(",", move) + "]", vehicle.TeamId);
            if (!IsMoveValid(vehicle, move))
            {
                return false;
            }
            var card = move[0];
            if (DoesMoveAttack(card))
            {
                resolveAttack(fighter, move);
                return true;
            }
            else
            {
                fighter.MoveList.Add(card);
                fighter.removeCard(card);
                if (move.Length > 1)
                {
                    if (!DoesMoveAttack(move[1]))
                    {
                        return false;
                    }
                    //attaque indirecte
                    resolveAttack(fighter, move.Skip(1).ToArray());
                    return true;
                }
                replenishCards(fighter);
            }
            Log.Debug("[Team #{2}] Fighter {0} got cards {1}", vehicle.Id, fighter.PrintCards(), card, vehicle.TeamId);
            Log.Debug("Remaining Cards {0}", "{" + String.Join(",", remainingCards) + "}");
            return true;
        }

        /// <summary>
        /// Résoud l'attaque
        /// </summary>
        /// <param name="fighter">Le combattant qui attaque</param>
        /// <param name="move">La liste des cartes jouées, validée</param>
        /// <returns>True si attaque gagne, false sinon</returns>
        private void resolveAttack(Fighter attacker, int[] move)
        {
            Fighter defenser = this.Fighters.Single(f => f.VehicleId != attacker.VehicleId);
            int defensePower = defenser.howMany(move[0]);
            foreach (var card in move)
            {
                attacker.MoveList.Add(0);
                attacker.removeCard(card);
                Log.Info("[Team #{3}] [Team #{2}] Vehicle {0} attack with a {1}", attacker.VehicleId, card, attacker.TeamId, defenser.TeamId);
                if (defenser.hasCard(card))
                {
                    defenser.MoveList.Add(0);
                    defenser.removeCard(card);
                    Log.Info("[Team #{3}] [Team #{2}] Vehicle {0} parries with a {1}", defenser.VehicleId, card, attacker.TeamId, defenser.TeamId);
                }
                else
                {
                    attacker.IsWinner = true;
                    Log.Debug("Vehicle {0} wins", attacker.VehicleId);
                    return;
                }
            }
            //fin du combat sans vainqueur
            //si le défenseur n'a plus de carte il perd
            if (!defenser.hasCard())
            {
                attacker.IsWinner = true;
            }
            //repioche
            replenishCards(attacker);
        }

        public bool IsWon()
        {
            return Fighters.Any(f => f.IsWinner);
        }

        public bool IsDraw()
        {
            return remainingCards.Count == 0;
        }

        public Fighter GetWinner()
        {
            return Fighters.SingleOrDefault(f => f.IsWinner);
        }

        public Fighter GetLoser()
        {
            if (GetWinner() != null)
            {
                return Fighters.SingleOrDefault(f => !f.IsWinner);
            }
            //else
            return null;
        }

        private Fighter getFighter(Vehicle v)
        {
            return Fighters.Single(f => f.VehicleId == v.Id);
        }

        private bool replenishCards(Fighter f)
        {
            int nbCards = f.howManymissingCards();

            for (int i = 0; i < nbCards; i++)
            {
                if (remainingCards.Count == 0)
                {
                    return false;
                }
                //else
                var newCard = remainingCards.RandomElement();
                f.addCard(newCard);
                remainingCards.Remove(newCard);
            }
            return true;
        }

    }
}
