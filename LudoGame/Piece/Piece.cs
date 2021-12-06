using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudoGame
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

        public Piece(PieceNumber id, BoardLayer layer)
        {
            Id = id;
            Color = (Color)(int)layer;
            IsMatured = false;
            StartingSpot = GlobalConstant.StartingSpot[(int)layer];
            EndingSpot = GlobalConstant.EndingSpot[(int)layer];
        }

        public bool FromSquareSpotToSquareSpot(int diceValue)
        {
            if (!CurrentSpot.HasValue) return false;

            if (EndingSpot == SquareSpot.FiftyFirst)
            {
                return (int)CurrentSpot + diceValue <= (int)EndingSpot;                        
            }

            return ((int)CurrentSpot > (int)EndingSpot || (int)CurrentSpot + diceValue <= (int)EndingSpot);            
        }

        public void Move(SquareSpot destSpot) => CurrentSpot = destSpot;        

        public void Move(Home destHome)
        {
            if (CurrentSpot != null) CurrentSpot = null;

            CurrentHome = destHome;
            
            if (CurrentHome == Home.Triangle) SetAsMatured();            
        }

        public bool IsLocked() => CurrentSpot != null || CurrentHome != null || !IsMatured;        

        public void Kill()
        {
            CurrentSpot = null;
            CurrentHome = null;
            IsMatured = false;
        }

        private void SetAsMatured()
        {
            CurrentSpot = null;
            CurrentHome = null;
            IsMatured = true;
        }
    }
}
