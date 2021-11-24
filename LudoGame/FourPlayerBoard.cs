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

        public void AddPlayer(string name, BoardLayer layer) => Players.Add(new Player(name, layer, AddPieces(layer)));        
        
        private IList<IPiece> AddPieces(BoardLayer layer)
        {
            IList<IPiece> pieces = new List<IPiece>();

            for (var pieceId = 1; pieceId <= 4; ++pieceId)
            {
                pieces.Add(CreatePiece(layer, (PieceNumber)pieceId));
            }

            return pieces;
        }

        private IPiece CreatePiece(BoardLayer layer, PieceNumber pieceId) => new Piece(pieceId, layer);

        public bool IsSafeSpot(SquareSpot selectedSpot)
        {            
            switch ((PieceSafeSpot)((int)selectedSpot))
            {
                case PieceSafeSpot.Zero:
                case PieceSafeSpot.Nineth:
                case PieceSafeSpot.Thirteenth:
                case PieceSafeSpot.TwentySecond:
                case PieceSafeSpot.TwentySixth:
                case PieceSafeSpot.ThirtyFifth:
                case PieceSafeSpot.ThirtyNineth:
                case PieceSafeSpot.FourtyEighth:
                    return true;
            }

            return false;
        }

        public bool PlayersRanked() => !(Players.Where(player => player.CanPlay() == true).Any());

        public bool IsTheSpotBlock(SquareSpot selectedSpot, IPiece piece) => SpotHasSamePiece(selectedSpot, piece) != null;

        public (IPiece, IPiece)? SpotHasSamePiece(SquareSpot selectedSpot, IPiece piece)
        {
            if (!PiecesAtSquare.ContainsKey(selectedSpot) || IsSafeSpot(selectedSpot)) return null;

            var pieces = (from p in PiecesAtSquare[selectedSpot]
                         where p.Color.Equals(piece.Color)
                         select p);

            return (pieces.Count() == 2)
                    ? (pieces.First(), pieces.Last())
                    : null;
        }

        private IDictionary<Color, List<IPiece>> GetPieces(SquareSpot spot, Color withoutThisColor)
        {
            IDictionary<Color, List<IPiece>> pieces = new Dictionary<Color, List<IPiece>>();

            if (PiecesAtSquare.ContainsKey(spot))
            {
                foreach (var p in PiecesAtSquare[spot])
                {
                    if (!p.Color.Equals(withoutThisColor))
                        pieces.Add(p.Color, p);
                }
            }

            return pieces;
        }

        public void KillOthersIfPossible(IPiece selectedPiece, SquareSpot othersSpot)
        {
            if (!selectedPiece.CurrentSpot.HasValue || IsSafeSpot(othersSpot)) return;

            Kill(GetPieces(othersSpot, selectedPiece.Color), othersSpot, (p) => p.Count() != 2);
        }

        public void KillOthersIfPossible((IPiece, IPiece) selectedPieces, SquareSpot othersSpot)
        {
            if ( IsSafeSpot(othersSpot)) return;
            if (selectedPieces.Item1.Color != selectedPieces.Item2.Color) return;
            if (selectedPieces.Item1.CurrentSpot != selectedPieces.Item2.CurrentSpot) return;
            if (!selectedPieces.Item1.CurrentSpot.HasValue || IsSafeSpot(selectedPieces.Item1.CurrentSpot.Value)) return;

            Kill(GetPieces(othersSpot, selectedPieces.Item1.Color), othersSpot, (p) => p.Count() == 2);
        }

        private void Kill(IDictionary<Color, List<IPiece>> selectedPieces, SquareSpot killingSpot, Predicate<List<IPiece>> checkIfPiecesKill)
        {
            foreach (var pieces in selectedPieces)
            {
                if (!checkIfPiecesKill(pieces.Value)) continue;
                foreach (var p in pieces.Value)
                {
                    p.Kill();
                    PiecesAtSquare[killingSpot].Remove(p);
                }
            }
        }
    }
}
