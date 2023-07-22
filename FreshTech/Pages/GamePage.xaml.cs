namespace FreshTech.Pages;

using AppCore.Models;
using FreshTech.Tools;
using FreshTech.Views.Game;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;

public partial class GamePage : ContentPage
{
    private static GameEngine _engine;

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
                    HideBorderToPlaceBuildings();
                }
            }
        }
    }
    public string AttaqueLabel
    {
        get => L_AttaqueLabel.Text;
        set => L_AttaqueLabel.Text = value;
    }

    private bool _is_init = false;
    private bool _request_loading = false;

    private int x_to_place_new_building = -1;
    private int y_to_place_new_building = -1;

    public GamePage()
	{
        InitializeComponent();
        // on demande à la carte de ce centrer
        gameMap.CenterMap();
        // on attend que la carte finisse de se construire, voir la méthode GameMap_FinishingLoaded
        // pour la suite
        gameMap.FinishingLoaded += GameMap_FinishingLoaded;
        _engine = new GameEngine();
        StartLoading();

    }

    #region STATIC

    public static GameEngine GetGameEngine()
    {
        return _engine;
    }

    public void GetAttaque()
    {
      var attaques = _engine.GetAttaques().First();
        var tempRestant = (attaques.DateApparition - DateTime.Now);
        AttaqueLabel = "Prochaine attaque arrive dans : " + tempRestant.Hours.ToString() + " hrs " + tempRestant.Minutes.ToString() + " mins ";
        
    }
    #endregion

    #region INIT

    private async void GameMap_FinishingLoaded()
    {
        // on recharge toutes les données du jeux
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
        GetAttaque();
        // on charge le placement de tout les bâtiments
        LoadPlacedBuildings();
        StopLoading();
        gameMap.TappedCoord += GameMap_TappedCoord;
    }

    private void LoadPlacedBuildings()
    {
        foreach(KeyValuePair<IConstruction, Placement?> dataBuilding in _engine.GetBuildingsInMap())
        {
            Placement? coord = dataBuilding.Value;
            if(coord != null)
            {
                AddView(dataBuilding.Key, coord.Value.X, coord.Value.Y);
            }
            else
            {
                throw new ArgumentNullException("Coord de la construction ne peut pas être null", "Placement");
            }
        }

        gameMap.ReloadViewElement();
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
            BorderBuildingsNotInMap.IsVisible = false;
            if (_request_loading)
            {
                StartLoading();
            }
            else
            {
                StopLoading();
            }
        }
    }

    #endregion

    #region VIEW

    private void AddView(IConstruction construction, int x, int y)
    {
        ElementCase visualBuilding = new ElementCase(construction);
        visualBuilding.ZIndex = 100;
        gameMap.AddElement(visualBuilding, x, y);
    }

    private void RemoveViewIfExist(IConstruction construction)
    {
        gameMap.RemoveElement(x =>
            x is ElementCase el &&
            el.Construction.GetConsId() == construction.GetConsId());
    }

    private void ShowBorderToPlaceBuildings(int x, int y)
    {
        if (IsEditMode)
        {
            // sauvegarde temporairement les coordonnées où il faudra placer le bâtiment
            x_to_place_new_building = x;
            y_to_place_new_building = y;
            BorderBuildingsNotInMap.IsVisible = true;
            // variable pour savoir si c'est la première vue créer
            bool isFirst = true;
            // affichage des constructions qui ne sont pas sur la carte
            // TODO : normalement, les bâtiments doivent s'empiler si elles sont du même type et avec le même état
            // voir : https://xd.adobe.com/view/4fc91b5c-3f6a-48d4-b95a-c6c58a7e0c13-1a9a/screen/1f10550a-d0bc-47ce-8707-30149c9be87b
            foreach (IConstruction construction in _engine.GetBuildingsNotInMap())
            {
                ElementCase el = new ElementCase(construction);
                el.Clicked += BuildingToAdd_Clicked;
                HSL_BuildingsToPlace.Children.Add(el);
                el.WidthRequest = el.HeightRequest = 50;
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    el.Margin = new Thickness(10, 0, 0, 0);
                }
            }
            gameMap.IsEnabled = false;
        }
    }

    private void HideBorderToPlaceBuildings()
    {
        BorderBuildingsNotInMap.IsVisible = false;
        foreach (ElementCase element in HSL_BuildingsToPlace.Children)
        {
            element.Clicked -= BuildingToAdd_Clicked;
        }
        HSL_BuildingsToPlace.Children.Clear();
        gameMap.IsEnabled = true;
    }

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
            rect.ZIndex = 0;
        }
        gameMap.AddElement(rect, x, y);
    }

    #endregion

    #endregion

    #region TAPPED_EVENT

    private async void GameMap_TappedCoord(int x, int y)
    {
        // afficher visuellement le clic de l'utilisateur
        ShowCaseClicked(x, y);
        gameMap.ReloadViewElement();

        if (IsEditMode)
        {
            await EditModeTapped(x, y);
        }
        else
        {
            //NormalModeTapped(x, y);
        }

    }

    private async Task EditModeTapped(int x, int y)
    {
        if (_engine.TryGetBuildingAtThisCoord(x, y, out IConstruction? construction) && construction != null)
        {
            // TODO : fonctionnement temporaire, il faut afficher une bulle sur le bâtiment sélectionner
            // voir : https://xd.adobe.com/view/4fc91b5c-3f6a-48d4-b95a-c6c58a7e0c13-1a9a/screen/9d466bb3-e9e2-4250-a21b-f4f523b39c8b
            if (await DisplayAlert("Enlever bâtiment",
                "Voulez-vous retirer ce bâtiment ?", "Oui", "Non"))
            {
                if (await _engine.RemoveBuildingPlacement(construction))
                {
                    RemoveViewIfExist(construction);
                    HideCaseClicked();
                    gameMap.ReloadViewElement();
                }
                else
                {
                    _ = DisplayAlert("Erreur", "L'opération n'a pas pu être effectué", "Ok");
                }
            }
        }
        else
        {
            ShowBorderToPlaceBuildings(x, y);
        }
    }

    private void NormalModeTapped(int x, int y)
    {
        // TODO : mettre en place le système de visualisation du bâtiment sélectionnée
    }

    #endregion

    #region LOADING

    internal void StartLoading()
    {
        if(_is_init)
        {
            AI_Loading.IsRunning = true;
            BorderAI.IsVisible = true;
        }
        else
        {
            _request_loading = true;
        }
    }

    internal void StopLoading()
    {
        if (_is_init)
        {
            AI_Loading.IsRunning = false;
            BorderAI.IsVisible = false;
        }
        else
        {
            _request_loading = false;
        }
    }

    #endregion

    #region EVENT

    private async void BuildingToAdd_Clicked(ElementCase clicked)
    {
        HideBorderToPlaceBuildings();
        StartLoading();
        if(await _engine.SetBuildingPlacement(clicked.Construction,
            x_to_place_new_building,
            y_to_place_new_building))
        {
            AddView(clicked.Construction, x_to_place_new_building, y_to_place_new_building);
            gameMap.ReloadViewElement();
        }
        else
        {
            _ = DisplayAlert("Erreur", "L'opération n'a pas pu être effectué", "Ok");
        }
        StopLoading();
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

    private void LabelCancelPlaceBuilding_Tapped(object sender, TappedEventArgs e)
    {
        HideBorderToPlaceBuildings();
        HideCaseClicked();
        gameMap.ReloadViewElement();
    }

    #endregion
}