using System;
using System.Collections.Generic;

namespace PiRhoSoft.MonsterMaker
{
	public class ExpressionParseException : Exception
	{
		public ExpressionParseException() : base(string.Format("Failed to parse expression - unexpected end of input"))
		{
		}

		public ExpressionParseException(string message) : base(message)
		{
		}

		public ExpressionParseException(ExpressionToken token) : base(string.Format("Failed to parse expression at {0} ({1})", token.Text, token.Location))
		{
			Token = token;
		}

		public ExpressionToken Token;
	}

	public class ExpressionParser
	{
		static ExpressionParser()
		{
			AddCommand<MinimumCommand>("Minimum");
			AddCommand<MaximumCommand>("Maximum");
			AddCommand<RandomCommand>("Random");
			AddCommand<FloorCommand>("Floor");
			AddCommand<CeilingCommand>("Ceiling");
			AddCommand<RoundCommand>("Round");
			AddCommand<TruncateCommand>("Truncate");

			AddPrefixOperator<NegateOperator>("-");
			AddPrefixOperator<InvertOperator>("!");

			AddInfixOperator<AddOperator>("+", OperatorPrecedence.Addition);
			AddInfixOperator<SubtractOperator>("-", OperatorPrecedence.Addition);
			AddInfixOperator<MultiplyOperator>("*", OperatorPrecedence.Multiplication);
			AddInfixOperator<DivideOperator>("/", OperatorPrecedence.Multiplication);
			AddInfixOperator<ModuloOperator>("%", OperatorPrecedence.Multiplication);
			AddInfixOperator<ExponentOperator>("^", OperatorPrecedence.Exponentiation);
			AddInfixOperator<AndOperator>("&&", OperatorPrecedence.And);
			AddInfixOperator<OrOperator>("||", OperatorPrecedence.Or);
			AddInfixOperator<EqualOperator>("==", OperatorPrecedence.Equality);
			AddInfixOperator<InequalOperator>("!=", OperatorPrecedence.Equality);
			AddInfixOperator<LessOperator>("<", OperatorPrecedence.Comparison);
			AddInfixOperator<GreaterOperator>(">", OperatorPrecedence.Comparison);
			AddInfixOperator<LessOrEqualOperator>("<=", OperatorPrecedence.Comparison);
			AddInfixOperator<GreaterOrEqualOperator>(">=", OperatorPrecedence.Comparison);
			AddInfixOperator<AssignOperator>("=", OperatorPrecedence.Assignment);
			AddInfixOperator<AddAssignOperator>("+=", OperatorPrecedence.Assignment);
			AddInfixOperator<SubtractAssignOperator>("-=", OperatorPrecedence.Assignment);
			AddInfixOperator<MultiplyAssignOperator>("*=", OperatorPrecedence.Assignment);
			AddInfixOperator<DivideAssignOperator>("/=", OperatorPrecedence.Assignment);
			AddInfixOperator<ModuloAssignOperator>("%=", OperatorPrecedence.Assignment);
			AddInfixOperator<ExponentAssignOperator>("^=", OperatorPrecedence.Assignment);
			AddInfixOperator<AndAssignOperator>("&=", OperatorPrecedence.Assignment);
			AddInfixOperator<OrAssignOperator>("|=", OperatorPrecedence.Assignment);
		}

		public static void AddCommand<ExpressionType>(string name) where ExpressionType : CommandExpression, new()
		{
			_commands.Add(name, new ExpressionCreator<ExpressionType>());
		}

		public static void AddPrefixOperator<ExpressionType>(string symbol) where ExpressionType : PrefixOperatorExpression, new()
		{
			_prefixOperators.Add(symbol, new ExpressionCreator<ExpressionType>());
		}

		public static void AddInfixOperator<ExpressionType>(string symbol, OperatorPrecedence precedence) where ExpressionType : InfixOperatorExpression, new()
		{
			_infixOperators.Add(symbol, new ExpressionCreator<ExpressionType>());
			_precedences.Add(symbol, precedence);
		}

		public static Expression Parse(List<ExpressionToken> tokens)
		{
			var parser = new ExpressionParser(tokens);
			return parser.Parse(0);
		}

		private abstract class ExpressionCreator
		{
			public abstract Expression Create();
		}

		private class ExpressionCreator<ExpressionType> : ExpressionCreator where ExpressionType : Expression, new()
		{
			public override Expression Create()
			{
				return new ExpressionType();
			}
		}

		private static Dictionary<string, ExpressionCreator> _commands = new Dictionary<string, ExpressionCreator>();
		private static Dictionary<string, ExpressionCreator> _prefixOperators = new Dictionary<string, ExpressionCreator>();
		private static Dictionary<string, ExpressionCreator> _infixOperators = new Dictionary<string, ExpressionCreator>();
		private static Dictionary<string, OperatorPrecedence> _precedences = new Dictionary<string, OperatorPrecedence>();

