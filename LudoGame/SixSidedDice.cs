using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudoGame
{
    public class SixSidedDice : IDice
    {
        private Random _random;
        public int? CurrentValue { get; set; }

        public SixSidedDice() => _random = new Random(DateTime.UtcNow.Millisecond);        

        public void Roll() => CurrentValue = _random.Next(1, 7);
    }
}
