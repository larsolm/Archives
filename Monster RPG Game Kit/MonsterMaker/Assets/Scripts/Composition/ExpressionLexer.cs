using System;
using System.Collections.Generic;

namespace PiRhoSoft.MonsterMaker
{
	public class ExpressionTokenizeException : Exception
	{
		public ExpressionTokenizeException(string expression, int location) : base(string.Format("Unable to tokenize expression '{0}' at location {1}", expression, location))
		{
			Expression = expression;
			Location = location;
		}

		public string Expression;
		public int Location;
	}

	public static class ExpressionLexer
	{
		public static List<ExpressionToken> Tokenize(string input)
		{
			var tokens = new List<ExpressionToken>();
			var whitespace = "";
			var start = 0;

			while (start < input.Length)
			{
				var c = input[start];

				if (char.IsWhiteSpace(c))
				{
					whitespace += c;
					start++;
				}
				else
				{
					if (char.IsDigit(c))
						AddInteger(tokens, input, start, ref start, whitespace);
					else if (c == '.')
						AddNumber(tokens, input, start, ref start, whitespace);
					else if (char.IsLetter(c))
						AddIdentifier(tokens, input, start, ref start, whitespace);
					else if (c == '(')
						AddStartGroup(tokens, input, start, ref start);
					else if (c == ')')
						AddEndGroup(tokens, input, start, ref start);
					else if (c == ',')
						AddSeparator(tokens, input, start, ref start);
					else if (IsOperatorCharacter(c))
						AddOperator(tokens, input, start, ref start);
					else
						throw new ExpressionTokenizeException(input, start);

					whitespace = "";
				}
			}

			return tokens;
		}

		private const string _operatorCharacters = "+-!^*/%<=>&|";

		private static bool IsIdentifierCharacter(char c)
		{
			return char.IsLetterOrDigit(c) || c == '_';
		}

		private static bool IsOperatorCharacter(char c)
		{
			return _operatorCharacters.IndexOf(c) >= 0;
		}

		private static int SkipInteger(string input, int start)
		{
			while (start < input.Length && char.IsDigit(input[start]))
				++start;

			return start;
		}

		private static int SkipIdentifier(string input, int start)
		{
			while (start < input.Length && IsIdentifierCharacter(input[start]))
				++start;

			return start;
		}

		private static int SkipOperator(string input, int start)
		{
			while (start < input.Length && IsOperatorCharacter(input[start]))
				++start;

			return start;
		}

		private static void AddInteger(List<ExpressionToken> tokens, string input, int start, ref int end, string whitespace)
		{
			end = SkipInteger(input, end + 1);

			if (end < input.Length && char.IsLetter(input[end]))
			{
				AddIdentifier(tokens, input, start, ref end, whitespace);
			}
			else if (end < input.Length && input[end] == '.')
			{
				AddNumber(tokens, input, start, ref end, whitespace);
			}
			else
			{
				var text = input.Substring(start, end - start);
				var integer = int.Parse(text);

				AddToken(tokens, new ExpressionToken { Location = start, Type = ExpressionTokenType.Integer, Text = text, Integer = integer, Number = integer }, whitespace);
			}
		}

		private static void AddNumber(List<ExpressionToken> tokens, string input, int start, ref int end, string whitespace)
		{
			end = SkipInteger(input, end + 1);

			var text = input.Substring(start, end - start);
			var number = float.Parse(text);

			AddToken(tokens, new ExpressionToken { Location = start, Type = ExpressionTokenType.Number, Text = text, Integer = (int)number, Number = number }, whitespace);
		}

		private static void AddIdentifier(List<ExpressionToken> tokens, string input, int start, ref int end, string whitespace)
		{
			end = SkipIdentifier(input, end + 1);

			while (input[end] == '.')
				end = SkipIdentifier(input, end + 1);

			var text = input.Substring(start, end - start);
			var type = ExpressionTokenType.Identifier;

			if (input[end] == '(')
			{
				++end;
				type = ExpressionTokenType.Command;
			}

			AddToken(tokens, new ExpressionToken { Location = start, Type = type, Text = text }, whitespace);
		}

		private static void AddStartGroup(List<ExpressionToken> tokens, string input, int start, ref int end)
		{
			tokens.Add(new ExpressionToken { Location = start, Type = ExpressionTokenType.StartGroup });
			end = start + 1;
		}

		private static void AddEndGroup(List<ExpressionToken> tokens, string input, int start, ref int end)
		{
			tokens.Add(new ExpressionToken { Location = start, Type = ExpressionTokenType.EndGroup });
			end = start + 1;
		}

		private static void AddSeparator(List<ExpressionToken> tokens, string input, int start, ref int end)
		{
			tokens.Add(new ExpressionToken { Location = start, Type = ExpressionTokenType.Separator });
			end = start + 1;
		}

		private static void AddOperator(List<ExpressionToken> tokens, string input, int start, ref int end)
		{
			end = SkipOperator(input, end + 1);
			var text = input.Substring(start, end - start);
			tokens.Add(new ExpressionToken { Location = start, Type = ExpressionTokenType.Operator, Text = text });
		}

		private static void AddToken(List<ExpressionToken> tokens, ExpressionToken token, string whitespace)
		{
			if (tokens.Count > 0)
			{
				var previous = tokens[tokens.Count - 1];

				if (previous.Type == ExpressionTokenType.Integer || previous.Type == ExpressionTokenType.Number || previous.Type == ExpressionTokenType.Identifier)
				{
					if (token.Type == ExpressionTokenType.Integer || token.Type == ExpressionTokenType.Number || token.Type == ExpressionTokenType.Identifier)
					{
						previous.Text = previous.Text + whitespace + token.Text;
						previous.Type = ExpressionTokenType.Identifier;
						return;
					}
					else if (token.Type == ExpressionTokenType.Command)
					{
						previous.Text = previous.Text + whitespace + token.Text;
						previous.Type = ExpressionTokenType.Command;
						return;
					}
				}
			}

			tokens.Add(token);
		}
	}
}
