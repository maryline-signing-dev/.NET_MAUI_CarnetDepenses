using System.Collections.ObjectModel;
using CarnetDepenses.Models;
using CarnetDepenses.Services;

namespace CarnetDepenses.ViewModels;

public class DepensesViewModel : BaseViewModel
{
    private readonly IDepenseRepository _repo;
    private string _libelle = "";
    private string _montantTexte = "";

    public ObservableCollection<Depense> Depenses { get; } = new();
    public decimal Total => Depenses.Sum(d => d.Montant);

    public string Libelle
    {
        get => _libelle;
        set
        {
            if (SetField(ref _libelle, value))
                AjouterCommand.Rafraichir();
        }
    }

    public string MontantTexte
    {
        get => _montantTexte;
        set
        {
            if (SetField(ref _montantTexte, value))
                AjouterCommand.Rafraichir();
        }
    }

    // Le repository est INJECTÉ par le conteneur (voir A6)
    public RelayCommand AjouterCommand { get; }
    public RelayCommandP SupprimerCommand { get; }

    public DepensesViewModel(IDepenseRepository repo)
    {
        _repo = repo;
        AjouterCommand = new RelayCommand(
            async () => await AjouterAsync(),
            () => !string.IsNullOrWhiteSpace(Libelle)
               && !string.IsNullOrWhiteSpace(MontantTexte));

        SupprimerCommand = new RelayCommandP(
            async p => await SupprimerAsync((Depense)p));
    }

    // Relit le stockage et remplit la liste affichée
    public async Task ChargerAsync()
    {
        Depenses.Clear();
        foreach (var d in await _repo.ListerAsync())
            Depenses.Add(d);
        OnPropertyChanged(nameof(Total));
    }

    private async Task AjouterAsync()
    {
        if (!decimal.TryParse(MontantTexte, out decimal m))
            return;
        // 1. Crée la Depense  // 2. la sauvegarde
        var d = new Depense { Libelle = Libelle, Montant = m };
        await _repo.AjouterAsync(d);
        await ChargerAsync();
        Libelle = MontantTexte = "";
    }

    private async Task SupprimerAsync(Depense d)
    {
        Depenses.Remove(d);
        await _repo.SupprimerAsync(d);
        OnPropertyChanged(nameof(Total));
    }
}