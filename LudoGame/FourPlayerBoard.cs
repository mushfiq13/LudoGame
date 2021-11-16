using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudoGame
{
    public class FourPlayerBoard : IBoard
    {
        public IDice Dice { get; private set; }
        public IList<IPlayer> Players { get; private set; }
        public IPlayer? CurrentPlayer { get; set; }
        public IDictionary<SquareSpot, List<IPiece>> PiecesAtSquare { get; }

        public FourPlayerBoard()
        {
            Dice = new SixSidedDice();
            Players = new List<IPlayer>();
            PiecesAtSquare = new Dictionary<SquareSpot, List<IPiece>>();
        }

        public void AddPlayer(string name, BoardLayer layer)
        {
            IList<IPiece> pieces = new List<IPiece>();
            
            for (var pieceId = 1; pieceId <= 4; ++pieceId)
            {
                var newPiece = new Piece();
                newPiece.Id = (PieceNumber)pieceId;
                newPiece.Color = (Color)(int)layer;
                newPiece.IsMatured = false;
                pieces.Add(newPiece);
            }

            Players.Add(new Player(name, layer, pieces));
        }        
    }
}
