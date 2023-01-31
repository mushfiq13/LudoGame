using LudoLib.Enums;
using LudoLib.IO;

namespace LudoLib.Utilities
{
	public class FourPlayerLudoGenerator : IGenerator
	{
		IBoard _board { get; }
		IFourPlayerOutputGenerator _outputProcessor { get; }
		IFourPlayerInputGenerator _inputGenerator { get; }

		public FourPlayerLudoGenerator(IBoard board, IFourPlayerOutputGenerator outputProcessor, IFourPlayerInputGenerator inputGenerator)
		{
			_board = board;
			_outputProcessor = outputProcessor;
			_inputGenerator = inputGenerator;
		}

		public void StartGame()
		{
			if (_board.Players.Where(player => player.CanPlay).Count() < 2)
			{
				throw new InvalidOperationException("Starting players must be at least 2...");
			}

			_board.CurrentPlayer = _board.Players.First();
		}

		public void PlayGame()
		{
			StartGame();

			var ranked = new Dictionary<BoardLayer, IPlayer>();

			while (!_board.PlayersRanked)
			{
				_outputProcessor.PlayerStatus(_board.CurrentPlayer);

				if (_board.CurrentPlayer.CanPlay)
				{
					_board.CurrentPlayer.RollDice(_board.Dice);

					if (!_board.Dice.CurrentValue.HasValue)
						throw new InvalidOperationException("Dice has no value.");

					_outputProcessor.PrintDiceValue(_board.Dice.CurrentValue.Value);
					MovePieceIfPossible(_board.CurrentPlayer, _board.Dice.CurrentValue.Value);
				}

				if (!_board.CurrentPlayer.CanPlay && !ranked.ContainsKey(_board.CurrentPlayer.Layer))
				{
					ranked[_board.CurrentPlayer.Layer] = _board.CurrentPlayer;
				}

				TurnPlayer();
			}

			_outputProcessor.PrintRanking(ranked);
		}

		private void TurnPlayer()
		{
			for (var i = 0; _board.CurrentPlayer != null && i < _board.Players.Count; ++i)
			{
				if (_board.CurrentPlayer.Layer != _board.Players[i].Layer) continue;

				_board.CurrentPlayer = _board.Players[(i + 1) % _board.Players.Count];

				break;
			}
		}

		private bool MovePieceIfPossible(IPlayer currentPlayer, int diceValue)
		{
			var piecesNextPossiblePosition = GetPiecesNextPossiblePosition(currentPlayer.Pieces, diceValue);

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
					_board.KillOthersIfPossible(pieces[0], possibleSpot.Value);
				else
					_board.KillOthersIfPossible((pieces[0], pieces[1]), possibleSpot.Value);
			}

			foreach (var piece in pieces)
			{
				_board.RemovePieceFromSpot(piece);

				if (possibleSpot.HasValue)
				{
					currentPlayer.TurnPiece(piece, possibleSpot.Value);
					_board.AddPieceToSpot(piece);
				}
				else if (possibleHome.HasValue)
				{
					currentPlayer.TurnPiece(piece, possibleHome.Value);
				}
			}

			return true;
		}

		private void PrintPossibleOptions(IList<(IList<IPiece>, SquareSpot?, Home?)> piecesPosition)
		{
			for (int i = 0; i < piecesPosition.Count; i++)
			{
				var pieceInfo = piecesPosition[i];

				_outputProcessor.PrintOptionNumber(i + 1);

				if (pieceInfo.Item2.HasValue)
					_outputProcessor.PrintPiecePossiblePosition(pieceInfo.Item1.Select(p => p.Id).ToList(), pieceInfo.Item2);
				else
					_outputProcessor.PrintPiecePossiblePosition(pieceInfo.Item1.Select(p => p.Id).ToList(), pieceInfo.Item3);
			}
		}

		private int ChooseOption(IList<(IList<IPiece>, SquareSpot?, Home?)> piecesPosition)
		{
			PrintPossibleOptions(piecesPosition);

			int option;

			do
			{
				option = _inputGenerator.ChoosePiece(piecesPosition.Count) - 1;
			}
			while (option < 0 || option >= piecesPosition.Count ||
					!piecesPosition[option].Item2.HasValue && !piecesPosition[option].Item3.HasValue);

			return option;
		}

		private bool AnyPieceCanMove(IList<(IList<IPiece>, SquareSpot?, Home?)> piecesPossiblePosition)
			=> piecesPossiblePosition.Where(p => p.Item2.HasValue || p.Item3.HasValue)
				.Select(x => x)
				.Any();

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
			if (_board.IsSafeSpot(selectedSpot))
			{
				return (null, null, null);
			}

			var pieces = _board.GetSameTypeOfPieces(selectedSpot, selectedType);

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
			for (var curSpot = (int)from + 1; curSpot < (int)to; curSpot = (curSpot + 1) % BoardConstants.MaxSpot)
			{
				if (!_board.CanPiecePassTheSpot((SquareSpot)curSpot, piece))
					return false;
			}

			return true;
		}

		private bool ValidatePath(SquareSpot from, Home to, IPiece piece)
		{
			return ValidatePath(from, (SquareSpot)((int)piece.EndingSpot + 1), piece);
		}
	}
}