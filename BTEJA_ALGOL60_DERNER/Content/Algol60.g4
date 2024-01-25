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
GLOBAL: 'GLOBAL';
PROCEDURE: 'PROCEDURE';
FUNCTION: 'FUNCTION';
INTEGER: [0-9]+;
REAL: [0-9]+ '.' [0-9]+;
BOOL: 'TRUE' | 'FALSE';
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
STRING: ('"' ~'"'* '"') | ('\'' ~'\''* '\'');
WHITESPACE: [ \t\r\n]+ -> skip;
COMMENT: '(*' .*? '*)' -> skip;

program: blockHead declaration* compoundTail;

block: blockHead declaration* statement* returnStatement? compoundTail;

blockHead: BEGIN;

compoundTail: END;

declaration: functionDeclaration | variableDeclaration | statement | ifBlock | whileBlock | typeDeclaration | arrayDeclaration;

variableDeclaration: typeDeclaration IDENTIFIER (ASSIGN expression)? SEMICOLON;

typeDeclaration: localOrGlobalType;

localOrGlobalType: variableType | GLOBAL variableType;

variableType: 'INTEGER' | 'REAL' | 'STRING';

arrayDeclaration: variableType IDENTIFIER LEFTSQUAREBRACKET INTEGER RIGHTSQUAREBRACKET (ASSIGN arrayInitialization)? SEMICOLON;

arrayInitialization: LEFTSQUAREBRACKET expression (COMMA expression)* RIGHTSQUAREBRACKET;

arrayAccess: IDENTIFIER LEFTSQUAREBRACKET expression RIGHTSQUAREBRACKET;

statement: ( assignment | callExpression ) SEMICOLON;

ifBlock: IF expression THEN block (ELSE elseIfBlock);

elseIfBlock: block | ifBlock;

whileBlock: WHILE expression block;

assignment: IDENTIFIER ASSIGN expression;

callExpression: IDENTIFIER LEFTPAREN (expression (COMMA expression)*)? RIGHTPAREN;

functionDeclaration: FUNCTION IDENTIFIER LEFTPAREN parameterList? RIGHTPAREN RETURNS type block;

parameterList: parameter (COMMA parameter)*;

parameter: variableType IDENTIFIER;

returnStatement: RETURN expression SEMICOLON;

type: INTEGER | REAL | STRING | BOOL;

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
