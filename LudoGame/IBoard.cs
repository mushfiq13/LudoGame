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
        IList<IPlayer> Ranking { get; }

        void AddPlayer(string name, BoardLayer layer);        

        bool IsSafeSpot(SquareSpot selectedSpot);
        bool IsTheSpotBlock(SquareSpot selectedSpot);
        (IPiece, IPiece)? SpotHasDoublePiece(SquareSpot selectedSpot);

        void RankPlayer(IPlayer player);
        bool PlayersRanked();
    }
}
