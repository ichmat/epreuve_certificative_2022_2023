namespace FreshTech.Pages;

using Microsoft.Maui.Controls;

public partial class GamePage : ContentPage
{
    public GamePage()
	{
        InitializeComponent();
        gameMap.CenterMap();
        gameMap.FinishingLoaded += GameMap_FinishingLoaded;
        StartLoading();
    }

    private void GameMap_FinishingLoaded()
    {
        StopLoading();
    }

    private void ContentPage_Unloaded(object sender, EventArgs e)
    {
        gameMap.FinishingLoaded -= GameMap_FinishingLoaded;
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