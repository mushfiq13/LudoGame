using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudoGame
{
    public enum Color
    {
        Red = (int)BoardLayer.First,
        Green = (int)BoardLayer.Second,
        Yellow = (int)BoardLayer.Third,
        Blue = (int)BoardLayer.Fourth
    }
}
