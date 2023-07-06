using AppCore.Models;
using AppCore.Services.GeneralMessage;
using AppCore.Services.GeneralMessage.Args;

namespace FreshTech.Pages;

public partial class EtatDesLieuxPage : ContentPage
{

    public int Distance { get; set; } = 42;
    public int Temps { get; set; } = 42;
    public EtatDesLieuxPage()
    {
        GetData();
        InitializeComponent();
        BindingContext = this;
    }
    private async void GetData()
    {
        base.OnAppearing();
        var stat = await App.client.SendAndGetResponse<Village>(new EPGetEntireVillage());
    }
    private async void OnTitleLabelTapped(object sender, EventArgs e)
    {
        // Ajoutez ici la logique de navigation pour revenir au menu précédent
        await Shell.Current.Navigation.PopAsync();

    }
}