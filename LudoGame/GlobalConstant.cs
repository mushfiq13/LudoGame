using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudoGame
{
    public class GlobalConstant
    {
        public const int MaxSpot = 52;
        public const int MaxHomeColumn = 5;

        public static readonly SquareSpot[] StartingSpot = { SquareSpot.Zero, SquareSpot.Thirteenth, SquareSpot.TwentySixth, SquareSpot.ThirtyNineth };
        public static readonly SquareSpot[] EndingSpot = { SquareSpot.Fiftieth, SquareSpot.Eleventh, SquareSpot.TwentyFourth, SquareSpot.ThirtySeventh };
    }
}
