namespace FreshTech.Pages;

public partial class ComptePage : ContentPage
{
    public ComptePage()
	{
		InitializeComponent();
	}
    private async void OnTitleLabelTapped(object sender, EventArgs e)
    {
        await Shell.Current.Navigation.PopAsync();
    }

    private void OnDeconnecterTapped(object sender, EventArgs e)
    {
         DisplayAlert("click", "decon", "canceml");
    }
    private void OnFacebookTapped(object sender, EventArgs e)
    {
        DisplayAlert("click", "facebook", "canceml");
    } 
    private void OnGoogleTapped(object sender, EventArgs e)
    {
        DisplayAlert("click", "google", "canceml");
    }
    private void OnChangerClicked(object sender, EventArgs e)
    {
        DisplayAlert("click", "Change", "canceml");
    }
}