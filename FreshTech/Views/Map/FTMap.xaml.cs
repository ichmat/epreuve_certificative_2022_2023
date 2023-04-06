using Microsoft.Maui.Devices.Sensors;
using System.Timers;

namespace FreshTech.Views.Map;

public partial class FTMap : ContentView, IDisposable
{
    private readonly IMapEngine _engine;

    private readonly List<MapPoint[]> _points = new List<MapPoint[]>();

    private List<MapPoint> _actual_points = new List<MapPoint>();

    private readonly GeolocationRequest _accuracy;
    private readonly System.Timers.Timer _timer;

    private const int MILLISEC_TRACK = 4000;
    private const double METERS_ACCURACY_NEED_CALIBRATION = 50;
    private const double METERS_DISTANCE_PICK = 10;

    public FTMap()
	{
		InitializeComponent();
        _engine = new FTOpenStreetMap();
        Content = _engine.GetMapView();
        _accuracy = new GeolocationRequest();
        _timer = new System.Timers.Timer();
        _timer.AutoReset = true;
        _timer.Elapsed += _timer_Elapsed;
        _timer.Interval = MILLISEC_TRACK;

        QualityMode();
    }

    internal void QualityMode()
    {
        _accuracy.DesiredAccuracy = GeolocationAccuracy.Best;
    }

    internal void NormalMode()
    {
        _accuracy.DesiredAccuracy = GeolocationAccuracy.High;
    }

    internal void EcoMode()
    {
        _accuracy.DesiredAccuracy = GeolocationAccuracy.Medium;
    }

    private bool _timeout = false;

    internal async Task<bool> WaitStableLocalisation(double timeOut = 10000)
    {
        bool stable = false;
        _timeout = false;
        System.Timers.Timer t = new System.Timers.Timer(timeOut);
        t.AutoReset = false;
        t.Elapsed += TimeOut_Elapsed;
        t.Start();
        do
        {
            Location? location = await Geolocation.Default.GetLocationAsync(_accuracy);

            if (location != null && location.Accuracy != null && 
                location.Accuracy <= METERS_ACCURACY_NEED_CALIBRATION)
            {
                stable = true;
            }

            if(!stable && !_timeout) await Task.Delay(50);
        } 
        while (!stable && !_timeout);
        t.Stop();
        t.Dispose();

        return stable;
    }

    internal async Task TrackUserNow()
    {
        Location location = await GetLocation();
        _engine.UpdateUserLocation(location);
        _engine.MooveScreenTo(location);
    }

    private void TimeOut_Elapsed(object sender, ElapsedEventArgs e)
    {
        _timeout = true;
    }

    private void _timer_Elapsed(object sender, ElapsedEventArgs e)
    {
        Application.Current.Dispatcher.Dispatch(TrackNow);
    }

    internal void StartTrack()
    {
        _timer.Start();
    }

    private async void TrackNow()
    {
        Location location = await GetLocation();
        _engine.UpdateUserLocation(location);
        if(_actual_points.Count == 0)
        {
            _actual_points.Add(new MapPoint(location));
            _engine.AddLine(location);
        }
        else
        {
            MapPoint last = _actual_points.Last();
            double metersDistance = Location.CalculateDistance(last.Latitude, last.Longitude, location, DistanceUnits.Kilometers) * 1000;
            if(metersDistance >= METERS_DISTANCE_PICK)
            {
                _actual_points.Add(new MapPoint(location));
                _engine.AddLine(location);
            }
        }
    }

    internal void StopTrack()
    {
        _timer.Stop();
        _engine.CutLine();
        _points.Add(_actual_points.ToArray());
        _actual_points.Clear();
    }

    internal async Task<LocalisationError> CheckLocationAvailable()
    {
        try
        {
            Location location = await Geolocation.Default.GetLocationAsync( new GeolocationRequest() { DesiredAccuracy = GeolocationAccuracy.Lowest });

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

    private async Task<Location> GetLocation()
    {
        return await Geolocation.Default.GetLocationAsync(_accuracy)!;
    }

    public void Dispose()
    {
        _timer.Dispose();
        _engine.Dispose();
        _points.Clear();
        _actual_points.Clear();
    }
}

internal enum LocalisationError
{
    None = 0,
    NotSupported = 1,
    NotEnabled = 2,
    NeedPermission = 3,
    Unknown = 4
}