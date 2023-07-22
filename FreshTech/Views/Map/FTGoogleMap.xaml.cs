using Microsoft.Maui.Controls;
using static FreshTech.Views.IMapEngine;

namespace FreshTech.Views.Map;

public partial class FTGoogleMap : ContentView
{
    private bool _is_location_available = false;

	public FTGoogleMap()
	{
		InitializeComponent();
    }

    private async Task<Location> GetLocation()
    {
        GeolocationRequest gr = new GeolocationRequest();
        gr.DesiredAccuracy = GeolocationAccuracy.Best;
        return await Geolocation.Default.GetLocationAsync(gr)!;
    }

}