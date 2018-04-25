﻿using MySql.Data.MySqlClient;
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
        public String table = "";
        public List<String[]> joins = new List<String[]>();
        public List<String[]> wheres = new List<String[]>();
        public List<String[]> orders = new List<String[]>();
        public Int32 limit;

        public String[] operates = { "=", "<", ">", "<=", ">=", "<>", "LIKE", "IS NULL", "IS NOT NULL" };

        public Query Select(String column = "*")
        {
            if(! this.columns.Contains(column)) this.columns.Add(column.Trim());

            return this;
        }

        public Query Distinct()
        {
            this.distinct = true;

            return this;
        }

        public Query From(String table)
        {
            this.table = table.Trim();

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

        public Query Where(String column, String operate, String value, String boolean = "and")
        {
            String[] where = { "basic", column, operate, value, boolean };

            this.wheres.Add(where);

            return this;
        }

        public Query OrWhere(String column, String operate = null, String value = null)
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

        public String ToSql()
        {
            return this.grammar.CompileSelect(this);
        }

        public List<dynamic> Get()
        {
            if(this.columns.Count == 0) this.Select();

            DataTable table = this.connection.Select(this.ToSql());

            return ToList(table);
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

        protected dynamic ModelInstance()
        {
            Type type = Type.GetType(this.TypeModel());

            return Activator.CreateInstance(type);
        }

        protected String TypeModel()
        {
            String assemblyName = typeof(Query).Assembly.GetName().Name;

            String className = char.ToUpper(this.table[0]) + this.table.Substring(1);

            return assemblyName + "." + className;
        }
    }
}