using System.Drawing;
using System.Windows.Forms;
using ProyectoConsolaObjetos1.Models.Base;
using ProyectoConsolaObjetos1.Services;

namespace ProyectoConsolaObjetos1.UI.Controls;

public sealed class UsuariosControl : UserControl
{
    private readonly AppState appState;
    private readonly DataGridView grid = new();
    private readonly TextBox nombre = new();
    private readonly TextBox correo = new();
    private readonly TextBox clave = new();
    private readonly CheckBox activo = new();

    public UsuariosControl(AppState appState)
    {
        this.appState = appState ?? throw new ArgumentNullException(nameof(appState));
        Dock = DockStyle.Fill;
        Padding = new Padding(12);
        ConstruirInterfaz();
        grid.DataSource = this.appState.Usuarios;
    }

    private void ConstruirInterfaz()
    {
        TableLayoutPanel layout = new()
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 92));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 46));

        FlowLayoutPanel editor = new() { Dock = DockStyle.Fill, WrapContents = false, Padding = new Padding(0, 8, 0, 0) };
        nombre.Width = 180;
        correo.Width = 210;
        clave.Width = 160;
        activo.Text = "Activo";
        activo.Checked = true;
        clave.PasswordChar = '*';
        editor.Controls.AddRange(new Control[]
        {
            new Label { Text = "Nombre", AutoSize = true, Margin = new Padding(0, 8, 5, 0) }, nombre,
            new Label { Text = "Correo", AutoSize = true, Margin = new Padding(12, 8, 5, 0) }, correo,
            new Label { Text = "Clave", AutoSize = true, Margin = new Padding(12, 8, 5, 0) }, clave,
            activo
        });

        grid.Dock = DockStyle.Fill;
        grid.AutoGenerateColumns = true;
        grid.AllowUserToAddRows = false;
        grid.ReadOnly = true;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        FlowLayoutPanel actions = new() { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
        Button guardar = new() { Text = "Guardar en JSON", AutoSize = true };
        Button agregar = new() { Text = "Agregar usuario", AutoSize = true };
        guardar.Click += (_, _) => Guardar();
        agregar.Click += (_, _) => AgregarUsuario();
        actions.Controls.AddRange(new Control[] { guardar, agregar });

        layout.Controls.Add(editor, 0, 0);
        layout.Controls.Add(grid, 0, 1);
        layout.Controls.Add(actions, 0, 2);
        Controls.Add(layout);
    }

    public void RefrescarDatos()
    {
        grid.DataSource = null;
        grid.DataSource = appState.Usuarios;
    }

    private void AgregarUsuario()
    {
        if (string.IsNullOrWhiteSpace(nombre.Text) || string.IsNullOrWhiteSpace(correo.Text))
        {
            MessageBox.Show("El nombre y el correo son obligatorios.", "Datos incompletos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            Usuario usuario = new()
            {
                Codigo = appState.Usuarios.Count == 0 ? 1 : appState.Usuarios.Max(item => item.Codigo) + 1,
                Nombre = nombre.Text.Trim(),
                Correo = correo.Text.Trim(),
                Clave = clave.Text,
                Activo = activo.Checked
            };
            appState.Usuarios.Add(usuario);
            nombre.Clear();
            correo.Clear();
            clave.Clear();
        }
        catch (ArgumentException exception)
        {
            MessageBox.Show(exception.Message, "Datos inválidos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void Guardar()
    {
        try
        {
            new JsonRepository<Usuario>("usuarios.json").Save(appState.Usuarios);
            MessageBox.Show("Usuarios guardados correctamente.", "Guardado", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (IOException exception)
        {
            MessageBox.Show($"No se pudieron guardar los usuarios: {exception.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
