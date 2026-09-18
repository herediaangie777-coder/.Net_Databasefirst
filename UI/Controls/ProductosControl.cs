using ProyectoConsolaObjetos1.Models.Academic;

namespace ProyectoConsolaObjetos1.UI.Controls;

public sealed class ProductosControl : UserControl
{
    private readonly AppState appState;
    private readonly DataGridView grid = new();
    private readonly BindingSource datos = new();
    private readonly TextBox txtBuscar = new();
    private readonly TextBox codigo = ControlUi.Campo("PROD-3938");
    private readonly TextBox nombre = ControlUi.Campo("Nombre del producto");
    private readonly TextBox categoria = ControlUi.Campo("Categoría");
    private readonly TextBox descripcion = ControlUi.Campo("Descripción");
    private readonly NumericUpDown precio = new() { DecimalPlaces = 2, Maximum = 1000000, Dock = DockStyle.Fill, Margin = new Padding(0, 2, 14, 7) };
    private readonly NumericUpDown stock = new() { Maximum = 1000000, Dock = DockStyle.Fill, Margin = new Padding(0, 2, 14, 7) };
    private readonly NumericUpDown stockMinimo = new() { Maximum = 1000000, Dock = DockStyle.Fill, Margin = new Padding(0, 2, 14, 7) };
    private readonly CheckBox activo = new() { Text = "Activo", Checked = true, AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(0, 4, 0, 6) };

    public ProductosControl(AppState appState)
    {
        this.appState = appState ?? throw new ArgumentNullException(nameof(appState)); Dock = DockStyle.Fill; BackColor = Color.White;
        codigo.ReadOnly = true; codigo.Enabled = false; codigo.BackColor = SystemColors.Control; codigo.Text = CodigoSiguiente().ToString("PROD-0000");
        grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Código", DataPropertyName = nameof(Producto.Codigo) }); grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Nombre", DataPropertyName = nameof(Producto.Nombre) }); grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Categoría", DataPropertyName = nameof(Producto.Categoria) }); grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Precio", DataPropertyName = nameof(Producto.PrecioVenta), DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } }); grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Stock", DataPropertyName = nameof(Producto.StockActual) }); grid.Columns.Add(new DataGridViewCheckBoxColumn { HeaderText = "Activo", DataPropertyName = nameof(Producto.Activo) }); ControlUi.ConfigurarGrid(grid);
        TableLayoutPanel formulario = ControlUi.Formulario(5);
        formulario.Controls.Add(ControlUi.Etiqueta("Código"), 0, 0); formulario.Controls.Add(codigo, 1, 0); formulario.Controls.Add(ControlUi.Etiqueta("Nombre"), 2, 0); formulario.Controls.Add(nombre, 3, 0);
        formulario.Controls.Add(ControlUi.Etiqueta("Categoría"), 0, 1); formulario.Controls.Add(categoria, 1, 1); formulario.Controls.Add(ControlUi.Etiqueta("Descripción"), 2, 1); formulario.Controls.Add(descripcion, 3, 1);
        formulario.Controls.Add(ControlUi.Etiqueta("Precio"), 0, 2); formulario.Controls.Add(precio, 1, 2); formulario.Controls.Add(ControlUi.Etiqueta("Stock actual"), 2, 2); formulario.Controls.Add(stock, 3, 2);
        formulario.Controls.Add(ControlUi.Etiqueta("Stock mínimo"), 0, 3); formulario.Controls.Add(stockMinimo, 1, 3); formulario.Controls.Add(activo, 3, 3);
        Panel acciones = new() { Dock = DockStyle.Top, Height = 48 }; Button agregar = ControlUi.Boton("Agregar producto"); Button eliminar = ControlUi.Boton("Eliminar seleccionado"); agregar.Click += (_, _) => Agregar(); eliminar.Click += (_, _) => Eliminar(); acciones.Controls.Add(agregar); acciones.Controls.Add(eliminar);
        Panel busqueda = ControlUi.Busqueda(txtBuscar, (_, _) => Filtrar(), (_, _) => ListarTodos());
        Controls.Add(grid); Controls.Add(busqueda); Controls.Add(acciones); Controls.Add(formulario); Controls.Add(new Label { Text = "Productos", Dock = DockStyle.Top, Height = 38, Font = new Font("Segoe UI", 20, FontStyle.Bold), ForeColor = ControlUi.Texto }); RefrescarDatos();
    }
    public void RefrescarDatos() { ListarTodos(); }
    private void Agregar()
    {
        if (string.IsNullOrWhiteSpace(nombre.Text) || string.IsNullOrWhiteSpace(categoria.Text) || precio.Value <= 0) { MessageBox.Show("Complete nombre, categoría y un precio mayor que cero."); return; }
        try { appState.Productos.Add(new Producto { Codigo = CodigoSiguiente(), Nombre = nombre.Text.Trim(), Categoria = categoria.Text.Trim(), Descripcion = descripcion.Text.Trim(), PrecioVenta = (float)precio.Value, StockActual = (int)stock.Value, StockMinimo = (int)stockMinimo.Value, Impuesto = 0.19f, Activo = activo.Checked }); Limpiar(); ListarTodos(); }
        catch (ArgumentException ex) { MessageBox.Show(ex.Message, "Datos inválidos"); }
    }
    private void Eliminar() { if (grid.CurrentRow?.DataBoundItem is Producto producto) appState.Productos.Remove(producto); }
    private void Filtrar()
    {
        string criterio = txtBuscar.Text.Trim();
        IEnumerable<Producto> resultados = string.IsNullOrWhiteSpace(criterio) ? appState.Productos : appState.Productos.Where(item => item.Codigo.ToString().Contains(criterio, StringComparison.OrdinalIgnoreCase) || item.Nombre.Contains(criterio, StringComparison.OrdinalIgnoreCase) || item.Categoria.Contains(criterio, StringComparison.OrdinalIgnoreCase));
        datos.DataSource = resultados.ToList(); grid.DataSource = datos;
    }
    private void ListarTodos() { txtBuscar.Clear(); datos.DataSource = appState.Productos.ToList(); grid.DataSource = datos; }
    private int CodigoSiguiente() => appState.Productos.Count == 0 ? 1 : appState.Productos.Max(item => item.Codigo) + 1;
    private void Limpiar() { codigo.Text = CodigoSiguiente().ToString("PROD-0000"); nombre.Clear(); categoria.Clear(); descripcion.Clear(); precio.Value = 0; stock.Value = 0; stockMinimo.Value = 0; activo.Checked = true; }
}