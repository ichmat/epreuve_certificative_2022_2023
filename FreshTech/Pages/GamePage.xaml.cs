namespace FreshTech.Pages;

using FreshTech.Views.Game;
using Microsoft.Maui.Controls;

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

    private async void GameMap_FinishingLoaded()
    {
        await _engine.ReloadAllData();
        StopLoading();
        gameMap.TappedCoord += GameMap_TappedCoord;
    }

    private void GameMap_TappedCoord(int x, int y)
    {
    }

    private void ContentPage_Unloaded(object sender, EventArgs e)
    {
        gameMap.FinishingLoaded -= GameMap_FinishingLoaded;
        gameMap.TappedCoord -= GameMap_TappedCoord;
    }

    internal void StartLoading()
    {
        AI_Loading.IsRunning = true;
    }

    internal void StopLoading()
    {
        AI_Loading.IsRunning = false;
    }
}