using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modelo;

namespace Controlador
{
    public class Usuario
    {
        private static dynamic NewUsuario()
        {
            return (dynamic) new Modelo.Usuario();
        }

        public static List<dynamic> Listar()
        {
            return NewUsuario().All();
        }
    }
}
