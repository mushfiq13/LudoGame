using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudoGame
{
    public class FourPlayerLudoInputGenerator
    {
        public int ChoosePiece()
        {
            Console.WriteLine("Which option do you want to choice?");
            if (!int.TryParse(Console.ReadLine(), out int option) || option < 1 || option > 4)
            {
                Console.WriteLine("Please provide a valid piece for which the piece can move!");
                option = -1;
            }
            return option;
        }
    }
}
