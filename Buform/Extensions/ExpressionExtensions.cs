using System.Linq.Expressions;
using System.Reflection;

namespace Buform;

internal static class ExpressionExtensions
{
    public static string GetMemberName<TValue>(this Expression<Func<TValue>>? expression)
    {
        return expression.GetMember().Name;
    }

    public static MemberInfo GetMember<TValue>(this Expression<Func<TValue>>? expression)
    {
        return expression?.Body switch
        {
            MemberExpression item => item.Member,
            UnaryExpression { Operand: MemberExpression item } => item.Member,
            _ => throw new ArgumentOutOfRangeException(
                expression?.GetType().ToString(),
                null,
                null
            ),
        };
    }
}
