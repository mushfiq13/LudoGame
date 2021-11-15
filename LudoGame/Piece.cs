using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudoGame
{
    public class Piece : IPiece
    {
        public PieceNumber Id { get; set; }
        public Color Color { get; set; }
        public (SquareNumber?, HomeColumn?) CurrentPosition { get; set; }
        public bool IsMatured { get; set; }
    }
}
