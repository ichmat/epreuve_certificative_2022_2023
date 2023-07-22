namespace FreshTech.Pages;

public partial class ConfidentialitePage : ContentPage
{
	public ConfidentialitePage()
	{
		InitializeComponent();
	}

    private async void TitleSpan_GoBack()
    {
        await Shell.Current.Navigation.PopAsync();
    }
}