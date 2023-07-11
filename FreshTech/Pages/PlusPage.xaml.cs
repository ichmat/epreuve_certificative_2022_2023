using FreshTech.Views.Game;

namespace FreshTech.Pages;

public partial class PlusPage : ContentPage
{
    private readonly GameEngine _engine;

    public PlusPage(GameEngine gameEngine)
	{
		InitializeComponent();
        _engine = gameEngine;
	}

    private async void TitleSpan_GoBack()
    {
        await Shell.Current.Navigation.PopAsync();
    }

    private async void Inventaire_Clicked()
    {

    }

    private async void Reparation_Clicked()
    {

    }

    private async void Construction_Clicked()
    {
        await Navigation.PushModalAsync(new ConstructionPage(_engine));
    }

    private async void Amelioration_Clicked()
    {

    }
}