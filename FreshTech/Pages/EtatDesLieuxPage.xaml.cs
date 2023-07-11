using AppCore.Models;
using AppCore.Services.GeneralMessage;
using AppCore.Services.GeneralMessage.Args;
using AppCore.Services.GeneralMessage.Response;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FreshTech.Pages;

public partial class EtatDesLieuxPage : ContentPage, INotifyPropertyChanged
{

    private int eau;
    public int Eau
    {
        get { return eau; }
        set
        {
            if (eau != value)
            {
                eau = value;
                OnPropertyChanged(nameof(Eau));
            }
        }
    }
    private int nourriture;
    public int Nourriture
    {
        get { return nourriture; }
        set
        {
            if (nourriture != value)
            {
                nourriture = value;
                OnPropertyChanged(nameof(Nourriture));
            }
        }
    }
    
    private int bonheur;
    public int Bonheur
    {
        get { return bonheur; }
        set
        {
            if (bonheur != value)
            {
                bonheur = value;
                OnPropertyChanged(nameof(Bonheur));
            }
        }
    }
    
    private int energie;
    public int Energie
    {
        get { return energie; }
        set
        {
            if (energie != value)
            {
                energie = value;
                OnPropertyChanged(nameof(Energie));
            }
        }
    }
    
    private int bois;
    public int Bois
    {
        get { return bois; }
        set
        {
            if (bois != value)
            {
                bois = value;
                OnPropertyChanged(nameof(Bois));
            }
        }
    }
    
    private int feraille;
    public int Feraille
    {
        get { return feraille; }
        set
        {
            if (feraille != value)
            {
                feraille = value;
                OnPropertyChanged(nameof(Feraille));
            }
        }
    }
    
    private int habitant;
    public int Habitant
    {
        get { return habitant; }
        set
        {
            if (habitant != value)
            {
                habitant = value;
                OnPropertyChanged(nameof(Habitant));
            }
        }
    }

    public int Temps { get; set; } = 42;
    public event PropertyChangedEventHandler PropertyChanged;
    public EtatDesLieuxPage()
    {
        InitializeComponent();
        BindingContext = this;
    }
    protected override  void OnAppearing()
    {
        base.OnAppearing();
         GetData();
    }
    private async void GetData()
    {
        ResponseGetEntireVillage dataTown = await App.client.SendAndGetResponse<ResponseGetEntireVillage>(new EPGetEntireVillage());
        var ressourcesPossedeEau = dataTown.RessourcesPossede.Select(x => x).Where(x => x.RessourceId == 1).First();
        var ressourcesPossedeNourriture = dataTown.RessourcesPossede.Select(x => x).Where(x => x.RessourceId == 2).First();
        var ressourcesPossedeBonheur = dataTown.RessourcesPossede.Select(x => x).Where(x => x.RessourceId == 3).First();
        var ressourcesPossedeEnergie = dataTown.RessourcesPossede.Select(x => x).Where(x => x.RessourceId == 4).First();
        var ressourcesPossedeBois = dataTown.RessourcesPossede.Select(x => x).Where(x => x.RessourceId == 5).First();
        var ressourcesPossedeFerraille = dataTown.RessourcesPossede.Select(x => x).Where(x => x.RessourceId == 6).First();
        var ressourcesPossedeHabitant = dataTown.RessourcesPossede.Select(x => x).Where(x => x.RessourceId == 7).First();

        Eau = ressourcesPossedeEau.Nombre;
        Nourriture = ressourcesPossedeNourriture.Nombre;
        Bonheur = ressourcesPossedeBonheur.Nombre;
        Energie = ressourcesPossedeEnergie.Nombre;
        Bois = ressourcesPossedeBois.Nombre;
        Feraille = ressourcesPossedeFerraille.Nombre;
        Habitant = ressourcesPossedeHabitant.Nombre;

        Console.WriteLine("");
    }
    
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private async void TitleSpan_GoBack()
    {
        await Shell.Current.Navigation.PopAsync();
    }
}