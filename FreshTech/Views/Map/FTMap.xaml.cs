using FreshTech.Tools;
using Microsoft.Maui.Devices.Sensors;
using System;
using System.Timers;
using MathNet.Filtering.Kalman;
using System.Diagnostics.Metrics;

namespace FreshTech.Views.Map;

public partial class FTMap : ContentView, IDisposable
{
    private readonly IMapEngine _engine;

    private readonly List<MapPoint[]> _points = new List<MapPoint[]>();

    private List<MapPoint> _actual_points = new List<MapPoint>();

    private readonly GeolocationRequest _accuracy;
    private readonly System.Timers.Timer _timerTrack;

    private const double METERS_ACCURACY_NEED_CALIBRATION = 50;
    private const double METERS_DISTANCE_PICK = 5;

    private short _millisec_tick = 1000;
    private short _millisec_track = 4000;

    private bool _is_init = false;

    private double _distanceKm = 0D;
    public double DistanceKm
    {
        get => _distanceKm;
        set
        {
            if(value != _distanceKm)
            {
                _distanceKm = value;
                if(_objectiveKm != null)
                {
                    Dispatcher.Dispatch(() =>
                    {
                        UpdateKmObjective();
                    });
                }
            }
        }
    }

    public double SpeedKm { get; private set; } = 0D;

    private double? _objectiveKm = null;
    public double? ObjectiveKm
    {
        get => _objectiveKm;
        set
        {
            if (value != _objectiveKm)
            {
                _objectiveKm = value;
                Dispatcher.Dispatch(() =>
                {
                    UpdateKmObjective();
                });
            }
        }
    }

    public double TotalSecActivity { get; private set; } = 0D;

    public double TotalSecPause { get; private set; } = 0D;

    public double TotalSecActualPause { get; private set; } = 0D;

    public double TotalSec { get => TotalSecPause + TotalSecActivity; }

    public DateTime StartActivity { get; private set; }

    private TrackState _state = TrackState.NotSet;

    public TrackState State
    {
        get => _state;
        set
        {
            if(value != _state)
            {
                _state = value;
                if(_is_init)
                {
                    switch (_state)
                    {
                        case TrackState.NotSet:
                            ButtonStart.IsVisible = true;
                            ButtonState.IsVisible = false;
                            ButtonStop.IsVisible = false;
                            BorderTimePause.IsVisible = false;
                            break;
                        case TrackState.Stopped:
                            ButtonStart.IsVisible = false;
                            ButtonState.IsVisible = false;
                            ButtonStop.IsVisible = false;
                            BorderTimePause.IsVisible = false;
                            break;
                        case TrackState.Paused:
                            ButtonStart.IsVisible = false;
                            ButtonState.IsVisible = true;
                            ButtonStop.IsVisible = false;
                            ButtonState.Text = "RESUME";
                            ButtonState.BackgroundColor = ColorsTools.Success;
                            BorderTimePause.IsVisible = true;
                            TotalSecActualPause = 0D;
                            UpdatePauseLabel();
                            break;
                        case TrackState.Running:
                            ButtonStart.IsVisible = false;
                            ButtonState.IsVisible = true;
                            ButtonStop.IsVisible = true;
                            ButtonState.Text = "PAUSE";
                            ButtonState.BackgroundColor = ColorsTools.Warning;
                            BorderTimePause.IsVisible = false;
                            break;
                    }
                }
            }
        }
    }

    public FTMap()
	{
		InitializeComponent();
        _engine = new FTOpenStreetMap();
        BorderMap.Content = _engine.GetMapView();
        _accuracy = new GeolocationRequest();
        _accuracy.RequestFullAccuracy = true;
        _timerTrack = new System.Timers.Timer();
        _timerTrack.AutoReset = true;
        _timerTrack.Elapsed += _timerTrack_Elapsed;
        _timerTrack.Interval = _millisec_track;
        _timerTick = new System.Timers.Timer();
        _timerTick.AutoReset = true;
        _timerTick.Interval = _millisec_tick;
        _timerTick.Elapsed += _timerTick_Elapsed;
        NormalMode();
       
    }

