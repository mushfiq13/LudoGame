using LudoLib.Enums;
using LudoLib.IO;
using LudoLib.Utilities;
using System;
using System.Collections.Generic;

namespace LudoGame
{
	public static class Factory
	{
		public static IBoard CreateFourPlayerBoard()
		{
			return new FourPlayerBoard(CreateSixSidedDice(), CreatePlayersCollection(), PiecesAtSquare());
		}

		public static IDice CreateSixSidedDice()
		{
			return new SixSidedDice(new Random(DateTime.UtcNow.Millisecond));
		}

		public static IList<IPlayer> CreatePlayersCollection()
		{
			return new List<IPlayer>();
		}

		public static IDictionary<SquareSpot, List<IPiece>> PiecesAtSquare()
		{
			return new Dictionary<SquareSpot, List<IPiece>>();
		}

		public static IGenerator CreateFourPlayerLudoGenerator(IBoard fourPlayerBoard = null)
		{
			IBoard board = fourPlayerBoard ?? CreateFourPlayerBoard();

			return new FourPlayerLudoGenerator(board, CreateFourPlayerOutputGenerator(), CreateFourPlayerInputGenerator());
		}

		//public static IBoard CreateBoard()
		//{
		//	return 
		//}

		private static IFourPlayerOutputGenerator CreateFourPlayerOutputGenerator()
		{
			return new FourPlayerOutputGenerator();
		}

		private static IFourPlayerInputGenerator CreateFourPlayerInputGenerator()
		{
			return new FourPlayerInputGenerator(new Random(DateTime.UtcNow.Millisecond));
		}
	}
}
