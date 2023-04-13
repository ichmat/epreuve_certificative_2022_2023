using AppCore.Services;

namespace FreshTech;

public partial class App : Application
{
	public static FTMClientManager client;

	public App()
	{
		InitializeComponent();
		client = new FTMClientManager();
		client.ConnexionStart().Wait();
        MainPage = new AppShell();
	}
}
