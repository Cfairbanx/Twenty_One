using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Casino.Interfaces;

namespace Casino.TwentyOne
{
    public class TwentyOneGame : Game, IWalkAway
    {
        public TwentyOneDealer Dealer { get; set; }

        public override void Play()
        {
            Dealer = new TwentyOneDealer();
            foreach (Player player in Players)
            {
                player.Hand = new List<Card>();
                player.Stay = false;
            }
            Dealer.Hand = new List<Card>();
            Dealer.Stay = false;
            Dealer.Deck = new Deck();
            Dealer.Deck.Shuffle();

            foreach (Player player in Players)
            {
                bool validAnswer = false;
                int bet = 0;
                while (!validAnswer)                                   //exception handle user input, take in bet
                {
                    Console.WriteLine("Place your bet.");
                    validAnswer = int.TryParse(Console.ReadLine(), out bet);
                    if (!validAnswer) Console.WriteLine("Please enter digits only, no decimals.");
                }
                if (bet < 0)                                        //if someone places a negative bet.
                {
                    throw new FraudException("Security! Kick this person out!");
                }
                bool successfullyBet = player.Bet(bet);
                if (!successfullyBet)
                {
                    return;                                         //return in a void method ends the method.
                }
                Bets[player] = bet;                                 //stores bet as a dictionary (keeping multiple players in mind)
            }
            for (int i = 0; i < 2; i++)
            {
                Console.WriteLine("Dealing...");
                foreach (Player player in Players)
                {
                    Console.Write("{0}: ", player.Name);    //.Write something to console then do not auto press enter
                    Dealer.Deal(player.Hand);
                    if (i == 1)                             //on second turn, i=1, check for black jack
                    {
                        bool blackJack = TwentyOneRules.CheckForBlackJack(player.Hand);     //pass in hand to check for blackjack
                        if (blackJack)
                        {
                            Console.WriteLine("BlackJack! {0} wins {1}", player.Name, Bets[player]);
                            player.Balance += Convert.ToInt32((Bets[player] * 1.5) + Bets[player]); //Add to balance winnings plus original bet
                            return;                                                                 //if playe rhas blackJack, round over.
                        }
                    }
                }

                Console.Write("Dealer: ");
                Dealer.Deal(Dealer.Hand);       
                if (i == 1)                                                             //Check for BlackJack on dealer's hand
                {
                    bool blackJack = TwentyOneRules.CheckForBlackJack(Dealer.Hand);
                    if (blackJack)
                    {
                        Console.WriteLine("Dealer has BlackJack! Everyone loses!");
                        foreach (KeyValuePair<Player, int> entry in Bets)               //iterate through bets dictionary
                        {
                            Dealer.Balance += entry.Value;                              //forward bets to dealer's balance
                        }
                        return;
                    }
                }
            }
            foreach (Player player in Players)
            {
                while (!player.Stay)                    //while player is not staying
                {
                    Console.WriteLine("Your cards are: ");
                    foreach (Card card in player.Hand)
                    {
                        Console.Write("{0} ", card.ToString());      //Show players their cards
                    }
                    Console.WriteLine("\n\nHit or Stay?");
                    string answer = Console.ReadLine().ToLower();
                    if (answer == "stay")                           //if stay, do not give additional cards
                    {
                        player.Stay = true;
                        break;
                    }
                    else if (answer == "hit")                       //if hit, deal another card
                    {
                        Dealer.Deal(player.Hand);
                    }
                    bool busted = TwentyOneRules.IsBusted(player.Hand);     //if player busts
                    if (busted)
                    {
                        Dealer.Balance += Bets[player];                     //forward bet to dealer balance
                        Console.WriteLine("{0} Busted! You lose your bet of {1}. Your balance is now {2}", player.Name, Bets[player], player.Balance);
                        Console.WriteLine("Do you want to play again?");                    //Check if player wants to continue playing
                        answer = Console.ReadLine().ToLower();
                        if (answer == "yes" || answer == "yeah" || answer == "y" || answer == "ya")
                        {
                            player.isActivelyPlaying = true;
                            return;
                        }
                        else
                        {
                            player.isActivelyPlaying = false;
                            return;
                        }
                    }
                }
            }
            Dealer.isBusted = TwentyOneRules.IsBusted(Dealer.Hand);         //check if dealer busts
            Dealer.Stay = TwentyOneRules.ShouldDealerStay(Dealer.Hand);     //check conditions if dealer hits or stays
            while (!Dealer.Stay && !Dealer.isBusted)                        //if dealer is set to hit, receive another card
            {
                Console.WriteLine("Dealer is hitting...");
                Dealer.Deal(Dealer.Hand);
                Dealer.isBusted = TwentyOneRules.IsBusted(Dealer.Hand);
                Dealer.Stay = TwentyOneRules.ShouldDealerStay(Dealer.Hand);
            }
            if (Dealer.Stay)
            {
                Console.WriteLine("Dealer is staying.");                    //Dealer does not get another card
            }
            if (Dealer.isBusted)                                            //Dealer busted, player wins
            {
                Console.WriteLine("Dealer Busted!");
                foreach (KeyValuePair<Player, int> entry in Bets)           //access dictionary with keyvaluepair
                {
                    Console.WriteLine("{0} won {1}!", entry.Key.Name, entry.Value);
                    Players.Where(x => x.Name == entry.Key.Name).First().Balance += (entry.Value * 2); //loop through keyvaluepair, find match, get player, take balance *2, give winnings to player
                    Dealer.Balance -= entry.Value;                                                      //subtract winnings from dealer balance
                }
                return;
            }
            foreach (Player player in Players)                                              //Compare hands for winner
            {
                bool? playerWon = TwentyOneRules.CompareHands(player.Hand, Dealer.Hand);    //bool? means bool can have a null
                if (playerWon == null)
                {
                    Console.WriteLine("Push! NO one wins.");
                    player.Balance += Bets[player];                                         //give player bet back
                }
                else if (playerWon == true)
                {
                    Console.WriteLine("{0} won {1}!", player.Name, Bets[player]);           //announce win
                    player.Balance += (Bets[player] * 2);                                   //give winnings
                    Dealer.Balance -= Bets[player];                                         //remove from Bets
                }
                else
                {
                    Console.WriteLine("Dealer wins {0}!", Bets[player]);
                    Dealer.Balance += Bets[player];
                }

                Console.WriteLine("Play again?");
                string answer = Console.ReadLine().ToLower();
                if (answer == "yes" || answer == "yeah" || answer == "y" || answer == "ya")
                {
                    player.isActivelyPlaying = true;
                }
                else
                {
                    player.isActivelyPlaying = false;
                }
            }
        }

        public override void ListPlayers()
        {
            Console.WriteLine("21 Players: ");
            base.ListPlayers();
        }
        public void WalkAway(Player player)
        {
            throw new NotImplementedException();
        }
    }
}
