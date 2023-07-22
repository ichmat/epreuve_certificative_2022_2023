namespace FreshTech.Pages;

public partial class LangagePage : ContentPage
{
	public LangagePage()
	{
		InitializeComponent();
	}

    private async void TitleSpan_GoBack()
    {
        await Shell.Current.Navigation.PopAsync();
    }
}