using Antlr4.Runtime;
using BTEJA_ALGOL60_DERNER.Content;

namespace BTEJA_ALGOL60_DERNER;

public class Class1
{
    static void Main()
    {
        var fileName = "Content\\priklad1.ss";
        //var fileName = "Content\\priklad2.ss";
        //var fileName = "Content\\priklad3.ss";
        
        var fileContents = File.ReadAllText(fileName);
        
        var inputStream = new AntlrInputStream(fileContents);
        var algol60Lexer = new Algol60Lexer(inputStream);
        var commonTokenStream = new CommonTokenStream(algol60Lexer);
        var algolParser = new Algol60Parser(commonTokenStream);

        var algol60Context = algolParser.program();
        var visitor = new Algol60Visitor();

        visitor.Visit(algol60Context);
    }
}