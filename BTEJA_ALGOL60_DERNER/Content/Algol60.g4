grammar Algol60;

BEGIN: 'BEGIN';
END: 'END';
SEMICOLON: ';';
COMMA: ',';
WHILE: 'WHILE';
IF: 'IF';
THEN: 'THEN';
ELSE: 'ELSE';
RETURN: 'RETURN';
PROCEDURE: 'PROCEDURE';
FUNCTION: 'FUNCTION';
WRITE: 'WRITE';
INT: [0-9]+;
DOUBLE: [0-9]+ '.' [0-9]+;
BOOL: 'true' | 'false';
ADDING_OPERATOR: '+' | '-';
MULTIPLYING_OPERATOR: '*' | '/';
COMPARE_OPERATOR: '==' | '!=' | '>' | '<' | '>=' | '<=';
BOOL_OPERATOR: 'AND' | 'OR';
EQUAL: '=';
LEFTPAREN: '(';
RIGHTPAREN: ')';
LEFTSQUAREBRACKET: '[';
RIGHTSQUAREBRACKET: ']';
ASSIGN: ':=';
IDENTIFIER: [a-zA-Z_][a-zA-Z0-9_]*;
STR: ('"' ~'"'* '"') | ('\'' ~'\''* '\'');
WHITESPACE: [ \t\r\n]+ -> skip;
COMMENT: '(*' .*? '*)' -> skip;

program: blockHead declaration* compoundTail;

block: blockHead declaration* statement* returnStatement? compoundTail;

blockHead: BEGIN;

compoundTail: END;

declaration: procedureDeclaration | functionDeclaration | variableDeclaration | statement | ifBlock | whileBlock | arrayDeclaration;

variableDeclaration: variableType IDENTIFIER (ASSIGN expression)? SEMICOLON;

variableType: 'INT' | 'DOUBLE' | 'STR' | 'BOOL';

arrayDeclaration: variableType IDENTIFIER LEFTSQUAREBRACKET INT RIGHTSQUAREBRACKET (ASSIGN arrayInitialization)? SEMICOLON | variableType IDENTIFIER LEFTSQUAREBRACKET INT RIGHTSQUAREBRACKET LEFTSQUAREBRACKET INT RIGHTSQUAREBRACKET (ASSIGN arrayInitialization)? SEMICOLON;

arrayInitialization: LEFTSQUAREBRACKET expression (COMMA expression)* RIGHTSQUAREBRACKET | LEFTSQUAREBRACKET LEFTSQUAREBRACKET expression (COMMA expression)* RIGHTSQUAREBRACKET LEFTSQUAREBRACKET expression (COMMA expression)* RIGHTSQUAREBRACKET RIGHTSQUAREBRACKET;

arrayAccess: IDENTIFIER LEFTSQUAREBRACKET expression RIGHTSQUAREBRACKET | IDENTIFIER LEFTSQUAREBRACKET expression RIGHTSQUAREBRACKET LEFTSQUAREBRACKET expression RIGHTSQUAREBRACKET;

statement: ( assignment | callExpression ) SEMICOLON;

ifBlock: IF expression THEN block (ELSE elseIfBlock);

elseIfBlock: block | ifBlock;

whileBlock: WHILE expression block;

assignment: IDENTIFIER ASSIGN expression;

callExpression: IDENTIFIER LEFTPAREN (expression (COMMA expression)*)? RIGHTPAREN;

functionDeclaration: FUNCTION IDENTIFIER LEFTPAREN parameterList? RIGHTPAREN RETURN type block;

procedureDeclaration: PROCEDURE IDENTIFIER LEFTPAREN parameterList? RIGHTPAREN block;

parameterList: parameter (COMMA parameter)*;

parameter: variableType IDENTIFIER;

returnStatement: RETURN expression SEMICOLON;

type: INT | DOUBLE | STR | BOOL;

expression
    : type                                          #constantExpression
    | IDENTIFIER                                    #identifierExpression
    | callExpression                                #procedureOrFunctionCallExpression
    | LEFTPAREN expression RIGHTPAREN               #parenthesizedExpression
    | expression MULTIPLYING_OPERATOR expression    #multiplicativeExpression
    | expression ADDING_OPERATOR expression         #additiveExpression
    | expression COMPARE_OPERATOR expression        #comparisonExpression
    | expression BOOL_OPERATOR expression           #booleanExpression
    | arrayAccess                                   #arrayExpression
    ;
