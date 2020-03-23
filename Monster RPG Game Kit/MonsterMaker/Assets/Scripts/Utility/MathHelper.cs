namespace PiRhoSoft.MonsterMaker
{
	public static class MathHelper
	{
		public static int IntExponent(int value, int exponent)
		{
			var result = 1;
			for (long i = 0; i < exponent; i++)
				result *= value;

			return result;
		}

		public static int LogBase2(int value)
		{
			var result = 0;

			value >>= 1;

			while (value > 0)
			{
				value >>= 1;
				++result;
			}

			return result;
		}
	}
}
