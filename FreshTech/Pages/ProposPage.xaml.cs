namespace FreshTech.Pages;

public partial class ProposPage : ContentPage
{
	public ProposPage()
	{
		InitializeComponent();
	}
    private async void OnTitleLabelTapped(object sender, EventArgs e)
    {
        // Ajoutez ici la logique de navigation pour revenir au menu précédent
        await Shell.Current.Navigation.PopAsync();
    }
    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.Navigation.PopAsync();

    }
}