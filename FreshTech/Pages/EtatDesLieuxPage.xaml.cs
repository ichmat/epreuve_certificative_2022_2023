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

    private readonly GameEngine _engine;

    public EtatDesLieuxPage(GameEngine gameEngine)
    {
        InitializeComponent();
        BindingContext = this;
        _engine = gameEngine;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        GetData();
    }

    private void GetData()
    {
        L_Eau.Title = _engine.GetRessourceQuantity(RESSOURCE.EAU).ToString();
        L_Nourriture.Title = _engine.GetRessourceQuantity(RESSOURCE.NOURRITURE).ToString();
        L_Bonheur.Title = _engine.GetRessourceQuantity(RESSOURCE.BONHEUR).ToString();
        L_Energie.Title = _engine.GetRessourceQuantity(RESSOURCE.ENERGIE).ToString();
        L_Bois.Title = _engine.GetRessourceQuantity(RESSOURCE.BOIS).ToString();
        L_Feraille.Title = _engine.GetRessourceQuantity(RESSOURCE.FERRAILLE).ToString();
        L_Habitant.Title = _engine.GetRessourceQuantity(RESSOURCE.HABITANT).ToString();
    }

    private async void TitleSpan_GoBack()
    {
        await Shell.Current.Navigation.PopAsync();
    }
}