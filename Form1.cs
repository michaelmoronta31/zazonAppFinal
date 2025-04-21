namespace Sistema_de_comida_rapida
{
    using System.Data.SqlClient;
    public partial class ZazónApp : Form
    {

        public static List<Producto> listaProductos = new List<Producto>();
        List<Orden> listaOrdenes = new List<Orden>();
        private int contadorId = 1;
        decimal total = 0;
        public ZazónApp()
        {
            InitializeComponent();
        }
        private void MostrarPanel(Panel panel)
        {
            panelMenu.Visible = false;
            panelOrdenes.Visible = false;
            panelEntregas.Visible = false;
            panelPago.Visible = false;

            panel.BringToFront();
            panel.Visible = true;
        }

        private void buttonMenu_Click(object sender, EventArgs e)
        {
            MostrarPanel(panelMenu);
        }
        private void ActualizarGrid()
        {
            dataGridViewMenu.DataSource = null;
            dataGridViewMenu.AutoGenerateColumns = true;
            dataGridViewMenu.DataSource = DatosCompartidos.Menu;
        }
        private void buttonAgregarProducto_Click(object sender, EventArgs e)
        {

            string nombre = textBoxProducto.Text;
            string categoria = textBoxCategoria.Text;
            string precio = textBoxPrecio.Text;

            if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(categoria) || string.IsNullOrEmpty(precio))
            {
                MessageBox.Show("Complete todos los campos.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!decimal.TryParse(precio, out decimal precioDecimal))
            {
                MessageBox.Show("Ingrese un número entero en el campo precio.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (precioDecimal <= 0)
            {
                MessageBox.Show("El precio debe ser mayor a cero.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Producto nuevoProducto = new Producto
            {
                Id = DatosCompartidos.Menu.Count + 1,
                Nombre = nombre,
                Categoria = categoria,
                Precio = precioDecimal
            };
            DatosCompartidos.Menu.Add(nuevoProducto);
            listaProductos.Add(nuevoProducto);
            InsertarProductoEnBD(nuevoProducto);
            ActualizarGrid();

            textBoxProducto.Clear();
            textBoxCategoria.Clear();
            textBoxPrecio.Clear();
        }

        private void buttonEliminarProducto_Click(object sender, EventArgs e)
        {
            if (dataGridViewMenu.SelectedRows.Count > 0)
            {
                DialogResult result = MessageBox.Show("¿Estás seguro que deseas eliminar este elemento?", "Confirmar Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    int idProducto = (int)dataGridViewMenu.SelectedRows[0].Cells[0].Value;

                    using (SqlConnection conexion = ConexionBD.ObtenerConexion())
                    {
                        conexion.Open();
                        string consulta = "DELETE FROM Productos WHERE ProductoID = @id";
                        using (SqlCommand comando = new SqlCommand(consulta, conexion))
                        {
                            comando.Parameters.AddWithValue("@id", idProducto);
                            comando.ExecuteNonQuery();
                        }
                    }

                    Producto productoAEliminar = DatosCompartidos.Menu.FirstOrDefault(p => p.Id == idProducto);
                    if (productoAEliminar != null)
                    {
                        DatosCompartidos.Menu.Remove(productoAEliminar);
                        ActualizarGrid();
                    }

                }
            }
            else
            {
                MessageBox.Show("Selecciona una fila para eliminar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        private void buttonActualizarProducto_Click(object sender, EventArgs e)
        {
            if (dataGridViewMenu.SelectedRows.Count > 0)
            {
                DataGridViewRow fila = dataGridViewMenu.SelectedRows[0];
                int idProducto = Convert.ToInt32(fila.Cells["Id"].Value);

                Producto producto = DatosCompartidos.Menu.FirstOrDefault(p => p.Id == idProducto);
                if (producto != null)
                {
                    producto.Nombre = textBoxProducto.Text;
                    producto.Categoria = textBoxCategoria.Text;

                    if (decimal.TryParse(textBoxPrecio.Text, out decimal nuevoPrecio))
                    {
                        producto.Precio = nuevoPrecio;

                        ActualizarGrid(); // Refrescar el DataGridView
                        MessageBox.Show("Producto actualizado correctamente.");
                    }
                    else
                    {
                        MessageBox.Show("El precio ingresado no es válido.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Seleccioná un producto para actualizar.");
            }
            textBoxProducto.Clear();
            textBoxCategoria.Clear();
            textBoxPrecio.Clear();
        }



        private void buttonVolverMenu_Click(object sender, EventArgs e)
        {
            panelMenu.Visible = false;
        }
        private void MostrarMenuEnOrdenes()
        {
            dataGridViewMenuOrdenes.DataSource = null;
            dataGridViewMenuOrdenes.AutoGenerateColumns = true;
            dataGridViewMenuOrdenes.DataSource = DatosCompartidos.Menu;
        }

        private void buttonOrdenes_Click(object sender, EventArgs e)
        {
            MostrarPanel(panelOrdenes);
            MostrarMenuEnOrdenes();
        }

        private void buttonVolverOrden_Click(object sender, EventArgs e)
        {
            panelOrdenes.Visible = false;
        }

        private void dataGridViewMenuOrdenes_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow fila = dataGridViewMenuOrdenes.Rows[e.RowIndex];

                string nombre = fila.Cells[1].Value.ToString();
                string categoria = fila.Cells[2].Value.ToString();
                string precio = fila.Cells[3].Value.ToString();
            }
        }

        private void buttonAggorden_Click(object sender, EventArgs e)
        {
            if (dataGridViewMenuOrdenes.CurrentRow != null)
            {
                string nombre = dataGridViewMenuOrdenes.CurrentRow.Cells["Nombre"].Value.ToString();
                decimal precio = Convert.ToDecimal(dataGridViewMenuOrdenes.CurrentRow.Cells["Precio"].Value);

                string dibujo = "========================";
                string item = $"{nombre} --- ${precio}";

                listBoxOrden.Items.Add(dibujo);
                listBoxOrden.Items.Add(item);
                listBoxOrden.Items.Add(dibujo);

                total += precio;
                labelTotalOrden.Text = $"Total: ${total:0}";
            }
        }
        private void buttonConfirmarOrden_Click_1(object sender, EventArgs e)
        {
            if (listBoxOrden.Items.Count == 0)
            {
                MessageBox.Show("No hay productos en la orden.");
                return;
            }
            total = 0;
            List<string> productos = new List<string>();
            decimal totalOrden = 0;
            foreach (var item in listBoxOrden.Items)
            {
                productos.Add(item.ToString());

                string[] partes = item.ToString().Split('$');
                if (partes.Length > 1 && decimal.TryParse(partes[1], out decimal precio))
                {
                    totalOrden += precio;
                }
            }
            Orden nuevaOrden = new Orden
            {
                Id = DatosCompartidos.SiguienteIdOrden++,
                Productos = productos,
                Total = totalOrden
            };
            DatosCompartidos.Entregas.Add(nuevaOrden);

            listBoxOrden.Items.Clear();
            labelTotalOrden.Text = "Total: $0";

            MessageBox.Show("Orden confirmada y enviada a entregas.");
            MostrarEntregas();
        }
        private void MostrarEntregas()
        {
            dataGridViewEntregas.DataSource = null;
            dataGridViewEntregas.DataSource = DatosCompartidos.Entregas
                .Select(o => new
                {
                    o.Id,
                    Total = $"${o.Total:0.00}",
                    Estado = o.Entregada ? "Entregada" : "Pendiente"
                }).ToList();
        }

        private void buttonEntregas_Click(object sender, EventArgs e)
        {
            MostrarPanel(panelEntregas);
            MostrarEntregas();
        }
        private void CargarDetalleOrden(int idOrden)
        {
            listBoxEntregas.Items.Clear();
            Orden orden = DatosCompartidos.Entregas.FirstOrDefault(o => o.Id == idOrden);

            if (orden != null)
            {
                foreach (string producto in orden.Productos)
                {
                    listBoxEntregas.Items.Add(producto);
                }

                labelltotalentregas.Text = $"Total: ${orden.Total:0}";
            }
        }
        private void dataGridViewEntregas_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                int idOrden = Convert.ToInt32(dataGridViewEntregas.Rows[e.RowIndex].Cells[0].Value);
                CargarDetalleOrden(idOrden);
            }
        }

        private void buttonConfirmarEntrega_Click(object sender, EventArgs e)
        {
            if (dataGridViewEntregas.SelectedRows.Count > 0)
            {
                int idOrden = Convert.ToInt32(dataGridViewEntregas.SelectedRows[0].Cells["Id"].Value);
                Orden orden = DatosCompartidos.Entregas.FirstOrDefault(o => o.Id == idOrden);

                if (orden != null)
                {
                    orden.Entregada = true;
                    MostrarEntregas();
                    listBoxEntregas.Items.Clear();
                    MessageBox.Show("¡Entrega confirmada!");

                    comboBoxOrden.Items.Clear();
                    foreach (var ordenEntregada in DatosCompartidos.Entregas.Where(o => o.Entregada))
                    {
                        comboBoxOrden.Items.Add(ordenEntregada.Id);
                    }
                    if (comboBoxOrden.Items.Count > 0)
                    {
                        comboBoxOrden.SelectedIndex = 0;
                    }

                    labelTotalPago.Text = $"Total:                       ${orden.Total:0}";


                }
            }
            else
            {
                MessageBox.Show("Selecciona una orden para marcar como entregada.");
            }
        }

        private void buttonVolverEntrega_Click(object sender, EventArgs e)
        {
            panelEntregas.Visible = false;
        }

        private void buttonPagos_Click(object sender, EventArgs e)
        {
            MostrarPanel(panelPago);
        }

        private void buttonVolverPago_Click(object sender, EventArgs e)
        {
            panelPago.Visible = false;
        }

        private void comboBoxOrden_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxOrden.SelectedItem != null)
            {
                int idSeleccionado = Convert.ToInt32(comboBoxOrden.SelectedItem);
                Orden orden = DatosCompartidos.Entregas.FirstOrDefault(o => o.Id == idSeleccionado);

                if (orden != null)
                {
                    labelTotalPago.Text = $"Total:                       ${orden.Total:0}";
                }
            }
        }

        private void buttonPago_Click(object sender, EventArgs e)
        {
            if (comboBoxOrden.SelectedItem == null)
            {
                MessageBox.Show("Selecciona una orden para pagar.");
                return;
            }

            if (comboBoxMetodoPago.SelectedItem == null)
            {
                MessageBox.Show("Selecciona un método de pago.");
                return;
            }

            int idOrden = Convert.ToInt32(comboBoxOrden.SelectedItem);
            Orden orden = DatosCompartidos.Entregas.FirstOrDefault(o => o.Id == idOrden && o.Entregada && !o.Pagada);

            if (orden != null)
            {
                orden.Pagada = true;
                orden.MetodoPago = comboBoxMetodoPago.SelectedItem.ToString();

                MessageBox.Show($"Pago confirmado para la orden #{orden.Id}.\nMétodo: {orden.MetodoPago}");

                comboBoxOrden.Items.Remove(comboBoxOrden.SelectedItem);
                labelTotalPago.Text = "Total:";
                comboBoxMetodoPago.SelectedIndex = -1;
            }
            else
            {
                MessageBox.Show("La orden ya fue pagada o no es válida.");
            }
        }
        private void buttonEliminarEntrega_Click(object sender, EventArgs e)
        {
            {
                if (dataGridViewEntregas.SelectedRows.Count > 0)
                {
                    int idOrden = Convert.ToInt32(dataGridViewEntregas.SelectedRows[0].Cells["Id"].Value);

                    DialogResult confirmacion = MessageBox.Show(
                        $"¿Seguro que quieres eliminar la entrega #{idOrden}?",
                        "Confirmar Eliminación",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning
                    );

                    if (confirmacion == DialogResult.Yes)
                    {
                        var orden = DatosCompartidos.Entregas.FirstOrDefault(o => o.Id == idOrden);
                        if (orden != null)
                        {
                            DatosCompartidos.Entregas.Remove(orden);
                            MostrarEntregas();
                            MessageBox.Show($"Entrega #{idOrden} eliminada.");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Selecciona una entrega para eliminar.");
                }

            }

        }
        private void InsertarProductoEnBD(Producto producto)
        {
            using (SqlConnection conn = ConexionBD.ObtenerConexion())
            {
                conn.Open();
                string query = "INSERT INTO Productos (Nombre, Precio, Categoria, Disponible) VALUES (@nombre, @precio, @categoria, @disponible)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@nombre", producto.Nombre);
                    cmd.Parameters.AddWithValue("@categoria", producto.Categoria);
                    cmd.Parameters.AddWithValue("@precio", producto.Precio);
                    cmd.Parameters.AddWithValue("@disponible", true);
                    cmd.ExecuteNonQuery();
                }
            }
        }


    }

}
