using System;
using System.Collections.Generic;

namespace LudoGame
{
    class Program
    {
        static void Main(string[] args)
        {
            IBoard fourPlayerBoard = new FourPlayerBoard();
            fourPlayerBoard.AddPlayer(BoardLayer.First);
            fourPlayerBoard.AddPlayer(BoardLayer.Second);
            fourPlayerBoard.AddPlayer(BoardLayer.Third);
            fourPlayerBoard.AddPlayer(BoardLayer.Fourth);

            IGenerator generator = new FourPlayerLudoGenerator();
            if (fourPlayerBoard.Players[3] != null)
            {
                generator.SetInitialPlayer(fourPlayerBoard, fourPlayerBoard.Players[3]);
            }
            generator.PlayGame(fourPlayerBoard);
        }                              
    }
}
