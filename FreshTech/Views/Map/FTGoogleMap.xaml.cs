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

    public async Task<LocalisationError> CheckLocationAvailable()
    {
        try
        {
            Location location = await Geolocation.Default.GetLastKnownLocationAsync();

            if (location != null)
                return LocalisationError.None;
            else
                return LocalisationError.Unknown;
        }
        catch (FeatureNotSupportedException fnsEx)
        {
            // Handle not supported on device exception
            return LocalisationError.NotSupported;
        }
        catch (FeatureNotEnabledException fneEx)
        {
            // Handle not enabled on device exception
            return LocalisationError.NotEnabled;
        }
        catch (PermissionException pEx)
        {
            // Handle permission exception
            return LocalisationError.NeedPermission;
        }
        catch (Exception ex)
        {
            // Unable to get location
            return LocalisationError.Unknown;
        }
    }
}