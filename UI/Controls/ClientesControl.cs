using ProyectoConsolaObjetos1.Models.Users;

namespace ProyectoConsolaObjetos1.UI.Controls;

public sealed class ClientesControl : UserControl
{
    private readonly AppState appState;
    private readonly DataGridView grid = new();
    private readonly BindingSource datos = new();
    private readonly TextBox txtBuscar = new();
    private readonly TextBox codigo = ControlUi.Campo("CLI-64557");
    private readonly TextBox nombre = ControlUi.Campo("Nombre completo");
    private readonly TextBox correo = ControlUi.Campo("correo@ejemplo.com");
    private readonly TextBox direccion = ControlUi.Campo("Dirección");
    private readonly CheckBox activo = new() { Text = "Activo", Checked = true, AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(0, 4, 0, 6) };

    public ClientesControl(AppState appState)
    {
        this.appState = appState ?? throw new ArgumentNullException(nameof(appState));
        Dock = DockStyle.Fill; BackColor = Color.White;
        codigo.ReadOnly = true; codigo.Enabled = false; codigo.BackColor = SystemColors.Control; codigo.Text = CodigoSiguiente().ToString("CLI-00000");
        grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Código", DataPropertyName = nameof(Cliente.Codigo) }); grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Nombre", DataPropertyName = nameof(Cliente.Nombre) }); grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Correo", DataPropertyName = nameof(Cliente.Correo) }); grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Dirección", DataPropertyName = nameof(Cliente.Direccion) }); grid.Columns.Add(new DataGridViewCheckBoxColumn { HeaderText = "Activo", DataPropertyName = nameof(Cliente.Activo) });
        ControlUi.ConfigurarGrid(grid);
        TableLayoutPanel formulario = ControlUi.Formulario(3);
        formulario.Controls.Add(ControlUi.Etiqueta("Código"), 0, 0); formulario.Controls.Add(codigo, 1, 0); formulario.Controls.Add(ControlUi.Etiqueta("Nombre"), 2, 0); formulario.Controls.Add(nombre, 3, 0);
        formulario.Controls.Add(ControlUi.Etiqueta("Correo"), 0, 1); formulario.Controls.Add(correo, 1, 1); formulario.Controls.Add(ControlUi.Etiqueta("Dirección"), 2, 1); formulario.Controls.Add(direccion, 3, 1); formulario.Controls.Add(activo, 1, 2);
        Panel acciones = new() { Dock = DockStyle.Top, Height = 48 }; Button agregar = ControlUi.Boton("Agregar cliente"); Button eliminar = ControlUi.Boton("Eliminar seleccionado"); agregar.Click += (_, _) => Agregar(); eliminar.Click += (_, _) => Eliminar(); acciones.Controls.Add(agregar); acciones.Controls.Add(eliminar);
        Panel busqueda = ControlUi.Busqueda(txtBuscar, (_, _) => Filtrar(), (_, _) => ListarTodos());
        Controls.Add(grid); Controls.Add(busqueda); Controls.Add(acciones); Controls.Add(formulario); Controls.Add(new Label { Text = "Clientes", Dock = DockStyle.Top, Height = 38, Font = new Font("Segoe UI", 20, FontStyle.Bold), ForeColor = ControlUi.Texto });
        RefrescarDatos();
    }

    public void RefrescarDatos() { ListarTodos(); }
    private void Agregar()
    {
        if (string.IsNullOrWhiteSpace(nombre.Text) || string.IsNullOrWhiteSpace(correo.Text) || string.IsNullOrWhiteSpace(direccion.Text)) { MessageBox.Show("Complete nombre, correo y dirección."); return; }
        try { appState.Clientes.Add(new Cliente { Codigo = CodigoSiguiente(), Nombre = nombre.Text.Trim(), Correo = correo.Text.Trim(), Direccion = direccion.Text.Trim(), Activo = activo.Checked }); Limpiar(); ListarTodos(); }
        catch (ArgumentException ex) { MessageBox.Show(ex.Message, "Datos inválidos"); }
    }
    private void Eliminar() { if (grid.CurrentRow?.DataBoundItem is Cliente cliente) appState.Clientes.Remove(cliente); }
    private void Filtrar()
    {
        string criterio = txtBuscar.Text.Trim();
        IEnumerable<Cliente> resultados = string.IsNullOrWhiteSpace(criterio) ? appState.Clientes : appState.Clientes.Where(item => item.Codigo.ToString().Contains(criterio, StringComparison.OrdinalIgnoreCase) || item.Nombre.Contains(criterio, StringComparison.OrdinalIgnoreCase) || item.Correo.Contains(criterio, StringComparison.OrdinalIgnoreCase));
        datos.DataSource = resultados.ToList(); grid.DataSource = datos;
    }
    private void ListarTodos() { txtBuscar.Clear(); datos.DataSource = appState.Clientes.ToList(); grid.DataSource = datos; }
    private int CodigoSiguiente() => appState.Clientes.Count == 0 ? 1 : appState.Clientes.Max(item => item.Codigo) + 1;
    private void Limpiar() { codigo.Text = CodigoSiguiente().ToString("CLI-00000"); nombre.Clear(); correo.Clear(); direccion.Clear(); activo.Checked = true; }
}