    public void Reset()
    {
        DistanceKm = 0D;
        SpeedKm = 0D;
        ObjectiveKm = null;
        TotalSecActivity = 0D;
        TotalSecPause = 0D;
        State = TrackState.Stopped;
        _timerTick.Stop();
        _timerTrack.Stop();

    }

    public void Dispose()
    {
        _timerTrack.Dispose();
        _engine.Dispose();
        _points.Clear();
        _actual_points.Clear();
    }

    public void SetEnableStart(bool isEnabled)
    {
        Dispatcher.Dispatch(() =>
        {
            this.ButtonStart.IsEnabled = isEnabled;
            this.ButtonStart.IsVisible = isEnabled;

            BorderSpeed.IsVisible = isEnabled;
            BorderTimeActivity.IsVisible = isEnabled;
        });
    }

    #region RESULT

    public double GetSpeedKmHMean()
    {
        double totalSpeedKmHRegister = 0;
        int nbVal = 0;

        foreach(MapPoint[] line in _points)
        {
            if(line.Length > 1)
            {
                for(int i = 0; i < line.Length - 1; ++i)
                {
                    double dKm = CalculateDistanceKm(line[i], line[i + 1]);
                    double hours = (line[i + 1].Date - line[i].Date).TotalHours;
                    totalSpeedKmHRegister += dKm / hours;
                    nbVal++;
                }
            }
        }

        if(nbVal == 0) return 0;

        return totalSpeedKmHRegister / nbVal;
    }

    #endregion

    #region LOADING

    public void StartLoading()
    {
        BorderMap.Opacity = 0.6;
        AI_Loading.IsRunning = true;
        ButtonStart.IsEnabled = false;
        ButtonState.IsEnabled = false;

        BorderMap.IsEnabled = false;
    }

    public void StopLoading()
    {
        BorderMap.Opacity = 1;
        AI_Loading.IsRunning = false;
        ButtonStart.IsEnabled = true;
        ButtonState.IsEnabled = true;
    
        BorderMap.IsEnabled = true;
    }

    #endregion

    #region MODE

    internal void QualityMode()
    {
        _accuracy.DesiredAccuracy = GeolocationAccuracy.Best;
        _millisec_tick = 100;
        _millisec_track = 3000;
        _timerTick.Interval = _millisec_tick;
        _timerTrack.Interval = _millisec_track;
    }

    internal void NormalMode()
    {
        _accuracy.DesiredAccuracy = GeolocationAccuracy.Best;
        _millisec_tick = 500;
        _millisec_track = 4000;
        _timerTick.Interval = _millisec_tick;
        _timerTrack.Interval = _millisec_track;
    }

    internal void EcoMode()
    {
        _accuracy.DesiredAccuracy = GeolocationAccuracy.High;
        _millisec_tick = 1000;
        _millisec_track = 5000;
        _timerTick.Interval = _millisec_tick;
        _timerTrack.Interval = _millisec_track;
    }

    #endregion

    #region CHECK_LOCALISATION

    internal async Task<bool> WaitStableLocalisation(double timeOutReset = 20000)
    {
        GeolocationRequest r = new GeolocationRequest();
        r.Timeout = TimeSpan.FromMilliseconds(timeOutReset);
        GeolocationAccuracy[] accuracies = new GeolocationAccuracy[]
        {
            GeolocationAccuracy.Low, GeolocationAccuracy.Medium, GeolocationAccuracy.High, GeolocationAccuracy.Best
        };
        int tryNum = 0;
        do
        {
            r.DesiredAccuracy = accuracies[tryNum];
            Task<Location?> t_location = Geolocation.Default.GetLocationAsync(r);

            await t_location.WaitAsync(new CancellationToken());

            Location? location = t_location.IsCompleted ? t_location.Result : null;

            if (location != null && location.Accuracy != null)
            {
                if (location.Accuracy <= METERS_ACCURACY_NEED_CALIBRATION)
                    return true;
                else
                    TrackUserNow(location);
            }
            tryNum++;
        }
        while (tryNum < accuracies.Length);

        return false;
    }

    internal async Task TrackUserNow()
    {
        Location location = await GetLocation();
        TrackUserNow(location);
    }

