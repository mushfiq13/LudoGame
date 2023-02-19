using LudoLib.Enums;

namespace LudoLib.Utilities;

public class Player : IPlayer
{
	public string Name { get; private set; }
	public IList<IPiece> Pieces { get; private set; }
	public BoardLayer Layer { get; private set; }

	public Player(string name, BoardLayer layer, IList<IPiece> pieces)
	{
		Name = name;
		Layer = layer;
		Pieces = pieces;
	}

	public bool CanPlay
	{
		get
		{
			return !IsPiecesMatured();
		}
	}

	private bool IsPiecesMatured() =>
		!Pieces.Where(piece => piece.IsMatured == false)
			.Any();

	public void RollDice(IDice dice) => dice.Roll();

	public void TurnPiece(IPiece piece, SquareSpot destSpot) => piece.Move(destSpot);

	public void TurnPiece(IPiece piece, Home destHome) => piece.Move(destHome);
}
