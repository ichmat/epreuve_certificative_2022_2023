namespace FreshTech;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		Connect();
    }

	private async void Connect()
	{
		await Task.Delay(2000);
		await App.client.ConnexionStart();
	}
}
