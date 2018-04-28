using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;
using System.Reflection;
using System.Data;

namespace Modelo
{
    public abstract class Model : DynamicObject
    {
        private Type type;
        protected String table;
        protected String primaryKey;
        protected Dictionary<String, Object> attributes;
        protected List<String> fillable;

        public Model(Dictionary<String, Object> attributes = null)
        {
            this.type = Type.GetType(this.TypeModel());
            this.table = this.GetTable();
            this.primaryKey = "id";
            this.attributes = new Dictionary<String, Object>();
            this.fillable = new List<String>();
            this.Fill(attributes);
        }

        private String TypeModel()
        {
            String assemblyName = this.GetType().Assembly.GetName().Name;

            String className = char.ToUpper(this.GetTable()[0]) + this.GetTable().Substring(1);

            return assemblyName + "." + className;
        }

        private String GetTable()
        {
            return this.GetType().Name.ToLower();
        }

        public dynamic Fill(Dictionary<String, Object> attributes = null)
        {
            if (attributes != null)
            {
                foreach (KeyValuePair<String, Object> attribute in attributes)
                {
                    String key = this.RemoveTableFromKey(attribute.Key);

                    if (isFillable(key))
                    {
                        this.SetAttribute(key, attribute.Value);
                    }
                }
            }

            return this;
        }

        public dynamic Fill(DataColumnCollection columns, DataRow row)
        {
            if (attributes != null)
            {
                foreach (DataColumn column in columns)
                {
                    attributes[column.ColumnName] = (row[column.ColumnName] == null) ? "" : row[column.ColumnName];
                }
            }

            return this;
        }

        public List<dynamic> All(List<String> columns = null)
        {
            return this.NewQueryBuilder().Get(columns);
        }

        public dynamic Make(Dictionary<String, Object> attributes)
        {
            return this.Fill(attributes);
        }

        public dynamic Create(Dictionary<String, Object> attributes)
        {
            Int32 id = this.NewQueryBuilder().Insert(attributes);

            return (id > 0) ? this.NewQueryBuilder().Find(id) : null;
        }

        public dynamic Update(Dictionary<String, Object> attributes)
        {
            if (!attributes.ContainsKey("id")) return null;

            Int32 affectingRows = this.NewQueryBuilder().Update(attributes);

            return (affectingRows > 0) ? this.NewQueryBuilder().Find(attributes["id"]) : null;
        }

        public void Delete(Object id)
        {
            this.NewQueryBuilder().Delete(id);
        }

        public Query NewQueryBuilder()
        {
            return new Query(this.type).From(this.table);
        }
        
        protected String RemoveTableFromKey(String key)
        {
            return key.Contains(".") ? key.Split('.').Last() : key;
        }

        protected Boolean isFillable(String key)
        {
            if (fillable.Contains(key) || key == this.primaryKey) return true;

            return (fillable.Count == 0 && key.Substring(0, 1) != "_");
        }

        protected void SetAttribute(String key, Object value)
        {
            this.attributes[key] = value;
        }

        public override Boolean TryGetMember(GetMemberBinder binder, out Object result)
        {
            String name = binder.Name.ToLower();

            if(! attributes.ContainsKey(name))
            {
                attributes[name] = "";
            }

            return attributes.TryGetValue(name, out result);
        }

        public override Boolean TrySetMember(SetMemberBinder binder, Object value)
        {
            this.attributes[binder.Name.ToLower()] = (value == null) ? "" : value;

            return true;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            Query query = this.NewQueryBuilder();

            result = query.GetType().InvokeMember(binder.Name,
                BindingFlags.OptionalParamBinding | BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public,
                null, query, args);

            return true;
        }
    }
}
