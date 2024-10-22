using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1MAUI
{
    internal static class Calculator
    {
        public static double Evaluate(string expression)
        {
            //Debug.Write("&&& " + expression + "\n");
            var lexer = new LabCalculatorLexer(new AntlrInputStream(expression));
            lexer.RemoveErrorListeners();
            lexer.AddErrorListener(new ThrowExceptionErrorListener());

            var tokens = new CommonTokenStream(lexer);
            var parser = new LabCalculatorParser(tokens);

            var tree = parser.compileUnit();

            var visitor = new LabCalculatorVisitor();

            var result = visitor.Visit(tree);
            //Debug.WriteLine("$$$" + result);
            return result;
        }
    }

}
