using BattleshipLiteLibrary;
using BattleshipLiteLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipLite
{
    class Program
    {
        static void Main(string[] args)
        {
            WelcomeMessage();

            PlayerInfoModel activePlayer = CreatePlayer("Player 1");
            PlayerInfoModel opponent = CreatePlayer("Player 2");
            PlayerInfoModel winner = null;

            do
            {
                DisplayShotGrid(activePlayer);

                RecordPlayerShot(activePlayer, opponent);

                //determine if the game should continue
                bool doesGameContinue = GameLogic.PlayerStillActive(opponent);

                if (doesGameContinue == true)
                {
                    //Swap using a tuple
                    (activePlayer, opponent) = (opponent, activePlayer);
                }
                
                else
                {
                    winner = activePlayer;
                }
                

            } while (winner == null);

            IdentifyWinner(winner);

            Console.ReadLine();
        }

        private static void IdentifyWinner(PlayerInfoModel winner)
        {
            Console.WriteLine("Congrations to {0} for winning!", winner.UserName);
            Console.WriteLine("{0} took {1} shots.", winner.UserName, GameLogic.GetShotCount(winner));
        }

        private static void RecordPlayerShot(PlayerInfoModel activePlayer, PlayerInfoModel opponent)
        {

            bool isValidShot = false;
            string row = "";
            int column = 0;


            do
            {
                string shot = AskForShot(activePlayer);
                try
                {
                    (row, column) = GameLogic.SplitShotIntoRowAndColumn(shot);
                    isValidShot = GameLogic.ValidateShot(row, column, activePlayer);
                }
                catch (Exception ex)
                {

                    isValidShot = false;
                }

                if(isValidShot == false)
                {
                    Console.WriteLine("Invalid shot location. Please try again.");
                }

            } while (isValidShot == false);

            //determine shot results
            bool isAHit = GameLogic.IdentifyShotResult(opponent, row, column);
            //record results
            GameLogic.MarkShotResult(activePlayer, row, column, isAHit);

            DisplayShotResult(row, column, isAHit);

        }

        private static void DisplayShotResult(string row, int column, bool isAHit)
        {
            if (isAHit)
            {
                Console.WriteLine("{0}{1} is a hit!", row, column);
            }
            else
            {
                Console.WriteLine("{0}{1} is a miss.", row, column);
            }

            Console.WriteLine();
        }

        private static string AskForShot(PlayerInfoModel player)
        {
            Console.Write("{0}, please enter your shot selection: ", player.UserName);

            string output = Console.ReadLine();

            return output;
        }

        private static void DisplayShotGrid(PlayerInfoModel activePlayer)
        {
            string currentRow = activePlayer.ShotGrid[0].SpotLetter;

            foreach (var gridSpot in activePlayer.ShotGrid)
            {
                if (gridSpot.SpotLetter != currentRow)
                {
                    Console.WriteLine();
                    currentRow = gridSpot.SpotLetter;
                }

                if (gridSpot.Status == GridSpotStatus.Empty)
                {
                    Console.Write(" {0}{1} ", gridSpot.SpotLetter, gridSpot.SpotNumber);
                }
                else if (gridSpot.Status == GridSpotStatus.Hit)
                {
                    Console.Write(" X  ");
                }
                else if (gridSpot.Status == GridSpotStatus.Miss)
                {
                    Console.Write(" O  ");
                }
                else
                {
                    Console.Write(" ?  ");
                }
            }
            Console.WriteLine();
            Console.WriteLine();
        }

        private static void WelcomeMessage()
        {
            Console.WriteLine("Welcome to Battleship Lite");
            Console.WriteLine("Created by Adam Wyatt");
            Console.WriteLine();
        }

        private static PlayerInfoModel CreatePlayer(string PlayerTitle)
        {
            PlayerInfoModel output = new PlayerInfoModel();

            Console.WriteLine("Player information for {0} ", PlayerTitle);

            //Ask User for Name
            output.UserName = AskForUsersName();
            //Load up the shot grid
            GameLogic.InitializeGrid(output);
            //Ask the user for their 5 ship placements
            PlaceShips(output);
            //Clear the console
            Console.Clear();

            return output;
        }

        private static string AskForUsersName()
        {
            Console.Write("What is your name: ");
            string output = Console.ReadLine();

            return output;
        }

        private static void PlaceShips(PlayerInfoModel model)
        {
            do
            {
                Console.Write("Where do you want to place your ship number {0}: ", model.ShipLocations.Count + 1);
                string location = Console.ReadLine();

                bool isValidLocation = false;

                try
                {
                    isValidLocation = GameLogic.PlaceShip(model, location);
                }
                catch (Exception ex)
                {

                    Console.WriteLine("Error: " + ex.Message);
                }

                if (isValidLocation == false)
                {
                    Console.WriteLine("That is not a valid location. Please try again.");
                }


            } while (model.ShipLocations.Count < 5);
        }
    }
}
