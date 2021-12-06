using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudoGame
{
    public interface IDice
    {
        int? CurrentValue { get; set; }
        void Roll();
    }
}
