namespace FreshTech.Pages;

public partial class ComptePage : ContentPage
{
    public ComptePage()
	{
		InitializeComponent();
	}

    private void OnDeconnecterTapped(object sender, EventArgs e)
    {
         DisplayAlert("click", "decon", "ok");
    }
    private void OnFacebookTapped(object sender, EventArgs e)
    {
        DisplayAlert("click", "facebook", "ok");
    } 
    private void OnGoogleTapped(object sender, EventArgs e)
    {
        DisplayAlert("click", "google", "ok");
    }
    private void OnChangerClicked(object sender, EventArgs e)
    {
        DisplayAlert("click", "Change", "ok");
    }

    private async void TitleSpan_GoBack()
    {
        await Shell.Current.Navigation.PopAsync();
    }
}