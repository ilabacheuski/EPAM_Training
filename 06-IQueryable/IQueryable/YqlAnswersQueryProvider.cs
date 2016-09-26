using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace IQueryableTask
{
    public class YqlAnswersQueryProvider : IQueryProvider
    {
        public IQueryable CreateQuery(Expression expression)
        {
            // TODO: Implement CreateQuery

            try
            {
                return (IQueryable)Activator.CreateInstance(typeof(YqlAnswerSearch), new object[] { expression });
            }
            catch (System.Reflection.TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }

        //public IQueryable<Question> CreateQuery<Question>(Expression expression)
        public IQueryable<Question> CreateQuery<Question>(Expression expression)
        {
            // TODO: Implement CreateQuery
            return (IQueryable<Question>)new YqlAnswerSearch(expression);
        }

        public object Execute(Expression expression)
        {
            // TODO: Implement Execute
            
            throw new NotImplementedException();

            // HINT: Use GetYqlQuery to build query
            // HINT: Use YqlAnswersService to fetch data
        }

        public TResult Execute<TResult>(Expression expression)
        {
            // TODO: Implement Execute
            throw new NotImplementedException();
        }

        /// <summary>
        /// Generates YQL Query
        /// </summary>
        /// <param name="expression">Expression tree</param>
        /// <returns></returns>
        public string GetYqlQuery(Expression expression)
        {
            // TODO: Implement GetYqlQuery
            return new QueryTranslator().Translate(expression);

            // HINT: Create a class derived from ExpressionVisitor
        }
    }

    internal class QueryTranslator : ExpressionVisitor
    {
        System.Text.StringBuilder sb;
        internal QueryTranslator()
        {
        }

        internal string Translate(Expression expression)
        {
            this.sb = new System.Text.StringBuilder();
            sb.Append("select * from answers.search");

            this.Visit(expression);
            var rawResult = this.sb.ToString();

            if (rawResult.Contains(" category_name=") && !rawResult.Contains(" query="))
            {
                throw new InvalidOperationException("There is no Subject.Contains() part of query");
            }

            sb.Clear();
            int position = 0;
            int posKey;
            int posWhitespace = 0;
            string bufVal;
            while(position<rawResult.Length)
            {
                posKey = rawResult.IndexOf(" type=", position);
                if (posKey == -1)
                {
                    sb.Append(rawResult, position, rawResult.Length - position);
                    break;
                }
                sb.Append(rawResult, position, posKey+6);

                position += posKey + 6;

                posWhitespace = rawResult.IndexOf(" ", position);
                if (posWhitespace == -1)
                {
                    posWhitespace = rawResult.Length- position;
                }
                bufVal = "\"" + Enum.GetName(typeof(QuestionType), Int32.Parse(rawResult.Substring(position, posWhitespace))).ToLower() + "\"";
                sb.Append(bufVal);
                position += posWhitespace;
            }

            return sb.ToString();
        }

        private static Expression StripQuotes(Expression e)
        {

            while (e.NodeType == ExpressionType.Quote)
            {

                e = ((UnaryExpression)e).Operand;

            }

            return e;

        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {

            if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "Where") {

                sb.Append(" where");

                this.Visit(m.Arguments[0]);

                //sb.Append(") AS T WHERE “);

                LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);

                this.Visit(lambda.Body);

                return m;

            } else if (m.Method.Name == "Contains")
            {
                this.Visit(m.Object);
                sb.Append(m.Arguments[0]);
                return m;
            }

            throw new InvalidOperationException();
            //NotSupportedException(string.Format("The method ‘{ 0 }’ is not supported", m.Method.Name));

        }

        protected override Expression VisitUnary(UnaryExpression u)
        {

            switch (u.NodeType)
            {

                case ExpressionType.Not:

                    //sb.Append(" NOT ");

                    this.Visit(u.Operand);

                    break;
                case ExpressionType.Convert:

                    this.Visit(u.Operand);

                    break;

                default:

                    throw new NotSupportedException(string.Format("The unary operator ‘{ 0 }’ is not supported", u.NodeType));

            }

            return u;

        }

        protected override Expression VisitBinary(BinaryExpression b)
        {

            //sb.Append("(");

            this.Visit(b.Left);

            switch (b.NodeType)
            {

                case ExpressionType.And:

                    sb.Append(" and");

                    break;
                case ExpressionType.AndAlso:
                    sb.Append(" and");

                    break;

                case ExpressionType.Or:

                    sb.Append(" or");

                    break;

                case ExpressionType.Equal:

                    sb.Append("=");

                    break;

                case ExpressionType.NotEqual:

                    sb.Append("<>");

                    break;

                case ExpressionType.LessThan:

                    sb.Append("<");

                    break;

                case ExpressionType.LessThanOrEqual:

                    sb.Append("<=");

                    break;

                case ExpressionType.GreaterThan:

                    sb.Append(">");

                    break;

                case ExpressionType.GreaterThanOrEqual:

                    sb.Append(">=");

                    break;

                default:

                    throw new NotSupportedException(string.Format("The binary operator ‘{ 0 }’ is not supported", b.NodeType));

            }


            this.Visit(b.Right);

            //sb.Append(")");

            return b;

        }

        protected override Expression VisitConstant(ConstantExpression c)
        {

            IQueryable q = c.Value as IQueryable;

            if (q != null)
            {

                // assume constant nodes w/ IQueryables are table references

                //sb.Append("select * from ");

                //sb.Append(q.ElementType.Name);

            }

            else if (c.Value == null)
            {

                sb.Append("NULL");

            }

            else
            {

                switch (Type.GetTypeCode(c.Value.GetType()))
                {

                    case TypeCode.Boolean:

                        sb.Append(((bool)c.Value) ? 1 : 0);

                        break;

                    case TypeCode.String:

                        sb.Append("\"");

                        sb.Append(c.Value);

                        sb.Append("\"");

                        break;

                    case TypeCode.Object:

                        throw new NotSupportedException(string.Format("The constant for ‘{ 0}’ is not supported", c.Value));

                    default:

                        sb.Append(c.Value);

                        break;

                }

            }

            return c;

        }



        protected override Expression VisitMember(MemberExpression m)
        {

            if (m.Expression != null && m.Expression.NodeType == ExpressionType.Parameter)
            {
                if (m.Member.Name == "Subject")
                {
                    sb.Append(" query=");
                } else if (m.Member.Name == "Type")
                {
                    sb.Append(" type");
                } else if (m.Member.Name == "Category")
                {
                    sb.Append(" category_name");
                } else
                {
                    throw new NotSupportedException(string.Format("Query by member {0} is not supported", m.Member.Name));
                }

                return m;

            }

            throw new NotSupportedException(string.Format("The member ‘{ 0 }’ is not supported", m.Member.Name));

        }
    }
}
