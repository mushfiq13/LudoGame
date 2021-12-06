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

        public static readonly SquareSpot[] StartingSpot = { SquareSpot.First, SquareSpot.Fourteenth, SquareSpot.TwentySeventh, SquareSpot.Fortieth };
        public static readonly SquareSpot[] EndingSpot = { SquareSpot.FiftyFirst, SquareSpot.Twelfth, SquareSpot.TwentyFifth, SquareSpot.ThirtyEighth };
    }
}
