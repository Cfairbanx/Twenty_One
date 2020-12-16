using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Casino.TwentyOne
{
    public class TwentyOneRules
    {
        //It is considered best practice to make a method Private when it will only be used within the class in which it was created.

        private static Dictionary<Face, int> _cardValues = new Dictionary<Face, int> ()      //static so that you do not have to create an object to access.
        {                                                                                   //Private class name convention starts with _
            [Face.Two] = 2,
            [Face.Three] = 3,
            [Face.Four] = 4,
            [Face.Five] = 5,
            [Face.Six] = 6,
            [Face.Seven] = 7,
            [Face.Eight] = 8,
            [Face.Nine] = 9,
            [Face.Ten] = 10,
            [Face.Jack] = 10,
            [Face.Queen] = 10,
            [Face.King] = 10,
            [Face.Ace] = 1
        };

        private static int[] GetAllPossibleHandValues(List<Card> Hand)  //Ace can be either 1 or 11, players choice. This will provide the functionality for Ace value switch
        {
            int aceCount = Hand.Count(x => x.Face == Face.Ace);         //Lambda function because Hand is a list
            int[] result = new int[aceCount + 1];
            int value = Hand.Sum(x => _cardValues[x.Face]);             //Sum is method of list, looks up dictionary and gets value
            result[0] = value;
            if (result.Length == 1) return result;                      //Do not have to use {} on if statement if only one line.

            for (int i = 1; i < result.Length; i++)
            {
                value += (i * 10);                                      //for each ace, make a separate value and add 10.
                result[i] = value;
            }
            return result;
        }
        public static bool CheckForBlackJack(List<Card> Hand)
        {
            int[] possibleValues = GetAllPossibleHandValues(Hand);
            int value = possibleValues.Max();                           //Lambda expression, max, the maximum value
            if (value == 21) return true;
            else return false;
        }

        public static bool IsBusted(List<Card> Hand)                    //Check if player busts, cards value over 21
        {
            int value = GetAllPossibleHandValues(Hand).Min();
            if (value > 21) return true;
            else return false;
        }
        
        public static bool ShouldDealerStay(List<Card> Hand)            //Setting rules for dealer play
        {
            int[] possibleHandValues = GetAllPossibleHandValues(Hand);  //Check values of dealers hand
            foreach (int value in possibleHandValues)
            {
                if (value > 16 && value < 22)                           //if cards = greater than 16 and less than 22, stay
                {
                    return true;
                }
            }
            return false;                                               //else, hit. Deal another card
        }

        public static bool? CompareHands(List<Card> PlayerHand, List<Card> DealerHand)
        {
            int[] playerResults = GetAllPossibleHandValues(PlayerHand);     //int[] for cards in hand
            int[] dealerResults = GetAllPossibleHandValues(DealerHand);

            int playerScore = playerResults.Where(x => x < 22).Max();       //playerresults where int is < 22, get largest value (because of ace)
            int dealerScore = dealerResults.Where(x => x < 22).Max();       //dealerresults where int is < 22, get largest value (because of ace)

            if (playerScore > dealerScore) return true;                     
            else if (playerScore < dealerScore) return false;
            else return null;

        }
    }
}
