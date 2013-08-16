using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SPCommon.Tests.UnitTests
{
    [TestClass]
    public class CAMLBuilderTests
    {
        [TestMethod]
        public void CAMLBuilder_ProcessSingleExpression()
        {
            var expression = GetSinglExpression();
            var builder = new CAMLBuilder(expression);
            var checkString = GetSingleCheckString(expression);
            Assert.IsTrue(builder.ToString().Equals(checkString));
        }


        [TestMethod]
        public void CAMLBuilder_ProcessConditionExpression()
        {
            var expression = GetConditionExpression();
            var builder = new CAMLBuilder(expression);
            var checkString = GetConditionCheckString(expression);
            Assert.IsTrue(builder.ToString().Equals(checkString));
        }

        [TestMethod]
        public void CAMLBuilder_ProcessChainedExpressionWithOneElement()
        {
            var expression = GetSinglExpression();
            var chain = new CAMLChainedExpression
            {
                Condition = CAMLCondition.And,
                Expressions = new List<CAMLExpression> { expression } 
            };
            var builder = new CAMLBuilder(chain);
            var checkString = GetSingleCheckString(expression);
            Assert.IsTrue(builder.ToString().Equals(checkString));
        }

        [TestMethod]
        public void CAMLBuilder_ProcessChainedExpressionWithTwoElements()
        {
            var expression = GetSinglExpression();
            var chain = new CAMLChainedExpression
            {
                Condition = CAMLCondition.And,
                Expressions = new List<CAMLExpression> { expression, expression }
            };
            var builder = new CAMLBuilder(chain);
            var checkString = GetConditionCheckString(GetConditionExpression());
            Assert.IsTrue(builder.ToString().Equals(checkString));
        }

        [TestMethod]
        public void CAMLBuilder_ProcessChainedExpressionWithThreeElements()
        {
            var expression = GetSinglExpression();
            var chain = new CAMLChainedExpression
            {
                Condition = CAMLCondition.And,
                Expressions = new List<CAMLExpression> { expression, expression, expression }
            };
            var builder = new CAMLBuilder(chain);
            var checkString = GetThreeChainString(chain);
            Assert.IsTrue(builder.ToString().Equals(checkString));
        }

        
        #region Private helper

        private static CAMLConditionExpression GetConditionExpression()
        {
            return new CAMLConditionExpression
            {
                Left = GetSinglExpression(),
                Right = GetSinglExpression(),
                Condition = CAMLCondition.And
            };
        }

        private static string GetConditionCheckString(CAMLConditionExpression expression)
        {
            return String.Format(@"<Where><{0}>{1}{2}</{0}></Where>",
                                expression.Condition,
                                GetSingleString(expression.Left),
                                GetSingleString(expression.Right));
        }

        private string GetThreeChainString(CAMLChainedExpression chain)
        {
            var expression = chain.Expressions[0];
            return String.Format(@"<Where><{0}><{0}>{1}{1}</{0}>{1}</{0}></Where>",
                                chain.Condition,
                                GetSingleString(expression));
        }

        private static CAMLExpression GetSinglExpression()
        {
            return new CAMLExpression
            {
                Column = "Test",
                Type = "Text",
                Operator = CAMLOperator.Eq,
                Value = "Some value"
            };
        }

        private static string GetSingleCheckString(CAMLExpression expression)
        {
            return String.Format(@"<Where>{0}</Where>", GetSingleString(expression));
        }

        private static string GetSingleString(CAMLExpression expression)
        {
            return String.Format(@"<{0}><FieldRef Name=""{1}""/><Value Type=""{2}"">{3}</Value></{0}>",
                expression.Operator,
                expression.Column,
                expression.Type,
                expression.Value);            
        }

        #endregion
    }

    enum CAMLOperator
    {
        Eq, Contains, Neq, Like
    }

    enum  CAMLCondition
    {
        And, Or
    }

    class CAMLExpression
    {
        public string Column { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public CAMLOperator Operator { get; set; }
    }

    class CAMLConditionExpression
    {
        public CAMLExpression Left { get; set; }
        public CAMLExpression Right { get; set; }
        public CAMLCondition Condition { get; set; }
    }

    class CAMLChainedExpression
    {
        public IList<CAMLExpression> Expressions { get; set; }
        public CAMLCondition Condition { get; set; }
    }

    class CAMLBuilder
    {
        private readonly CAMLExpression _expression;
        private readonly CAMLConditionExpression _conditionExpression;
        private readonly CAMLChainedExpression _chaindExpression;

        public CAMLBuilder(CAMLExpression expression)
        {
            _expression = expression;
        }

        public CAMLBuilder(CAMLConditionExpression conditionExpression)
        {
            _conditionExpression = conditionExpression;
        }

        public CAMLBuilder(CAMLChainedExpression chaindExpression)
        {
            _chaindExpression = chaindExpression;
        }

        public override string ToString()
        {
            return String.Format(@"<Where>{0}</Where>", GetCamlQuery());
        }

        public string GetCamlQuery()
        {
            var camlQuery = string.Empty;
            if (_expression != null)
                camlQuery = GetSingleExpression(_expression);
            else if (_conditionExpression != null)
                camlQuery = GetConditionExpression(_conditionExpression);
            else if (_chaindExpression != null)
                camlQuery = GetChainedExpression(_chaindExpression);
            return camlQuery;
        }

        private static string GetChainedExpression(CAMLChainedExpression chainedExpression)
        {
            var statementList = chainedExpression.Expressions.Select(GetSingleExpression).ToList();
            var statementStack = new Stack<string>();
            foreach(var statement in statementList)
                statementStack.Push(statement);

            Func<Stack<string>, string> process = null;
            process = (stack) =>
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
            return String.Format(@"<FieldRef Name=""{0}""/><Value Type=""{1}"">{2}</Value>",
                                    expression.Column,
                                    expression.Type,
                                    expression.Value);            
        }
    }
}
