using AppCore.Models;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.UI.Maui;
using Microsoft.Maui.Controls.Shapes;
using NetTopologySuite.Algorithm;
using NetTopologySuite.Geometries;
using NetTopologySuite.GeometriesGraph;
using static FreshTech.Views.IMapEngine;

namespace FreshTech.Views.Map;

public partial class FTOpenStreetMap : ContentView, IMapEngine
{
    private readonly MapView map;
    private bool first_init_location_user = true;
    private MemoryLayer _lines;

    private Coordinate? _last_coordinate = null;

    public FTOpenStreetMap()
	{
		InitializeComponent();
        map = new MapView();
        map.Map?.Layers.Add(Mapsui.Tiling.OpenStreetMap.CreateTileLayer());
        Content = map;
        _lines = CreateLineLayer();
        map.Map.Layers.Add(_lines);
        map.MyLocationEnabled = false;
        map.MyLocationFollow = false;
        map.IsMyLocationButtonVisible = false;
    }

    public void Dispose() => map.Dispose();

    public void AddLine(Microsoft.Maui.Devices.Sensors.Location location)
    {
        Mapsui.UI.Maui.Position position = new Mapsui.UI.Maui.Position(location.Latitude, location.Longitude);
        if(_last_coordinate == null)
        {
            _last_coordinate = position.ToCoordinate();
        }
        else
        {
            ((List<IFeature>)_lines.Features).Add(CreateLineFeature(_last_coordinate, position.ToCoordinate()));
            _last_coordinate = position.ToCoordinate();
        }
    }

    public void UpdateUserLocation(Microsoft.Maui.Devices.Sensors.Location location)
    {
        if (first_init_location_user)
        {
            first_init_location_user = false;
            map.MyLocationEnabled = true;
            map.MyLocationFollow = true;
            map.IsMyLocationButtonVisible = true;
        }
        var position = new Mapsui.UI.Maui.Position(location.Latitude, location.Longitude);

        map.MyLocationLayer.UpdateMyLocation(position);
    }

    public void MooveScreenTo(Microsoft.Maui.Devices.Sensors.Location to, double? zoomMetersAccuracy = null)
    {
        Mapsui.UI.Maui.Position position = new Mapsui.UI.Maui.Position(to.Latitude, to.Longitude);
        MPoint p = position.ToMapsui();
        if(zoomMetersAccuracy == null)
        {
            map.Navigator.NavigateTo(p,50);
        }
        else
        {
            // TODO
        }
    }

    public void CutLine()
    {
        _last_coordinate = null;
        _lines = CreateLineLayer();
        map.Map.Layers.Add(_lines);
    }

    public View GetMapView() => this;

    private void AddPin(Mapsui.UI.Maui.Position position)
    {
        var pin = new Pin
        {
            Position = position,
            Label = "custom pin",
            Address = "custom detail info",
        };

        map.Pins.Add(pin);
    }

    private static GeometryFeature CreateLineFeature(Coordinate start, Coordinate end)
    {
        return new GeometryFeature
        {
            Geometry = CreateLine(start, end),
            Styles = new List<IStyle> { new VectorStyle { Line = new Pen(new Mapsui.Styles.Color(0, 0, 0), 3) } }
        };
    }

    private static GeometryFeature CreateLineFeature(Coordinate[] coords)
    {
        return new GeometryFeature
        {
            Geometry = CreateLine(coords),
            Styles = new List<IStyle> { new VectorStyle { Line = new Pen(new Mapsui.Styles.Color(0, 0, 0), 3) } }
        };
    }

    private static LineString CreateLine(Coordinate start, Coordinate end)
    {
        return new LineString(new[]
        {
            start,
            end,
        });
    }

    private static LineString CreateLine(Coordinate[] coords)
    {
        return new LineString(coords);
    }

    private static MemoryLayer CreateLineLayer()
    {
        return new MemoryLayer
        {
            Name = null,
            Features = new List<IFeature>(),
            Style = null,
            IsMapInfoLayer = true
        };
    }

    private static MemoryLayer CreateLineLayer(params Coordinate[] coords)
    {
        return new MemoryLayer
        {
            Name = null,
            Features = new[] { CreateLineFeature(coords) },
            Style = null,
            IsMapInfoLayer = true
        };
    }

    private MemoryLayer CreateLineWithPins(String name, List<Pin> geoWaypoints)
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