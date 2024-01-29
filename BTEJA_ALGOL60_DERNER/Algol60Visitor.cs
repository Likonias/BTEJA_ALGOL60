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

        if (context.INT() is {} i)
            return int.Parse(i.GetText());

        if (context.DOUBLE() is { } r)
            return double.Parse(r.GetText());

        if (context.STR() is { } s)
            return s.GetText()[1..^1];

        if (context.BOOL() is { } b)
            return b.GetText() == "true";

        throw new Exception("Unknown type.");
    }
    
    private bool IsCorrectType(string type, object? value)
    {
        switch (type)
        {
            case "INT":
                return value is int;
            case "DOUBLE":
                return value is double;
            case "STR":
                return value is string;
            case "BOOL":
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

    public override object? VisitWhileBlock(Algol60Parser.WhileBlockContext context)
    {
        var conditionValue = Visit(context.expression());

        if (IsTrue(conditionValue))
        {
            do
            {
                Visit(context.block());
                conditionValue = Visit(context.expression());
            } while (IsTrue(conditionValue));
        }

        return null;
    }
    
    private bool IsTrue(object? value)
    {
        if (value is bool b)
            return b;
        throw new Exception($"Is not a bool {value}");
    }
    
    public override object? VisitComparisonExpression(Algol60Parser.ComparisonExpressionContext context)
    {
        var left = Visit(context.expression(0));
        var right = Visit(context.expression(1));

        var op = context.COMPARE_OPERATOR().GetText();

        return op switch
        {
            "<" => LessThan(left, right),
            "<=" => LessThanEquals(left, right),
            ">" => GreaterThan(left, right),
            ">=" => GreaterThanEquals(left, right),
            "==" => EqualsEquals(left, right),
            "!=" => NotEquals(left, right),
            _ => throw new Exception($"Unsupported comparison operator: {op}")
        };
    }
    
    private bool LessThan(object? left, object? right)
    {
        
        if (left is int leftInt && right is int rightInt)
            return leftInt < rightInt;
        
        if (left is double leftDouble && right is double rightDouble)
            return leftDouble < rightDouble;
        
        if (left is int lInt && right is double rDouble)
            return lInt < rDouble;
        
        if (left is int lDouble && right is int rInt)
            return lDouble < rInt;
        
        throw new Exception("Unsupported less than comparison");
    }

    private bool LessThanEquals(object? left, object? right)
    {
        if (left is int leftInt && right is int rightInt)
            return leftInt <= rightInt;
        
        if (left is double leftDouble && right is double rightDouble)
            return leftDouble <= rightDouble;
        
        if (left is int lInt && right is double rDouble)
            return lInt <= rDouble;
        
        if (left is int lDouble && right is int rInt)
            return lDouble <= rInt;

        throw new Exception("Unsupported less than or equals comparison");
    }

    private bool GreaterThan(object? left, object? right)
    {
        if (left is int leftInt && right is int rightInt)
            return leftInt > rightInt;
        
        if (left is double leftDouble && right is double rightDouble)
            return leftDouble > rightDouble;
        
        if (left is int lInt && right is double rDouble)
            return lInt > rDouble;
        
        if (left is int lDouble && right is int rInt)
            return lDouble > rInt;

        throw new Exception("Unsupported greater than comparison");
    }

    private bool GreaterThanEquals(object? left, object? right)
    {
        if (left is int leftInt && right is int rightInt)
            return leftInt >= rightInt;
        
        if (left is double leftDouble && right is double rightDouble)
            return leftDouble >= rightDouble;
        
        if (left is int lInt && right is double rDouble)
            return lInt >= rDouble;
        
        if (left is int lDouble && right is int rInt)
            return lDouble >= rInt;

        throw new Exception("Unsupported greater than or equals comparison");
    }

    private bool EqualsEquals(object? left, object? right)
    {
        return Equals(left, right);
    }

    private bool NotEquals(object? left, object? right)
    {
        return !Equals(left, right);
    }
    
    public override object? VisitIfBlock(Algol60Parser.IfBlockContext context)
    {
        var conditionValue = Visit(context.expression());
    
        if (IsTrue(conditionValue))
        {
            Visit(context.block());
        }
        else if (context.elseIfBlock() != null)
        {
            Visit(context.elseIfBlock());
        }

        return null;
    }
    
    public override object? VisitArrayDeclaration(Algol60Parser.ArrayDeclarationContext context)
    {
        var arrName = context.IDENTIFIER().GetText();
        int size1 = int.Parse(context.INT(0).GetText());
        int size2;
        var arrayType = context.variableType().GetText();

        object?[,] array;

        if (context.INT().Length == 1)
        {
            // One-dimensional array
            size2 = 1;
            array = new object?[size1, size2];
        }
        else
        {
            // Two-dimensional array
            size2 = int.Parse(context.INT(1).GetText());
            array = new object?[size1, size2];
        }

        Variables[arrName] = array;

        if (context.arrayInitialization() is { } initializationContext)
        {
            var initializationValues = initializationContext.expression();

            for (int j = 0; j < array.GetLength(1); j++)
            {
                for (int i = 0; i < array.GetLength(0); i++)
                {
                    var index = i + j * array.GetLength(0);

                    if (index < initializationValues.Length)
                    {
                        var value = Visit(initializationValues[index]);

                        if (!IsCorrectType(arrayType, value))
                        {
                            throw new Exception($"Incorrect data type - cannot assign {value} to {arrayType}");
                        }

                        array[i, j] = value;
                    }
                    else
                    {
                        if(index > (size1 * size2))
                            throw new Exception($"Array size {arrName} exceeded");
                    }
                }
            }
        }

        return null;
    }
    
    public override object? VisitArrayAccess(Algol60Parser.ArrayAccessContext context)
    {
        var arrName = context.IDENTIFIER().GetText();
        var index1 = (int)(Visit(context.expression(0)) ?? throw new Exception("Wrong index!"));
    
        if (!Variables.ContainsKey(arrName))
            throw new Exception($"Array doesn't exist: {arrName}");

        var array = (object?[,])Variables[arrName]!;

        if (context.expression().Length == 1)
        {
            // One-dimensional array access
            if (index1 < 0 || index1 >= array.GetLength(0))
                throw new Exception($"Array index out of bounds for {arrName}");

            return array[index1, 0];
        }
        else
        {
            // Two-dimensional array access
            var index2 = (int)(Visit(context.expression(1)) ?? throw new Exception("Wrong index!"));

            if (index1 < 0 || index1 >= array.GetLength(0) || index2 < 0 || index2 >= array.GetLength(1))
                throw new Exception($"Array index out of bounds for {arrName}");

            return array[index1, index2];
        }
    }

    
    public override object? VisitFunctionDeclaration(Algol60Parser.FunctionDeclarationContext context)
    {
        var funcName = context.IDENTIFIER().GetText();
        var parameters = context.parameterList()?.parameter().Select(p => p.IDENTIFIER().GetText()).ToList() ?? new List<string>();
        var returnType = context.type();

        Variables[funcName] = context;

        return null;
    }

    public override object? VisitProcedureDeclaration(Algol60Parser.ProcedureDeclarationContext context)
    {
        var procName = context.IDENTIFIER().GetText();
        var parameters = context.parameterList()?.parameter().Select(p => p.IDENTIFIER().GetText()).ToList() ?? new List<string>();
        
        Variables[procName] = context;

        return null;
    }

    public override object? VisitCallExpression(Algol60Parser.CallExpressionContext context)
    {
        var callName = context.IDENTIFIER().GetText();
        var args = context.expression().Select(Visit).ToArray();

        if (Variables.TryGetValue(callName, out var funcObj) && funcObj is Func<object?[], object?> func)
        {
            return func(args);
        }
        else if (Variables.TryGetValue(callName, out var funcContextObj) && funcContextObj is Algol60Parser.FunctionDeclarationContext funcContext)
        {
            var parameters = funcContext.parameterList()?.parameter().Select(p => p.IDENTIFIER().GetText()).ToList() ?? new List<string>();
            if (parameters.Count != args.Length)
                throw new Exception($"Incorrect number of arguments for function: {callName}");

            var localVariables = new Dictionary<string, object?>();
            for (int i = 0; i < parameters.Count; i++)
            {
                localVariables[parameters[i]] = args[i];
            }

            var previousVariables = new Dictionary<string, object?>(Variables);
            Variables = localVariables;
            Visit(funcContext.block());
            object? returnValue = Variables.GetValueOrDefault("_returnValue");
            Variables = previousVariables;

            return returnValue;
            
        }else if (Variables.TryGetValue(callName, out var procContextObj) && funcContextObj is Algol60Parser.ProcedureDeclarationContext procContext)
        {
            
            var parameters = procContext.parameterList()?.parameter().Select(p => p.IDENTIFIER().GetText()).ToList() ?? new List<string>();
            if (parameters.Count != args.Length)
                throw new Exception($"Incorrect number of arguments for procedure: {callName}");

            var localVariables = new Dictionary<string, object?>();
            for (int i = 0; i < parameters.Count; i++)
            {
                localVariables[parameters[i]] = args[i];
            }

            var previousVariables = new Dictionary<string, object?>(Variables);
            Variables = localVariables;
            Visit(procContext.block());
            object? returnValue = Variables.GetValueOrDefault("_returnValue");
            Variables = previousVariables;

            return returnValue;
            
        }
        else
        {
            throw new Exception($"Function or procedure with name {callName} is not declared");
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