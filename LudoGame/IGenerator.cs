using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudoGame
{
    public interface IGenerator
    {
        void SetInitialPlayer(IBoard board, IPlayer player);
        bool StartGame(IBoard board);
        void PlayGame(IBoard board);
    }
}
