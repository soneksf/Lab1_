using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1MAUI
{
    class LabCalculatorVisitor : LabCalculatorBaseVisitor<double>
    {
        //таблиця ідентифікаторів (тут для прикладу)
        //в лабораторній роботі заміните на свою!!!!
        Dictionary<string, double> tableIdentifier = new Dictionary<string, double>();

        public override double VisitCompileUnit(LabCalculatorParser.CompileUnitContext context)
        {
            return Visit(context.expression());
        }

        public override double VisitNumberExpr(LabCalculatorParser.NumberExprContext context)
        {
            var result = double.Parse(context.GetText());
            Debug.WriteLine("VisitNumberExpr" + result);

            return result;
        }

        //IdentifierExpr
        public override double VisitIdentifierExpr(LabCalculatorParser.IdentifierExprContext context)
        {
            var result = context.GetText();
            double value;
            //видобути значення змінної з таблиці
            if (tableIdentifier.TryGetValue(result.ToString(), out value))
            {
                return value;
            }
            else
            {
                return 0.0;
            }
        }

        public override double VisitParenthesizedExpr(LabCalculatorParser.ParenthesizedExprContext context)
        {
            return Visit(context.expression());
        }

        public override double VisitExponentialExpr(LabCalculatorParser.ExponentialExprContext context)
        {
            var left = WalkLeft(context);
            var right = WalkRight(context);

            Debug.WriteLine("{0} ^ {1}", left, right);
            return System.Math.Pow(left, right);
        }

        public override double VisitAdditiveExpr(LabCalculatorParser.AdditiveExprContext context)
        {
            var left = WalkLeft(context);
            var right = WalkRight(context);

            if (context.operatorToken.Type == LabCalculatorLexer.ADD)
            {
                Debug.WriteLine("{0} + {1}", left, right);
                return left + right;
            }
            else //LabCalculatorLexer.SUBTRACT
            {
                Debug.WriteLine("{0} - {1}", left, right);
                return left - right;
            }
        }

        public override double VisitMultiplicativeExpr(LabCalculatorParser.MultiplicativeExprContext context)
        {
            var left = WalkLeft(context);
            var right = WalkRight(context);

            if (context.operatorToken.Type == LabCalculatorLexer.MULTIPLY)
            {
                Debug.WriteLine("{0} * {1}", left, right);
                return left * right;
            }
            else if (context.operatorToken.Type == LabCalculatorLexer.MOD)
            {
                Debug.WriteLine("{0} % {1}", left, right);
                return (int)left % (int)right;
            }
            else if (context.operatorToken.Type == LabCalculatorLexer.DIV)
            {
                Debug.WriteLine("{0} // {1}", left, right);
                return (int)(left / right);
            }
            else //LabCalculatorLexer.DIVIDE
            {
                Debug.WriteLine("{0} / {1}", left, right);
                return left / right;
            }
        }

public override double VisitMaxExpr(LabCalculatorParser.MaxExprContext context)
{
    var left = WalkLeft(context);
    var right = WalkRight(context);

    
    double result = Math.Max(left, right);

    Debug.WriteLine("max({0}, {1}) = {2}", left, right, result);

    return result;
}
public override double VisitMinExpr(LabCalculatorParser.MinExprContext context)
{
    var left = WalkLeft(context);
    var right = WalkRight(context);

    
    double result = Math.Min(left, right);

    Debug.WriteLine("min({0}, {1}) = {2}", left, right, result);

    return result;
}
        private double WalkLeft(LabCalculatorParser.ExpressionContext context)
        {
            return Visit(context.GetRuleContext<LabCalculatorParser.ExpressionContext>(0));
        }

        private double WalkRight(LabCalculatorParser.ExpressionContext context)
        {
            return Visit(context.GetRuleContext<LabCalculatorParser.ExpressionContext>(1));
        }
    }

}
