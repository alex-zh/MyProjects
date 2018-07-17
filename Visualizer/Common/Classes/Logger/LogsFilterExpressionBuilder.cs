using System;
using System.Linq.Expressions;

namespace Common.Classes.Logger
{
    public class LogsFilterExpressionBuilder
    {
        public Func<LogItem, bool> Build(bool includeErrors = true, bool includeWarnings = false, bool includeInfo = false)
        {
            Expression filter = Expression.Equal(Expression.Constant(true), Expression.Constant(true));

            ParameterExpression messageTypeParameter = Expression.Parameter(typeof(LogItem), "MessageType");

            MemberExpression member = Expression.Property(messageTypeParameter, GetPropertyName<LogItem, LogMessageTypes>(x => x.MessageType));
            ConstantExpression errorConstant = Expression.Constant(LogMessageTypes.Error);
            ConstantExpression warningConstant = Expression.Constant(LogMessageTypes.Warning);
            ConstantExpression infoConstant = Expression.Constant(LogMessageTypes.Info);

            if (includeErrors == false)
                filter = Expression.AndAlso(filter, Expression.NotEqual(member, errorConstant));

            if (includeWarnings == false)
                filter = Expression.AndAlso(filter, Expression.NotEqual(member, warningConstant));

            if (includeInfo == false)
                filter = Expression.AndAlso(filter, Expression.NotEqual(member, infoConstant));

            return Expression.Lambda<Func<LogItem, bool>>(filter, messageTypeParameter).Compile();
        }

        public string GetPropertyName<T, R>(Expression<Func<T, R>> expression)
        {
            var memberExpression = (MemberExpression)expression.Body;
            return memberExpression.Member.Name;
        }
    }
}
