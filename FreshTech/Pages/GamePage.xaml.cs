namespace FreshTech.Pages;

using Microsoft.Maui.Controls;

public partial class GamePage : ContentPage
{
    public GamePage()
	{
        InitializeComponent();
        gameMap.CenterMap();
    }
}