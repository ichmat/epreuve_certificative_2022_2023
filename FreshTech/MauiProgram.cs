using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace FreshTech;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
                //fonts.AddFont("OpenSans-Regular.ttf", "TwCen");
                //fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("TwCenClassMTStd-Regular.otf", "TwCenRegular");
                fonts.AddFont("TwCenMTStd.otf", "TwCen");
                fonts.AddFont("TwCenMTStd-Bold.otf", "TwCenBold");
                fonts.AddFont("TwCenMTStd-BoldCond.otf", "TwCenBoldCond");
                fonts.AddFont("TwCenMTStd-ExtraBold.otf", "TwCenExtraBold");
                fonts.AddFont("TwCenMTStd-ExtraBoldCond.otf", "TwCenExtraBoldCond");
                fonts.AddFont("TwCenMTStd-Italic.otf", "TwCenItalic");
                fonts.AddFont("TwCenMTStd-Light.otf", "TwCenLight");
                fonts.AddFont("TwCenMTStd-LightItalic.otf", "TwCenLightItalic");
                fonts.AddFont("TwCenMTStd-MediumCond.otf", "TwCenMediumCond");
                fonts.AddFont("TwCenMTStd-SemiMedium.otf", "TwCenSemiMedium");
                fonts.AddFont("TwCenMTStd-UltraBold.otf", "TwCenUltraBold");
                fonts.AddFont("TwCenMTStd-UltraBoldCond.otf", "TwCenUltraBoldCond");
                fonts.AddFont("TwCenMTStd-UltraBoldIt.otf", "TwCenUltraBoldIt");
            }).UseMauiMaps()
            .UseSkiaSharp(true);

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
