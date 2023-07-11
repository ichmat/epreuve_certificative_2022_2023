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

    private async void OnInventaireClicked(object sender, EventArgs e)
    {

    }
    
    private async void OnReparationClicked(object sender, EventArgs e)
    {

    }
    
    private async void OnConstructionclicked(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new ConstructionPage(_engine));
    }
    
    private async void OnAmeliorationClicked(object sender, EventArgs e)
    {


    }

    private async void TitleSpan_GoBack()
    {
        await Shell.Current.Navigation.PopAsync();
    }
}