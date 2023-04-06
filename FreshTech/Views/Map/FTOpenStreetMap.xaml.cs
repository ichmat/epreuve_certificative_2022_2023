using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Styles;
using Mapsui.Tiling.Layers;
using Mapsui.UI.Maui;
using NetTopologySuite.Geometries;
using static FreshTech.Views.IMapEngine;

namespace FreshTech.Views.Map;

public partial class FTOpenStreetMap : ContentView
{
    private readonly MapView map;

    public FTOpenStreetMap()
	{
		InitializeComponent();
        map = new MapView();
        map.Map?.Layers.Add(Mapsui.Tiling.OpenStreetMap.CreateTileLayer());
        Content = map;
        test();
    }

    private async void test()
    {
        Microsoft.Maui.Devices.Sensors.Location l = await GetLocation();
        Pin pin = new Pin(map);
        map.IsMyLocationButtonVisible = true;
        map.MyLocationEnabled = true;
        map.MyLocationFollow = true;
        pin.Position = new Mapsui.UI.Maui.Position(l.Latitude, l.Longitude);
        Microsoft.Maui.Devices.Sensors.Location l2 = await GetLocation();
    }

    private async Task<Microsoft.Maui.Devices.Sensors.Location> GetLocation()
    {
        GeolocationRequest gr = new GeolocationRequest();
        gr.DesiredAccuracy = GeolocationAccuracy.Best;
        return await Geolocation.Default.GetLocationAsync(gr)!;
    }

    public async Task<LocalisationError> CheckLocationAvailable()
    {
        try
        {
            Microsoft.Maui.Devices.Sensors.Location location = await Geolocation.Default.GetLastKnownLocationAsync();

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

    private static ILayer CreateLineLayer(Coordinate start, Coordinate end)
    {
        return new MemoryLayer
        {
            Features = new[] { CreateLineFeature(start, end) },
        };
    }

    private static GeometryFeature CreateLineFeature(Coordinate start, Coordinate end)
    {
        return new GeometryFeature
        {
            Geometry = CreateLine(start, end),
            Styles = new List<IStyle> { new VectorStyle { Line = new Pen(new Mapsui.Styles.Color(0,0,0), 6) } }
        };
    }

    private static LineString CreateLine(Coordinate start, Coordinate end)
    {
        return new LineString(new[]
        {
            start,
            end
        });
    }
}