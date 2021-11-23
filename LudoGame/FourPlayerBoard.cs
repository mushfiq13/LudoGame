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
            Players.Add(new Player(name, layer, AddPieces(layer)));
        }
        
        private IList<IPiece> AddPieces(BoardLayer layer)
        {
            IList<IPiece> pieces = new List<IPiece>();

            for (var pieceId = 1; pieceId <= 4; ++pieceId)
            {
                pieces.Add(CreatePiece(layer, (PieceNumber)pieceId));
            }

            return pieces;
        }

        private IPiece CreatePiece(BoardLayer layer, PieceNumber pieceId)
        {
            return new Piece(pieceId, layer);
        }

        public bool IsSafeSpot(SquareSpot selectedSpot)
        {            
            switch ((PieceSafePosition)((int)selectedSpot))
            {
                case PieceSafePosition.Zero:
                case PieceSafePosition.Nineth:
                case PieceSafePosition.Thirteenth:
                case PieceSafePosition.TwentySecond:
                case PieceSafePosition.TwentySixth:
                case PieceSafePosition.ThirtyFifth:
                case PieceSafePosition.ThirtyNineth:
                case PieceSafePosition.FourtyEighth:
                    return true;
            }

            return false;
        }

        public void RankPlayer(IPlayer player) => Ranking.Add(player);

        public bool PlayersRanked() => Players.Where(player => player.CanPlay() == true).Any() ? false : true;

        public bool IsTheSpotBlock(SquareSpot selectedSpot)
        {            
            return SpotHasDoublePiece(selectedSpot) != null ? true : false;
        }

        public (IPiece, IPiece)? SpotHasDoublePiece(SquareSpot selectedSpot)
        {
            if (!PiecesAtSquare.ContainsKey(selectedSpot) || IsSafeSpot(selectedSpot)) return null;

            foreach (var curPiece in PiecesAtSquare[selectedSpot])
            {
                var pieces = (from piece in PiecesAtSquare[selectedSpot]
                             where piece.Color.Equals(curPiece.Color)
                             select piece);
                if (pieces.Count() == 2)
                    return (pieces.First(), pieces.Last());
            }

            return null;
        }

        public void KillOtherIfPossible(IPiece selectedPiece)
        {
            var curSpot = selectedPiece.CurrentSpot;

            if (curSpot.HasValue && !IsSafeSpot(curSpot.Value))
            {
                var pieces = PiecesAtSquare[curSpot.Value].Where(p => p.Color != selectedPiece.Color).Select(p => p);
                if (!pieces.Any()) return;

            }
        }
    }
}
