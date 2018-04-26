using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Modelo;

namespace ORM
{
    public partial class FormUsuarios : Form
    {
        public FormUsuarios()
        {
            InitializeComponent();
        }

        private void FormUsuarios_Load(object sender, EventArgs e)
        {
            ListarUsuarios();
        }

        private void ListarUsuarios()
        {
            dynamic usuario = new Usuario();

            List<dynamic> usuarios = usuario.All();

            foreach (dynamic item in usuarios)
            {
                ListViewItem listItem = new ListViewItem(item.id.ToString());
                listItem.SubItems.Add(item.apellidos);
                listItem.SubItems.Add(item.nombres);
                listItem.SubItems.Add(item.direccion);
                listItem.SubItems.Add(item.telefono);
                listItem.SubItems.Add(item.usuario);
                listItem.SubItems.Add(item.categoria);

                lvUsuarios.Items.Add(listItem);
            }
        }
    }
}
