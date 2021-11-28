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

        private IList<IPiece> GetKillingPieces(SquareSpot killingSpot, Color killWithoutThisColor, Predicate<IList<IPiece>> check)
        {
            IList<IPiece> pieces = new List<IPiece>();

            if (!PiecesAtSquare.ContainsKey(killingSpot)) return pieces;

            var colors = new Color[] { Color.Red, Color.Green, Color.Blue, Color.Yellow };
            foreach (var color in colors)
            {
                if (color == killWithoutThisColor) continue;

                var getSamePieces = (from piece in PiecesAtSquare[killingSpot]
                                     where piece.Color == color
                                     select piece).ToList();

                if (check(getSamePieces))
                {
                    foreach (var i in getSamePieces)
                        pieces.Add(i);
                }
            }
            if (pieces.Count > 0)
            {
                foreach (var item in pieces)
                {
                    Console.Write(item.Id + ", " + item.Color + " .. ");
                }
                Console.WriteLine();
            }
            return pieces;
        }

        public void KillOthersIfPossible(IPiece selectedPiece, SquareSpot othersSpot)
        {
            if (!selectedPiece.CurrentSpot.HasValue || IsSafeSpot(othersSpot)) return;            

            Kill(GetKillingPieces(othersSpot, selectedPiece.Color, (collection) => collection.Count() != 2),
                othersSpot);
        }

        public void KillOthersIfPossible((IPiece, IPiece) selectedPieces, SquareSpot othersSpot)
        {
            if ( IsSafeSpot(othersSpot)) return;
            if (selectedPieces.Item1.Color != selectedPieces.Item2.Color) return;
            if (selectedPieces.Item1.CurrentSpot != selectedPieces.Item2.CurrentSpot) return;
            if (!selectedPieces.Item1.CurrentSpot.HasValue || IsSafeSpot(selectedPieces.Item1.CurrentSpot.Value)) return;
            
            Kill(GetKillingPieces(othersSpot, selectedPieces.Item1.Color, (collection) => collection.Count() == 2),
                othersSpot);
        }

        private void Kill(IList<IPiece> selectedPieces, SquareSpot killingSpot)
        {
            foreach (var piece in selectedPieces)
            {
                piece.Kill();
                PiecesAtSquare.Remove(killingSpot, piece);
            }
        }
    }
}
