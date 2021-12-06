using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudoGame
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

        bool FromSquareSpotToSquareSpot(int diceValue);
        
        void Move(SquareSpot destSpot);
        void Move(Home destSpot);

        bool IsLocked();
        void Kill();
    }
}
