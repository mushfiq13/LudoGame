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
        BoardLayer Layer { get; }
        bool IsAllPiecesMatured { get; }
        bool CanPlay { get; }

        void RollDice(IDice dice);                
        
        void TurnPiece(IPiece piece, SquareSpot destSpot);
        void TurnPiece(IPiece piece, Home destHome);
    }
}
