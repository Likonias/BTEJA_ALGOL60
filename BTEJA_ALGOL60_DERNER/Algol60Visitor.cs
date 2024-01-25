using System.Linq.Expressions;
using System.Net.Sockets;
using BTEJA_ALGOL60_DERNER.Content;

namespace BTEJA_ALGOL60_DERNER;

public class Algol60Visitor: Algol60BaseVisitor<object?>
{
    
    private Dictionary<string, object?> Variables { get; set; } = new();

    public Algol60Visitor()
    {
        Variables["write"] = new Func<object?[], object?>(Write);
    }

    public override object? VisitAssignment(Algol60Parser.AssignmentContext context)
    {

        var varName = context.IDENTIFIER().GetText();

        var value = Visit(context.expression());

        Variables[varName] = value;
        
        return null;
    }

    public override object? VisitType(Algol60Parser.TypeContext context)
    {

        if (context.INTEGER() is {} i)
            return int.Parse(i.GetText());

        if (context.DOUBLE() is { } r)
            return double.Parse(r.GetText());

        if (context.STRING() is { } s)
            return s.GetText()[1..^1];

        if (context.BOOL() is { } b)
            return b.GetText() == "true";

        throw new Exception("Unknown type.");
    }
    
    private bool IsCorrectType(string type, object? value)
    {
        switch (type)
        {
            case "int":
                return value is int;
            case "double":
                return value is float;
            case "str":
                return value is string;
            case "bool":
                return value is bool;
            default:
                throw new Exception($"Unsopported type {type}");
        }
    }

    public override object? VisitIdentifierExpression(Algol60Parser.IdentifierExpressionContext context)
    {
        
        var varName = context.IDENTIFIER().GetText();

        if (!Variables.ContainsKey(varName))
            throw new Exception($"Variable {varName} is not defined");

        return Variables[varName];

    }

    public override object? VisitVariableDeclaration(Algol60Parser.VariableDeclarationContext context)
    {
        var varName = context.IDENTIFIER().GetText();
        var declaredType = context.variableType().GetText();

        object? value = null;

        if (context.expression() != null)
        {
            value = Visit(context.expression());

            if (!IsCorrectType(declaredType, value))
            {
                throw new Exception($"Wrong type for {varName} with value {value} with a type of a {declaredType}");
            }
        }

        Variables[varName] = value;

        return Variables[varName];
    }

    public override object? VisitAdditiveExpression(Algol60Parser.AdditiveExpressionContext context)
    {

        var left = Visit(context.expression(0));
        var right = Visit(context.expression(1));

        var op = context.ADDING_OPERATOR().GetText();

        return op switch
        {
            "+" => Add(left, right),
            "-" => Subtract(left, right),
            _ => throw new Exception("Wrong operator!")
        };
        
    }

    private object? Add(object? left, object? right)
    {
        if (left is int leftInt && right is int rightInt)
            return leftInt + rightInt;
        
        if (left is double leftDouble && right is double rightDouble)
            return leftDouble + rightDouble;
        
        if (left is int lInt && right is double rDouble)
            return lInt + rDouble;
        
        if (left is int lDouble && right is int rInt)
            return lDouble + rInt;
        
        if (left is string)
            return $"{left}{right}";
        
        if (right is string)
            return $"{left}{right}";

        throw new Exception($"Not implemented addition between {left?.GetType()} and {right?.GetType()}");
        
    }

    private object? Subtract(object? left, object? right)
    {
        if (left is int leftInt && right is int rightInt)
            return leftInt - rightInt;
        
        if (left is double leftDouble && right is double rightDouble)
            return leftDouble - rightDouble;
        
        if (left is int lInt && right is double rDouble)
            return lInt - rDouble;
        
        if (left is int lDouble && right is int rInt)
            return lDouble - rInt;
        
        throw new Exception($"Not implemented subtraction between {left?.GetType()} and {right?.GetType()}");
    }

    public override object? VisitMultiplicativeExpression(Algol60Parser.MultiplicativeExpressionContext context)
    {
        var left = Visit(context.expression(0));
        var right = Visit(context.expression(1));

        var op = context.MULTIPLYING_OPERATOR().GetText();

        return op switch
        {
            "*" => Multiply(left, right),
            "/" => Devide(left, right),
            _ => throw new Exception("Wrong operator!")
        };
    }

    private object? Multiply(object? left, object? right)
    {
        
        if (left is int leftInt && right is int rightInt)
            return leftInt * rightInt;
        
        if (left is double leftDouble && right is double rightDouble)
            return leftDouble * rightDouble;
        
        if (left is int lInt && right is double rDouble)
            return lInt * rDouble;
        
        if (left is int lDouble && right is int rInt)
            return lDouble * rInt;
        
        throw new Exception($"Cannot multiply with {left?.GetType()} and {right?.GetType()}");
        
    }
    
    private object? Devide(object? left, object? right)
    {

        if (right is 0)
            throw new Exception("Can not devide by 0!");
        
        if (left is int leftInt && right is int rightInt)
            return leftInt * rightInt;
        
        if (left is double leftDouble && right is double rightDouble)
            return leftDouble * rightDouble;
        
        if (left is int lInt && right is double rDouble)
            return lInt * rDouble;
        
        if (left is int lDouble && right is int rInt)
            return lDouble * rInt;
        
        throw new Exception($"Cannot devide with {left?.GetType()} and {right?.GetType()}");
        
    }

    private object? Write(object?[] args)
    {
        foreach (var arg in args)
        {
            Console.WriteLine(arg);
        }

        return null;
    }
}