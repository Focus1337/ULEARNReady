using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Reflection;

namespace Reflection.Differentiation
{
    public class Algebra
    {
        public static Expression<Func<double, double>> Differentiate(Expression<Func<double, double>> f) {
            Func<double, double> cf = f.Compile();

            if (f.Body.ToString().Contains("ToString"))
            {
                throw new ArgumentException("ToString");
            }

            if (f.Body.ToString().Contains("Max"))
            {
                throw new ArgumentException("Max");
            }

            ParameterExpression paramExpr = Expression.Parameter(typeof(double), "arg");
            ConstantExpression eps = Expression.Constant(1e-7);

            BinaryExpression addExpr = Expression.Add(
                paramExpr,
                eps
            );

            InvocationExpression callWithConst = Expression.Invoke(
                f,
                new Expression[] { addExpr }
            );

            InvocationExpression callWithoutConst = Expression.Invoke(
                f,
                new Expression[] { paramExpr }
            );

            BinaryExpression subtractExpr = Expression.Subtract(
                callWithConst,
                callWithoutConst
            );

            BinaryExpression devideExpr = Expression.Divide(
                subtractExpr,
                eps
            );

            LambdaExpression lambda = Expression.Lambda(
                 devideExpr,
                 new List<ParameterExpression>() { paramExpr }
            );

            Expression<Func<double, double>> exp = (Expression<Func<double, double>>)lambda;

            /*
                double eps = 1e-7;
                var ff = f.Compile();
                Expression<Func<double, double>> exp = (x) => (ff(x + eps) - ff(x)) / eps;
            */
            return exp;
        }
    }
}