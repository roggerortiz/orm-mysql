using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Controlador;

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
            List<dynamic> usuarios = Usuario.Listar();

            foreach (dynamic usuario in usuarios)
            {
                ListViewItem listItem = new ListViewItem(usuario.id.ToString());
                listItem.SubItems.Add(usuario.apellidos);
                listItem.SubItems.Add(usuario.nombres);
                listItem.SubItems.Add(usuario.direccion);
                listItem.SubItems.Add(usuario.telefono);
                listItem.SubItems.Add(usuario.usuario);
                listItem.SubItems.Add(usuario.categoria);

                listUsuarios.Items.Add(listItem);
            }
        }
    }
}
