using ProyectoConsolaObjetos1.Models;

namespace ProyectoConsolaObjetos1.UI.Controls;

public sealed class ProductosControl : UserControl
{
    private readonly AppState appState;
    private readonly DataGridView grid = new();
    private readonly TextBox txtBuscar = new();
    private readonly TextBox nombre = ControlUi.Campo("Nombre del producto");
    private readonly TextBox categoria = ControlUi.Campo("Categoría");
    private readonly TextBox descripcion = ControlUi.Campo("Descripción");
    private readonly NumericUpDown precio = new() { DecimalPlaces = 2, Maximum = 1000000, Dock = DockStyle.Fill, Margin = new Padding(0, 2, 14, 7) };
    private readonly NumericUpDown stock = new() { Maximum = 1000000, Dock = DockStyle.Fill, Margin = new Padding(0, 2, 14, 7) };
    private readonly NumericUpDown stockMinimo = new() { Maximum = 1000000, Dock = DockStyle.Fill, Margin = new Padding(0, 2, 14, 7) };
    private readonly CheckBox activo = new() { Text = "Activo", Checked = true, AutoSize = true, Anchor = AnchorStyles.Left };

    public ProductosControl(AppState appState)
    {
        this.appState = appState ?? throw new ArgumentNullException(nameof(appState));
        Dock = DockStyle.Fill;
        BackColor = Color.White;
        grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Código", DataPropertyName = nameof(Producto.Codigo) });
        grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Nombre", DataPropertyName = nameof(Producto.Nombre) });
        grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Categoría", DataPropertyName = nameof(Producto.Categoria) });
        grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Precio", DataPropertyName = nameof(Producto.PrecioVenta), DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });
        grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Stock", DataPropertyName = nameof(Producto.StockActual) });
        grid.Columns.Add(new DataGridViewCheckBoxColumn { HeaderText = "Activo", DataPropertyName = nameof(Producto.Activo) });
        ControlUi.ConfigurarGrid(grid);

        TableLayoutPanel formulario = ControlUi.Formulario(4);
        formulario.Controls.Add(ControlUi.Etiqueta("Nombre"), 0, 0); formulario.Controls.Add(nombre, 1, 0);
        formulario.Controls.Add(ControlUi.Etiqueta("Categoría"), 2, 0); formulario.Controls.Add(categoria, 3, 0);
        formulario.Controls.Add(ControlUi.Etiqueta("Descripción"), 0, 1); formulario.Controls.Add(descripcion, 1, 1); formulario.SetColumnSpan(descripcion, 3);
        formulario.Controls.Add(ControlUi.Etiqueta("Precio"), 0, 2); formulario.Controls.Add(precio, 1, 2);
        formulario.Controls.Add(ControlUi.Etiqueta("Stock actual"), 2, 2); formulario.Controls.Add(stock, 3, 2);
        formulario.Controls.Add(ControlUi.Etiqueta("Stock mínimo"), 0, 3); formulario.Controls.Add(stockMinimo, 1, 3); formulario.Controls.Add(activo, 3, 3);

        Panel acciones = new() { Dock = DockStyle.Top, Height = 48 };
        Button agregar = ControlUi.Boton("Agregar producto"); agregar.Click += (_, _) => Agregar();
        Button eliminar = ControlUi.Boton("Eliminar seleccionado"); eliminar.Click += (_, _) => Eliminar();
        Button refrescar = ControlUi.Boton("Refrescar tabla"); refrescar.Click += (_, _) => RefrescarDatos();
        acciones.Controls.Add(agregar); acciones.Controls.Add(eliminar); acciones.Controls.Add(refrescar);
        Panel busqueda = ControlUi.Busqueda(txtBuscar, (_, _) => RefrescarDatos(), (_, _) => { txtBuscar.Clear(); RefrescarDatos(); });

        Controls.Add(grid); Controls.Add(busqueda); Controls.Add(acciones); Controls.Add(formulario);
        Controls.Add(new Label { Text = "Productos", Dock = DockStyle.Top, Height = 38, Font = new Font("Segoe UI", 20, FontStyle.Bold), ForeColor = ControlUi.Texto });
        RefrescarDatos();
    }

    public void RefrescarDatos()
    {
        try { grid.DataSource = appState.ObtenerProductos(txtBuscar.Text); }
        catch (Exception exception) { MostrarError(exception); }
    }

    private void Agregar()
    {
        if (string.IsNullOrWhiteSpace(nombre.Text) || string.IsNullOrWhiteSpace(categoria.Text) || precio.Value <= 0)
        {
            MessageBox.Show("Nombre, categoría y precio son obligatorios.", "Datos incompletos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        try
        {
            appState.AgregarProducto(new Producto { Nombre = nombre.Text.Trim(), Categoria = categoria.Text.Trim(), Descripcion = descripcion.Text.Trim(), PrecioVenta = precio.Value, StockActual = (int)stock.Value, StockMinimo = (int)stockMinimo.Value, Impuesto = 0.19f, Activo = activo.Checked });
            Limpiar();
            RefrescarDatos();
        }
        catch (Exception exception) { MostrarError(exception); }
    }

    private void Eliminar()
    {
        if (grid.CurrentRow?.DataBoundItem is not Producto producto) return;
        try { appState.EliminarProducto(producto.Codigo); RefrescarDatos(); }
        catch (Exception exception) { MostrarError(exception); }
    }

    private void Limpiar() { nombre.Clear(); categoria.Clear(); descripcion.Clear(); precio.Value = 0; stock.Value = 0; stockMinimo.Value = 0; activo.Checked = true; }
    private void MostrarError(Exception exception) => MessageBox.Show(exception.Message, "Error de base de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
}
