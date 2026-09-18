namespace ProyectoConsolaObjetos1.UI.Controls;

public sealed class ClientesControl : UsuariosControl
{
    public ClientesControl(AppState appState) : base(appState, AppState.RolCliente, "Clientes", false)
    {
    }
}
