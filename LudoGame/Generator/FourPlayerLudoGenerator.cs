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
        private FourPlayerLudoOutputGenerator outputProcessor;
        private FourPlayerLudoInputGenerator inputGenerator;

        public FourPlayerLudoGenerator(IBoard board)
        {
            Board = board;
            outputProcessor = new FourPlayerLudoOutputGenerator();
            inputGenerator = new FourPlayerLudoInputGenerator();
        }

        public void SetInitialPlayer() => Board.CurrentPlayer = Board.Players.First();

        public bool StartGame() => Board.Players.Where(player => player.CanPlay()).Count() >= 2;

        public void PlayGame()
        {
            if (!StartGame())
                throw new InvalidOperationException("Players must be at least 2.");
            
            SetInitialPlayer();

            if (Board.CurrentPlayer == null) return;

            var ranked = new Dictionary<BoardLayer, IPlayer>();

            while (!Board.PlayersRanked())
            {
                outputProcessor.PlayerStatus(Board.CurrentPlayer);

                if (Board.CurrentPlayer.CanPlay())
                {
                    Board.CurrentPlayer.RollDice(Board.Dice);
                    outputProcessor.PrintDiceValue(Board.Dice.CurrentValue.Value);
                    MovePieceIfPossible();
                }
                
                if (!Board.CurrentPlayer.CanPlay() && !ranked.ContainsKey(Board.CurrentPlayer.Layer))
                {
                    ranked[Board.CurrentPlayer.Layer] = Board.CurrentPlayer;
                }

                TurnPlayer();
            }

            outputProcessor.PrintRanking(ranked);
        }

        private void TurnPlayer()
        {
            for (var i = 0; Board.CurrentPlayer != null && i < Board.Players.Count; ++i)
            {
                if (Board.CurrentPlayer.Layer != Board.Players[i].Layer) continue;
                Board.CurrentPlayer = Board.Players[(i + 1) % Board.Players.Count];
                break;
            }
        }

        private bool MovePieceIfPossible()
        {
            if (Board.CurrentPlayer == null || Board.Dice.CurrentValue == null)
            {
                return false;
            }

            var piecesNextPossiblePosition = GetPiecesNextPossiblePosition(Board.CurrentPlayer.Pieces, Board.Dice.CurrentValue.Value);             

            if (!AnyPieceCanMove(piecesNextPossiblePosition))
            {
                return false;
            }            

            var option = ChooseOption(piecesNextPossiblePosition);
            var pieces = piecesNextPossiblePosition[option].Item1;
            var possibleSpot = piecesNextPossiblePosition[option].Item2;
            var possibleHome = piecesNextPossiblePosition[option].Item3;

            if (possibleSpot.HasValue)
            {
                if (pieces.Count == 1)
                    Board.KillOthersIfPossible(pieces[0], possibleSpot.Value);
                else
                    Board.KillOthersIfPossible((pieces[0], pieces[1]), possibleSpot.Value);
            }

            foreach (var piece in pieces)
            {
                Board.RemovePieceFromSpot(piece);
                if (possibleSpot.HasValue)
                {                    
                    Board.CurrentPlayer.MovePiece(piece, possibleSpot.Value);
                    Board.AddPieceToSpot(piece);
                }
                else if (possibleHome.HasValue)
                {
                    Board.CurrentPlayer.MovePiece(piece, possibleHome.Value);
                }
            }

            return true;
        }

        private void PrintPossibleOptions(IList<(IList<IPiece>, SquareSpot?, Home?)> piecesPosition)
        {
            for (int i = 0; i < piecesPosition.Count; i++)
            {
                var pieceInfo = piecesPosition[i];
               
                outputProcessor.PrintOptionNumber(i + 1);
                
                if (pieceInfo.Item2.HasValue)
                    outputProcessor.PrintPiecePossiblePosition(pieceInfo.Item1.Select(p => p.Id).ToList(), pieceInfo.Item2);
                else
                    outputProcessor.PrintPiecePossiblePosition(pieceInfo.Item1.Select(p => p.Id).ToList(), pieceInfo.Item3);
            }
        }

        private int ChooseOption(IList<(IList<IPiece>, SquareSpot?, Home?)> piecesPosition)
        {
            PrintPossibleOptions(piecesPosition);

            int option;

            do { option = inputGenerator.ChoosePiece(piecesPosition.Count) - 1; }
            while (option < 0 || option >= piecesPosition.Count ||
                    (!piecesPosition[option].Item2.HasValue && !piecesPosition[option].Item3.HasValue));

            return option;
        }

        private bool AnyPieceCanMove(IList<(IList<IPiece>, SquareSpot?, Home?)> piecesPossiblePosition) =>
            piecesPossiblePosition.Where(p => (p.Item2.HasValue || p.Item3.HasValue)).Select(x => x).Any();

        private IList<(IList<IPiece>, SquareSpot?, Home?)> GetPiecesNextPossiblePosition(IList<IPiece> selectedPieces, int diceValue)
        {            
            var possiblePosition = new List<(IList<IPiece>, SquareSpot?, Home?)>();
            var pieceStatus = new Dictionary<PieceNumber, bool>();
            
            foreach (var piece in selectedPieces)
            {
                if (pieceStatus.ContainsKey(piece.Id)) continue;
                
                if (piece.CurrentSpot.HasValue)
                {
                    var getDoublePieces = GetDoublePiecesIfSpotHasSameType(piece.CurrentSpot.Value, piece.Color, diceValue);
                    if (getDoublePieces.Item1.HasValue)
                    {
                        possiblePosition.Add(
                            (new List<IPiece> { getDoublePieces.Item1.Value.Item1, getDoublePieces.Item1.Value.Item2 },
                            getDoublePieces.Item2, getDoublePieces.Item3));
                        pieceStatus[getDoublePieces.Item1.Value.Item1.Id] = true;
                        pieceStatus[getDoublePieces.Item1.Value.Item2.Id] = true;
                        continue;
                    }
                }

                var position = GetNextPositionOfPiece(piece, diceValue);
                possiblePosition.Add((new List<IPiece> { piece }, position.Item1, position.Item2));
                pieceStatus[piece.Id] = true;
            }            

            return possiblePosition;
        }

        private ((IPiece, IPiece)?, SquareSpot?, Home?) GetDoublePiecesIfSpotHasSameType(SquareSpot selectedSpot, Color selectedType, int diceValue)
        {
            if (Board.IsSafeSpot(selectedSpot))
            {
                return (null, null, null);
            }
            
            var pieces = Board.GetSameTypeOfPieces(selectedSpot, selectedType);
            if (pieces == null || pieces.Count() != 2)
            {
                return (null, null, null);
            }               

            if (diceValue % 2 != 0)
                return ((pieces.First(), pieces.Last()), null, null);

            var position = pieces.First().GetWhereCanMove(diceValue / 2);
            return ((pieces.First(), pieces.Last()), position.Item1, position.Item2);
        }

        private (SquareSpot?, Home?) GetNextPositionOfPiece(IPiece selectedPiece, int diceValue)
        {
            var position = selectedPiece.GetWhereCanMove(diceValue);

            if (!IsPathValidForSinglePiece(selectedPiece, (position.Item1, position.Item2)))
            {
                return (null, null);
            }

            return (position.Item1, position.Item2);
        }       

        private bool IsPathValidForSinglePiece(IPiece selectedPiece, (SquareSpot?, Home?) destination)
        {
            if (selectedPiece.IsMatured) return false;
            // piece is at inside layer...
            if (!selectedPiece.CurrentSpot.HasValue && !selectedPiece.CurrentHome.HasValue && destination.Item1.HasValue) return true;

            if (selectedPiece.CurrentSpot.HasValue)
            {
                if (destination.Item1.HasValue) return ValidatePath(selectedPiece.CurrentSpot.Value, destination.Item1.Value, selectedPiece);
                if (destination.Item2.HasValue) return ValidatePath(selectedPiece.CurrentSpot.Value, destination.Item2.Value, selectedPiece);
            }

            return selectedPiece.CurrentHome.HasValue && destination.Item2.HasValue;
        }

        private bool ValidatePath(SquareSpot from, SquareSpot to, IPiece piece)
        {
            for (var curSpot = (int)from + 1; curSpot < (int)to; curSpot = (curSpot + 1) % GlobalConstant.MaxSpot)
            {
                if (!Board.CanPiecePassTheSpot((SquareSpot)curSpot, piece))
                    return false;
            }

            return true;
        }

        private bool ValidatePath(SquareSpot from, Home to, IPiece piece)
        {
            for (var curSpot = (int)from + 1; curSpot <= (int)piece.EndingSpot; curSpot = (curSpot + 1) % GlobalConstant.MaxSpot)
            {
                if (!Board.CanPiecePassTheSpot((SquareSpot)curSpot, piece))
                    return false;
            }

            return true;
        }        
    }
}