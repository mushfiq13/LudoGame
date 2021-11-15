using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudoGame
{
    public interface IGenerator
    {
        void SetInitialPlayer(IBoard fourPlayerBoard, IPlayer getPlayer);
        void PlayGame(IBoard fourPlayerBoard);
    }
}
