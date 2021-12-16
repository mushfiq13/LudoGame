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
        public IDictionary<SquareSpot, List<IPiece>> PiecesAtSquare { get; private set; }        

        public bool PlayersRanked
        {
            get
            {
                return !(Players.Where(player => player.CanPlay == true).Any());
            }
        }

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
                case PieceSafeSpot.First:
                case PieceSafeSpot.Tenth:
                case PieceSafeSpot.Fourteenth:
                case PieceSafeSpot.TwentyThird:
                case PieceSafeSpot.TwentySeventh:
                case PieceSafeSpot.ThirtySixth:
                case PieceSafeSpot.Fortieth:
                case PieceSafeSpot.FourtyNineth:
                    return true;
            }

            return false;
        }        

        public IList<IPiece>? GetSameTypeOfPieces(SquareSpot selectedSpot, Color pieceType)
        {
            if (!PiecesAtSquare.ContainsKey(selectedSpot)) return null;
            return PiecesAtSquare[selectedSpot].Where(piece => piece.Color == pieceType).Select(piece => piece).ToList();
        }

        private IList<IPiece> GetPieces(SquareSpot selectedSpot, Color withoutThisColor, Predicate<IList<IPiece>> checkPiecesCanGet)
        {
            List<IPiece> pieces = new List<IPiece>();
            var colors = new Color[] { Color.Red, Color.Green, Color.Blue, Color.Yellow };

            foreach (var color in colors)
            {
                if (withoutThisColor == color) continue;

                var getPieces = GetSameTypeOfPieces(selectedSpot, color);
                if (getPieces == null || !checkPiecesCanGet(getPieces)) continue;

                pieces.AddRange(getPieces);
            }

            return pieces;
        }

        public bool CanPiecePassTheSpot(SquareSpot selectedSpot, IPiece piece) =>
            IsSafeSpot(selectedSpot) || !GetPieces(selectedSpot, piece.Color, pieces => pieces.Count() == 2).Any();

        public void KillOthersIfPossible(IPiece movingPiece, SquareSpot othersSpot)
        {
            if (!movingPiece.CurrentSpot.HasValue || IsSafeSpot(othersSpot)) return;
            Kill(GetPieces(othersSpot, movingPiece.Color, piece => piece.Count() != 2),
                othersSpot);
        }

        public void KillOthersIfPossible((IPiece, IPiece) movingPieces, SquareSpot othersSpot)
        {
            if (IsSafeSpot(othersSpot)) return;
            if (movingPieces.Item1.Color != movingPieces.Item2.Color) return;
            if (movingPieces.Item1.CurrentSpot != movingPieces.Item2.CurrentSpot) return;
            if (!movingPieces.Item1.CurrentSpot.HasValue || IsSafeSpot(movingPieces.Item1.CurrentSpot.Value)) return;

            Kill(GetPieces(othersSpot, movingPieces.Item1.Color, piece => piece.Count() == 2),
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

        public void AddPieceToSpot(IPiece piece)
        {
            if (piece.CurrentSpot.HasValue)
                PiecesAtSquare.Add(piece.CurrentSpot.Value, piece);
        }

        public void RemovePieceFromSpot(IPiece piece)
        {
            if (piece.CurrentSpot.HasValue)
                PiecesAtSquare.Remove(piece.CurrentSpot.Value, piece);
        }               
    }
}
