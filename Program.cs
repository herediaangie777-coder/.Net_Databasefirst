using System.Windows.Forms;
using ProyectoConsolaObjetos1.UI;

namespace ProyectoConsolaObjetos1;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }
}
