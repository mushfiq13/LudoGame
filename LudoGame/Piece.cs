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
        public (SquareSpot?, HomeColumn?) CurrentPosition { get; set; }
        public bool IsMatured { get; set; }

        public bool CanMoveToSquareSpot(int diceValue, SquareSpot currentSpot, SquareSpot endingSpotOfLayer)
        {
            if (endingSpotOfLayer == SquareSpot.FiftyFirst)
            {
                return (int)currentSpot + diceValue <= (int)endingSpotOfLayer
                        ? true
                        : false;
            }

            if ((int)currentSpot > (int)endingSpotOfLayer || (int)currentSpot + diceValue <= (int)endingSpotOfLayer)
            {
                return true;
            }

            return false;
        }
    }
}
