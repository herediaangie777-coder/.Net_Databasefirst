using System.Windows.Forms;

namespace ProyectoConsolaObjetos1.UI;

public sealed class MainForm : Form
{
    private readonly AppState appState;
    private readonly Panel contenido = new();
    private readonly Button clientesButton;
    private readonly Button empleadosButton;
    private readonly Button productosButton;
    private readonly Button ventasButton;
    private readonly Controls.ClientesControl clientesControl;
    private readonly Controls.EmpleadosControl empleadosControl;
    private readonly Controls.ProductosControl productosControl;
    private readonly Controls.VentasControl ventasControl;

    public MainForm(AppState? appState = null)
    {
        this.appState = appState ?? new AppState();
        Text = "ProyectoConsolaObjetos1 - Gestión";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(1020, 650);
        Size = new Size(1240, 780);
        BackColor = Color.White;

        Panel menu = new() { Dock = DockStyle.Left, Width = 224, BackColor = Color.FromArgb(30, 144, 255), Padding = new Padding(22, 28, 22, 24) };
        Label marca = new() { Text = "GESTIÓN", Dock = DockStyle.Top, Height = 42, ForeColor = Color.White, Font = new Font("Segoe UI", 18, FontStyle.Bold), TextAlign = ContentAlignment.MiddleLeft };
        Label subtitulo = new() { Text = "Menú de", Dock = DockStyle.Bottom, Height = 28, ForeColor = Color.White, Font = new Font("Segoe UI", 10, FontStyle.Bold), TextAlign = ContentAlignment.MiddleLeft };
        FlowLayoutPanel opciones = new() { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, Padding = new Padding(0, 24, 0, 0) };
        clientesButton = CrearBotonMenu("Clientes");
        empleadosButton = CrearBotonMenu("Empleados");
        productosButton = CrearBotonMenu("Productos");
        ventasButton = CrearBotonMenu("Ventas");
        clientesControl = new Controls.ClientesControl(this.appState);
        empleadosControl = new Controls.EmpleadosControl(this.appState);
        productosControl = new Controls.ProductosControl(this.appState);
        ventasControl = new Controls.VentasControl(this.appState);
        clientesButton.Click += (_, _) => MostrarVista(clientesControl, clientesButton);
        empleadosButton.Click += (_, _) => MostrarVista(empleadosControl, empleadosButton);
        productosButton.Click += (_, _) => MostrarVista(productosControl, productosButton);
        ventasButton.Click += (_, _) => MostrarVista(ventasControl, ventasButton);
        opciones.Controls.AddRange(new Control[] { clientesButton, empleadosButton, productosButton, ventasButton });
        Button refrescarTodo = CrearBotonMenu("Refrescar datos");
        refrescarTodo.Dock = DockStyle.Bottom;
        refrescarTodo.Click += (_, _) => RefrescarTodasLasVistas();
        menu.Controls.Add(opciones);
        menu.Controls.Add(subtitulo);
        menu.Controls.Add(refrescarTodo);
        menu.Controls.Add(marca);

        contenido.Dock = DockStyle.Fill;
        contenido.Padding = new Padding(28, 24, 28, 24);
        Controls.Add(contenido);
        Controls.Add(menu);
        MostrarVista(clientesControl, clientesButton);
    }

    private void MostrarVista(Control vista, Button botonActivo)
    {
        contenido.Controls.Clear();
        vista.Dock = DockStyle.Fill;
        contenido.Controls.Add(vista);
        clientesButton.BackColor = Color.Transparent;
        empleadosButton.BackColor = Color.Transparent;
        productosButton.BackColor = Color.Transparent;
        ventasButton.BackColor = Color.Transparent;
        botonActivo.BackColor = Color.FromArgb(0, 112, 220);
        if (vista is Controls.ClientesControl clientes) clientes.RefrescarDatos();
        if (vista is Controls.EmpleadosControl empleados) empleados.RefrescarDatos();
        if (vista is Controls.ProductosControl productos) productos.RefrescarDatos();
        if (vista is Controls.VentasControl ventas) ventas.RefrescarDatos();
    }

    private void RefrescarTodasLasVistas()
    {
        clientesControl.RefrescarDatos();
        empleadosControl.RefrescarDatos();
        productosControl.RefrescarDatos();
        ventasControl.RefrescarDatos();
    }

    private static Button CrearBotonMenu(string texto)
    {
        return new Button { Text = texto, Width = 180, Height = 42, FlatStyle = FlatStyle.Flat, BackColor = Color.Transparent, ForeColor = Color.White, Font = new Font("Segoe UI", 10, FontStyle.Bold), FlatAppearance = { BorderColor = Color.White, BorderSize = 1 }, Margin = new Padding(0, 0, 0, 10), TextAlign = ContentAlignment.MiddleCenter };
    }
}
