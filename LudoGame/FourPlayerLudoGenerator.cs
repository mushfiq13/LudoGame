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

        public void SetInitialPlayer(IBoard fourPlayerBoard, IPlayer getPlayer)
        {
            fourPlayerBoard.CurrentPlayer = getPlayer;
        }

        public void PlayGame(IBoard fourPlayerBoard)
        {            
            for (int i = 0; i < 4; ++i)
            {
                if (!PlayerCanPlay(fourPlayerBoard.Players[i]))
                {
                    outputProcessor.PlayerUnAvailable();
                    return;
                }
            }

            if (fourPlayerBoard.CurrentPlayer == null)
            {
                SetInitialPlayer(fourPlayerBoard, fourPlayerBoard.Players[(int)BoardLayer.First - 1]);
            }

            while (IsPlayerAvailable(fourPlayerBoard))
            {
                IPlayer? currentPlayer = fourPlayerBoard.CurrentPlayer;
                outputProcessor.PlayerStatus(currentPlayer);

                if (PlayerCanPlay(currentPlayer))
                {
                    fourPlayerBoard.Dice.Roll();
                    outputProcessor.DiceValue(fourPlayerBoard.Dice.CurrentValue);
                    MoveAPiece(fourPlayerBoard);
                }
                                
                TurnPlayer(fourPlayerBoard);
            }
        }

        public bool IsPlayerAvailable(IBoard fourPlayerBoard)
        {
            for (var i = 0; i < 4; ++i)
            {
                if (PlayerCanPlay(fourPlayerBoard.Players[i]))
                {                    
                    return true;
                }
            }
            return false;
        }

        private bool PlayerCanPlay(IPlayer? player)
        {
            for (int i = 0; player != null && i < 4; ++i)
            {
                if (!player.Pieces[i].IsMatured)
                {
                    return true;
                }                
            }
            return false;
        }

        private bool TurnPlayer(IBoard fourPlayerBoard)
        {            
            for (int i = 0; fourPlayerBoard.CurrentPlayer != null && i < 4; ++i)
            {                
                if (GetColor(fourPlayerBoard.CurrentPlayer) == GetColor(fourPlayerBoard.Players[i]))
                {
                    fourPlayerBoard.CurrentPlayer = fourPlayerBoard.Players[(i + 1) % 4];
                    return true;
                }
            }
            return false;
        }

        private Color GetColor(IPlayer player)
        {
            return (Color)((int)player.Layer);
        }

        private bool MoveAPiece(IBoard fourPlayerBoard)
        {
            if (fourPlayerBoard.CurrentPlayer == null || fourPlayerBoard.Dice.CurrentValue == null)
            {
                return false;
            }

            var possiblePositions = GetPossiblePositions(fourPlayerBoard.CurrentPlayer, fourPlayerBoard.Dice.CurrentValue.Value);                        
            if (!AnyPieceCanMove(possiblePositions))
            {
                return false;
            }

            foreach (var value in possiblePositions)
            {
                outputProcessor.PossiblePositionStatus(value.Item1, value.Item2, value.Item3);
            }

            var option = ChoosePiece(possiblePositions);
            IPiece piece = fourPlayerBoard.CurrentPlayer.Pieces[option];
            var square = possiblePositions[option].Item2;
            var home = possiblePositions[option].Item3;

            if (piece.CurrentPosition.Item1.HasValue)
            {
                fourPlayerBoard.PiecesAtSquare.Remove(piece.CurrentPosition.Item1.Value, piece);
            }

            if (square.HasValue)
            {
                fourPlayerBoard.PiecesAtSquare.Add(square.Value, piece);
                piece.CurrentPosition = (square.Value, null);
            }
            else
            {
                piece.CurrentPosition = (null, home.Value);
                if (piece.CurrentPosition.Item2 == HomeColumn.Sixth)
                {
                    piece.CurrentPosition = (null, null);
                    piece.IsMatured = true;
                }
            }

            return true;
        }

        private int ChoosePiece(List<(PieceNumber, SquareNumber?, HomeColumn?)> piecesPosition)
        {
            int option;
            do
            {
                option = inputGenerator.ChoosePiece();
                --option;
            } while (option < 0 || option > 3 ||
                    (!piecesPosition[option].Item2.HasValue && !piecesPosition[option].Item3.HasValue));

            return option;
        }

        private bool AnyPieceCanMove(List<(PieceNumber, SquareNumber?, HomeColumn?)> piecesPosition)
        {
            foreach (var value in piecesPosition)
            {                
                if (value.Item2.HasValue || value.Item3.HasValue)
                    return true;
            }
            return false;
        }

        private List<(PieceNumber, SquareNumber?, HomeColumn?)> GetPossiblePositions(IPlayer player, byte diceValue)
        {
            var possiblePositions = new List<(PieceNumber, SquareNumber?, HomeColumn?)>();

            for (var i = 0; i < 4; ++i)
            {
                IPiece piece = player.Pieces[i];
                (SquareNumber?, HomeColumn?) toPosition = (null, null);
                BoardLayer playerLayer = (BoardLayer)((int)piece.Color);
                switch (playerLayer)
                {
                    case BoardLayer.First:
                        toPosition = GetPosition(piece, diceValue, SquareNumber.First, SquareNumber.FiftyFirst);
                        break;
                    case BoardLayer.Second:
                        toPosition = GetPosition(piece, diceValue, SquareNumber.Fourteenth, SquareNumber.Twelfth);
                        break;
                    case BoardLayer.Third:
                        toPosition = GetPosition(piece, diceValue, SquareNumber.TwentySeventh, SquareNumber.TwentyFifth);
                        break;
                    case BoardLayer.Fourth:
                        toPosition = GetPosition(piece, diceValue, SquareNumber.Fortieth, SquareNumber.ThirtyEighth);
                        break;
                }
                
                possiblePositions.Add((piece.Id, toPosition.Item1, toPosition.Item2));
            }

            return possiblePositions;
        }
        
        private (SquareNumber?, HomeColumn?) GetPosition(IPiece piece, byte diceValue, SquareNumber startingSquareOfLayer, SquareNumber endingSquareOfLayer)
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
                return diceValue == 6 ? (startingSquareOfLayer, null) : (null, null);
            }

            if (!toSquare.HasValue && toHome.HasValue)
            {
                return (toHome <= (int)HomeColumn.Sixth)
                        ? (null, (HomeColumn)toHome)
                        : (null, null);
            }                
            
            if (toSquare.HasValue && !toHome.HasValue)
            {
                toSquare = toSquare.Value / ((int)SquareNumber.FiftySecond + 1) + toSquare.Value % ((int)SquareNumber.FiftySecond + 1);
                return PieceCanMoveToSquare(diceValue, endingSquareOfLayer, piece.CurrentPosition.Item1.Value)
                        ? ((SquareNumber)toSquare, null)
                        : (null, (HomeColumn)(toSquare - (int)endingSquareOfLayer));
            }

            return (null, null);
        }

        private bool PieceCanMoveToSquare(byte diceValue, SquareNumber endingSquareOfLayer, SquareNumber currentSquare)
        {
            if (endingSquareOfLayer == SquareNumber.FiftyFirst)
            {
                return (int)currentSquare + diceValue <= (int)endingSquareOfLayer
                        ? true
                        : false;
            }

            if ((int)currentSquare > (int)endingSquareOfLayer || (int)currentSquare + diceValue <= (int)endingSquareOfLayer)
            {
                return true;
            }

            return false;
        }
    }
}