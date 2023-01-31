using LudoLib.Enums;

namespace LudoLib
{
	public class BoardConstants
	{
		public const int MaxSpot = 52;
		public const int MaxHomeColumn = 5;
		public const int MaxDiceSide = 6;

		public static readonly SquareSpot[] StartingSpot = { SquareSpot.First, SquareSpot.Fourteenth, SquareSpot.TwentySeventh, SquareSpot.Fortieth };
		public static readonly SquareSpot[] EndingSpot = { SquareSpot.FiftyFirst, SquareSpot.Twelfth, SquareSpot.TwentyFifth, SquareSpot.ThirtyEighth };
	}
}
