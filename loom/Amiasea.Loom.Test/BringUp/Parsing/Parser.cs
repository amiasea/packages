using System;
using System.Collections.Generic;
using System.Globalization;
using Amiasea.Loom.AST;

namespace Amiasea.Loom.Test.BringUp
{
    public sealed class Parser
    {
        private readonly List<Token> _tokens;
        private int _position;

        public Parser(List<Token> tokens)
        {
            _tokens = tokens ?? throw new ArgumentNullException(nameof(tokens));
        }

        private Token Current => _tokens[_position];
        private Token Advance() => _tokens[_position++];

        private bool Match(TokenKind kind)
        {
            if (Current.Kind == kind)
            {
                Advance();
                return true;
            }
            return false;
        }

        private Token Expect(TokenKind kind)
        {
            if (Current.Kind != kind)
                throw new InvalidOperationException($"Expected {kind} but found {Current.Kind}.");

            return Advance();
        }

        private string ExpectName()
        {
            var token = Expect(TokenKind.Name);
            return token.Value;
        }

        // ------------------------------------------------------------
        // DOCUMENT
        // ------------------------------------------------------------

        public DocumentNode ParseDocument()
        {
            var operations = new List<OperationNode>();

            while (Current.Kind != TokenKind.EndOfFile)
                operations.Add(ParseOperation());

            return new DocumentNode(
                operations,
                Array.Empty<FragmentDefinitionNode>()
            );
        }

        // ------------------------------------------------------------
        // OPERATION
        // ------------------------------------------------------------

        private OperationNode ParseOperation()
        {
            var selectionSet = ParseSelectionSet();

            return new OperationNode(
                "anonymous",
                OperationKind.Query,
                selectionSet
            );
        }

        // ------------------------------------------------------------
        // SELECTION SET
        // ------------------------------------------------------------

        private SelectionSetNode ParseSelectionSet()
        {
            Expect(TokenKind.LeftBrace);

            var fields = new List<FieldNode>();

            while (!Match(TokenKind.RightBrace))
                fields.Add(ParseField());

            return new SelectionSetNode(fields);
        }

        // ------------------------------------------------------------
        // FIELD
        // ------------------------------------------------------------

        private FieldNode ParseField()
        {
            var name = ExpectName();
            string? alias = null;

            if (Match(TokenKind.Colon))
            {
                alias = name;
                name = ExpectName();
            }

            var arguments = Match(TokenKind.LeftParen)
                ? ParseArguments()
                : Array.Empty<ArgumentNode>();

            SelectionSetNode? selectionSet = null;

            if (Current.Kind == TokenKind.LeftBrace)
                selectionSet = ParseSelectionSet();

            return new FieldNode(
                name,
                alias,
                arguments,
                selectionSet
            );
        }

        // ------------------------------------------------------------
        // ARGUMENTS
        // ------------------------------------------------------------

        private IReadOnlyList<ArgumentNode> ParseArguments()
        {
            var args = new List<ArgumentNode>();

            while (!Match(TokenKind.RightParen))
            {
                var name = ExpectName();
                Expect(TokenKind.Colon);
                var value = ParseValueLiteral();
                args.Add(new ArgumentNode(name, value));

                Match(TokenKind.Comma);
            }

            return args;
        }

        // ------------------------------------------------------------
        // VALUE LITERALS
        // ------------------------------------------------------------

        private ValueNode ParseValueLiteral()
        {
            switch (Current.Kind)
            {
                case TokenKind.Int:
                    {
                        var token = Advance();
                        var value = long.Parse(token.Value, CultureInfo.InvariantCulture);
                        return new IntValueNode(value);
                    }

                case TokenKind.Float:
                    {
                        var token = Advance();
                        var value = double.Parse(token.Value, CultureInfo.InvariantCulture);
                        return new FloatValueNode(value);
                    }

                case TokenKind.String:
                    {
                        var token = Advance();
                        return new StringValueNode(token.Value);
                    }

                case TokenKind.True:
                    Advance();
                    return new BooleanValueNode(true);

                case TokenKind.False:
                    Advance();
                    return new BooleanValueNode(false);

                case TokenKind.Null:
                    Advance();
                    return NullValueNode.Instance;

                case TokenKind.LeftBracket:
                    return ParseListLiteral();

                case TokenKind.LeftBrace:
                    return ParseObjectLiteral();

                default:
                    throw new InvalidOperationException(
                        $"Unexpected token in value literal: {Current.Kind}"
                    );
            }
        }

        private ValueNode ParseListLiteral()
        {
            Expect(TokenKind.LeftBracket);

            var items = new List<ValueNode>();

            while (!Match(TokenKind.RightBracket))
            {
                items.Add(ParseValueLiteral());
                Match(TokenKind.Comma);
            }

            return new ListValueNode(items);
        }

        private ValueNode ParseObjectLiteral()
        {
            Expect(TokenKind.LeftBrace);

            var fields = new List<ObjectFieldNode>();

            while (!Match(TokenKind.RightBrace))
            {
                var name = ExpectName();
                Expect(TokenKind.Colon);
                var value = ParseValueLiteral();
                fields.Add(new ObjectFieldNode(name, value));

                Match(TokenKind.Comma);
            }

            return new ObjectValueNode(fields);
        }
    }
}