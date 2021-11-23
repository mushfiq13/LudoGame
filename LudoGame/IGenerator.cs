using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudoGame
{
    public interface IGenerator
    {
        IBoard Board { get; }

        void SetInitialPlayer(IPlayer player);
        bool StartGame();
        void PlayGame();
    }
}
