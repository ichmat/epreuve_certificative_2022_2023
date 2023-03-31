using FreshTech.Pages;

namespace FreshTech;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();

	}

	private void OnLoginClicked(object sender, EventArgs e)
	{
		Navigation.PushModalAsync(new LoginPage());
	}
}

