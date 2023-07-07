namespace FreshTech.Pages;

public partial class LangagePage : ContentPage
{
	public LangagePage()
	{
		InitializeComponent();
	}
    private async void OnTitleLabelTapped(object sender, EventArgs e)
    {
        // Ajoutez ici la logique de navigation pour revenir au menu précédent
        await Shell.Current.Navigation.PopAsync();
        DisplayAlert("click", "OnproposClicked", "canceml");
    }
}