using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Acklann.Daterpillar.Linq
{
    public static class LinqToSqlConverter
    {
        static LinqToSqlConverter()
        {
            _comparisonPattern = new Regex(@"(==|!=|<|<=|>|>=)");
            _operatorPattern = new Regex($"({string.Join("|", Enum.GetNames(typeof(ExpressionType)))})");
        }

        internal static Regex _operatorPattern, _comparisonPattern;

        public static IEnumerable<string> ToColumnList<T>(params Expression<Func<T, object>>[] expressions)
        {
            Type type = typeof(T);
            string[] names = null;

            foreach (var exp in expressions)
            {
                if (exp.Body is MemberExpression me)
                {
                    names = new string[1] { me.Member.Name };
                }
                else if (exp.Body is UnaryExpression ue)
                {
                    var ume = ue.Operand as MemberExpression;
                    names = new string[1] { ume?.Member.Name };
                }
                else if (exp.Body is NewExpression ne)
                {
                    names = ne.Members.Select(x => x.Name).ToArray();
                }

                foreach (var memberName in names)
                {
                    PropertyInfo property = type.GetRuntimeProperties().FirstOrDefault(p => p.Name == memberName);
                    ColumnAttribute attribute = property?.GetCustomAttribute<ColumnAttribute>();
                    if (attribute != null)
                    {
                        yield return (string.IsNullOrEmpty(attribute.Name) ? memberName : attribute.Name);
                    }
                    else if (string.IsNullOrEmpty(memberName) == false)
                    {
                        yield return memberName;
                    }
                }
            }
        }

        public static IEnumerable<string> ToAssignments<T>(object obj, params Expression<Func<T, object>>[] expressions)
        {
            Type type = typeof(T);
            string[] names = null;

            foreach (var exp in expressions)
            {
                if (exp.Body is MemberExpression me)
                {
                    names = new string[1] { me.Member.Name };
                }
                else if (exp.Body is UnaryExpression ue)
                {
                    var ume = ue.Operand as MemberExpression;
                    names = new string[1] { ume?.Member.Name };
                }
                else if (exp.Body is NewExpression ne)
                {
                    names = ne.Members.Select(x => x.Name).ToArray();
                }

                foreach (var memberName in names)
                {
                    PropertyInfo property = type.GetRuntimeProperties().FirstOrDefault(p => p.Name == memberName);
                    ColumnAttribute attribute = property?.GetCustomAttribute<ColumnAttribute>();
                    if (attribute != null)
                    {
                        string col = (string.IsNullOrEmpty(attribute.Name) ? memberName : attribute.Name);
                        yield return $"{col}={property.GetValue(obj).ToSQL()}";
                    }
                    else if (string.IsNullOrEmpty(memberName) == false)
                    {
                        yield return $"{memberName}={property.GetValue(obj).ToSQL()}";
                    }
                }
            }
        }

        public static string ToComparisons<T>(Syntax syntax, Expression<Func<T, bool>> expression)
        {
            string left, right;
            string paramName = expression.Parameters[0]?.Name ?? string.Empty;

            if (expression.Body is BinaryExpression be)
            {
                string op = ToOperator(be.NodeType);
                left = Reduce<T>(be.Left, syntax);
                right = Reduce<T>(be.Right, syntax);

                return $"{left} {op} {right}";
            }
            else return null;
        }

        #region Private Members

        internal static string Reduce<T>(Expression exp, Syntax syntax)
        {
            if (exp is MemberExpression me)
            {
                string columnName = GetColumnName<T>(me).Escape(syntax);
                return columnName;
            }
            else if (exp is ConstantExpression ce)
            {
                return ce.Value.ToSQL();
            }
            else if (exp is UnaryExpression ue)
            {
                throw new NotSupportedException($"the '{ue}' exprssion is not supported.");
            }
            else if (exp is BinaryExpression be)
            {
                string left = Reduce<T>(be.Left, syntax);
                string right = Reduce<T>(be.Right, syntax);

                string op = ToOperator(be.NodeType);
                bool canReduceFurther = IsOperator(be.NodeType);

                return (canReduceFurther ? $"({left} {op} {right})" : $"{left} {op} {right}");
            }
            else return null;
        }

        internal static string GetColumnName<T>(MemberExpression me)
        {
            Type type = typeof(T);
            PropertyInfo prop = type.GetRuntimeProperties().FirstOrDefault(x => x.Name == me.Member.Name);
            ColumnAttribute attribute = prop?.GetCustomAttribute<ColumnAttribute>();

            if (attribute != null)
            {
                return (string.IsNullOrEmpty(attribute.Name) ? prop.Name : attribute.Name);
            }
            else return me.Member.Name;
        }

        internal static bool IsOperator(this ExpressionType type)
        {
            return type == ExpressionType.AndAlso || type == ExpressionType.OrElse;
        }

        internal static string ToOperator(ExpressionType type)
        {
            switch (type)
            {
                default:
                    throw new NotSupportedException($"the {type} exprssion is not supported.");

                case ExpressionType.Equal:
                    return "=";

                case ExpressionType.NotEqual:
                    return "<>";

                case ExpressionType.LessThan:
                    return "<";

                case ExpressionType.LessThanOrEqual:
                    return "<=";

                case ExpressionType.GreaterThan:
                    return ">";

                case ExpressionType.GreaterThanOrEqual:
                    return ">=";

                case ExpressionType.AndAlso:
                    return "AND";

                case ExpressionType.OrElse:
                    return "OR";
            }
        }

        internal static bool IsComparsion(this ExpressionType type)
        {
            return
                type == ExpressionType.Equal ||
                type == ExpressionType.NotEqual ||
                type == ExpressionType.LessThan ||
                type == ExpressionType.LessThanOrEqual ||
                type == ExpressionType.GreaterThan ||
                type == ExpressionType.GreaterThanOrEqual;
        }

        internal static string ReplaceOperator(Match match)
        {
            string value = match.Value;
            switch (value)
            {
                default:
                    return value;

                case "==":
                    return "=";

                case "!=":
                    return "<>";
            }
        }

        internal static string ReplaceComparer(Match match)
        {
            string value = match.Value;
            switch (value)
            {
                default:
                    throw new NotSupportedException($"the {value} exprssion is not supported.");

                case nameof(ExpressionType.AndAlso):
                    return "AND";

                case nameof(ExpressionType.OrElse):
                    return "OR";

                case nameof(ExpressionType.Equal):
                    return "==";

                case nameof(ExpressionType.NotEqual):
                    return "<>";

                case nameof(ExpressionType.LessThan):
                    return "<";

                case nameof(ExpressionType.LessThanOrEqual):
                    return "<=";

                case nameof(ExpressionType.GreaterThan):
                    return ">";

                case nameof(ExpressionType.GreaterThanOrEqual):
                    return ">=";
            }
        }

        #endregion Private Members
    }
}