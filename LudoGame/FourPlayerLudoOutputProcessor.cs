using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudoGame
{
    public class FourPlayerLudoOutputProcessor
    {
        public void PlayerStatus(IPlayer? player)
        {
            if (player == null)
            {
                return;
            }
            Console.WriteLine("\n------------------------------------------------------");
            Console.WriteLine($"Player ID   : {(int)player.Layer}");
            Console.WriteLine($"BoardLayer  : {player.Layer}");
            Console.WriteLine($"Piece Color : {player.Pieces[0].Color}");
            for (int i = 0; i < 4; ++i)
            {
                PrintAsOrdinalNumber(i + 1);
                if (player.Pieces[i].CurrentPosition.Item1.HasValue)
                {
                    Console.WriteLine($" Piece at {player.Pieces[i].CurrentPosition.Item1.Value} square");
                }
                else if (player.Pieces[i].CurrentPosition.Item2.HasValue)
                {
                    Console.WriteLine($" Piece at {player.Pieces[i].CurrentPosition.Item2.Value} home-column");
                }
                else if (player.Pieces[i].IsMatured)
                {
                    Console.WriteLine(" Piece at Inside Home Triangle.");
                }
                else
                {
                    Console.WriteLine(" Piece at Inside Layer.");
                }
            }
        }

        public void PossiblePositionStatus(PieceNumber pieceId, SquareNumber? square, HomeColumn? home)
        {
            PrintAsOrdinalNumber((int)pieceId);

            if (square.HasValue)
            {
                Console.WriteLine($" piece can be placed at {square.Value} square");
            }
            else if (home.HasValue)
            {
                Console.WriteLine($" piece can be placed at {home.Value} home-column");
            }
            else
            {
                Console.WriteLine($" piece can not move");
            }
        }

        private void PrintAsOrdinalNumber(int number)
        {
            switch (number)
            {
                case 1:
                    Console.Write("1st");
                    break;
                case 2:
                    Console.Write("2nd");
                    break;
                case 3:
                    Console.Write("3rd");
                    break;
                case 4:
                    Console.Write("4th");
                    break;                
            }
        }

        public void PlayerUnAvailable()
        {
            Console.WriteLine("Player UnAvailable!");
        }

        public void DiceValue(byte? diceValue)
        {
            Console.WriteLine($"Dice value is {diceValue}");
        }
    }
}
