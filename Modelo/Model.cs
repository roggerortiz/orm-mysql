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
        protected String table = null, primaryKey = "id";
        protected Dictionary<String, Object> attributes = new Dictionary<String, Object>();
        protected List<String> fillable = new List<String>();

        public Model(Dictionary<String, Object> attributes = null)
        {
            this.Fill(attributes);
        }

        public Model Fill(Dictionary<String, Object> attributes)
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

        public Model Fill(DataColumnCollection columns, DataRow row)
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

        public dynamic Find(Object id)
        {
            return new Query().Find(id);
        }

        public List<dynamic> All()
        {
            return new Query().From(this.table).Get();
        }

        public dynamic Make(Dictionary<String, Object> attributes)
        {
            Query query = new Query();

            dynamic model = query.ModelInstance();
            model.Fill(attributes);

            return model;
        }

        public dynamic Create(Dictionary<String, Object> attributes)
        {
            Query query = new Query();

            Int32 id = query.Insert(attributes);

            if (id > 0) return this.Find(id);

            return null;
        }

        public dynamic Update(Dictionary<String, Object> attributes)
        {
            Query query = new Query();

            if (!attributes.ContainsKey("id")) return null;

            Int32 affectingRows = query.Update(attributes);

            if (affectingRows > 0) return this.Find(attributes["id"]);

            return null;
        }

        public void Delete(Object id)
        {
            new Query().Delete(id);
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
            Query query = new Query().From(this.table);

            Type type = query.GetType();

            result = type.InvokeMember(binder.Name,
                BindingFlags.OptionalParamBinding | BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public,
                null, query, args);

            return true;
        }
    }
}
