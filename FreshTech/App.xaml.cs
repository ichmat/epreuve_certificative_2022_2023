using AppCore.Services;
using FreshTech.Pages;

namespace FreshTech;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

        //MainPage = new MainPage();
       MainPage = new AppShell();
	}
}
