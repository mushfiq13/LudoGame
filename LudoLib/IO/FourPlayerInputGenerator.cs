namespace LudoLib.IO
{
	public class FourPlayerInputGenerator : IFourPlayerInputGenerator
	{
		private Random _random;

		public FourPlayerInputGenerator(Random randomUtcNowMillisecond)
			=> _random = randomUtcNowMillisecond;

		public int ChoosePiece(int maxOptions)
		{
			Console.WriteLine("Which option do you want to choice?");
			//if (!int.TryParse(Console.ReadLine(), out int option) || option < 1 || option > maxOptions)
			//{
			//    Console.WriteLine("Please provide a valid piece for which the piece can move!");
			//    option = -1;
			//}
			//return option;
			return _random.Next(1, maxOptions + 1);
		}
	}
}
