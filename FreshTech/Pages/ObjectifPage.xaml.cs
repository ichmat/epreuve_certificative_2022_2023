namespace FreshTech.Pages;

public partial class ObjectifPage : ContentPage
{
	public ObjectifPage()
	{
		InitializeComponent();
	}
    private async void OnTitleLabelTapped(object sender, EventArgs e)
    {
        // Ajoutez ici la logique de navigation pour revenir au menu pr�c�dent
        await Shell.Current.Navigation.PopAsync();

    }
}