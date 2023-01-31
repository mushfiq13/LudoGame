using LudoLib.Enums;
using LudoLib.Utilities;

namespace LudoLib.IO
{
	public interface IFourPlayerOutputGenerator
	{
		void PlayerStatus(IPlayer? player);
		void PlayerUnAvailable();
		void PrintAsOrdinalNumber(int number);
		void PrintDiceValue(int diceValue);
		void PrintOptionNumber(int number);
		void PrintPiecePossiblePosition<T>(IList<PieceNumber> pieceId, T? value);
		void PrintRanking<T>(IDictionary<T, IPlayer> rank) where T : notnull;
	}
}