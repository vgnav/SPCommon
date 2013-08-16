using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCommon.CAML;
using SPCommon.Interface;

namespace SPCommon.Tests.UnitTests
{
    [TestClass]
    public class CAMLBuilderTests
    {
        [TestMethod]
        public void CAMLBuilder_SingleExpression()
        {
            var expression = GetSinglExpression();
            var builder = new CAMLBuilder(expression);
            var checkString = GetSingleCheckString(expression);
            Assert.IsTrue(builder.ToString().Equals(checkString));
        }


        [TestMethod]
        public void CAMLBuilder_ConditionExpression()
        {
            var expression = GetConditionExpression();
            var builder = new CAMLBuilder(expression);
            var checkString = GetConditionCheckString(expression);
            Assert.IsTrue(builder.ToString().Equals(checkString));
        }

        [TestMethod]
        public void CAMLBuilder_Chained_1_Expression()
        {
            var expression = GetSinglExpression();
            var chain = new CAMLChainedExpression
            {
                Condition = CAMLCondition.And,
                Expressions = new List<CAMLExpression> { expression as CAMLExpression} 
            };
            var builder = new CAMLBuilder(chain);
            var checkString = GetSingleCheckString(expression);
            Assert.IsTrue(builder.ToString().Equals(checkString));
        }

        [TestMethod]
        public void CAMLBuilder_Chained_2_Expression()
        {
            var expression = GetSinglExpression() as CAMLExpression;
            var chain = new CAMLChainedExpression
            {
                Condition = CAMLCondition.And,
                Expressions = new List<CAMLExpression> { expression, expression }
            };
            var builder = new CAMLBuilder(chain);
            var checkString = GetConditionCheckString(GetConditionExpression() as CAMLConditionExpression);
            Assert.IsTrue(builder.ToString().Equals(checkString));
        }

        [TestMethod]
        public void CAMLBuilder_Chained_3_Expression()
        {
            var expression = GetSinglExpression() as CAMLExpression;
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

        private static ICAMLExpression GetConditionExpression()
        {
            return new CAMLConditionExpression
            {
                Left = GetSinglExpression() as CAMLExpression,
                Right = GetSinglExpression() as CAMLExpression,
                Condition = CAMLCondition.And
            };
        }

        private static string GetConditionCheckString(ICAMLExpression caml)
        {
            var expression = caml as CAMLConditionExpression;
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

        private static ICAMLExpression GetSinglExpression()
        {
            return new CAMLExpression
            {
                Column = "Test",
                Type = "Text",
                Operator = CAMLOperator.Eq,
                Value = "Some value"
            };
        }

        private static string GetSingleCheckString(ICAMLExpression expression)
        {
            return String.Format(@"<Where>{0}</Where>", GetSingleString(expression));
        }

        private static string GetSingleString(ICAMLExpression caml)
        {
            var expression = caml as CAMLExpression;
            return String.Format(@"<{0}><FieldRef Name=""{1}""/><Value Type=""{2}""><![CDATA[{3}]]></Value></{0}>",
                expression.Operator,
                expression.Column,
                expression.Type,
                expression.Value);            
        }

        #endregion
    }


}
