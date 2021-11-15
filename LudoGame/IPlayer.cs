using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudoGame
{
    public interface IPlayer
    {
        string Name { get; }
        IList<IPiece> Pieces { get; }
        BoardLayer Layer { get; set; }
        int RollDice(IDice dice);
        bool IsAllPiecesMatured();
    }
}
