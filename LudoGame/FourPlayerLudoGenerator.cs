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

        public bool StartGame() => Board.Players.Where(player => player.CanPlay()).Count() >= 2;

        public void PlayGame()
        {
            SetInitialPlayer(Board.Players[(int)BoardLayer.First - 1]);

            if (Board.CurrentPlayer == null) return;

            IDictionary<BoardLayer, IPlayer> ranked = new Dictionary<BoardLayer, IPlayer>();

            while (!Board.PlayersRanked())
            {
                Console.WriteLine();
                foreach (var i in Board.PiecesAtSquare)
                {
                    Console.WriteLine((int)i.Key + ".");
                    foreach (var j in i.Value)
                        Console.Write($"({j.Id}, {j.Color}), ");
                    Console.WriteLine();
                }
                Console.WriteLine();
                outputProcessor.PlayerStatus(Board.CurrentPlayer);

                if (Board.CurrentPlayer.CanPlay())
                {
                    Board.CurrentPlayer.RollDice(Board.Dice);
                    outputProcessor.PrintDiceValue(Board.Dice.CurrentValue.Value);
                    MoveAPiece();
                }
                
                if (!Board.CurrentPlayer.CanPlay() && !ranked.ContainsKey(Board.CurrentPlayer.Layer))
                {
                    ranked[Board.CurrentPlayer.Layer] = Board.CurrentPlayer;
                }

                TurnPlayer();
            }

            outputProcessor.PrintRanking(ranked);
        }

        private bool TurnPlayer()
        {
            for (var i = 0; Board.CurrentPlayer != null && i < Board.Players.Count; ++i)
            {
                if (Board.CurrentPlayer.Layer != Board.Players[i].Layer) continue;
                Board.CurrentPlayer = Board.Players[(i + 1) % Board.Players.Count];
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

            var option = ChooseOption(possiblePosition);
            var pieces = possiblePosition[option];
            
            if (pieces.Item2.HasValue)
            {
                if (pieces.Item1.Count == 1) Board.KillOthersIfPossible(pieces.Item1[0], pieces.Item2.Value);
                else Board.KillOthersIfPossible((pieces.Item1[0], pieces.Item1[1]), pieces.Item2.Value);
            }

            foreach (var p in pieces.Item1)
            {
                if (pieces.Item2.HasValue)
                {
                    RemovePieceFromSpot(p);
                    Board.CurrentPlayer.MovePiece(p, pieces.Item2.Value);
                    AddPieceToSpot(p);
                }
                else if (pieces.Item3.HasValue)
                    Board.CurrentPlayer.MovePiece(p, pieces.Item3.Value);
            }

            return true;
        }

        private bool RemovePieceFromSpot(IPiece piece) =>
            piece.CurrentSpot.HasValue && Board.PiecesAtSquare.Remove(piece.CurrentSpot.Value, piece);

        private void AddPieceToSpot(IPiece piece)
        {
            if (piece.CurrentSpot.HasValue)
                Board.PiecesAtSquare.Add(piece.CurrentSpot.Value, piece);
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

        private int ChooseOption(IList<(IList<IPiece>, SquareSpot?, HomeColumn?)> possiblePosition)
        {
            PrintPossibleOptions(possiblePosition);

            int option;

            do { option = inputGenerator.ChoosePiece(possiblePosition.Count) - 1; }
            while (option < 0 || option >= possiblePosition.Count ||
                    (!possiblePosition[option].Item2.HasValue && !possiblePosition[option].Item3.HasValue));

            return option;
        }

        private bool AnyPieceCanMove(IList<(IList<IPiece>, SquareSpot?, HomeColumn?)> position) =>
            position.Where(p => (p.Item2.HasValue || p.Item3.HasValue) == true).Any();

        private IList<(IList<IPiece>, SquareSpot?, HomeColumn?)> GetPiecesNextPossiblePosition(IPlayer player, int diceValue)
        {            
            IList<(IList<IPiece>, SquareSpot?, HomeColumn?)> possiblePosition = new List<(IList<IPiece>, SquareSpot?, HomeColumn?)>();
            IDictionary<PieceNumber, bool> pieceStatus = new Dictionary<PieceNumber, bool>();
            
            foreach (var piece in player.Pieces)
            {
                if (pieceStatus.ContainsKey(piece.Id)) continue;
                
                if (piece.CurrentSpot.HasValue)
                {
                    var getDoublePieces = GetDoublePiecesIfSpotHasSameType(piece.CurrentSpot.Value, piece, diceValue);
                    if (getDoublePieces.Item1.HasValue)
                    {
                        possiblePosition.Add(
                            (new List<IPiece> { getDoublePieces.Item1.Value.Item1, getDoublePieces.Item1.Value.Item2 },
                            getDoublePieces.Item2, getDoublePieces.Item3));
                        pieceStatus[getDoublePieces.Item1.Value.Item1.Id] = true;
                        pieceStatus[getDoublePieces.Item1.Value.Item2.Id] = true;
                        Console.WriteLine("double pieces!");
                        continue;
                    }
                }

                var position = GetPositionIfPossibleForSinglePiece(piece, diceValue);
                possiblePosition.Add((new List<IPiece> { piece }, position.Item1, position.Item2));
                pieceStatus[piece.Id] = true;
                Console.WriteLine("Single pieces!");
            }

            //for (var i = 0; i < 4; ++i)
            //{
            //    IPiece piece = player.Pieces[i];

            //    if (pieceStatus.ContainsKey(piece.Id) && pieceStatus[piece.Id]) continue;

            //    var selectedPieces = GetSingleOrDoublePieces(piece, diceValue);
            //    possiblePosition.Add(selectedPieces);

            //    foreach (var p in selectedPieces.Item1)
            //        pieceStatus[p.Id] = true;
            //}

            return possiblePosition;
        }

        private ((IPiece, IPiece)?, SquareSpot?, HomeColumn?) GetDoublePiecesIfSpotHasSameType(SquareSpot selectedSpot, IPiece selectedPiece, int diceValue)
        {                        
            if (!Board.IsSafeSpot(selectedSpot))
            {
                var doublePieces = SpotHasSamePiece(selectedSpot, selectedPiece);
                if (doublePieces != null)
                {
                    if ((diceValue % 2 != 0))
                        return ((doublePieces.Value.Item1, doublePieces.Value.Item2), null, null);

                    var position = GetPosition(doublePieces.Value.Item1, diceValue / 2);
                    return ((doublePieces.Value.Item1, doublePieces.Value.Item2), position.Item1, position.Item2);
                }
            }

            return (null, null, null);
        }

        private (SquareSpot?, HomeColumn?) GetPositionIfPossibleForSinglePiece(IPiece selectedPiece, int diceValue)
        {
            var position = GetPosition(selectedPiece, diceValue);

            if (!IsPathValidForSinglePiece(selectedPiece, (position.Item1, position.Item2)))
            {
                return (null, null);
            }

            return (position.Item1, position.Item2);
        }

        private (IList<IPiece>, SquareSpot?, HomeColumn?) GetSingleOrDoublePieces(IPiece selectedPiece, int diceValue)
        {
            (SquareSpot?, HomeColumn?) position;

            if (selectedPiece.CurrentSpot.HasValue)
            {
                var doublePieces = SpotHasSamePiece(selectedPiece.CurrentSpot.Value, selectedPiece);
                if (doublePieces != null && doublePieces.Value.Item1.Color == selectedPiece.Color)
                {
                    if ((diceValue & 1) == 1)
                        return (new List<IPiece> { doublePieces.Value.Item1, doublePieces.Value.Item2 }, null, null);

                    position = GetPosition(doublePieces.Value.Item1, diceValue / 2);
                    return (new List<IPiece> { doublePieces.Value.Item1, doublePieces.Value.Item2 }, position.Item1, position.Item2);
                }
            }

            position = GetPosition(selectedPiece, diceValue);

            if (!IsPathValidForSinglePiece(selectedPiece, (position.Item1, position.Item2)))
            {
                return (new List<IPiece> { selectedPiece }, null, null);
            }

            return (new List<IPiece> { selectedPiece }, position.Item1, position.Item2);
        }

        private (IPiece, IPiece)? SpotHasSamePiece(SquareSpot selectedSpot, IPiece piece)
        {
            if (!Board.PiecesAtSquare.ContainsKey(selectedSpot)) return null;

            var pieces = (from p in Board.PiecesAtSquare[selectedSpot]
                          where p.Color.Equals(piece.Color)
                          select p);

            return (pieces.Count() == 2)
                    ? (pieces.First(), pieces.Last())
                    : null;
        }

        private bool IsPathValidForSinglePiece(IPiece selectedPiece, (SquareSpot?, HomeColumn?) destination)
        {
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
            for (var curSpot = (int)from + 1; curSpot < (int)to; curSpot = ((int)curSpot + 1) % GlobalConstant.MaxSpot)
            {
                if (!PiceCanPassTheSpot((SquareSpot)curSpot, piece))
                    return false;
            }

            return true;
        }

        private bool PiceCanPassTheSpot(SquareSpot selectedSpot, IPiece piece)
        {
            if (Board.IsSafeSpot(selectedSpot) || !Board.PiecesAtSquare.ContainsKey(selectedSpot))
                return true;

            var colors = new Color[] { Color.Red, Color.Green, Color.Blue, Color.Yellow };
            foreach (var color in colors)
            {
                if (piece.Color == color) continue;
                var getPieces = from p in Board.PiecesAtSquare[selectedSpot]
                                where p.Color == color
                                select p;
                if (getPieces.Count() == 2)
                    return false;
            }
            return true;
        }

        private bool ValidatePath(SquareSpot from, HomeColumn to, IPiece piece)
        {
            for (var curSpot = (int)from + 1; curSpot <= (int)piece.EndingSpot; curSpot = ((int)curSpot + 1) % GlobalConstant.MaxSpot)
            {
                if (!PiceCanPassTheSpot((SquareSpot)curSpot, piece))
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
                var toHome = (int)homeColumn + diceValue;
                return (toHome <= (int)HomeColumn.Fifth) ? (null, (HomeColumn)toHome) : (null, null);
            }

            if (squareSpot.HasValue && !homeColumn.HasValue)
            {
                var toSquare = (int)squareSpot + diceValue;
                return piece.FromSquareSpotToSquareSpot(diceValue)
                        ? ((SquareSpot)(toSquare % GlobalConstant.MaxSpot), null)
                        : (null, (HomeColumn)(toSquare - (int)piece.EndingSpot - 1));
            }

            return (null, null);
        }
    }
}