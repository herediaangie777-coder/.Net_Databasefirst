using ProyectoConsolaObjetos1.Models.Users;

namespace ProyectoConsolaObjetos1.UI.Controls;

public sealed class EmpleadosControl : UserControl
{
    private readonly AppState appState;
    private readonly DataGridView grid = new();
    private readonly BindingSource datos = new();
    private readonly TextBox txtBuscar = new();
    private readonly TextBox codigo = ControlUi.Campo("EMP-35340");
    private readonly TextBox nombre = ControlUi.Campo("Nombre completo");
    private readonly TextBox correo = ControlUi.Campo("correo@ejemplo.com");
    private readonly TextBox clave = ControlUi.Campo("Clave");
    private readonly CheckBox activo = new() { Text = "Activo", Checked = true, AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(0, 4, 0, 6) };

    public EmpleadosControl(AppState appState)
    {
        this.appState = appState ?? throw new ArgumentNullException(nameof(appState)); Dock = DockStyle.Fill; BackColor = Color.White;
        codigo.ReadOnly = true; codigo.Enabled = false; codigo.BackColor = SystemColors.Control; codigo.Text = CodigoSiguiente().ToString("EMP-00000");
        grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Código", DataPropertyName = nameof(Empleado.Codigo) }); grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Nombre", DataPropertyName = nameof(Empleado.Nombre) }); grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Correo", DataPropertyName = nameof(Empleado.Correo) }); grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Clave", DataPropertyName = nameof(Empleado.Clave) }); grid.Columns.Add(new DataGridViewCheckBoxColumn { HeaderText = "Activo", DataPropertyName = nameof(Empleado.Activo) }); ControlUi.ConfigurarGrid(grid);
        TableLayoutPanel formulario = ControlUi.Formulario(3);
        formulario.Controls.Add(ControlUi.Etiqueta("Código"), 0, 0); formulario.Controls.Add(codigo, 1, 0); formulario.Controls.Add(ControlUi.Etiqueta("Nombre"), 2, 0); formulario.Controls.Add(nombre, 3, 0);
        formulario.Controls.Add(ControlUi.Etiqueta("Correo"), 0, 1); formulario.Controls.Add(correo, 1, 1); formulario.Controls.Add(ControlUi.Etiqueta("Clave"), 2, 1); formulario.Controls.Add(clave, 3, 1); formulario.Controls.Add(activo, 1, 2);
        Panel acciones = new() { Dock = DockStyle.Top, Height = 48 }; Button agregar = ControlUi.Boton("Agregar empleado"); Button eliminar = ControlUi.Boton("Eliminar seleccionado"); agregar.Click += (_, _) => Agregar(); eliminar.Click += (_, _) => Eliminar(); acciones.Controls.Add(agregar); acciones.Controls.Add(eliminar);
        Panel busqueda = ControlUi.Busqueda(txtBuscar, (_, _) => Filtrar(), (_, _) => ListarTodos());
        Controls.Add(grid); Controls.Add(busqueda); Controls.Add(acciones); Controls.Add(formulario); Controls.Add(new Label { Text = "Empleados", Dock = DockStyle.Top, Height = 38, Font = new Font("Segoe UI", 20, FontStyle.Bold), ForeColor = ControlUi.Texto }); RefrescarDatos();
    }
    public void RefrescarDatos() { ListarTodos(); }
    private void Agregar()
    {
        if (string.IsNullOrWhiteSpace(nombre.Text) || string.IsNullOrWhiteSpace(correo.Text) || string.IsNullOrWhiteSpace(clave.Text)) { MessageBox.Show("Complete nombre, correo y clave."); return; }
        try { appState.Empleados.Add(new Empleado { Codigo = CodigoSiguiente(), Nombre = nombre.Text.Trim(), Correo = correo.Text.Trim(), Clave = clave.Text.Trim(), Activo = activo.Checked }); Limpiar(); ListarTodos(); }
        catch (ArgumentException ex) { MessageBox.Show(ex.Message, "Datos inválidos"); }
    }
    private void Eliminar() { if (grid.CurrentRow?.DataBoundItem is Empleado empleado) appState.Empleados.Remove(empleado); }
    private void Filtrar()
    {
        string criterio = txtBuscar.Text.Trim();
        IEnumerable<Empleado> resultados = string.IsNullOrWhiteSpace(criterio) ? appState.Empleados : appState.Empleados.Where(item => item.Codigo.ToString().Contains(criterio, StringComparison.OrdinalIgnoreCase) || item.Nombre.Contains(criterio, StringComparison.OrdinalIgnoreCase) || item.Correo.Contains(criterio, StringComparison.OrdinalIgnoreCase));
        datos.DataSource = resultados.ToList(); grid.DataSource = datos;
    }
    private void ListarTodos() { txtBuscar.Clear(); datos.DataSource = appState.Empleados.ToList(); grid.DataSource = datos; }
    private int CodigoSiguiente() => appState.Empleados.Count == 0 ? 1 : appState.Empleados.Max(item => item.Codigo) + 1;
    private void Limpiar() { codigo.Text = CodigoSiguiente().ToString("EMP-00000"); nombre.Clear(); correo.Clear(); clave.Clear(); activo.Checked = true; }
}