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
        IDictionary<SquareNumber, List<IPiece>> PiecesAtSquare { get; }
        void AddPlayer(BoardLayer layer);
    }
}
