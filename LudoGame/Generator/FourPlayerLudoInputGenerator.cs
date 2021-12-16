using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudoGame
{
    public class FourPlayerLudoInputGenerator
    {
        private Random _random;

        public FourPlayerLudoInputGenerator()
        {
            _random = new Random(DateTime.UtcNow.Millisecond);
        }

        public int ChoosePiece(int maxOptions)
        {
            Console.WriteLine("Which option do you want to choice?");
            //if (!int.TryParse(Console.ReadLine(), out int option) || option < 1 || option > maxOptions)
            //{
            //    Console.WriteLine("Please provide a valid piece for which the piece can move!");
            //    option = -1;
            //}
            //return option;
            return _random.Next(1, maxOptions + 1);
        }
    }
}
