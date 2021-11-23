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
            foreach (var piece in player.Pieces)
            {
                PrintAsOrdinalNumber((int)piece.Id);
                if (piece.CurrentSpot.HasValue)
                {
                    Console.WriteLine($" Piece at {piece.CurrentSpot.Value} square");
                }
                else if (piece.CurrentHome.HasValue)
                {
                    Console.WriteLine($" Piece at {piece.CurrentHome.Value} home-column");
                }
                else if (piece.IsMatured)
                {
                    Console.WriteLine(" Piece at Inside Home Triangle.");
                }
                else
                {
                    Console.WriteLine(" Piece at Inside Layer.");
                }
            }
        }

        public void PrintPiecePossiblePosition<T>(IList<PieceNumber> pieceId, T? value)
        {
            if (typeof(T).Equals(typeof(SquareSpot)))
            {
                Console.WriteLine($"{string.Join(", ", pieceId)} piece(s) can be placed at {value} square");
            }
            else if (typeof(T).Equals(typeof(HomeColumn)))
            {
                Console.WriteLine($"{string.Join(", ", pieceId)} piece(s) can be placed at {value} home-column");
            }
            else
            {
                Console.WriteLine($"{string.Join(", ", pieceId)} piece(s) can not move!");
            }
        }

        public void PrintAsOrdinalNumber(int number)
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

        public void PrintDiceValue(int diceValue)
        {
            Console.WriteLine($"Dice value is {diceValue}");
        }        

        public void PrintOptionNumber(int number)
        {
            Console.Write(number + ". ");
        }
    }
}
