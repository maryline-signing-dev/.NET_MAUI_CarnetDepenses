using System.Windows.Input;

namespace CarnetDepenses.ViewModels;

// Variante de RelayCommand qui reçoit un paramètre (ex: la dépense à supprimer)
public partial class RelayCommandP : ICommand
{
    private readonly Action<object?> _executer;
    private readonly Func<bool>? _peutExecuter;

    public RelayCommandP(Action<object?> executer,
                         Func<bool>? peutExecuter = null)
    {
        _executer = executer;
        _peutExecuter = peutExecuter;
    }

    public bool CanExecute(object? p)
        => _peutExecuter?.Invoke() ?? true;

    public void Execute(object? p) => _executer(p);

    public event EventHandler? CanExecuteChanged;

    public void Rafraichir()
        => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}