using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_de_comida_rapida
{
    public static class DatosCompartidos
    {
        public static List<Producto> Menu = new List<Producto>();

        public static List<Orden> Entregas = new List<Orden>();
        public static int SiguienteIdOrden = 1;
    }
}
