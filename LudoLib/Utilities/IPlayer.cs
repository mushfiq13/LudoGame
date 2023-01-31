using LudoLib.Enums;

namespace LudoLib.Utilities
{
	public interface IPlayer
	{
		string Name { get; }
		bool IsAllPiecesMatured { get; }
		bool CanPlay { get; }

		IList<IPiece> Pieces { get; }
		BoardLayer Layer { get; }

		void RollDice(IDice dice);
		void TurnPiece(IPiece piece, SquareSpot destSpot);
		void TurnPiece(IPiece piece, Home destHome);
	}
}
