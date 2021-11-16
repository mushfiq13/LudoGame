using System;
using System.Collections.Generic;

namespace LudoGame
{
    class Program
    {
        static void Main(string[] args)
        {
            IBoard fourPlayerBoard = new FourPlayerBoard();
            fourPlayerBoard.AddPlayer("A", BoardLayer.First);
            fourPlayerBoard.AddPlayer("B", BoardLayer.Second);
            fourPlayerBoard.AddPlayer("C", BoardLayer.Third);
            fourPlayerBoard.AddPlayer("D", BoardLayer.Fourth);

            IGenerator generator = new FourPlayerLudoGenerator();
            if (generator.StartGame(fourPlayerBoard))
                generator.PlayGame(fourPlayerBoard);
        }                              
    }
}
