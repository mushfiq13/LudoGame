using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudoGame
{
    public interface IBoard
    {
        IDice Dice { get; }
        IList<IPlayer> Players { get; }
        IPlayer? CurrentPlayer { get; set; }
        IDictionary<SquareSpot, List<IPiece>> PiecesAtSquare { get; }
        bool PlayersRanked { get; }

        void AddPlayer(string name, BoardLayer layer);        

        bool IsSafeSpot(SquareSpot selectedSpot);

        IList<IPiece>? GetSameTypeOfPieces(SquareSpot selectedSpot, Color pieceType);
        bool CanPiecePassTheSpot(SquareSpot selectedSpot, IPiece piece);

        void KillOthersIfPossible(IPiece selectedPiece, SquareSpot othersSpot);
        void KillOthersIfPossible((IPiece, IPiece) selectedPieces, SquareSpot othersSpot);

        void RemovePieceFromSpot(IPiece piece);
        void AddPieceToSpot(IPiece piece);        
    }
}
