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

        public bool IsAllPiecesMatured() => !(Pieces.Where(piece => piece.IsMatured == false).Any());

        public bool CanPlay() => !IsAllPiecesMatured();
        
        public void MovePiece(IPiece piece, SquareSpot destSpot) => piece.Move(destSpot);

        public void MovePiece(IPiece piece, Home destHome) => piece.Move(destHome);       
    }
}
