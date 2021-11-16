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

        bool IsSafeSpot(SquareSpot? square, HomeColumn? home);

        void RankPlayer(IPlayer player);
    }
}
