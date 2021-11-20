using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudoGame
{
    public class FourPlayerLudoGenerator : IGenerator
    {
        FourPlayerLudoOutputProcessor outputProcessor;
        FourPlayerLudoInputGenerator inputGenerator;

        public FourPlayerLudoGenerator()
        {
            outputProcessor = new FourPlayerLudoOutputProcessor();
            inputGenerator = new FourPlayerLudoInputGenerator();
        }

        public void SetInitialPlayer(IBoard board, IPlayer player) => board.CurrentPlayer = player;

        public bool StartGame(IBoard board) =>        
            board.Players.Where(player => player.CanPlay() == true).Count() >= 2;
        
        public void PlayGame(IBoard board)
        {            
            SetInitialPlayer(board, board.Players[(int)BoardLayer.First - 1]);

            if (board.CurrentPlayer == null) return;

            while (!PlayersRanked(board))
            {
                outputProcessor.PlayerStatus(board.CurrentPlayer);

                if (board.CurrentPlayer.CanPlay())
                {
                    board.CurrentPlayer.RollDice(board.Dice);                    
                    outputProcessor.DiceValue(board.Dice.CurrentValue.Value);
                    MoveAPiece(board);
                }
                
                TurnPlayer(board);
            }
        }

        public bool PlayersRanked(IBoard board) =>
            board.Players.Where(player => player.CanPlay() == true).Any() ? false : true;        

        private bool TurnPlayer(IBoard board)
        {            
            for (var i = 0; board.CurrentPlayer != null && i < board.Players.Count; ++i)
            {
                if (board.CurrentPlayer.Layer != board.Players[i].Layer) continue;
                board.CurrentPlayer = board.Players[(i + 1) % 4];
                return true;
            }

            return false;
        }        

        private bool MoveAPiece(IBoard board)
        {
            if (board.CurrentPlayer == null || board.Dice.CurrentValue == null)
            {
                return false;
            }

            var piecesNewPosition = GetPiecesNextPossiblePosition(board.CurrentPlayer, board.Dice.CurrentValue.Value);                        
            
            if (!AnyPieceCanMove(piecesNewPosition))
            {
                return false;
            }

            PrintPiecePossiblePosition(piecesNewPosition);            

            var option = ChoosePiece(piecesNewPosition);
            IPiece piece = board.CurrentPlayer.Pieces[option];

            RemovePiece(board, piece);
            board.CurrentPlayer.MovePiece(piece, piecesNewPosition[option].Item2, piecesNewPosition[option].Item3);
            AddPiece(board, piece);

            return true;
        }
        
        private bool RemovePiece(IBoard board, IPiece piece)
        {
            if (piece.CurrentPosition.Item1.HasValue)
            {
                board.PiecesAtSquare.Remove(piece.CurrentPosition.Item1.Value, piece);
                return true;
            }

            return false;
        }

        private bool AddPiece(IBoard board, IPiece piece)
        {
            if (piece.CurrentPosition.Item1.HasValue)
            {
                board.PiecesAtSquare.Add(piece.CurrentPosition.Item1.Value, piece);
                return true;
            }

            return false;
        }

        private void PrintPiecePossiblePosition(IList<(PieceNumber, SquareSpot?, HomeColumn?)> piecesNewPosition)
        {
            foreach (var value in piecesNewPosition)
            {
                outputProcessor.PrintPiecePossiblePosition(value.Item1, value.Item2, value.Item3);
            }
        }

        private int ChoosePiece(IList<(PieceNumber, SquareSpot?, HomeColumn?)> piecesPosition)
        {
            int option;
            
            do { option = inputGenerator.ChoosePiece() - 1; }
            while (option < 0 || option > 3 ||
                  (!piecesPosition[option].Item2.HasValue && !piecesPosition[option].Item3.HasValue));

            return option;
        }

        private bool AnyPieceCanMove(IList<(PieceNumber, SquareSpot?, HomeColumn?)> piecesPosition) =>
            piecesPosition.Where(position => (position.Item2.HasValue || position.Item3.HasValue) == true).Any();        

        private IList<(PieceNumber, SquareSpot?, HomeColumn?)> GetPiecesNextPossiblePosition(IPlayer player, int diceValue)
        {
            var possiblePositions = new List<(PieceNumber, SquareSpot?, HomeColumn?)>();

            for (var i = 0; i < 4; ++i)
            {
                IPiece piece = player.Pieces[i];
                (SquareSpot?, HomeColumn?) toPosition = (null, null);
                BoardLayer playerLayer = (BoardLayer)((int)piece.Color);
                switch (playerLayer)
                {
                    case BoardLayer.First:
                        toPosition = GetPosition(piece, diceValue, SquareSpot.First, SquareSpot.FiftyFirst);
                        break;
                    case BoardLayer.Second:
                        toPosition = GetPosition(piece, diceValue, SquareSpot.Fourteenth, SquareSpot.Twelfth);
                        break;
                    case BoardLayer.Third:
                        toPosition = GetPosition(piece, diceValue, SquareSpot.TwentySeventh, SquareSpot.TwentyFifth);
                        break;
                    case BoardLayer.Fourth:
                        toPosition = GetPosition(piece, diceValue, SquareSpot.Fortieth, SquareSpot.ThirtyEighth);
                        break;
                }
                
                possiblePositions.Add((piece.Id, toPosition.Item1, toPosition.Item2));
            }

            return possiblePositions;
        }
        
        private (SquareSpot?, HomeColumn?) GetPosition(IPiece piece, int diceValue, SquareSpot startingSpotOfLayer, SquareSpot endingSpotOfLayer)
        {
            if (piece.IsMatured) return (null, null);

            int? toSquare = (piece.CurrentPosition.Item1 != null)
                            ? (int)piece.CurrentPosition.Item1 + diceValue
                            : null;
            int? toHome = (piece.CurrentPosition.Item2 != null)
                            ? (int)piece.CurrentPosition.Item2 + diceValue
                            : null;

            if (!toSquare.HasValue && !toHome.HasValue)
            {
                return diceValue == 6 ? (startingSpotOfLayer, null) : (null, null);
            }

            if (!toSquare.HasValue && toHome.HasValue)
            {
                return (toHome <= (int)HomeColumn.Sixth)
                        ? (null, (HomeColumn)toHome)
                        : (null, null);
            }                
            
            if (toSquare.HasValue && !toHome.HasValue)
            {
                toSquare = toSquare.Value / ((int)SquareSpot.FiftySecond + 1) + toSquare.Value % ((int)SquareSpot.FiftySecond + 1);
                return piece.CanMoveToSquareSpot(diceValue, piece.CurrentPosition.Item1.Value, endingSpotOfLayer)
                        ? ((SquareSpot)toSquare, null)
                        : (null, (HomeColumn)(toSquare - (int)endingSpotOfLayer));
            }

            return (null, null);
        }        
    }
}