    internal void TrackUserNow(Location location)
    {
        _engine.UpdateUserLocation(location);
        _engine.MooveScreenTo(location);
    }

    internal async Task<LocalisationError> CheckLocationAvailable()
    {
        try
        {
            Location location = await Geolocation.Default.GetLocationAsync(new GeolocationRequest() { DesiredAccuracy = GeolocationAccuracy.Lowest });

            if (location != null)
                return LocalisationError.None;
            else
                return LocalisationError.Unknown;
        }
        catch (FeatureNotSupportedException)
        {
            // Handle not supported on device exception
            return LocalisationError.NotSupported;
        }
        catch (FeatureNotEnabledException)
        {
            // Handle not enabled on device exception
            return LocalisationError.NotEnabled;
        }
        catch (PermissionException)
        {
            // Handle permission exception
            return LocalisationError.NeedPermission;
        }
        catch (Exception)
        {
            // Unable to get location
            return LocalisationError.Unknown;
        }
    }

    #endregion

    #region TRACKING

    internal void StartTrack()
    {
        State = TrackState.Running;
        _timerTrack.Start();
        _timerTick.Start();
        StartActivity = DateTime.Now;
    }

    internal void ResumeTrack()
    {
        State = TrackState.Running;
        _timerTrack.Start();
    }

    internal void PauseTrack()
    {
        State = TrackState.Paused;
        _timerTrack.Stop();
        _engine.CutLine();
        _points.Add(_actual_points.ToArray());
        _actual_points.Clear();
    }

    internal void StopTrack()
    {
        State = TrackState.Stopped;
        _timerTrack.Stop();
        _timerTick.Stop();

        _engine.CutLine();
        _points.Add(_actual_points.ToArray());
        _actual_points.Clear();
    }

