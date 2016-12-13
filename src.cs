using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace CustomLINQTest
{
    class Entity
    {

        internal QueryContext QueryContext;

        public void DumpQuery()
        {
            if (QueryContext == null)
                throw new InvalidOperationException("not a query object");

            Console.WriteLine($"[{GetType()}]");
            foreach(var cond in QueryContext.Conditions)
            {
                Console.WriteLine($"   - {cond.PropertyName} {cond.Op} {cond.Value}");
            }
        }
    }
    class QueryContext
    {
        public List<QueryCondition> Conditions;

        public QueryContext()
        {
            Conditions = new List<QueryCondition>();
        }
    }

    enum ConditionOp
    {
        Eq,
        Gt,
        Lt,
        Ge,
        Le,
        Not
    }
    class QueryCondition
    {
        public ConditionOp Op;

        public string PropertyName;
        public string Value;
    }

    class QueryBuilder<T>
        where T : Entity, new()
    {
        public static T Query
        {
            get
            {
                var queryObject = new T();
                queryObject.QueryContext = new QueryContext();
                return queryObject;
            }
        }
    }

    static class LinqQueryExt
    {
        public static T Where<T>(this T obj, Expression<Func<T, bool>> exp)
            where T : Entity
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            if (obj.QueryContext == null)
                throw new ArgumentException("not a query object");

            var body = exp.Body as BinaryExpression;
            var condition = new QueryCondition();

            condition.PropertyName = body.Left.ToString();
            condition.Value = body.Right.ToString();

            if (body.NodeType == ExpressionType.Equal)
                condition.Op = ConditionOp.Eq;
            else if (body.NodeType == ExpressionType.GreaterThan)
                condition.Op = ConditionOp.Gt;
            else if (body.NodeType == ExpressionType.LessThan)
                condition.Op = ConditionOp.Lt;
            else if (body.NodeType == ExpressionType.NotEqual)
                condition.Op = ConditionOp.Not;
            else if (body.NodeType == ExpressionType.GreaterThanOrEqual)
                condition.Op = ConditionOp.Ge;
            else if (body.NodeType == ExpressionType.LessThanOrEqual)
                condition.Op = ConditionOp.Le;

            obj.QueryContext.Conditions.Add(condition);

            return obj;
        }
    }

    class Player : Entity
    {
        public string Name;
        public int Level;
    }

    class Program
    {
        static void Main(string[] args)
        {
            QueryBuilder<Player>
                .Query
                .Where(x => x.Name == "park")
                .Where(x => x.Level > 10)
                .DumpQuery();
        }
    }
}
