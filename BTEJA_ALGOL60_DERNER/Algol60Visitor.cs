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
            "/" => Divide(left, right),
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
    
    private object? Divide(object? left, object? right)
    {

        if (right is 0)
            throw new Exception("Can not devide by 0!");
        
        if (left is int leftInt && right is int rightInt)
            return leftInt / rightInt;
        
        if (left is double leftDouble && right is double rightDouble)
            return leftDouble / rightDouble;
        
        if (left is int lInt && right is double rDouble)
            return lInt / rDouble;
        
        if (left is int lDouble && right is int rInt)
            return lDouble / rInt;
        
        throw new Exception($"Cannot divided with {left?.GetType()} and {right?.GetType()}");
        
    }

    public override object? VisitArrayDeclaration(Algol60Parser.ArrayDeclarationContext context)
    {
        
        var arrName = context.IDENTIFIER().GetText();
        var size = int.Parse(context.INTEGER().GetText());
        var arrayType = context.variableType().GetText();

        var array = new object?[size];

        Variables[arrName] = array;

        if (context.arrayInitialization() is { } initializationContext)
        {
            var initializationValues = initializationContext.expression();

            for (int i = 0; i < initializationValues.Length; i++)
            {
                var value = Visit(initializationValues[i]);

                if (!IsCorrectType(arrayType, value))
                {
                    throw new Exception($"Incorrect data type - cannot assign {value} to {arrayType}");
                }

                if (i < size)
                {
                    array[i] = value;
                }
                else
                {
                    throw new Exception($"Array size {arrName} exceeded");
                }
            }
        }

        return null;
    }

    public override object? VisitArrayAccess(Algol60Parser.ArrayAccessContext context)
    {
        
        var arrName = context.IDENTIFIER().GetText();
        var index = (int)(Visit(context.expression()) ?? throw new Exception("Wrong index!"));

        if (!Variables.ContainsKey(arrName))
            throw new Exception($"Array doesnt exist: {arrName}");

        var array = (object?[])Variables[arrName]!;

        return array[index];
        
    }
    
    public override object? VisitFunctionDeclaration(Algol60Parser.FunctionDeclarationContext context)
    {
        var funcName = context.IDENTIFIER().GetText();
        var parameters = context.parameterList()?.parameter().Select(p => p.IDENTIFIER().GetText()).ToList() ?? new List<string>();
        var returnType = context.type().GetText();

        Variables[funcName] = context;

        return null;
    }

    public override object? VisitCallExpression(Algol60Parser.CallExpressionContext context)
    {
        var funcName = context.IDENTIFIER().GetText();
        var args = context.expression().Select(Visit).ToArray();

        if (Variables.TryGetValue(funcName, out var funcObj) && funcObj is Func<object?[], object?> func)
        {
            return func(args);
        }
        else if (Variables.TryGetValue(funcName, out var funcContextObj) && funcContextObj is Algol60Parser.FunctionDeclarationContext funcContext)
        {
            var parameters = funcContext.parameterList()?.parameter().Select(p => p.IDENTIFIER().GetText()).ToList() ?? new List<string>();
            if (parameters.Count != args.Length)
                throw new Exception($"Incorrect number of arguments for function: {funcName}");

            var localVariables = new Dictionary<string, object?>();
            for (int i = 0; i < parameters.Count; i++)
            {
                localVariables[parameters[i]] = args[i];
            }

            var previousVariables = new Dictionary<string, object?>(Variables);
            Variables = localVariables;
            var returnValue = Visit(funcContext.block());
            Variables = previousVariables;

            if (Variables.TryGetValue("_returnValue", out var returnValueFromBlock))
            {
                Variables.Remove("_returnValue");
                return returnValueFromBlock;
            }

            return returnValue;
        }
        else
        {
            throw new Exception($"Function with name {funcName} is not declared");
        }
    }

    public override object? VisitReturnStatement(Algol60Parser.ReturnStatementContext context)
    {
        Variables["_returnValue"] = Visit(context.expression());
        return Variables["_returnValue"];
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