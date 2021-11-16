using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudoGame
{
    public class Player : IPlayer
    {
        public string Name { get; }
        public IList<IPiece> Pieces { get; }
        public BoardLayer Layer { get; set; }

        public Player(string name, BoardLayer layer, IList<IPiece> pieces)
        {
            Name = name;
            Layer = layer;
            Pieces = pieces;
        }

        public void RollDice(IDice dice) => dice.Roll();

        public bool IsAllPiecesMatured() =>
            Pieces.Where(piece => piece.IsMatured == false).Any() ? false : true;

        public bool CanPlay() => IsAllPiecesMatured() ? false : true;
        
        public void MovePiece(IPiece piece, SquareSpot? square, HomeColumn? home)
        {
            if (square.HasValue)
            {
                piece.CurrentPosition = (square.Value, null);
            }
            else if (home.HasValue)
            {
                piece.CurrentPosition = (null, home.Value);
                if (piece.CurrentPosition.Item2 == HomeColumn.Sixth)
                {
                    piece.CurrentPosition = (null, null);
                    piece.IsMatured = true;
                }
            }
        }
    }
}
