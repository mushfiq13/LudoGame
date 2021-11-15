using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudoGame
{
    public class FourPlayerBoard : IBoard
    {
        public IDice Dice { get; private set; }
        public IList<IPlayer> Players { get; private set; }
        public IPlayer? CurrentPlayer { get; set; }
        public IDictionary<SquareNumber, List<IPiece>> PiecesAtSquare { get; }

        public FourPlayerBoard()
        {
            Dice = new SixSidedDice();
            Players = new List<IPlayer>();
            PiecesAtSquare = new Dictionary<SquareNumber, List<IPiece>>();
        }

        public void AddPlayer(string name, BoardLayer layer)
        {
            Players.Add(new Player(layer));
        }
    }
}
