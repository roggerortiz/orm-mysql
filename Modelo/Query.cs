using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelo
{
    public class Query
    {
        public Connection connection = new Connection();
        public Grammar grammar = new Grammar();

        public Boolean distinct;
        public List<String> columns = new List<String>();
        public String from = "";
        public List<String[]> joins = new List<String[]>();
        public List<String[]> wheres = new List<String[]>();
        public List<String[]> orders = new List<String[]>();
        public Int32 limit = -1;
        public Int32 offset = -1;

        public String[] operates = { "=", "<", ">", "<=", ">=", "<>", "LIKE", "IS NULL", "IS NOT NULL" };

        public Query Select(List<String> columns = null)
        {
            if (columns == null)
            {
                this.columns.Add("*");
            }
            else
            {
                this.columns = columns;
                if (!this.columns.Contains("id")) columns.Add("id");
            }

            return this;
        }

        public Query Distinct()
        {
            this.distinct = true;

            return this;
        }

        public Query From(String table)
        {
            this.from = table.Trim();

            return this;
        }

        public Query Join(String table, String first, String operate, String second, String type = "inner")
        {
            String [] join = { table, first, operate, second, type };

            this.joins.Add(join);

            return this;
        }

        public Query LeftJoin(String table, String first, String operate = null, String second = null)
        {
            this.Join(table, first, operate, second, "left");

            return this;
        }

        public Query RightJoin(String table, String first, String operate = null, String second = null)
        {
            this.Join(table, first, operate, second, "right");

            return this;
        }

        public Query Where(String column, String operate, Object value, String boolean = "and")
        {
            String[] where = { "basic", column, operate, value.ToString(), boolean };

            this.wheres.Add(where);

            return this;
        }

        public Query OrWhere(String column, String operate = null, Object value = null)
        {
            this.Where(column, operate, value, "or");

            return this;
        }

        public Query WhereNull(String column, String boolean = "and", Boolean not = false)
        {
            String[] where = { "null", column, (not ? "IS NOT NULL" : "IS NULL"), null, boolean };

            this.wheres.Add(where);

            return this;
        }

        public Query OrWhereNull(String column)
        {
            return this.WhereNull(column, "or");
        }

        public Query WhereNotNull(String column, String boolean = "and")
        {
            return this.WhereNull(column, boolean, true);
        }

        public Query OrWhereNotNull(String column)
        {
            return this.WhereNotNull(column, "or");
        }

        public Query WhereRaw(String sql, String boolean = "and")
        {
            String[] where = { "raw", sql, null, null, boolean };

            this.wheres.Add(where);

            return this;
        }

        public Query OrWhereRaw(String sql)
        {
            return this.WhereRaw(sql, "or");
        }

        public Query OrderBy(String column, String direction = "asc")
        {
            String[] order = { column, ((direction.ToLower() == "asc") ? "asc" : "desc") };

            this.orders.Add(order);

            return this;
        }

        public Query OrderByDesc(String column)
        {
            return this.OrderBy(column, "desc");
        }

        public Query Limit(Int32 value)
        {
            if(value >= 0) {
                this.limit = value;
            }

            return this;
        }

        public Query Offset(Int32 value)
        {
            if (value >= 0)
            {
                this.offset = value;
            }

            return this;
        }

        public String ToSql()
        {
            return this.grammar.CompileSelect(this);
        }

        public dynamic Find(Object id, List<String> columns = null)
        {
            return this.Where(this.from + "id", "=", id).Limit(1).Get(columns).First();
        }

        public dynamic First(List<String> columns = null)
        {
            return this.OrderBy("id").Limit(1).Get(columns).First();
        }

        public List<dynamic> Get(List<String> columns = null)
        {
            if (columns != null) this.columns = columns;

            if (this.columns.Count == 0) this.columns.Add("*");

            DataTable table = this.connection.Select(this.ToSql());

            return this.ToList(table);
        }

        public Int32 Insert(Dictionary<String, Object> attributes)
        {
            String sql = this.grammar.CompileInsert(this, attributes);

            return this.connection.Statement(sql);
        }

        public Int32 Update(Dictionary<String, Object> attributes)
        {
            String sql = this.grammar.CompileUpdate(this, attributes);

            return this.connection.AffectingStatement(sql);
        }

        public void Delete(Object id)
        {
            if (id != null)
            {
                this.Where(this.from + ".id", "=", id);
            }

            String sql = this.grammar.CompileDelete(this);
        }

        protected List<dynamic> ToList(DataTable table)
        {
            List<dynamic> list = new List<dynamic>();

            foreach (DataRow row in table.Rows)
            {
                dynamic model = this.ModelInstance();
                model.Fill(table.Columns, row);

                list.Add(model);
            }

            return list;
        }

        public dynamic ModelInstance()
        {
            Type type = Type.GetType(this.TypeModel());

            return Activator.CreateInstance(type);
        }

        protected String TypeModel()
        {
            String assemblyName = this.GetType().Assembly.GetName().Name;

            String className = char.ToUpper(this.from[0]) + this.from.Substring(1);

            return assemblyName + "." + className;
        }
    }
}
