using AppCore.Models;
using FreshTech.Views.Game;
using FreshTech.Views.Inventaire;

namespace FreshTech.Pages;

public partial class InventairePage : ContentPage
{
    private readonly GameEngine _engine;

   
    public InventairePage(GameEngine gameEngine)
	{
        InitializeComponent();
        _engine = gameEngine;
    }

    private void GetData()
    {
        var objetPossedete = _engine.GetObjetsWithQuantity();
        var batimentsPossede = _engine.GetBuildingsNotInMapOrderByConsInfoId();
        ContentInventory card;

        foreach(var t in objetPossedete)
        {
            if(t.Value != 0)
            {
            card = new ContentInventory(t);
            FL_Objets.Children.Add(card);
            }
        } 
        foreach(var bat in batimentsPossede)
        {
            if (bat.Key != 0)
            {
                card = new ContentInventory(bat);
                FL_Batiments.Children.Add(card);
            }
        }
    }

    private async void TitleSpan_GoBack()
    {
        await Shell.Current.Navigation.PopAsync();
    }

    private void ContentPage_Loaded(object sender, EventArgs e)
    {
        Dispatcher.Dispatch(GetData);
    }
}