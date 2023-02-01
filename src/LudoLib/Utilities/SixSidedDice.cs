namespace LudoLib.Utilities
{
	public class SixSidedDice : IDice
	{
		private Random _random;
		public int? CurrentValue { get; set; }

		public SixSidedDice(Random randomUtcNowMillisecond)
			=> _random = randomUtcNowMillisecond;

		public void Roll() => CurrentValue = _random.Next(1, 7);
	}
}
