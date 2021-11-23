using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudoGame
{
    public class FourPlayerLudoGenerator : IGenerator
    {
        public IBoard Board { get; private set; }
        private FourPlayerLudoOutputProcessor outputProcessor;
        private FourPlayerLudoInputGenerator inputGenerator;

        public FourPlayerLudoGenerator(IBoard board)
        {
            Board = board;
            outputProcessor = new FourPlayerLudoOutputProcessor();
            inputGenerator = new FourPlayerLudoInputGenerator();
        }

        public void SetInitialPlayer(IPlayer player) => Board.CurrentPlayer = player;

        public bool StartGame() => Board.Players.Where(player => player.CanPlay() == true).Count() >= 2;

        public void PlayGame()
        {
            SetInitialPlayer(Board.Players[(int)BoardLayer.First - 1]);

            if (Board.CurrentPlayer == null) return;

            while (!Board.PlayersRanked())
            {
                outputProcessor.PlayerStatus(Board.CurrentPlayer);

                if (Board.CurrentPlayer.CanPlay())
                {
                    Board.CurrentPlayer.RollDice(Board.Dice);
                    outputProcessor.PrintDiceValue(Board.Dice.CurrentValue.Value);
                    MoveAPiece();
                }

                TurnPlayer();
            }
        }

        private bool TurnPlayer()
        {
            for (var i = 0; Board.CurrentPlayer != null && i < Board.Players.Count; ++i)
            {
                if (Board.CurrentPlayer.Layer != Board.Players[i].Layer) continue;
                Board.CurrentPlayer = Board.Players[(i + 1) % 4];
                return true;
            }

            return false;
        }

        private bool MoveAPiece()
        {
            if (Board.CurrentPlayer == null || Board.Dice.CurrentValue == null)
            {
                return false;
            }

            var possiblePosition = GetPiecesNextPossiblePosition(Board.CurrentPlayer, Board.Dice.CurrentValue.Value);            

            if (!AnyPieceCanMove(possiblePosition))
            {
                return false;
            }

            var option = ChoosePiece(possiblePosition);
            var pieces = possiblePosition[option];

            foreach (var p in pieces.Item1)
            {
                RemovePiece(p);
                
                if (pieces.Item2.HasValue)
                    Board.CurrentPlayer.MovePiece(p, pieces.Item2.Value);
                else if (pieces.Item3.HasValue)
                    Board.CurrentPlayer.MovePiece(p, pieces.Item3.Value);

                AddPiece(p);
            }

            return true;
        }

        private bool RemovePiece(IPiece piece)
        {
            if (piece.CurrentSpot.HasValue)
            {
                Board.PiecesAtSquare.Remove(piece.CurrentSpot.Value, piece);
                return true;
            }

            return false;
        }

        private bool AddPiece(IPiece piece)
        {
            if (piece.CurrentSpot.HasValue)
            {
                Board.PiecesAtSquare.Add(piece.CurrentSpot.Value, piece);
                return true;
            }

            return false;
        }

        private void PrintPossibleOptions(IList<(IList<IPiece>, SquareSpot?, HomeColumn?)> position)
        {
            for (int i = 0; i < position.Count; i++)
            {
                var pieceInfo = position[i];
               
                outputProcessor.PrintOptionNumber(i + 1);
                
                if (pieceInfo.Item2.HasValue)
                    outputProcessor.PrintPiecePossiblePosition(pieceInfo.Item1.Select(p => p.Id).ToList(), pieceInfo.Item2);
                else
                    outputProcessor.PrintPiecePossiblePosition(pieceInfo.Item1.Select(p => p.Id).ToList(), pieceInfo.Item3);
            }
        }

        private int ChoosePiece(IList<(IList<IPiece>, SquareSpot?, HomeColumn?)> possiblePosition)
        {
            PrintPossibleOptions(possiblePosition);

            int option;

            do { option = inputGenerator.ChoosePiece() - 1; }
            while (option < 0 || option >= possiblePosition.Count);

            return option;
        }

        private bool AnyPieceCanMove(IList<(IList<IPiece>, SquareSpot?, HomeColumn?)> position) =>
            position.Where(p => (p.Item2.HasValue || p.Item3.HasValue) == true).Select(p => p).Any();

        private IList<(IList<IPiece>, SquareSpot?, HomeColumn?)> GetPiecesNextPossiblePosition(IPlayer player, int diceValue)
        {            
            IList<(IList<IPiece>, SquareSpot?, HomeColumn?)> possiblePosition = new List<(IList<IPiece>, SquareSpot?, HomeColumn?)>();
            IDictionary<PieceNumber, bool> pieceFlags = new Dictionary<PieceNumber, bool>();

            for (var i = 0; i < 4; ++i)
            {
                IPiece piece = player.Pieces[i];
                
                if (pieceFlags.ContainsKey(piece.Id) && pieceFlags[piece.Id]) continue;

                var selectedPieces = GetBlockedPiecesOrDefault(piece, diceValue);
                possiblePosition.Add(selectedPieces);
                
                foreach (var p in selectedPieces.Item1)
                    pieceFlags[p.Id] = true;
            }

            return possiblePosition;
        }

        private (IList<IPiece>, SquareSpot?, HomeColumn?) GetBlockedPiecesOrDefault(IPiece selectedPiece, int diceValue)
        {
            (IPiece?, IPiece?) nextPieces = (selectedPiece, null);

            if (selectedPiece.CurrentSpot.HasValue && diceValue % 2 == 0)
            {
                var blockedPieces = Board.SpotHasDoublePiece(selectedPiece.CurrentSpot.Value);
                if (blockedPieces != null && blockedPieces.Value.Item1.Color == selectedPiece.Color)
                {
                    nextPieces = (blockedPieces.Value.Item1, blockedPieces.Value.Item2);
                }
            }

            (SquareSpot?, HomeColumn?) position;

            if (nextPieces.Item2 != null)
            {
                position = GetPosition(nextPieces.Item1, diceValue / 2);
            }
            else
            {
                position = GetPosition(nextPieces.Item1, diceValue);
            }

            if (!IsPathValid((selectedPiece.CurrentSpot, selectedPiece.CurrentHome),
                            (position.Item1, position.Item2), selectedPiece))
            {
                return (new List<IPiece>(), null, null);
            }
            
            return nextPieces.Item2 == null
                   ? (new List<IPiece> { nextPieces.Item1 }, position.Item1, position.Item2)
                   : (new List<IPiece> { nextPieces.Item1, nextPieces.Item2 }, position.Item1, position.Item2);
        }

        private bool IsPathValid((SquareSpot?, HomeColumn?) begin, (SquareSpot?, HomeColumn?) end, IPiece piece)
        {
            // piece is at inside layer...
            if (!begin.Item1.HasValue && !begin.Item2.HasValue && end.Item1.HasValue) return true;

            if (begin.Item1.HasValue)
            {
                if (end.Item1.HasValue) return ValidatePath(begin.Item1.Value, end.Item1.Value);
                if (end.Item2.HasValue) return ValidatePath(begin.Item1.Value, end.Item2.Value, piece);
            }

            return begin.Item2.HasValue && end.Item2.HasValue ? true : false;
        }

        private bool ValidatePath(SquareSpot from, SquareSpot to)
        {
            for (var curSpot = (int)from + 1; curSpot < (int)to; curSpot = ((int)curSpot + 1) % GlobalConstant.MaxSpot)
            {
                if (Board.IsTheSpotBlock((SquareSpot)curSpot))
                    return false;
            }

            return true;
        }

        private bool ValidatePath(SquareSpot from, HomeColumn to, IPiece piece)
        {
            for (var curSpot = (int)from + 1; curSpot <= (int)piece.EndingSpot; curSpot = ((int)curSpot + 1) % GlobalConstant.MaxSpot)
            {
                if (Board.IsTheSpotBlock((SquareSpot)curSpot))
                    return false;
            }

            return true;
        }

        private (SquareSpot?, HomeColumn?) GetPosition(IPiece piece, int diceValue)
        {
            if (piece.IsMatured) return (null, null);

            SquareSpot? squareSpot = piece.CurrentSpot;
            HomeColumn? homeColumn = piece.CurrentHome;

            if (!squareSpot.HasValue && !homeColumn.HasValue)
            {
                return diceValue == 6 ? (piece.StartingSpot, null) : (null, null);
            }

            if (!squareSpot.HasValue && homeColumn.HasValue)
            {
                int toHome = (int)homeColumn + diceValue;
                return (toHome <= (int)HomeColumn.Fifth)
                        ? (null, (HomeColumn)toHome)
                        : (null, null);
            }

            if (squareSpot.HasValue && !homeColumn.HasValue)
            {
                int toSquare = (int)squareSpot + diceValue;
                toSquare %= GlobalConstant.MaxSpot;
                return piece.FromSquareSpotToSquareSpot(diceValue)
                        ? ((SquareSpot)toSquare, null)
                        : (null, (HomeColumn)(toSquare - (int)piece.EndingSpot - 1));
            }

            return (null, null);
        }
    }
}