namespace FreshTech.Pages;

public partial class ProposPage : ContentPage
{
	public ProposPage()
	{
		InitializeComponent();
	}

    private async void TitleSpan_GoBack()
    {
        await Shell.Current.Navigation.PopAsync();
    }
}