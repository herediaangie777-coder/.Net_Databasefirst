using ProyectoConsolaObjetos1.Models.Academic;
using ProyectoConsolaObjetos1.Models.Details;
using ProyectoConsolaObjetos1.Models.Users;
using ProyectoConsolaObjetos1.UI;

using System.ComponentModel;

namespace ProyectoConsolaObjetos1.UI.Controls;

public sealed class VentasControl : UserControl
    {
        private readonly AppState appState;
        private readonly DataGridView grid = new();
        private readonly DataGridView detallesGrid = new();
        private readonly BindingSource datos = new();
        private readonly BindingList<DetalleVenta> detallesPendientes = new();
        private readonly TextBox txtBuscar = new();
        private readonly TextBox codigo = ControlUi.Campo("VEN-37362");
        private readonly ComboBox cliente = new() { DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Fill, Margin = new Padding(0, 2, 0, 7) };
        private readonly ComboBox empleado = new() { DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Fill, Margin = new Padding(0, 2, 0, 7) };
        private readonly ComboBox producto = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 230, Margin = new Padding(0, 2, 8, 7) };
        private readonly NumericUpDown cantidad = new() { Minimum = 1, Maximum = 100000, Value = 1, Width = 82, Margin = new Padding(0, 2, 8, 7) };

        public VentasControl(AppState appState)
        {
            this.appState = appState ?? throw new ArgumentNullException(nameof(appState));
            Dock = DockStyle.Fill;
            BackColor = Color.White;
            codigo.ReadOnly = true;
            codigo.Enabled = false;
            codigo.BackColor = SystemColors.Control;
            codigo.Text = CodigoSiguiente().ToString("VEN-00000");

            ConfigurarGrillaVentas();
            ConfigurarGrillaDetalles();
            cliente.DisplayMember = nameof(Cliente.Nombre); cliente.DataSource = appState.Clientes;
            empleado.DisplayMember = nameof(Empleado.Nombre); empleado.DataSource = appState.Empleados;
            producto.DisplayMember = nameof(Producto.Nombre); producto.DataSource = appState.Productos;

            TableLayoutPanel formulario = new() { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 2, RowCount = 4, Padding = new Padding(0, 8, 0, 8) };
            formulario.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
            formulario.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            formulario.Controls.Add(ControlUi.Etiqueta("Código de venta"), 0, 0); formulario.Controls.Add(codigo, 1, 0);
            formulario.Controls.Add(ControlUi.Etiqueta("Cliente"), 0, 1); formulario.Controls.Add(cliente, 1, 1);
            formulario.Controls.Add(ControlUi.Etiqueta("Empleado"), 0, 2); formulario.Controls.Add(empleado, 1, 2);
            formulario.Controls.Add(ControlUi.Etiqueta("Producto y cantidad"), 0, 3);

            FlowLayoutPanel selector = new() { Dock = DockStyle.Fill, AutoSize = true, WrapContents = false };
            selector.Controls.Add(producto);
            selector.Controls.Add(new Label { Text = "Cantidad", AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(0, 9, 8, 0), ForeColor = ControlUi.Texto });
            selector.Controls.Add(cantidad);
            Button anadir = ControlUi.Boton("Añadir producto a la lista"); anadir.Click += (_, _) => AñadirProducto(); selector.Controls.Add(anadir);
            formulario.Controls.Add(selector, 1, 3);

            Panel acciones = new() { Dock = DockStyle.Top, Height = 48 };
            Button crear = ControlUi.Boton("Crear venta"); crear.Click += (_, _) => CrearVenta();
            Button eliminar = ControlUi.Boton("Eliminar venta"); eliminar.Click += (_, _) => EliminarVenta();
            acciones.Controls.Add(crear); acciones.Controls.Add(eliminar);

            Panel detallesTitulo = new() { Dock = DockStyle.Top, Height = 28 };
            detallesTitulo.Controls.Add(new Label { Text = "Detalles de la venta actual", Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = ControlUi.Texto });
            Panel ventasTitulo = new() { Dock = DockStyle.Top, Height = 28 };
            ventasTitulo.Controls.Add(new Label { Text = "Ventas registradas", Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = ControlUi.Texto });
            Panel busqueda = ControlUi.Busqueda(txtBuscar, (_, _) => Filtrar(), (_, _) => ListarTodos());

            Controls.Add(grid);
            Controls.Add(busqueda);
            Controls.Add(ventasTitulo);
            Controls.Add(acciones);
            Controls.Add(detallesGrid);
            Controls.Add(detallesTitulo);
            Controls.Add(formulario);
            Controls.Add(new Label { Text = "Ventas", Dock = DockStyle.Top, Height = 38, Font = new Font("Segoe UI", 20, FontStyle.Bold), ForeColor = ControlUi.Texto });
            RefrescarDatos();
        }

        public void RefrescarDatos() => ListarTodos();

        private void ConfigurarGrillaVentas()
        {
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Código", Name = "Codigo" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Cliente", Name = "Cliente" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Empleado", Name = "Empleado" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Fecha", Name = "Fecha" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Total", Name = "Total", DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });
            ControlUi.ConfigurarGrid(grid);
            grid.CellFormatting += FormatearVenta;
        }

        private void ConfigurarGrillaDetalles()
        {
            detallesGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Código producto", DataPropertyName = nameof(DetalleVenta.ProductoCodigo) });
            detallesGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Producto", DataPropertyName = nameof(DetalleVenta.ProductoNombre) });
            detallesGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Cantidad", DataPropertyName = nameof(DetalleVenta.Cantidad) });
            detallesGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Precio unitario", DataPropertyName = nameof(DetalleVenta.PrecioUnitario), DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });
            detallesGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Subtotal", DataPropertyName = nameof(DetalleVenta.Subtotal), DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });
            ControlUi.ConfigurarGrid(detallesGrid);
            detallesGrid.Height = 145;
            detallesGrid.DataSource = detallesPendientes;
        }

        private void AñadirProducto()
        {
            if (producto.SelectedItem is not Producto seleccionado) { MessageBox.Show("Seleccione un producto."); return; }
            DetalleVenta? existente = detallesPendientes.FirstOrDefault(item => item.ProductoCodigo == seleccionado.Codigo);
            if (existente is null)
            {
                detallesPendientes.Add(new DetalleVenta { ProductoCodigo = seleccionado.Codigo, ProductoNombre = seleccionado.Nombre, Cantidad = (int)cantidad.Value, PrecioUnitario = seleccionado.PrecioVenta });
            }
            else
            {
                existente.Cantidad += (int)cantidad.Value;
                detallesGrid.Refresh();
            }
        }

        private void CrearVenta()
        {
            if (cliente.SelectedItem is not Cliente clienteSeleccionado || empleado.SelectedItem is not Empleado empleadoSeleccionado || detallesPendientes.Count == 0)
            {
                MessageBox.Show("Seleccione cliente, empleado y añada al menos un producto.");
                return;
            }

            Venta venta = new() { Codigo = CodigoSiguiente(), Cliente = clienteSeleccionado, Empleado = empleadoSeleccionado, FechaVenta = DateTime.Now };
            foreach (DetalleVenta detalle in detallesPendientes)
            {
                Producto? productoSeleccionado = appState.Productos.FirstOrDefault(item => item.Codigo == detalle.ProductoCodigo);
                if (productoSeleccionado is not null)
                {
                    venta.Productos.Add(productoSeleccionado);
                    venta.Detalles.Add(new DetalleVenta { ProductoCodigo = detalle.ProductoCodigo, ProductoNombre = detalle.ProductoNombre, Cantidad = detalle.Cantidad, PrecioUnitario = detalle.PrecioUnitario });
                }
            }
            appState.Ventas.Add(venta);
            appState.ActualizarDetalles();
            detallesPendientes.Clear();
            codigo.Text = CodigoSiguiente().ToString("VEN-00000");
            ListarTodos();
        }

        private void EliminarVenta()
        {
            if (grid.CurrentRow?.DataBoundItem is Venta venta)
            {
                appState.Ventas.Remove(venta);
                appState.ActualizarDetalles();
                ListarTodos();
            }
        }

        private void Filtrar()
        {
            string criterio = txtBuscar.Text.Trim();
            IEnumerable<Venta> resultados = string.IsNullOrWhiteSpace(criterio) ? appState.Ventas : appState.Ventas.Where(item => item.Codigo.ToString().Contains(criterio, StringComparison.OrdinalIgnoreCase) || (item.Cliente?.Nombre?.Contains(criterio, StringComparison.OrdinalIgnoreCase) ?? false) || (item.Empleado?.Nombre?.Contains(criterio, StringComparison.OrdinalIgnoreCase) ?? false));
            datos.DataSource = resultados.ToList();
            grid.DataSource = datos;
        }

        private void ListarTodos()
        {
            txtBuscar.Clear();
            datos.DataSource = appState.Ventas.ToList();
            grid.DataSource = datos;
        }

        private void FormatearVenta(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || grid.Rows[e.RowIndex].DataBoundItem is not Venta venta) return;
            e.Value = e.ColumnIndex switch { 0 => venta.Codigo, 1 => venta.Cliente?.Nombre, 2 => venta.Empleado?.Nombre, 3 => venta.FechaVenta.ToString("dd/MM/yyyy"), 4 => venta.Detalles.Sum(item => item.Subtotal), _ => e.Value };
        }

        private int CodigoSiguiente() => appState.Ventas.Count == 0 ? 1 : appState.Ventas.Max(item => item.Codigo) + 1;
    }
