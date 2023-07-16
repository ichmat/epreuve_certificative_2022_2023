namespace FreshTech.Pages;

using FreshTech.Tools;
using FreshTech.Views.Game;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;

public partial class GamePage : ContentPage
{
    private GameEngine _engine;

    private bool _is_edit_mode = false;
    private bool IsEditMode
    {
        get => _is_edit_mode;
        set
        {
            if(_is_edit_mode != value)
            {
                _is_edit_mode = value;
                if (_is_edit_mode)
                {
                    ButtonCurrentSituation.IsVisible = false;
                    ButtonEdit.IsVisible = false;
                    ButtonPlus.IsVisible = false;

                    ButtonValidate.IsVisible = true;
                }
                else
                {
                    ButtonCurrentSituation.IsVisible = true;
                    ButtonEdit.IsVisible = true;
                    ButtonPlus.IsVisible = true;

                    ButtonValidate.IsVisible = false;
                }
            }
        }
    }

    private bool _is_init = false;

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

    private void ContentPage_SizeChanged(object sender, EventArgs e)
    {
        if (!_is_init)
        {
            _is_init = true;
            ButtonValidate.IsVisible = false;
        }
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

    private async void GameMap_TappedCoord(int x, int y)
    {
        // afficher visuellement le clic de l'utilisateur
        ShowCaseClicked(x, y);
        gameMap.ReloadViewElement(); 

        if(IsEditMode)
        {
            if (_engine.TryGetBuildingAtThisCoord(x, y, out IConstruction? construction))
            {
                // temporaire 
                if(await DisplayAlert("Enlever bâtiment",
                    "Voulez-vous retirer ce bâtiment ?", "Oui", "Non"))
                {
                    if (await _engine.RemoveBuildingPlacement(construction!))
                    {
                        // retirer visuellement le bâtiment
                    }
                    else
                    {
                        _ = DisplayAlert("Erreur", "L'opération n'a pas pu être effectué", "Ok");
                    }
                }
            }
            else
            {
                // ajouter les bâtiments
            }
        }
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
        IsEditMode = !IsEditMode;
    }

    private async void ButtonPlus_Clicked()
    {
        await Navigation.PushModalAsync(new PlusPage(_engine));
    }

    private void ButtonValidate_Clicked(object sender, EventArgs e)
    {
        IsEditMode = !IsEditMode;
    }
}