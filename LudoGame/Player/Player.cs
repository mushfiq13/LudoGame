using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudoGame
{
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

        public bool IsAllPiecesMatured
        {
            get
            {
                return !(Pieces.Where(piece => piece.IsMatured == false).Any());
            }
        }

        public bool CanPlay
        {
            get
            {
                return !IsAllPiecesMatured;
            }
        }

        public void RollDice(IDice dice) => dice.Roll();        
        
        public void TurnPiece(IPiece piece, SquareSpot destSpot) => piece.Move(destSpot);

        public void TurnPiece(IPiece piece, Home destHome) => piece.Move(destHome);       
    }
}
