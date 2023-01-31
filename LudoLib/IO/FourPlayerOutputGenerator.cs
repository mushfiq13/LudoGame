using LudoLib.Enums;
using LudoLib.Utilities;

namespace LudoLib.IO
{
	public class FourPlayerOutputGenerator : IFourPlayerOutputGenerator
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
					Console.WriteLine($" Piece at {(int)piece.CurrentSpot.Value} square");
				}
				else if (piece.CurrentHome.HasValue)
				{
					Console.WriteLine($" Piece at {(int)piece.CurrentHome.Value} home-column");
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
			Console.Write($"{string.Join(", ", pieceId)} piece(s) can ");

			if (value == null)
			{
				Console.WriteLine($"not move!");
			}
			else if (value.GetType() == typeof(SquareSpot))
			{
				Console.WriteLine($"be placed at {value} square");
			}
			else if (value.GetType() == typeof(Home))
			{
				Console.WriteLine($"be placed at {value} home-column");
			}
			else
			{
				throw new InvalidOperationException("Given piece(s) are not valid.");
			}
		}

		public void PrintAsOrdinalNumber(int number)
		{
			switch (number)
			{
				case 1:
					Console.Write("1st");
					return;
				case 2:
					Console.Write("2nd");
					return;
				case 3:
					Console.Write("3rd");
					return;
				case 4:
					Console.Write("4th");
					return;
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

		public void PrintRanking<T>(IDictionary<T, IPlayer> rank) where T : notnull
		{
			Console.WriteLine("Players Ranking...");

			var count = 0;

			foreach (var player in rank.Values)
			{
				Console.WriteLine($"{++count}.");
				Console.WriteLine($"{player.Name}");
				Console.WriteLine($"{player.Layer}");
				Console.WriteLine();
			}
		}
	}
}
