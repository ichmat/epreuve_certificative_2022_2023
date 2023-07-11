using AppCore.Models;
using AppCore.Services;
using AppCore.Services.GeneralMessage;
using AppCore.Services.GeneralMessage.Args;
using AppCore.Services.GeneralMessage.Response;
using FreshTech.Views.Game;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FreshTech.Pages;

public partial class EtatDesLieuxPage : ContentPage, INotifyPropertyChanged
{
    #region PROPERTY

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

    #endregion

    public event PropertyChangedEventHandler PropertyChanged;

    private readonly GameEngine _engine;

    public EtatDesLieuxPage(GameEngine gameEngine)
    {
        InitializeComponent();
        BindingContext = this;
        _engine = gameEngine;
    }

    protected override  void OnAppearing()
    {
        base.OnAppearing();
        GetData();
    }

    private void GetData()
    {
        Eau = _engine.GetRessourceQuantity(RESSOURCE.EAU);
        Nourriture = _engine.GetRessourceQuantity(RESSOURCE.NOURRITURE);
        Bonheur = _engine.GetRessourceQuantity(RESSOURCE.BONHEUR);
        Energie = _engine.GetRessourceQuantity(RESSOURCE.ENERGIE);
        Bois = _engine.GetRessourceQuantity(RESSOURCE.BOIS);
        Feraille = _engine.GetRessourceQuantity(RESSOURCE.FERRAILLE);
        Habitant = _engine.GetRessourceQuantity(RESSOURCE.HABITANT);
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