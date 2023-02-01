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

		private static IDice CreateSixSidedDice()
		{
			return new SixSidedDice(new Random(DateTime.UtcNow.Millisecond));
		}

		private static IList<IPlayer> CreatePlayersCollection()
		{
			return new List<IPlayer>();
		}

		private static IDictionary<SquareSpot, List<IPiece>> PiecesAtSquare()
		{
			return new Dictionary<SquareSpot, List<IPiece>>();
		}

		public static IGenerator CreateFourPlayerLudoGenerator(IBoard fourPlayerBoard = null)
		{
			IBoard board = fourPlayerBoard ?? CreateFourPlayerBoard();

			return new FourPlayerLudoGenerator(board, CreateFourPlayerOutputGenerator(), CreateFourPlayerInputGenerator());
		}

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
