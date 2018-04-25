using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Modelo
{
    public class Connection
    {
        private MySqlConnection connection = new MySqlConnection();

        public Connection()
        {
            this.connection.ConnectionString = this.ConnectionString();
        }

        private String ConnectionString()
        {
            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder(); //constructor de cadena de conexion

            try
            {
                XmlDocument xml = new XmlDocument();
                xml.Load("database.xml");

                XmlNodeList nodeList;
                
                nodeList = xml.GetElementsByTagName("Server");
                if(nodeList.Count > 0) builder.Server = nodeList[0].InnerText;
                
                nodeList = xml.GetElementsByTagName("Database");
                if (nodeList.Count > 0) builder.Database = nodeList[0].InnerText;
                
                nodeList = xml.GetElementsByTagName("UserID");
                if(nodeList.Count > 0) builder.UserID = nodeList[0].InnerText;
                
                nodeList = xml.GetElementsByTagName("Password");
                if(nodeList.Count > 0) builder.Password = nodeList[0].InnerText;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al Obtener la Cadena de Conexión: " + ex.Message);
            }

            return builder.ConnectionString;
        }

        public void Open()
        {
            try
            {
                if (this.connection.State != ConnectionState.Open)
                {
                    this.connection.Open();
                }
            } catch (Exception ex)
            {
                Console.WriteLine("Error al Abrir la Conexión con la Base de Datos: " + ex.Message);
            }
        }

        public void Close()
        {
            try
            {
                if (this.connection.State == ConnectionState.Open)
                {
                    this.connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al Cerrar la Conexión con la Base de Datos: " + ex.Message);
            }
        }

        public DataTable Select(String sql)
        {
            DataTable table = new DataTable();

            try
            {
                this.Open();

                MySqlDataAdapter adapter = new MySqlDataAdapter(sql, this.connection);
                adapter.Fill(table);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error on Run Select: " + ex.Message);
            }
            finally
            {
                this.Close();
            }

            return table;
        }
    }
}
