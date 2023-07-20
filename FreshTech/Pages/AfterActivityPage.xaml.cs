using AppCore.Property;
using AppCore.Services;
using AppCore.Services.GeneralMessage.Args;
using FreshTech.Tools;
using FreshTech.Views;
using FreshTech.Views.Game;
using System.Globalization;

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
    private DateTime _save_date_start_activity;

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

    internal void SetEndActivity(ActivityEngine activity_engine, DifficulteCourse difficulty, double distanceKm, double totalSecActivity, double totalSecPause, double meanSpeedKmH, DateTime dateStartActivity)
    {
        if (!_is_activity_set)
        {
            _is_activity_set = true;
            _activity_engine = activity_engine;
            _difficulty = difficulty;
            _save_date_start_activity = dateStartActivity;
            /*
            _save_distance_km = distanceKm;
            _save_total_sec_activity = totalSecActivity;
            _save_total_sec_pause = totalSecPause;
            _save_mean_speed_km_h = meanSpeedKmH;
            */
            
            _save_distance_km = _activity_engine.GetDistanceKm(_difficulty) * 0.8;
            _save_total_sec_activity = _activity_engine.GetTotalSecActivity(_difficulty) * 0.8;
            _save_total_sec_pause = _activity_engine.GetTotalSecPauseActivity(_difficulty) * 0.8;
            _save_mean_speed_km_h = _activity_engine.GetMeanSpeedKmH(_difficulty) * 0.8;
            
            Init();
        }
    }

    private async void Init()
    {
        if(_is_activity_set && _is_init)
        {
            activityIndicator.Level = _difficulty;
            FillDCObjective(DC_Distance, _save_distance_km, _activity_engine.GetDistanceKm(_difficulty), " Km");
            FillDCObjectiveTime(DC_Time, _save_total_sec_activity, _activity_engine.GetTotalSecActivity(_difficulty));
            FillDCObjectiveTime(DC_Pause, _save_total_sec_pause, _activity_engine.GetTotalSecPauseActivity(_difficulty));
            FillDCObjective(DC_MeanSpeed, _save_mean_speed_km_h, _activity_engine.GetMeanSpeedKmH(_difficulty), " Km/H");

            ResultAward? award = await TrySendActivityAndGetReward();
            if(award != null)
            {
                DC_Award_Bois.Title = award.RealAwardWood.ToString();
                DC_Award_Ferraille.Title = award.RealAwardScrapMetal.ToString();
                int nbCommon = award.CountNumberRarity(TypeRarete.COMMUN);
                if(nbCommon > 0)
                    DC_Obj_Common.Title = nbCommon.ToString();
                else
                    DC_Obj_Common.IsVisible = false;

                int nbRare = award.CountNumberRarity(TypeRarete.RARE);
                if (nbRare > 0)
                    DC_Obj_Rare.Title = nbRare.ToString();
                else
                    DC_Obj_Rare.IsVisible = false;

                int nbEpic = award.CountNumberRarity(TypeRarete.EPIC);
                if (nbEpic > 0)
                    DC_Obj_Epic.Title = nbEpic.ToString();
                else
                    DC_Obj_Epic.IsVisible = false;

                int nbLegendary = award.CountNumberRarity(TypeRarete.LEGENDAIRE);
                if (nbLegendary > 0)
                    DC_Obj_Legendary.Title = nbLegendary.ToString();
                else
                    DC_Obj_Legendary.IsVisible = false;

                // met à jour les données en cache
                GamePage.GetGameEngine().UpdateWithAward(award);
            }
        }
    }

    private void FillDCObjective(DataContent view, double initial, double objective, string textAfter)
    {
        bool isOk = initial >= objective;
        view.Title = Math.Round(initial, 3).ToString(CultureInfo.InvariantCulture) + textAfter;
        view.SubTitle = "Objectif : " + Math.Round(objective, 3).ToString(CultureInfo.InvariantCulture) + textAfter;
        if (isOk)
        {
            view.ImageSourceRight = "round_checked.svg";
            view.Color = ColorsTools.Success;
        }
        else
        {
            view.ImageSourceRight = "round_cross.svg";
            view.Color = ColorsTools.Danger;
        }
    }

    private void FillDCObjectiveTime(DataContent view, double initial, double objective, string textAfter = "")
    {
        bool isOk = initial <= objective;
        view.Title = TimeSpan.FromSeconds(initial).ToString(@"hh\:mm\:ss") + textAfter;
        view.SubTitle = "Objectif : " + TimeSpan.FromSeconds(objective).ToString(@"hh\:mm\:ss") + textAfter;
        if (isOk)
        {
            view.ImageSourceRight = "round_checked.svg";
            view.Color = ColorsTools.Success;
        }
        else
        {
            view.ImageSourceRight = "round_cross.svg";
            view.Color = ColorsTools.Danger;
        }
    }

    private async Task<ResultAward?> TrySendActivityAndGetReward()
    {
        StartLoading();
        ResultAward? a = await App.client.SendAndGetResponse<ResultAward>(
            new EPPublishCourse(
                _difficulty,
                _save_distance_km,
                _save_total_sec_activity,
                _save_total_sec_pause,
                _save_mean_speed_km_h,
                _save_date_start_activity
            ));
        StopLoading();
        return a;
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

    private void Finish_Clicked(object sender, EventArgs e)
    {
        StartLoading();
        Shell.Current.Navigation.PopToRootAsync(true);
    }
}