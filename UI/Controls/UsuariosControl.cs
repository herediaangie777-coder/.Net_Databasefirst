using ProyectoConsolaObjetos1.Models;

namespace ProyectoConsolaObjetos1.UI.Controls;

public class UsuariosControl : UserControl
{
    protected readonly AppState AppState;
    private readonly int rol;
    private readonly string titulo;
    private readonly bool mostrarClave;
    private readonly DataGridView grid = new();
    private readonly TextBox txtBuscar = new();
    private readonly TextBox codigo = ControlUi.Campo("Código");
    private readonly TextBox nombre = ControlUi.Campo("Nombre completo");
    private readonly TextBox correo = ControlUi.Campo("correo@ejemplo.com");
    private readonly TextBox clave = ControlUi.Campo("Clave");
    private readonly TextBox direccion = ControlUi.Campo("Dirección");
    private readonly CheckBox activo = new() { Text = "Activo", Checked = true, AutoSize = true, Anchor = AnchorStyles.Left };

    public UsuariosControl(AppState appState, int rol = 0, string titulo = "Usuarios", bool mostrarClave = true)
    {
        AppState = appState ?? throw new ArgumentNullException(nameof(appState));
        this.rol = rol;
        this.titulo = titulo;
        this.mostrarClave = mostrarClave;
        Dock = DockStyle.Fill;
        BackColor = Color.White;
        clave.PasswordChar = '*';
        codigo.ReadOnly = true;
        codigo.Enabled = false;
        codigo.BackColor = SystemColors.Control;
        codigo.Text = "Automático";

        grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "ID", DataPropertyName = nameof(Usuario.Id) });
        grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Código", DataPropertyName = nameof(Usuario.Codigo) });
        grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Nombre", DataPropertyName = nameof(Usuario.Nombre) });
        grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Correo", DataPropertyName = nameof(Usuario.Correo) });
        grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Rol", DataPropertyName = nameof(Usuario.Rol) });
        grid.Columns.Add(new DataGridViewCheckBoxColumn { HeaderText = "Activo", DataPropertyName = nameof(Usuario.Activo) });
        ControlUi.ConfigurarGrid(grid);

        TableLayoutPanel formulario = ControlUi.Formulario(4);
        formulario.Controls.Add(ControlUi.Etiqueta("Código"), 0, 0); formulario.Controls.Add(codigo, 1, 0);
        formulario.Controls.Add(ControlUi.Etiqueta("Nombre"), 2, 0); formulario.Controls.Add(nombre, 3, 0);
        formulario.Controls.Add(ControlUi.Etiqueta("Correo"), 0, 1); formulario.Controls.Add(correo, 1, 1);
        if (mostrarClave)
        {
            formulario.Controls.Add(ControlUi.Etiqueta("Clave"), 2, 1); formulario.Controls.Add(clave, 3, 1);
            formulario.Controls.Add(ControlUi.Etiqueta("Dirección"), 0, 2); formulario.Controls.Add(direccion, 1, 2);
            formulario.Controls.Add(activo, 3, 2);
        }
        else
        {
            formulario.Controls.Add(ControlUi.Etiqueta("Dirección"), 2, 1); formulario.Controls.Add(direccion, 3, 1);
            formulario.Controls.Add(activo, 1, 2);
        }

        Panel acciones = new() { Dock = DockStyle.Top, Height = 48 };
        Button agregar = ControlUi.Boton("Agregar usuario"); agregar.Click += (_, _) => Agregar();
        Button eliminar = ControlUi.Boton("Eliminar seleccionado"); eliminar.Click += (_, _) => Eliminar();
        Button refrescar = ControlUi.Boton("Refrescar tabla"); refrescar.Click += (_, _) => RefrescarDatos();
        acciones.Controls.Add(agregar); acciones.Controls.Add(eliminar); acciones.Controls.Add(refrescar);
        Panel busqueda = ControlUi.Busqueda(txtBuscar, (_, _) => RefrescarDatos(), (_, _) => { txtBuscar.Clear(); RefrescarDatos(); });

        Controls.Add(grid); Controls.Add(busqueda); Controls.Add(acciones); Controls.Add(formulario);
        Controls.Add(new Label { Text = titulo, Dock = DockStyle.Top, Height = 38, Font = new Font("Segoe UI", 20, FontStyle.Bold), ForeColor = ControlUi.Texto });
        RefrescarDatos();
    }

    public virtual void RefrescarDatos()
    {
        try
        {
            grid.DataSource = AppState.ObtenerUsuarios(rol == 0 ? null : rol, txtBuscar.Text);
        }
        catch (Exception exception)
        {
            MostrarError(exception);
        }
    }

    private void Agregar()
    {
        if (string.IsNullOrWhiteSpace(nombre.Text) || string.IsNullOrWhiteSpace(correo.Text) || (mostrarClave && string.IsNullOrWhiteSpace(clave.Text)))
        {
            MessageBox.Show(mostrarClave ? "Nombre, correo y clave son obligatorios." : "Nombre y correo son obligatorios.", "Datos incompletos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        try
        {
            AppState.AgregarUsuario(new Usuario { Codigo = 0, Nombre = nombre.Text.Trim(), Correo = correo.Text.Trim(), Clave = mostrarClave ? clave.Text : string.Empty, Activo = activo.Checked, Rol = rol, Direccion = direccion.Text.Trim() });
            Limpiar();
            RefrescarDatos();
        }
        catch (Exception exception) { MostrarError(exception); }
    }

    private void Eliminar()
    {
        if (grid.CurrentRow?.DataBoundItem is not Usuario usuario) return;
        try { AppState.EliminarUsuario(usuario.Id); RefrescarDatos(); }
        catch (Exception exception) { MostrarError(exception); }
    }

    private void Limpiar() { codigo.Text = "Automático"; nombre.Clear(); correo.Clear(); clave.Clear(); direccion.Clear(); activo.Checked = true; }
    private void MostrarError(Exception exception) => MessageBox.Show(exception.Message, "Error de base de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
}
