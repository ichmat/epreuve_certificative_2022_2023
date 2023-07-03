using AppCore.Services;
using FreshTech.Views;
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

        // remove border from BorderlessEntry
        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(nameof(BorderlessEntry), (handler, view) =>
		{
			if(view is BorderlessEntry)
			{
#if __ANDROID__
                handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);

#elif __IOS__
			handler.PlatformView.BackgroundColor = UIKit.UIColor.Clear;
			handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
#endif
            }
        });
	}


}