    private void _timerTrack_Elapsed(object sender, ElapsedEventArgs e)
    {
        Application.Current.Dispatcher.Dispatch(TrackNow);
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
                UpdateNewDistance();
                UpdateSpeed();
                _engine.AddLine(location);
            }
        }
    }

    private UKF[] filters = Array.Empty<UKF>();
    private double init_lat = 0;
    private double init_lon = 0;

    private Location ApplyFilter(Location location)
    {
        if(filters.Length == 0)
        {
            filters = new[] { new UKF(), new UKF() };
            init_lat = location.Latitude;
            init_lon = location.Longitude;
        }
        else
        {
            filters[0].Update(new[] { location.Latitude - init_lat });
            filters[1].Update(new[] { location.Longitude - init_lon });
            location.Latitude = filters[0].getState()[0] + init_lat;
            location.Longitude = filters[1].getState()[0] + init_lon;
        }
        
        return location;

    }

    private void UpdateSpeed()
    {
        if (_actual_points.Count > 1)
        {
            MapPoint m1 = _actual_points[_actual_points.Count - 2];
            MapPoint m2 = _actual_points[_actual_points.Count - 1];

            double dKm = CalculateDistanceKm(m1, m2);
            double hours = (m2.Date - m1.Date).TotalHours;
            SpeedKm = dKm / hours;
            UpdateSpeedLabel();
        }
    }

    private double CalculateDistanceKm(MapPoint m1, MapPoint m2)
    {
        return Location.CalculateDistance(
                m1.Latitude,
                m1.Longitude,
                m2.Latitude,
                m2.Longitude,
                DistanceUnits.Kilometers
                );
    }

    private void UpdateNewDistance()
    {
        if(_actual_points.Count > 1)
        {
            DistanceKm += CalculateDistanceKm(
                _actual_points[_actual_points.Count - 2],
                _actual_points[_actual_points.Count - 1]
                );
        }
    }

    private async Task<Location> GetLocation()
    {
        Task<Location?> t_localisation = Geolocation.Default.GetLocationAsync(_accuracy);
        Location? location;
        try
        {
            await t_localisation.WaitAsync(TimeSpan.FromMilliseconds(500));
            location = t_localisation.Result;
        }
        catch (TimeoutException)
        {
            t_localisation = Geolocation.GetLastKnownLocationAsync();
            await t_localisation.WaitAsync(TimeSpan.FromMilliseconds(500));
            location = t_localisation.Result;
        }
        return location!;
    }

    #region TICK

    private readonly System.Timers.Timer _timerTick;

    private void _timerTick_Elapsed(object sender, ElapsedEventArgs e)
    {
        Dispatcher.Dispatch(Tick);
    }

    private void Tick()
    {
        switch(State)
        {
            case TrackState.Stopped:
                _timerTick.Stop();
                break;
            case TrackState.Paused:
                TotalSecPause += _millisec_tick / 1000D;
                TotalSecActualPause += _millisec_tick / 1000D;
                UpdatePauseLabel();
                break;
            case TrackState.Running:
                TotalSecActivity += _millisec_tick / 1000D;
                UpdateActivityLabel();
                break;
        }
    }

    #endregion

    #endregion

    #region VIEW

    private void UpdateKmObjective()
    {
        Dispatcher.Dispatch(() => {
            if (ObjectiveKm != null)
            {
                if (BorderObjective.IsVisible != true)
                {
                    BorderObjective.IsVisible = true;
                }
                L_ObjectiveKm.Text = Math.Round(DistanceKm, 2).ToString() + '/' + Math.Round(ObjectiveKm.Value, 2).ToString() + "Km";
                double percent = DistanceKm / ObjectiveKm.Value;
                if (percent > 1) { percent = 1; }
                if (percent < 0) { percent = 0; }
                BorderBar.WidthRequest = BorderBarParent.Width * percent;
            }
            else if (ObjectiveKm == null && BorderObjective.IsVisible)
            {
                BorderObjective.IsVisible = false;
            }
        });
    }

    private void UpdateSpeedLabel()
    {
        Dispatcher.Dispatch(() =>
        {
            L_Speed.Text = Math.Round(SpeedKm,2).ToString() + " KM/H";
        });
    }

    private void UpdateActivityLabel()
    {
        Dispatcher.Dispatch(() =>
        {
            L_Time_Activity.Text = TimeSpan.FromSeconds(TotalSecActivity).ToString(@"hh\:mm\:ss");
        });
    }

    private void UpdatePauseLabel()
    {
        Dispatcher.Dispatch(() =>
        {
            L_Time_Pause_Actual.Text = TimeSpan.FromSeconds(TotalSecActualPause).ToString(@"hh\:mm\:ss");
            L_Time_Pause_Total.Text = TimeSpan.FromSeconds(TotalSecPause).ToString(@"hh\:mm\:ss");
        });
    }

    #endregion

    #region EVENT

    private void ContentView_Unloaded(object sender, EventArgs e)
    {
        Dispose();
    }

    private void ButtonStart_Clicked(object sender, EventArgs e)
    {
        if(ObjectiveKm != null)
        {
            StartTrack();
        }
    }

    private void ButtonState_Clicked(object sender, EventArgs e)
    {
        if(State == TrackState.Running)
        {
            PauseTrack();
        }
        else if(State == TrackState.Paused)
        {
            ResumeTrack();
        }
    }

    private void ButtonStop_Clicked(object sender, EventArgs e)
    {
        if(State == TrackState.Running)
        {
            StopTrack();
            StopTracking?.Invoke();
        }
    }

    private void ContentView_SizeChanged(object sender, EventArgs e)
    {
        if (!_is_init)
        {
            _is_init = true;
            ButtonStart.IsVisible = false;
            ButtonState.IsVisible = false;
            ButtonStop.IsVisible = false;
            BorderObjective.IsVisible = false;
            BorderTimePause.IsVisible = false;
            BorderTimeActivity.IsVisible = false;
            BorderSpeed.IsVisible = false;
        }
    }

    #endregion

    public delegate void OnStopTracking();
    public event OnStopTracking StopTracking;
}

public enum TrackState
{
    NotSet = -1,
    Stopped = 0,
    Running = 1,
    Paused = 2,
}

internal enum LocalisationError
{
    None = 0,
    NotSupported = 1,
    NotEnabled = 2,
    NeedPermission = 3,
    Unknown = 4
}