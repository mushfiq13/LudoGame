namespace LudoLib.Utilities
{
	public interface IDice
	{
		int? CurrentValue { get; set; }
		void Roll();
	}
}
