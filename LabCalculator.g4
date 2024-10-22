grammar LabCalculator;

/*
 * Parser Rules
 */
compileUnit : expression EOF;

expression
    : 'min' LPAREN expression COMMA expression RPAREN                # MinExpr
    | 'max' LPAREN expression COMMA expression RPAREN                # MaxExpr
    | LPAREN expression RPAREN                                       # ParenthesizedExpr
    | expression EXPONENT expression                                 # ExponentialExpr
    | expression operatorToken=(MULTIPLY | DIVIDE | MOD | DIV) expression # MultiplicativeExpr
    | expression operatorToken=(ADD | SUBTRACT) expression           # AdditiveExpr
    | NUMBER                                                         # NumberExpr
    | IDENTIFIER                                                     # IdentifierExpr
    ;

/*
 * Lexer Rules
 */
NUMBER      : INT ('.' INT)?;
IDENTIFIER  : [a-zA-Z]+[1-9][0-9]+;
INT         : ('0'..'9')+;
EXPONENT    : '^';
MULTIPLY    : '*';
DIVIDE      : '/';
SUBTRACT    : '-';
ADD         : '+';
LPAREN      : '(';
RPAREN      : ')';
COMMA       : ',';
MIN         : 'min';
MAX         : 'max';
MOD         : '%';
DIV         : '//';
WS          : [ \t\r\n]+ -> skip;
