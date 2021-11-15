using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudoGame
{
    public class Player : IPlayer
    {
        public IList<IPiece> Pieces { get; }
        public BoardLayer Layer { get; set; }

        public Player(BoardLayer layer)
        {
            Pieces = new List<IPiece>();
            Layer = layer;            

            for (var pieceId = 1; pieceId <= 4; pieceId++)
            {
                var newPiece = new Piece();
                newPiece.Id = (PieceNumber)pieceId;
                newPiece.Color = (Color)(int)layer;
                newPiece.IsMatured = false;
                Pieces.Add(newPiece);
            }
        }        
    }
}
