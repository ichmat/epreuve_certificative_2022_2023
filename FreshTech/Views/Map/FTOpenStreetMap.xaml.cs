using AppCore.Models;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.UI.Maui;
using NetTopologySuite.Geometries;
using static FreshTech.Views.IMapEngine;

namespace FreshTech.Views.Map;

public partial class FTOpenStreetMap : ContentView
{
    private readonly MapView map;
    private const string LineLayerName = "Line Layer";
    public FTOpenStreetMap()
	{
		InitializeComponent();
        map = new MapView();
        map.Map?.Layers.Add(Mapsui.Tiling.OpenStreetMap.CreateTileLayer());
        Content = map;
        
        ProcessCoordonate();
    }

    private async void ProcessCoordonate()
    {
        Microsoft.Maui.Devices.Sensors.Location location = await Geolocation.Default.GetLocationAsync();
        var position = new Mapsui.UI.Maui.Position(location.Latitude, location.Longitude);

        map.IsMyLocationButtonVisible = true;
        map.MyLocationLayer.UpdateMyLocation(position);
        map.MyLocationLayer.IsMoving = true;
        map.MyLocationEnabled = true;
        map.MyLocationFollow = true;


        var pin = new Pin
        {
            Position = position,
            Label = "custom pin",
            Address = "custom detail info",
        };

        map.Pins.Add(pin);


        var pin2 = new Pin
        {
            Position = new(0,0),
            Label = "custom pin",
            Address = "custom detail info",
        };

        map.Pins.Add(pin2);

        List<Pin> pins = new List<Pin>();
        pins.Add(pin2);
        pins.Add(pin);

       var testline =  CreateLine("tata", pins);
        var position2 = new Mapsui.UI.Maui.Position(location.Latitude, location.Longitude);
        map.Map.Layers.Add(CreateLineLayer(position.ToCoordinate(), new(0, 0)));

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
            Microsoft.Maui.Devices.Sensors.Location location = await Geolocation.Default.GetLocationAsync();

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
    private static GeometryFeature CreateLineFeature(Coordinate start, Coordinate end)
    {
        return new GeometryFeature
        {
            Geometry = CreateLine2(start, end),
            ["Name"] = "Line 1",
            Styles = new List<IStyle> { new VectorStyle { Line = new Pen(new Mapsui.Styles.Color(0, 0, 0), 6) } }
        };
    }
    private static LineString CreateLine2(Coordinate start, Coordinate end)
    {
        var offsetX = 44;
        var offsetY = 4;
        var stepSize = -2000000;

        return new LineString(new[]
        {
           start,
            end,

        });
    }
    private static ILayer CreateLineLayer(Coordinate start, Coordinate end)
    {
        return new MemoryLayer
        {
            Name = LineLayerName,
            Features = new[] { CreateLineFeature(start, end) },
            Style = null,
            IsMapInfoLayer = true
        };
    }
    private MemoryLayer CreateLine(String name, List<Pin> geoWaypoints)
    {
        var featureList = new List<IFeature>();
        var suggestions = new Coordinate[geoWaypoints.Count];
        int i = 0;
        foreach (var pin in geoWaypoints)
        {
            suggestions[i] = new Coordinate(pin.Position.Longitude, pin.Position.Latitude);
            i++;
        }

        return new MemoryLayer
        {
            Features = featureList,
            Name = name,
            Style = new VectorStyle { Line = new Pen(new Mapsui.Styles.Color(0, 0, 0), 6) } 
        };
    }


}