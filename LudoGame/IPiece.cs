using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudoGame
{
    public interface IPiece
    {
        PieceNumber Id { get; set; }
        Color Color { get; set; }
        (SquareSpot?, HomeColumn?) CurrentPosition { get; set; }
        bool IsMatured { get; set; }
    }
}
