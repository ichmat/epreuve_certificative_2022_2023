namespace FreshTech.Pages;

public partial class ConfidentialitePage : ContentPage
{
	public ConfidentialitePage()
	{
		InitializeComponent();
	}
    private async void OnTitleLabelTapped(object sender, EventArgs e)
    {
        await Shell.Current.Navigation.PopAsync();
    }
}