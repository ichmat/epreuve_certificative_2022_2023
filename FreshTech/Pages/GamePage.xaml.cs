namespace FreshTech.Pages;

using FreshTech.Tools;
using FreshTech.Views.Game;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;

public partial class GamePage : ContentPage
{
    private GameEngine _engine;

    public GamePage()
	{
        InitializeComponent();
        gameMap.CenterMap();
        gameMap.FinishingLoaded += GameMap_FinishingLoaded;
        _engine = new GameEngine();
        StartLoading();
    }

    #region INIT

    private async void GameMap_FinishingLoaded()
    {
        await _engine.ReloadAllData();
        if (_engine.TownNotCreated)
        {
            // normalement quand l'utilisateur n'a pas encore de village dans la BDD, il faut lui 
            // afficher un tutoriel pour qu'il comprenne comment marche le village. Une fois 
            // le tuto terminé, il faut créer le village.
            if(!await _engine.CreateUserVillage())
            {
                // une erreur est survenu, pour éviter tout problème supplémentaire, on arrête l'application.
                await DisplayAlert("erreur", "Une erreur est survenu pendant le processus de création du village. Veuillez-vous reconnecter.", "Ok");
                App.Current.Quit();
            }
            else
            {
                await _engine.ReloadUserTown();
            }
        }
        StopLoading();
        gameMap.TappedCoord += GameMap_TappedCoord;
    }

    private void ContentPage_Unloaded(object sender, EventArgs e)
    {
        gameMap.FinishingLoaded -= GameMap_FinishingLoaded;
        gameMap.TappedCoord -= GameMap_TappedCoord;
    }

    #endregion

    #region VISUAL_CASE_CLICKED

    private Rectangle rect;

    private void HideCaseClicked()
    {
        if (rect != null)
        {
            gameMap.RemoveElement(rect);
        }
    }

    private void ShowCaseClicked(int x, int y)
    {
        if (rect != null)
        {
            HideCaseClicked();
        }
        else
        {
            rect = new Rectangle();
            rect.VerticalOptions = LayoutOptions.Fill;
            rect.HorizontalOptions = LayoutOptions.Fill;
            rect.BackgroundColor = ColorsTools.Primary;
            rect.Opacity = 0.4;
        }
        gameMap.AddElement(rect, x, y);
    }

    #endregion

    private void GameMap_TappedCoord(int x, int y)
    {
        // afficher visuellement le click de l'utilisateur
        ShowCaseClicked(x, y);
        gameMap.ReloadViewElement();
    }

    internal void StartLoading()
    {
        AI_Loading.IsRunning = true;
    }

    internal void StopLoading()
    {
        AI_Loading.IsRunning = false;
    }

    private async void ButtonCurrentSituation_Clicked()
    {
        await Navigation.PushModalAsync(new EtatDesLieuxPage(_engine));
    }

    private void ButtonEdit_Clicked()
    {

    }

    private async void ButtonPlus_Clicked()
    {
        await Navigation.PushModalAsync(new PlusPage(_engine));
    }
}