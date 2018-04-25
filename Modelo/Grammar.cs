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
            return (this.CompileColumns(query) + " " + this.CompileFrom(query) + " " + this.CompileJoins(query) + " " + this.CompileWheres(query) + " " + this.CompileOrders(query)).Trim();
        }

        protected String CompileColumns(Query query)
        {
            String sql = query.distinct ? "SELECT DISTINCT " : "SELECT ";

            return sql + String.Join(", ", query.columns);
        }

        protected String CompileFrom(Query query)
        {
            return "FROM " + query.table;
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

                switch(where[0])
                {
                    case "basic": sql += where[1] + " " + where[2] + " " + where[3];
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
    }
}
