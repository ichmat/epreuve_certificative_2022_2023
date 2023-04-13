using AppCore.Services;
using Mapsui.Providers.Wms;

namespace FreshTech;

public partial class App : Application
{
	public static FTMClientManager client;

	public App()
	{
		InitializeComponent();
        client = new FTMClientManager();
        MainPage = new AppShell();
	}
}
