using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace WebApplicationCoreTest1.Extensions
{
    public class ProperCoercion
    {
        public static Expression<Func<TTo, bool>> Convert<TFrom, TTo>(Expression<Func<TFrom, bool>> expr)
        {
            var subsititues = new Dictionary<Expression, Expression>();

            var oldParam = expr.Parameters[0];
            var newParam = Expression.Parameter(typeof(TTo), oldParam.Name);

            subsititues.Add(oldParam, newParam);
            var body = ConvertNode(expr.Body, subsititues);

            return Expression.Lambda<Func<TTo, bool>>(body, newParam);
        }

        private static Expression ConvertNode(Expression node, IDictionary<Expression, Expression> subs)
        {
            if (node == null) return null;
            if (subs.ContainsKey(node)) return subs[node];

            switch (node.NodeType)
            {
                case ExpressionType.Constant:
                    return node;

                case ExpressionType.MemberAccess:
                    {
                        var me = (MemberExpression)node;
                        var newNode = ConvertNode(me.Expression, subs);
                        return Expression.MakeMemberAccess(newNode, newNode.Type.GetMember(me.Member.Name).Single());
                    }

                case ExpressionType.Equal:
                    {
                        var be = (BinaryExpression)node;
                        return Expression.MakeBinary(be.NodeType, ConvertNode(be.Left, subs), ConvertNode(be.Right, subs), be.IsLiftedToNull, be.Method);
                    }

                default:
                    throw new NotSupportedException(node.NodeType.ToString());
            }
        }
    }
}