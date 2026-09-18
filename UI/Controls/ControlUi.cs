using System.Drawing;
using System.Windows.Forms;

namespace ProyectoConsolaObjetos1.UI.Controls;

internal static class ControlUi
{
    public static readonly Color Azul = Color.FromArgb(0, 128, 255);
    public static readonly Color Texto = Color.FromArgb(34, 47, 62);
    public static readonly Color Borde = Color.FromArgb(213, 222, 232);

    public static Label Etiqueta(string texto) => new() { Text = texto, AutoSize = true, Anchor = AnchorStyles.Left, ForeColor = Texto, Font = new Font("Segoe UI", 9, FontStyle.Bold), Margin = new Padding(0, 7, 8, 4) };
    public static TextBox Campo(string placeholder) => new() { PlaceholderText = placeholder, Dock = DockStyle.Fill, Height = 28, Margin = new Padding(0, 2, 14, 7) };
    public static Button Boton(string texto) => new() { Text = texto, AutoSize = true, Height = 34, BackColor = Azul, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, FlatAppearance = { BorderSize = 0 }, Font = new Font("Segoe UI", 9, FontStyle.Bold), Margin = new Padding(0, 4, 8, 4), Padding = new Padding(12, 0, 12, 0) };

    public static Panel Busqueda(TextBox campo, EventHandler buscar, EventHandler limpiar)
    {
        Panel panel = new() { Dock = DockStyle.Top, Height = 48, Padding = new Padding(0, 4, 0, 6) };
        campo.Width = 260; campo.Dock = DockStyle.Left; campo.Margin = new Padding(0, 0, 8, 0); campo.PlaceholderText = "Buscar por nombre, código...";
        Button botonBuscar = Boton("Buscar / Filtrar"); botonBuscar.Dock = DockStyle.Left; botonBuscar.Click += buscar;
        Button botonLimpiar = Boton("Limpiar / Listar todos"); botonLimpiar.Dock = DockStyle.Left; botonLimpiar.Click += limpiar;
        panel.Controls.Add(botonLimpiar); panel.Controls.Add(botonBuscar); panel.Controls.Add(campo);
        return panel;
    }

    public static void ConfigurarGrid(DataGridView grid)
    {
        grid.Dock = DockStyle.Fill; grid.AutoGenerateColumns = false; grid.ReadOnly = true; grid.AllowUserToAddRows = false; grid.AllowUserToDeleteRows = false; grid.RowHeadersVisible = false; grid.BackgroundColor = Color.White; grid.BorderStyle = BorderStyle.FixedSingle; grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal; grid.GridColor = Borde; grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; grid.ColumnHeadersHeight = 38;
        grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle { BackColor = Azul, ForeColor = Color.White, Font = new Font("Segoe UI", 9, FontStyle.Bold), Alignment = DataGridViewContentAlignment.MiddleLeft };
        grid.DefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.White, ForeColor = Texto, SelectionBackColor = Color.FromArgb(218, 237, 255), SelectionForeColor = Texto, Padding = new Padding(6, 0, 6, 0) };
        grid.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(247, 250, 253) };
    }

    public static TableLayoutPanel Formulario(int filas)
    {
        TableLayoutPanel tabla = new() { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 4, RowCount = filas, Padding = new Padding(0, 8, 0, 8) };
        tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 92)); tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50)); tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 92)); tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        for (int indice = 0; indice < filas; indice++) tabla.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        return tabla;
    }

    public static int Codigo(string texto, int siguiente)
    {
        string numeros = new(texto.Where(char.IsDigit).ToArray());
        return int.TryParse(numeros, out int codigo) && codigo > 0 ? codigo : siguiente;
    }
}