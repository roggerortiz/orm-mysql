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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Prueba()
        {
            dynamic usuario = new Usuario();

            List<dynamic> lista = usuario.Select().Get();

            foreach (dynamic item in lista)
            {
                MessageBox.Show(item.Nombres);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Prueba();
        }
    }
}
