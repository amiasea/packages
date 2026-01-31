using Amiasea.Loom.AST;

namespace Amiasea.Loom.Test.BringUp
{
    public static class TestParser
    {
        public static (DocumentNode document, OperationNode operation) Parse(string query)
        {
            query = query.TrimStart('\uFEFF');

            var lexer = new Lexer(query);
            var tokens = lexer.Lex();

            var parser = new Parser(tokens);
            var document = parser.ParseDocument();

            // bring-up: assume single anonymous operation
            var operation = document.Operations[0];

            return (document, operation);
        }
    }
}