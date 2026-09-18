using System.ComponentModel;
using ProyectoConsolaObjetos1.Models;

namespace ProyectoConsolaObjetos1.UI.Controls;

public sealed class VentasControl : UserControl
{
    private readonly AppState appState;
    private readonly DataGridView dgvVentas = new();
    private readonly DataGridView dgvCarrito = new();
    private readonly TextBox txtBuscar = new();
    private readonly TextBox codigo = ControlUi.Campo("VEN-00001");
    private readonly ComboBox cliente = new() { DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Fill };
    private readonly ComboBox empleado = new() { DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Fill };
    private readonly ComboBox producto = new() { DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Fill };
    private readonly NumericUpDown cantidad = new() { Minimum = 1, Maximum = 100000, Value = 1, Dock = DockStyle.Fill };
    private readonly BindingList<DetalleVentaVista> detallesPendientes = new();
    private readonly Label totalAcumulado = new() { Text = "Total Acumulado: $0.00", AutoSize = true, Anchor = AnchorStyles.Left, ForeColor = ControlUi.Texto, Font = new Font("Segoe UI", 11, FontStyle.Bold) };

    public VentasControl(AppState appState)
    {
        this.appState = appState ?? throw new ArgumentNullException(nameof(appState));
        AutoScroll = true;
        BackColor = Color.White;
        Padding = new Padding(10);

        codigo.ReadOnly = true;
        codigo.Enabled = false;
        codigo.BackColor = SystemColors.Control;

        ConfigurarGrillaVentas();
        ConfigurarGrillaCarrito();

        TableLayoutPanel contenido = new()
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Padding = new Padding(0),
            AutoSize = false
        };
        contenido.MinimumSize = new Size(900, 620);
        contenido.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        contenido.RowStyles.Add(new RowStyle(SizeType.Absolute, 350));
        contenido.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        contenido.Controls.Add(CrearZonaSuperior(), 0, 0);
        contenido.Controls.Add(CrearZonaHistorial(), 0, 1);
        Controls.Add(contenido);
        RefrescarDatos();
    }

    public async void RefrescarDatos()
    {
        await RefrescarDatosAsync();
    }

    private async Task RefrescarDatosAsync()
    {
        try
        {
            dgvVentas.DataSource = await appState.ObtenerVentasAsync(txtBuscar.Text);
            codigo.Text = $"VEN-{await appState.SiguienteCodigoVentaAsync():00000}";
            await ConfigurarCombosAsync();
        }
        catch (Exception exception) { MostrarError(exception); }
    }

    private Control CrearZonaSuperior()
    {
        TableLayoutPanel columnas = new()
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Padding = new Padding(0, 0, 0, 10)
        };
        columnas.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 350));
        columnas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        columnas.Controls.Add(CrearPanelRegistro(), 0, 0);
        columnas.Controls.Add(CrearPanelCarrito(), 1, 0);
        return columnas;
    }

    private Control CrearPanelRegistro()
    {
        Panel panel = new() { Dock = DockStyle.Fill, Padding = new Padding(0, 0, 10, 0), MinimumSize = new Size(330, 0) };
        TableLayoutPanel formulario = new()
        {
            Dock = DockStyle.Top,
            ColumnCount = 2,
            RowCount = 6,
            AutoSize = true,
            Padding = new Padding(0, 0, 0, 8)
        };
        formulario.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 118));
        formulario.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        Label titulo = CrearTitulo("Registrar Venta");
        formulario.Controls.Add(titulo, 0, 0);
        formulario.SetColumnSpan(titulo, 2);
        formulario.Controls.Add(ControlUi.Etiqueta("Código de venta"), 0, 1); formulario.Controls.Add(codigo, 1, 1);
        formulario.Controls.Add(ControlUi.Etiqueta("Cliente"), 0, 2); formulario.Controls.Add(cliente, 1, 2);
        formulario.Controls.Add(ControlUi.Etiqueta("Empleado"), 0, 3); formulario.Controls.Add(empleado, 1, 3);
        formulario.Controls.Add(ControlUi.Etiqueta("Producto"), 0, 4); formulario.Controls.Add(producto, 1, 4);
        formulario.Controls.Add(ControlUi.Etiqueta("Cantidad"), 0, 5); formulario.Controls.Add(cantidad, 1, 5);

        Button anadir = ControlUi.Boton("Añadir al carrito");
        anadir.Dock = DockStyle.Top;
        anadir.Click += (_, _) => AnadirProducto();
        panel.Controls.Add(anadir);
        panel.Controls.Add(formulario);
        return panel;
    }

    private Control CrearPanelCarrito()
    {
        TableLayoutPanel panel = new()
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Padding = new Padding(10, 0, 0, 0)
        };
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 160));
        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        panel.Controls.Add(CrearTitulo("Detalles de la Venta Actual", 10, 10), 0, 0);
        panel.Controls.Add(dgvCarrito, 0, 1);
        panel.Controls.Add(CrearFilaCarrito(), 0, 2);
        return panel;
    }

    private Control CrearFilaCarrito()
    {
        TableLayoutPanel fila = new() { Dock = DockStyle.Fill, ColumnCount = 2, Padding = new Padding(0, 8, 0, 0) };
        fila.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        fila.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        fila.Controls.Add(totalAcumulado, 0, 0);

        FlowLayoutPanel botones = new() { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = false, AutoSize = true };
        Button quitar = ControlUi.Boton("Quitar producto");
        quitar.Click += (_, _) => QuitarDetalle();
        Button crear = ControlUi.Boton("Crear venta");
        crear.Click += async (_, _) => await CrearVentaAsync();
        botones.Controls.Add(quitar);
        botones.Controls.Add(crear);
        fila.Controls.Add(botones, 1, 0);
        return fila;
    }

    private Control CrearZonaHistorial()
    {
        TableLayoutPanel historial = new() { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, Padding = new Padding(0, 4, 0, 0) };
        historial.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        historial.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        historial.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        historial.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        historial.Controls.Add(CrearTitulo("Ventas Registradas", 10, 10), 0, 0);
        historial.Controls.Add(ControlUi.Busqueda(txtBuscar, async (_, _) => await RefrescarDatosAsync(), async (_, _) => { txtBuscar.Clear(); await RefrescarDatosAsync(); }), 0, 1);
        historial.Controls.Add(dgvVentas, 0, 2);
        return historial;
    }

    private static Label CrearTitulo(string texto, int size = 20, int leftMargin = 0) => new() { Text = texto, Dock = DockStyle.Fill, Font = new Font("Segoe UI", Math.Max(9, size), FontStyle.Bold), ForeColor = ControlUi.Texto, TextAlign = ContentAlignment.MiddleLeft, Margin = new Padding(leftMargin, 0, 0, 0) };

    private void ConfigurarGrillaVentas()
    {
        dgvVentas.Name = "dgvVentas";
        dgvVentas.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Código", DataPropertyName = nameof(Venta.Codigo) });
        dgvVentas.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Cliente", Name = "Cliente" });
        dgvVentas.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Empleado", Name = "Empleado" });
        dgvVentas.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Fecha", DataPropertyName = nameof(Venta.FechaVenta), DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" } });
        ControlUi.ConfigurarGrid(dgvVentas);
        dgvVentas.CellFormatting += FormatearVenta;
    }

    private void ConfigurarGrillaCarrito()
    {
        dgvCarrito.Name = "dgvCarrito";
        dgvCarrito.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Producto", DataPropertyName = nameof(DetalleVentaVista.Producto) });
        dgvCarrito.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Cantidad", DataPropertyName = nameof(DetalleVentaVista.Cantidad) });
        dgvCarrito.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Precio Unitario", DataPropertyName = nameof(DetalleVentaVista.PrecioUnitario), DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });
        dgvCarrito.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Subtotal", DataPropertyName = nameof(DetalleVentaVista.Subtotal), DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });
        ControlUi.ConfigurarGrid(dgvCarrito);
        dgvCarrito.Dock = DockStyle.Fill;
        dgvCarrito.DataSource = detallesPendientes;
    }

    private async Task ConfigurarCombosAsync()
    {
        try
        {
            cliente.DataSource = await appState.ObtenerUsuariosAsync(AppState.RolCliente);
            cliente.DisplayMember = nameof(Usuario.Nombre);
            cliente.ValueMember = nameof(Usuario.Id);
            empleado.DataSource = await appState.ObtenerUsuariosAsync(AppState.RolEmpleado);
            empleado.DisplayMember = nameof(Usuario.Nombre);
            empleado.ValueMember = nameof(Usuario.Id);
            producto.DataSource = await appState.ObtenerProductosAsync();
            producto.DisplayMember = nameof(Producto.Nombre);
            producto.ValueMember = nameof(Producto.Codigo);
        }
        catch (Exception exception) { MostrarError(exception); }
    }

    private void AnadirProducto()
    {
        if (producto.SelectedItem is not Producto seleccionado) { MessageBox.Show("Seleccione un producto."); return; }
        DetalleVentaVista? existente = detallesPendientes.FirstOrDefault(item => item.ProductoCodigo == seleccionado.Codigo);
        if (existente is null) detallesPendientes.Add(new DetalleVentaVista(seleccionado.Codigo, seleccionado.Nombre, (int)cantidad.Value, seleccionado.PrecioVenta));
        else { existente.Cantidad += (int)cantidad.Value; dgvCarrito.Refresh(); }
        ActualizarTotal();
    }

    private void QuitarDetalle()
    {
        if (dgvCarrito.CurrentRow?.DataBoundItem is not DetalleVentaVista detalle) return;
        detallesPendientes.Remove(detalle);
        ActualizarTotal();
    }

    private async Task CrearVentaAsync()
    {
        if (cliente.SelectedValue is not int clienteId || empleado.SelectedValue is not int empleadoId || detallesPendientes.Count == 0)
        {
            MessageBox.Show("Seleccione cliente, empleado y añada al menos un producto.");
            return;
        }
        try
        {
            await appState.AgregarVentaAsync(clienteId, empleadoId, detallesPendientes.Select(item => (item.ProductoCodigo, item.Cantidad)));
            detallesPendientes.Clear();
            ActualizarTotal();
            await RefrescarDatosAsync();
        }
        catch (Exception exception) { MostrarError(exception); }
    }

    private void FormatearVenta(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || dgvVentas.Rows[e.RowIndex].DataBoundItem is not Venta venta) return;
        e.Value = e.ColumnIndex switch { 1 => venta.Cliente?.Nombre, 2 => venta.Empleado?.Nombre, _ => e.Value };
    }

    private void ActualizarTotal() => totalAcumulado.Text = $"Total Acumulado: {detallesPendientes.Sum(item => item.Subtotal):C2}";
    private void MostrarError(Exception exception) => MessageBox.Show(exception.Message, "Error de base de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);

    private sealed class DetalleVentaVista
    {
        public DetalleVentaVista(int productoCodigo, string producto, int cantidad, decimal precioUnitario) { ProductoCodigo = productoCodigo; Producto = producto; Cantidad = cantidad; PrecioUnitario = precioUnitario; }
        public int ProductoCodigo { get; }
        public string Producto { get; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; }
        public decimal Subtotal => Cantidad * PrecioUnitario;
    }
}
