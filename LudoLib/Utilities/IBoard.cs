

using LudoLib.Enums;

namespace LudoLib.Utilities
{
	public interface IBoard
	{
		bool PlayersRanked { get; }
		IDice Dice { get; }
		IList<IPlayer> Players { get; }
		IPlayer? CurrentPlayer { get; set; }
		IDictionary<SquareSpot, List<IPiece>> PiecesAtSquare { get; }

		void AddPlayer(string name, BoardLayer layer);

		bool IsSafeSpot(SquareSpot selectedSpot);

		IList<IPiece>? GetSameTypeOfPieces(SquareSpot selectedSpot, Color pieceType);
		bool CanPiecePassTheSpot(SquareSpot selectedSpot, IPiece piece);

		void KillOthersIfPossible(IPiece movingPiece, SquareSpot othersSpot);
		void KillOthersIfPossible((IPiece, IPiece) movingPieces, SquareSpot othersSpot);

		void RemovePieceFromSpot(IPiece piece);
		void AddPieceToSpot(IPiece piece);
	}
}
