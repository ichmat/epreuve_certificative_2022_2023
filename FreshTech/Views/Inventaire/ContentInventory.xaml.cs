using AppCore.Models;
using FreshTech.Tools;
using FreshTech.Views.Game;
using System.Diagnostics.Metrics;

namespace FreshTech.Views.Inventaire;

public partial class ContentInventory : ContentView
{
	public Brush ColorShadow
	{
		get => mainBorderShadow.Brush;
		set => mainBorderShadow.Brush = value;
	}

    public string Title
    {
        get => L_Title.Text;
        set => L_Title.Text = value;
    }
        public string Number
    {
        get => L_Number.Text;
        set => L_Number.Text = value;
    }

    public ContentInventory(KeyValuePair<Objet,int> objectPossede)
	{
		InitializeComponent();
		CreateObjectCard(objectPossede);
	}
    public ContentInventory(KeyValuePair<int, List<IConstruction>> buildingPossede)
    {
        InitializeComponent();
        CreateBuildingCard(buildingPossede);
    }

    public void CreateObjectCard(KeyValuePair<Objet, int> objectPossede)
	{

        Title = objectPossede.Key.Nom;

        switch (objectPossede.Key.Rarete)
        {
            case AppCore.Property.TypeRarete.COMMUN:
                break;

            case AppCore.Property.TypeRarete.RARE:
                ColorShadow = ColorsTools.RareBrush;
                break;
            case AppCore.Property.TypeRarete.EPIC:
                ColorShadow = ColorsTools.EpicBrush;
                break;
            case AppCore.Property.TypeRarete.LEGENDAIRE:
                ColorShadow = ColorsTools.LegendaryBrush;
                break;

            default:
                break;
        }
        Number = objectPossede.Value.ToString();
    }
    
    public void CreateBuildingCard(KeyValuePair<int, List<IConstruction>> buildingPossede)
	{
        Title = buildingPossede.Value.First().GetConstructionName();
        Number = buildingPossede.Key.ToString();
    }
}