		private static ExpressionType CreateExpression<ExpressionType>(string name, Dictionary<string, ExpressionCreator> creators) where ExpressionType : Expression
		{
			ExpressionCreator creator;
			if (!creators.TryGetValue(name, out creator))
				throw new ExpressionParseException(string.Format("Command '{0}' not found", name));
			
			return creator.Create() as ExpressionType;
		}

		private static Expression CreateCommand(ExpressionToken token, List<Expression> parameters)
		{
			var expression = CreateExpression<CommandExpression>(token.Text, _commands);
			expression.Name = token.Text;
			expression.Parameters = parameters;
			return expression;
		}

		private static Expression CreatePrefixOperator(ExpressionToken token, Expression right)
		{
			var expression = CreateExpression<PrefixOperatorExpression>(token.Text, _prefixOperators);
			expression.Symbol = token.Text;
			expression.Right = right;
			return expression;
		}

		private static Expression CreateInfixOperator(Expression left, ExpressionToken token, Expression right)
		{
			var expression = CreateExpression<InfixOperatorExpression>(token.Text, _infixOperators);
			expression.Left = left;
			expression.Symbol = token.Text;
			expression.Right = right;
			return expression;
		}

		private ExpressionParser(List<ExpressionToken> tokens)
		{
			_tokens = tokens;
			_index = 0;
		}

		private List<ExpressionToken> _tokens;
		private int _index;

		private Expression Parse(int precedence)
		{
			var expression = ParsePrefix();

			while (_index < _tokens.Count && precedence < GetPrecedence(_tokens[_index]))
				expression = ParseInfix(expression);

			return expression;
		}

		private Expression ParsePrefix()
		{
			var token = TakeNextToken();

			switch (token.Type)
			{
				case ExpressionTokenType.Integer: return new IntegerExpression(token.Integer);
				case ExpressionTokenType.Number: return new NumberExpression(token.Number);
				case ExpressionTokenType.Identifier: return new IdentifierExpression(token.Text);
				case ExpressionTokenType.StartGroup: return ParseGroup();
				case ExpressionTokenType.Command: return ParseCommand(token);
				case ExpressionTokenType.Operator: return ParsePrefixOperator(token);
			}

			throw new ExpressionParseException(token);
		}

		private Expression ParseInfix(Expression left)
		{
			var token = TakeNextToken();

			if (token.Type == ExpressionTokenType.Operator)
				return ParseInfixOperator(token, left);

			throw new ExpressionParseException(token);
		}

		private Expression ParseGroup()
		{
			var expression = Parse(0);
			SkipNextToken(ExpressionTokenType.EndGroup);
			return expression;
		}

		private Expression ParseCommand(ExpressionToken token)
		{
			var nextToken = ViewNextToken();
			var parameters = new List<Expression>();

			while (nextToken.Type != ExpressionTokenType.EndGroup)
			{
				var parameter = Parse(0);
				parameters.Add(parameter);
				nextToken = ViewNextToken();

				if (nextToken.Type == ExpressionTokenType.Separator)
				{
					TakeNextToken();
					nextToken = ViewNextToken();
				}
			}

			SkipNextToken(ExpressionTokenType.EndGroup);
			return CreateCommand(token, parameters);
		}

		private Expression ParsePrefixOperator(ExpressionToken token)
		{
			var right = Parse(5);
			return CreatePrefixOperator(token, right);
		}

		private Expression ParseInfixOperator(ExpressionToken token, Expression left)
		{
			var precedence = GetAssociativePrecedence(token);
			var right = Parse(precedence);
			return CreateInfixOperator(left, token, right);
		}

		private ExpressionToken TakeNextToken()
		{
			if (_index >= _tokens.Count)
				throw new ExpressionParseException();

			return _tokens[_index++];
		}

		private ExpressionToken ViewNextToken()
		{
			if (_index >= _tokens.Count)
				throw new ExpressionParseException();

			return _tokens[_index];
		}

		private void SkipNextToken(ExpressionTokenType type)
		{
			var token = TakeNextToken();

			if (token.Type != type)
				throw new ExpressionParseException(token);
		}
		
		private int GetPrecedence(ExpressionToken token)
		{
			switch (token.Type)
			{
				case ExpressionTokenType.Integer: return int.MaxValue - 1;
				case ExpressionTokenType.Number: return int.MaxValue - 1;
				case ExpressionTokenType.Identifier: return int.MaxValue - 1;
				case ExpressionTokenType.Command: return int.MaxValue - 1;
				case ExpressionTokenType.StartGroup: return int.MaxValue;
				case ExpressionTokenType.EndGroup: return 0;
				case ExpressionTokenType.Separator: return 0;
			}

			OperatorPrecedence precedence;
			return _precedences.TryGetValue(token.Text, out precedence) ? precedence.Value : 0;
		}

		private int GetAssociativePrecedence(ExpressionToken token)
		{
			OperatorPrecedence precedence;
			return _precedences.TryGetValue(token.Text, out precedence) ? precedence.AssociativeValue : 0;
		}
	}
}
