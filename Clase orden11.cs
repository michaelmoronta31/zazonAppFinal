using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_de_comida_rapida
{
    public class Orden
    {
        public int Id { get; set; }
        public List<string> Productos { get; set; } = new List<string>();
        public decimal Total { get; set; }
        public bool Entregada { get; set; }
        public bool Pagada { get; set; }
        public string MetodoPago { get; set; }
    }
}
