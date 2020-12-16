using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Casino;
using Casino.TwentyOne;


namespace TwentyOne
{
    class Program
    { 
        static void Main(string[] args)
        {
            const string casinoName = "Grand Hotel and Casino";         //const can never be changed while program is running

            Console.WriteLine("Welcome to the {0}. Let's start by telling me your name.", casinoName);
            string playerName = Console.ReadLine();
            if (playerName.ToLower() == "admin")                        //admin to display all exceptions in program
            {
                List<ExceptionEntity> Exceptions = ReadExceptions();
                foreach (var exception in Exceptions)
                {
                    Console.Write(exception.Id + " | ");
                    Console.Write(exception.ExceptionType + " | ");
                    Console.Write(exception.ExceptionMessage + " | ");
                    Console.WriteLine();
                }
                Console.ReadLine();
                return;
            }

            bool validAnswer = false;                                                           //any time you are getting user input, exception handling is neede
            int bank = 0;
            while (!validAnswer)
            {
                Console.WriteLine("And how much money did you bring today?");                   //ask user for amount to start
                validAnswer = int.TryParse(Console.ReadLine(), out bank);                       //TryParse attempts to parse input, if it cannot, prompt error
                if (!validAnswer) Console.WriteLine("Please enter digits only, no decimals.");  //prompt user of needed format
            }

            Console.WriteLine("Hello, {0}. Would you like to join a game of 21 right now?", playerName);
            string answer = Console.ReadLine().ToLower();

            if (answer == "yes" || answer == "yeah" || answer == "y" || answer == "ya")
            {
                Player player = new Player(playerName, bank);           //initialize a new player
                player.Id = Guid.NewGuid();                             //global unique identifier
                using (StreamWriter file = new StreamWriter(@"C:\Users\ccfai\Logs\log.txt", true))
                {
                    file.WriteLine(player.Id);
                }
                Game game = new TwentyOneGame();                        //create game, polymorphism
                game += player;                                         //add player to game
                player.isActivelyPlaying = true;                        //property of player, check to make sure player is playing
                while (player.isActivelyPlaying && player.Balance > 0)  //allow play while certain conditions are met
                {
                    try
                    {
                        game.Play();
                    }
                    catch (FraudException ex)                               //start with more specific exceptions 
                    {
                        Console.WriteLine(ex.Message);
                        UpdateDbWithException(ex);
                        Console.ReadLine();
                        return;
                    }
                    catch (Exception ex)                                       //catch more generic exceptions
                    {
                        Console.WriteLine("An error occured. Please contact your System Administrator");
                        UpdateDbWithException(ex);
                        Console.ReadLine();
                        return;
                    }


                }
                game -= player;
                Console.WriteLine("Thank you for playing!");
            }
            Console.WriteLine("Feel free to look around the casino. Bye for now.");
            Console.ReadLine();
        }
        
        //--------------------------------------------------------DATABASE -----------------------------------------------------------------------

        private static void UpdateDbWithException(Exception ex) //using ADO.NET, MUST have a string to connect to db. Contains info about database instance.
        {
            string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TwentyOneGame;Integrated Security=True; 
                                        Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            string queryString = "INSERT INTO Exceptions (ExceptionType, ExceptionMessage, TimeStamp) VALUES " +
                                    "(@ExceptionType, @ExceptionMessage, @TimeStamp)";              //use parameterized queries to protect from SQL injection

            using (SqlConnection connection = new SqlConnection(connectionString))      //using statement to prevent unnecessary use of memory
            {                                                                           //manage and control memory on external resources
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.Add("@ExceptionType", SqlDbType.VarChar);            //by naming data type, protects against sql injection
                command.Parameters.Add("@ExceptionMessage", SqlDbType.VarChar);
                command.Parameters.Add("@TimeStamp", SqlDbType.DateTime);
                                                                                        //Now get values to add
                command.Parameters["@ExceptionType"].Value = ex.GetType().ToString();   //gettype grabs data type, change value to string to add to db
                command.Parameters["@ExceptionMessage"].Value = ex.Message;
                command.Parameters["@TimeStamp"].Value = DateTime.Now;

                connection.Open();                                                      //establish connection to Db
                command.ExecuteNonQuery();                                              //execute command, non query because it is an INSERT statement
                connection.Close();                                                     //end connection
            }
        }

        private static List<ExceptionEntity> ReadExceptions()                           //Method to display exceptions to admin, admin when entered as name by user
        {
            string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TwentyOneGame;Integrated Security=True; 
                                        Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;
                                        ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            string queryString = @"Select Id, ExceptionType, ExceptionMessage, TimeStamp From Exceptions";

            List<ExceptionEntity> Exceptions = new List<ExceptionEntity>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();                         //reader loops through each record in Db

                while (reader.Read())
                {
                    ExceptionEntity exception = new ExceptionEntity();
                    exception.Id = Convert.ToInt32(reader["Id"]);
                    exception.ExceptionType = reader["ExceptionType"].ToString();
                    exception.ExceptionMessage = reader["ExceptionMessage"].ToString();
                    exception.TimeStamp = Convert.ToDateTime(reader["TimeStamp"]);
                    Exceptions.Add(exception);
                }
                connection.Close();
            }
            return Exceptions;
        }
    }   
}
//-----------------------------------------------------------NOTE------------------------------------------------------------------
//
//Main method is entrance point, should be clear and easily read. 
// Most of program functionality will come from the Play() method that will have its own class.
