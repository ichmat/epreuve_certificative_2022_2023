using AppCore.Property;
using AppCore.Services;

namespace FreshTech.Pages;

public partial class AfterActivityPage : ContentPage
{
    private bool _is_init = false;
    private bool _is_activity_set = false;

    private ActivityEngine _activity_engine;
    private DifficulteCourse _difficulty = DifficulteCourse.Normal;
    private double _save_distance_km;
    private double _save_total_sec_activity;
    private double _save_total_sec_pause;
    private double _save_mean_speed_km_h;

    public AfterActivityPage()
	{
		InitializeComponent();
	}

    #region INIT

    private void ContentPage_SizeChanged(object sender, EventArgs e)
    {
        if (!_is_init)
        {
            _is_init = true;
            Init();
        }
    }

    internal void SetEndActivity(ActivityEngine activity_engine, DifficulteCourse difficulty, double distanceKm, double totalSecActivity, double totalSecPause, double meanSpeedKmH)
    {
        if (!_is_activity_set)
        {
            _is_activity_set = true;
            _activity_engine = activity_engine;
            _difficulty = difficulty;
            _save_distance_km = distanceKm;
            _save_total_sec_activity = totalSecActivity;
            _save_total_sec_pause = totalSecPause;
            _save_mean_speed_km_h = meanSpeedKmH;
            Init();
        }
    }

    private void Init()
    {
        if(_is_activity_set && _is_init)
        {
            activityIndicator.Level = _difficulty;
        }
    }

    #endregion

    public void StartLoading()
    {
        AI_Loading.IsRunning = true;
        scollViews.IsVisible = false;
    }

    public void StopLoading()
    {
        AI_Loading.IsRunning = false;
        scollViews.IsVisible = true;
    }
}