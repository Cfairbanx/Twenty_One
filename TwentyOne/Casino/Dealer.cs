using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Casino
{
    public class Dealer 
    {
        public string Name { get; set; }
        public Deck Deck { get; set; }
        public int Balance { get; set; }


        public void Deal(List<Card> Hand)
        {
            Hand.Add(Deck.Cards.First());                               //first() is a method available to lists, grabs the first element
            string card = string.Format(Deck.Cards.First().ToString() + "\n");
            Console.WriteLine(card);                                     
            using (StreamWriter file = new StreamWriter(@"C:\Users\ccfai\Logs\log.txt", true)) //Appends info to log file, to keep a record of all cards dealt
            {
                file.WriteLine(DateTime.Now);
                file.WriteLine(card);                                   //writes the chosen card to the log
            }
            Deck.Cards.RemoveAt(0);                                     //removeat() is a method available to lists, removes an element (placement of element)
        }
    }
}
