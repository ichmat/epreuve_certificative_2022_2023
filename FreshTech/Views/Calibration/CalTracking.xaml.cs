using AppCore.Models;
using AppCore.Services.GeneralMessage.Args;
using FreshTech.Pages;

namespace FreshTech.Views.Calibration;

public partial class CalTracking : ContentView
{
    private bool _is_init = false;
    private readonly CalibrationPage _parent;

	public CalTracking(CalibrationPage parent)
	{
		InitializeComponent();
        _parent = parent;
	}

    private async void InitMap()
    {
        mapInstance.StartLoading();
        mapInstance.QualityMode(); 
        if(await mapInstance.WaitStableLocalisation(8000))
        {
            mapInstance.TrackUserNow(await Geolocation.Default.GetLastKnownLocationAsync());
            mapInstance.SetEnableStart(true);
            mapInstance.ObjectiveKm = 5;
            mapInstance.StopTracking += MapInstance_StopTracking;
        }
        else
        {
            mapInstance.SetEnableStart(false);
            await _parent.DisplayAlert("Localisation", "la localisation du téléphone est difficile à récupérer. Veuillez évitez les espaces fermé.", "Ok");
        }
        mapInstance.StopLoading();
    }

    private async void SaveStatAndGoNext()
    {
        _parent.StartLoading();

        int fatigueValue = Convert.ToInt32(Math.Round(sliderFatigueLevel.Value));

        double kmObjectif = mapInstance.DistanceKm;
        double secActivityObjectif = mapInstance.TotalSecActivity;
        double secPauseObjectif = mapInstance.TotalSecPause;
        double meanSpeedKmHObjectif = mapInstance.GetSpeedKmHMean();

        switch (fatigueValue)
        {
            case 1:
                kmObjectif = Math.Round(kmObjectif * 1.35, 2);
                secActivityObjectif = Math.Round(secActivityObjectif * 1.2);
                secPauseObjectif = Math.Round(secPauseObjectif / 2);
                meanSpeedKmHObjectif = Math.Round(meanSpeedKmHObjectif * 1.3, 2);
                break;
            case 2:
                kmObjectif = Math.Round(kmObjectif * 1.2, 2);
                secActivityObjectif = Math.Round(secActivityObjectif * 1.1);
                secPauseObjectif = Math.Round(secPauseObjectif /1.5);
                meanSpeedKmHObjectif = Math.Round(meanSpeedKmHObjectif * 1.15, 2);
                break;
            case 3:
                kmObjectif = Math.Round(kmObjectif, 2);
                secActivityObjectif = Math.Round(secActivityObjectif);
                secPauseObjectif = Math.Round(secPauseObjectif);
                meanSpeedKmHObjectif = Math.Round(meanSpeedKmHObjectif, 2);
                break;
            case 4:
                kmObjectif = Math.Round(kmObjectif / 1.2, 2);
                secActivityObjectif = Math.Round(secActivityObjectif / 1.1);
                secPauseObjectif = Math.Round(secPauseObjectif * 1.2);
                meanSpeedKmHObjectif = Math.Round(meanSpeedKmHObjectif / 1.15, 2);
                break;
            case 5:
                kmObjectif = Math.Round(kmObjectif / 1.35, 2);
                secActivityObjectif = Math.Round(secActivityObjectif / 1.2);
                secPauseObjectif = Math.Round(secPauseObjectif * 2);
                meanSpeedKmHObjectif = Math.Round(meanSpeedKmHObjectif / 1.3, 2);
                break;
        }

        Stat stat = new Stat();
        stat.UtilisateurId = App.client.CurrentUser.UtilisateurId;
        stat.ObjectifDistanceKm = kmObjectif;
        stat.ObjectifPauseSecMax = secPauseObjectif;
        stat.ObjectifTempsSecMax = secActivityObjectif;
        stat.ObjectifVitesseMoyenneKmH = meanSpeedKmHObjectif;

        bool res = await App.client.SendRequest(new EPSaveStat(stat));

        _parent.StopLoading();

        if (res)
        {

        }
        else
        {
            _ = _parent.DisplayAlert("Problème de communication", "Les informations n'ont pas pu être envoyer au serveur. Vérifier votre connexion et réessayer.", "Ok");
        }
    }

    private void MapInstance_StopTracking()
    {
        mapInstance.IsVisible = false;
    }

    private void Start_Clicked(object sender, EventArgs e)
    {
        StackPresentation1.IsVisible = false;
        StackPresentation2.IsVisible = false;
        mapInstance.IsVisible = true;
        InitMap();
    }

    private void Back_Clicked(object sender, EventArgs e)
    {
        _parent.GoToMainPage();
    }

    private void ContentView_SizeChanged(object sender, EventArgs e)
    {
        if (!_is_init)
        {
            _is_init = true;
            mapInstance.IsVisible = false;
            // TEST ONLY
            StackPresentation1.IsVisible = false;
            StackPresentation2.IsVisible = false;
            StackSuccess.IsVisible = true;
        }
    }

    private void ContentView_Unloaded(object sender, EventArgs e)
    {
        mapInstance.StopTracking -= MapInstance_StopTracking;
    }

    private void sliderFatigueLevel_DragCompleted(object sender, EventArgs e)
    {
        sliderFatigueLevel.Value = Math.Round(sliderFatigueLevel.Value);
    }

    private void Next_Clicked(object sender, EventArgs e)
    {
        SaveStatAndGoNext();
    }

}