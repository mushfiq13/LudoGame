using LudoLib.Enums;

namespace LudoLib.Utilities
{
	public class Piece : IPiece
	{
		public PieceNumber Id { get; private set; }
		public Color Color { get; private set; }
		public SquareSpot? CurrentSpot { get; set; }
		public Home? CurrentHome { get; set; }
		public bool IsMatured { get; set; }
		public SquareSpot StartingSpot { get; private set; }
		public SquareSpot EndingSpot { get; private set; }

		public bool IsLocked
		{
			get
			{
				return CurrentSpot != null || CurrentHome != null || !IsMatured;
			}
		}

		public Piece(PieceNumber id, BoardLayer layer)
		{
			Id = id;
			Color = (Color)(int)layer;
			IsMatured = false;
			StartingSpot = BoardConstants.StartingSpot[(int)layer];
			EndingSpot = BoardConstants.EndingSpot[(int)layer];
		}

		public void Move(SquareSpot destSpot) => CurrentSpot = destSpot;

		public void Move(Home destHome)
		{
			if (CurrentSpot != null) CurrentSpot = null;

			CurrentHome = destHome;

			if (CurrentHome == Home.Triangle) SetAsMatured();
		}

		private void SetAsMatured()
		{
			CurrentSpot = null;
			CurrentHome = null;
			IsMatured = true;
		}

		public void Kill()
		{
			CurrentSpot = null;
			CurrentHome = null;
			IsMatured = false;
		}

		public (SquareSpot?, Home?) GetWhereCanMove(int diceValue)
		{
			if (!IsMatured && !CurrentSpot.HasValue && !CurrentHome.HasValue)
			{
				return diceValue == BoardConstants.MaxDiceSide ? (StartingSpot, null) : (null, null);
			}

			if (!CurrentSpot.HasValue && CurrentHome.HasValue)
			{
				var toHome = (int)CurrentHome + diceValue;
				return toHome <= (int)Home.Triangle ? (null, (Home)toHome) : (null, null);
			}

			if (CurrentSpot.HasValue && !CurrentHome.HasValue)
			{
				var toSquare = (int)CurrentSpot + diceValue;

				return CanMoveFromSpotToSpot(diceValue)
						? ((SquareSpot)(toSquare % BoardConstants.MaxSpot), null)
						: (null, (Home)(toSquare - (int)EndingSpot - 1));
			}

			return (null, null);
		}

		private bool CanMoveFromSpotToSpot(int diceValue)
		{
			if (!CurrentSpot.HasValue) return false;

			if (EndingSpot == SquareSpot.FiftyFirst)
			{
				return (int)CurrentSpot + diceValue <= (int)EndingSpot;
			}

			return (int)CurrentSpot > (int)EndingSpot || (int)CurrentSpot + diceValue <= (int)EndingSpot;
		}
	}
}
