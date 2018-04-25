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
    public abstract class Modelo : DynamicObject
    {
        protected String table = null, primaryKey = "id";
        protected Dictionary<String, Object> attributes = new Dictionary<String, Object>();
        protected List<String> fillable = new List<String>();

        public Modelo(Dictionary<String, Object> attributes = null)
        {
            this.Fill(attributes);
        }

        public Modelo Fill(Dictionary<String, Object> attributes)
        {
            if (attributes == null) attributes = new Dictionary<String, Object>();

            foreach(KeyValuePair<String, Object> attribute in attributes)
            {
                String key = this.RemoveTableFromKey(attribute.Key);

                if(isFillable(key))
                {
                    this.SetAttribute(key, attribute.Value.ToString());
                }
            }

            return this;
        }

        public Modelo Fill(DataColumnCollection columns, DataRow row)
        {
            if (attributes == null) attributes = new Dictionary<String, Object>();

            foreach (DataColumn column in columns)
            {
                attributes[column.ColumnName] = row[column.ColumnName];
            }

            return this;
        }
        
        protected String RemoveTableFromKey(String key)
        {
            return key.Contains(".") ? key.Split('.').Last() : key;
        }

        protected Boolean isFillable(String key)
        {
            if (fillable.Contains(key)) return true;

            return (fillable.Count == 0 && key.Substring(0, 1) != "_");
        }

        protected void SetAttribute(String key, String value)
        {
            this.attributes[key] = value;
        }

        public override Boolean TryGetMember(GetMemberBinder binder, out Object result)
        {
            String name = binder.Name.ToLower();

            if(! attributes.ContainsKey(name))
            {
                attributes[name] = null;
            }

            return attributes.TryGetValue(name, out result);
        }

        public override Boolean TrySetMember(SetMemberBinder binder, Object value)
        {
            this.attributes[binder.Name.ToLower()] = value;

            return true;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            Type type = typeof(Query);

            Query query = (Query) Activator.CreateInstance(type);
            query.From(this.table);

            result = type.InvokeMember(binder.Name,
                BindingFlags.OptionalParamBinding | BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public,
                null, query, args);

            return true;
        }
    }
}
