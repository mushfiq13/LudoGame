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
        public IList<IPlayer> Ranking { get; private set; }

        public FourPlayerBoard()
        {
            Dice = new SixSidedDice();
            Players = new List<IPlayer>();
            PiecesAtSquare = new Dictionary<SquareSpot, List<IPiece>>();
            Ranking = new List<IPlayer>();
        }

        public void AddPlayer(string name, BoardLayer layer)
        {
            Players.Add(new Player(name, layer, CreatePieces(layer)));
        }
        
        private IList<IPiece> CreatePieces(BoardLayer layer)
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

            return pieces;
        }

        public bool IsSafeSpot(SquareSpot? square)
        {            
            if (!square.HasValue) return false;
            
            switch ((PieceSafePosition)((int)square.Value))
            {
                case PieceSafePosition.First:
                case PieceSafePosition.Tenth:
                case PieceSafePosition.Fourteenth:
                case PieceSafePosition.TwentyThird:
                case PieceSafePosition.TwentySeventh:
                case PieceSafePosition.ThirtySixth:
                case PieceSafePosition.Fortieth:
                case PieceSafePosition.FourtyNineth:
                    return true;
            }

            return false;
        }

        public void RankPlayer(IPlayer player) => Ranking.Add(player);

        //public bool SpotIsBlock(SquareSpot spot)
        //{
        //    foreach (var piece in PiecesAtSquare[spot])
        //    {

        //    }
        //}
    }
}
