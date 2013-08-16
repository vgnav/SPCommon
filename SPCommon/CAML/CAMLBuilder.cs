using System;
using System.Collections.Generic;
using System.Linq;
using SPCommon.Interface;

namespace SPCommon.CAML
{
    #region Data types

    public enum CAMLOperator
    {
        Eq, Contains, Neq, Like
    }

    public enum CAMLCondition
    {
        And, Or
    }

    public class CAMLExpression : ICAMLExpression
    {
        public string Column { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public CAMLOperator Operator { get; set; }
    }

    public class CAMLConditionExpression : ICAMLExpression
    {
        public CAMLExpression Left { get; set; }
        public CAMLExpression Right { get; set; }
        public CAMLCondition Condition { get; set; }
    }

    public class CAMLChainedExpression : ICAMLExpression
    {
        public IList<CAMLExpression> Expressions { get; set; }
        public CAMLCondition Condition { get; set; }
    }

    #endregion

    public class CAMLBuilder
    {
        private readonly ICAMLExpression _expression;

        public CAMLBuilder(ICAMLExpression expression)
        {
            _expression = expression;
        }

        public override string ToString()
        {
            return String.Format(@"<Where>{0}</Where>", GetCamlQuery());
        }

        public string GetCamlQuery()
        {
            if (_expression == null) return string.Empty;
            if(_expression is CAMLExpression)
                return GetSingleExpression(_expression as CAMLExpression);
            if (_expression is CAMLConditionExpression)
                return GetConditionExpression(_expression as CAMLConditionExpression);
            if (_expression is CAMLChainedExpression)
                return GetChainedExpression(_expression as CAMLChainedExpression);
            throw new InvalidCastException(
                "_expression must be either CAMLExpression, CAMLConditionExpression, or CAMLChainedExpression type");
        }

        private static string GetChainedExpression(CAMLChainedExpression chainedExpression)
        {
            var statementList = chainedExpression.Expressions.Select(GetSingleExpression).ToList();
            var statementStack = new Stack<string>();
            foreach (var statement in statementList)
                statementStack.Push(statement);

            // Recursively build up query
            Func<Stack<string>, string> process = null;
            process = stack =>
            {
                if (stack.Count == 0) return string.Empty;
                if (stack.Count == 1) return stack.Pop();
                var statement1 = stack.Pop();
                var statement2 = stack.Pop();
                var expr = String.Format(@"<{0}>{1}{2}</{0}>", chainedExpression.Condition, statement1, statement2);
                stack.Push(expr);
                return process(stack);
            };

            return process(statementStack);

        }

        private static string GetConditionExpression(CAMLConditionExpression conditionExpression)
        {
            var condition = conditionExpression.Condition.ToString();
            var expression = String.Format(@"<{0}>{1}{2}</{0}>",
                                        condition,
                                        GetSingleExpression(conditionExpression.Left),
                                        GetSingleExpression(conditionExpression.Right));
            return expression;
        }

        private static string GetSingleExpression(CAMLExpression expression)
        {
            var op = expression.Operator.ToString();
            return String.Format(@"<{0}>{1}</{0}>", op, GetFieldRefExpression(expression));
        }

        private static string GetFieldRefExpression(CAMLExpression expression)
        {
            return String.Format(@"<FieldRef Name=""{0}""/><Value Type=""{1}""><![CDATA[{2}]]></Value>",
                                    expression.Column,
                                    expression.Type,
                                    expression.Value);
        }
    }
}
