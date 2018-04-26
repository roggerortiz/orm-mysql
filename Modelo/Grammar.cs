using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelo
{
    public class Grammar
    {
        public String CompileSelect(Query query)
        {
            return (
                this.CompileColumns(query) + " " + this.CompileFrom(query) + " " + 
                this.CompileJoins(query) + " " + this.CompileWheres(query) + " " + 
                this.CompileOrders(query) + " " + this.CompileLimit(query)
            ).Trim();
        }

        public String CompileInsert(Query query, Dictionary<String, Object> attributes)
        {
            String columns = this.Columnize(attributes.Keys.ToList());

            String parameters = this.Parameterize(attributes.Values.ToList());

            return "INSERT INTO " + query.from + "(" + columns + ") VALUES (" + parameters + "); SELECT MAX(id) FROM " + query.from +";";
        }

        public String CompileUpdate(Query query, Dictionary<String, Object> attributes)
        {
            List<String> setters = new List<String>();

            foreach (KeyValuePair<String, Object> attribute in attributes)
            {
                setters.Add(attribute.Key + " = " + this.Parameter(attribute.Value));
            }

            String columns = String.Join(", ", setters);

            return "UPDATE " + query.from + " SET " + columns;
        }

        public String CompileDelete(Query query)
        {
            String wheres = (query.wheres.Count > 0) ? this.CompileWheres(query) : "";

            return ("DELETE FROM " + query.from + " " + wheres).Trim();
        }

        protected String CompileColumns(Query query)
        {
            return (query.distinct ? "SELECT DISTINCT " : "SELECT ") + this.Columnize(query.columns);
        }

        protected String CompileFrom(Query query)
        {
            return "FROM " + query.from;
        }

        protected String CompileJoins(Query query)
        {
            List<String> joins = new List<String>();

            foreach (String[] join in query.joins)
            {
                joins.Add(join[4].ToUpper() + " JOIN " + join[0] + " ON " + join[1] + " " + join[2] + " " + join[3]);
            }

            return String.Join(" ", joins);
        }

        protected String CompileWheres(Query query)
        {
            List<String> wheres = new List<String>();

            foreach (String[] where in query.wheres)
            {
                String sql = ((where == query.wheres.First()) ? "WHERE" : where[4]) + " ";

                switch (where[0])
                {
                    case "basic": sql += where[1] + " " + where[2] + " " + this.Parameter(where[3]);
                        break;

                    case "null": sql += where[1] + " " + where[2];
                        break;

                    case "raw": sql += where[1];
                        break;
                }

                wheres.Add(sql);
            }

            return String.Join(" ", wheres);
        }

        protected String CompileOrders(Query query)
        {
            if (query.orders.Count == 0) return "";

            List<String> orders = new List<String>();

            foreach (String[] order in query.orders)
            {
                orders.Add(order[0] + " " + order[1].ToUpper());
            }

            return "ORDER BY " + String.Join(", ", orders);
        }

        protected String CompileLimit(Query query)
        {
            String limit = "";

            if (query.limit >= 0) limit += "LIMIT " + query.limit.ToString();

            if (query.offset >= 0) limit += " OFFSET " + query.offset.ToString();

            return limit;
        }

        protected String Columnize(List<String> columns)
        {
            return String.Join(", ", columns);
        }

        protected String Parameterize(List<Object> values)
        {
            List<String> parameters = new List<String>();

            foreach (Object value in values)
            {
                parameters.Add(this.Parameter(value));
            }

            return String.Join(", ", parameters);
        }

        protected String Parameter(Object value)
        {
            return (value.ToString() == "") ? "NULL" : ("'" + value.ToString() + "'");
        }
    }
}
