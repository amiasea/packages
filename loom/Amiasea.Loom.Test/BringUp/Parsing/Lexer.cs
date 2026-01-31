using System;
using System.Collections.Generic;
using System.Text;

namespace Amiasea.Loom.Test.BringUp
{
    public sealed class Lexer
    {
        private readonly string _text;
        private int _position;

        public Lexer(string text)
        {
            _text = text ?? throw new ArgumentNullException(nameof(text));
        }

        public List<Token> Lex()
        {
            var tokens = new List<Token>();

            while (true)
            {
                SkipWhitespace();

                if (IsEnd())
                {
                    tokens.Add(new Token(TokenKind.EndOfFile, string.Empty));
                    break;
                }

                var ch = Peek();

                if (char.IsLetter(ch) || ch == '_')
                {
                    tokens.Add(ReadNameOrKeyword());
                }
                else if (char.IsDigit(ch) || ch == '-')
                {
                    tokens.Add(ReadNumber());
                }
                else
                {
                    switch (ch)
                    {
                        case '{':
                            Advance();
                            tokens.Add(new Token(TokenKind.LeftBrace, "{"));
                            break;
                        case '}':
                            Advance();
                            tokens.Add(new Token(TokenKind.RightBrace, "}"));
                            break;
                        case '(':
                            Advance();
                            tokens.Add(new Token(TokenKind.LeftParen, "("));
                            break;
                        case ')':
                            Advance();
                            tokens.Add(new Token(TokenKind.RightParen, ")"));
                            break;
                        case '[':
                            Advance();
                            tokens.Add(new Token(TokenKind.LeftBracket, "["));
                            break;
                        case ']':
                            Advance();
                            tokens.Add(new Token(TokenKind.RightBracket, "]"));
                            break;
                        case ':':
                            Advance();
                            tokens.Add(new Token(TokenKind.Colon, ":"));
                            break;
                        case ',':
                            Advance();
                            tokens.Add(new Token(TokenKind.Comma, ","));
                            break;
                        case '!':
                            Advance();
                            tokens.Add(new Token(TokenKind.Exclamation, "!"));
                            break;
                        case '"':
                            tokens.Add(ReadString());
                            break;
                        default:
                            throw new InvalidOperationException($"Unexpected character '{ch}' at position {_position}.");
                    }
                }
            }

            return tokens;
        }

        private void SkipWhitespace()
        {
            while (!IsEnd() && char.IsWhiteSpace(Peek()))
                Advance();
        }

        private bool IsEnd() => _position >= _text.Length;

        private char Peek() => _text[_position];

        private char Advance() => _text[_position++];

        private Token ReadNameOrKeyword()
        {
            var sb = new StringBuilder();

            while (!IsEnd() && (char.IsLetterOrDigit(Peek()) || Peek() == '_'))
                sb.Append(Advance());

            var value = sb.ToString();

            return value switch
            {
                "true" => new Token(TokenKind.True, value),
                "false" => new Token(TokenKind.False, value),
                "null" => new Token(TokenKind.Null, value),
                _ => new Token(TokenKind.Name, value)
            };
        }

        private Token ReadNumber()
        {
            var sb = new StringBuilder();

            if (Peek() == '-')
                sb.Append(Advance());

            while (!IsEnd() && char.IsDigit(Peek()))
                sb.Append(Advance());

            var isFloat = false;

            if (!IsEnd() && Peek() == '.')
            {
                isFloat = true;
                sb.Append(Advance());

                while (!IsEnd() && char.IsDigit(Peek()))
                    sb.Append(Advance());
            }

            var value = sb.ToString();
            return new Token(isFloat ? TokenKind.Float : TokenKind.Int, value);
        }

        private Token ReadString()
        {
            var sb = new StringBuilder();

            Advance(); // opening quote

            while (!IsEnd() && Peek() != '"')
            {
                var ch = Advance();
                if (ch == '\\' && !IsEnd())
                {
                    var next = Advance();
                    sb.Append(next); // minimal escape handling
                }
                else
                {
                    sb.Append(ch);
                }
            }

            if (IsEnd() || Peek() != '"')
                throw new InvalidOperationException("Unterminated string literal.");

            Advance(); // closing quote

            return new Token(TokenKind.String, sb.ToString());
        }
    }
}