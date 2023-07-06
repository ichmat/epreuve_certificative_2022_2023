using BruTile.Wmts.Generated;

namespace FreshTech.Pages;

public partial class ObjectifPage : ContentPage
{

    public int Distance { get; set; } = 42;
    public int Temps { get; set; } = 42;
    public ObjectifPage()
	{
		InitializeComponent();
        BindingContext = this;
    }
    private async void OnTitleLabelTapped(object sender, EventArgs e)
    {
        // Ajoutez ici la logique de navigation pour revenir au menu précédent
        await Shell.Current.Navigation.PopAsync();

    }

}