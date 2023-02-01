using LudoLib.Enums;

namespace LudoLib.Utilities
{
	public interface IPiece
	{
		PieceNumber Id { get; }
		Color Color { get; }
		SquareSpot? CurrentSpot { get; set; }
		Home? CurrentHome { get; set; }
		bool IsMatured { get; set; }
		SquareSpot StartingSpot { get; }
		SquareSpot EndingSpot { get; }
		bool IsLocked { get; }

		void Move(SquareSpot destSpot);
		void Move(Home destSpot);
		void Kill();
		(SquareSpot?, Home?) GetWhereCanMove(int diceValue);
	}
}
