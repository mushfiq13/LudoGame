
using LudoLib.Enums;
using LudoLib.Utilities;

namespace LudoGame
{
	class Program
	{
		static void Main(string[] args)
		{
			IBoard fourPlayerBoard = Factory.CreateFourPlayerBoard();
			fourPlayerBoard.AddPlayer("A", BoardLayer.First);
			fourPlayerBoard.AddPlayer("B", BoardLayer.Second);
			fourPlayerBoard.AddPlayer("C", BoardLayer.Third);
			fourPlayerBoard.AddPlayer("D", BoardLayer.Fourth);

			IGenerator generator = Factory.CreateFourPlayerLudoGenerator(fourPlayerBoard);
			generator.PlayGame();
		}
	}
}
