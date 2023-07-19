using AppCore.Property;
using AppCore.Services;

namespace FreshTech.Pages;

public partial class CurrentActivityPage : ContentPage
{
    private ActivityEngine _activity_engine;
    private DifficulteCourse _difficulty = DifficulteCourse.Normal;

    private bool _is_init = false;
    private bool _is_activity_set = false;

    /// <summary>
    /// L'initialisation s'effectue dans la fonction <see cref="Init"/>
    /// </summary>
    public CurrentActivityPage()
	{
		InitializeComponent();
	}

    #region INIT

    public void SetActivity(ActivityEngine activityEngine, DifficulteCourse difficulty)
    {
        if (!_is_activity_set)
        {
            _is_activity_set = true;
            _activity_engine = activityEngine;
            _difficulty = difficulty;
            Init();
        }
    }

    private void ContentPage_SizeChanged(object sender, EventArgs e)
    {
        if (!_is_init)
        {
            _is_init = true;
            Init();
        }
    }

    /// <summary>
    /// Initialisation de l'activité si tout les conditions sont réunies <br></br>
    /// <see cref="_is_init"/> et <see cref="_is_activity_set"/> sont à <b>True</b>
    /// </summary>
    private async void Init()
    {
        if(_is_activity_set && _is_init)
        {
            map.StartLoading(); // lance l'animation de chargement
            map.QualityMode(); // demande à la carte une géolocalisation précise

            bool success = false;

            try
            {

                bool retry;
                do
                {
                    // attend la calibration de la géolocalisation
                    if (await map.WaitStableLocalisation(5000))
                    {
                        // stabilisation GPS OK
                        retry = false;
                        success = true;
                    }
                    else
                    {
                        // échec de la stabilisation GPS, proposition de réessayez 
                        retry = await DisplayAlert("Localisation", "La localisation du téléphone est difficile à récupérer. Veuillez évitez les espaces fermé. ", "Réessayer", "Annuler");
                    }
                } 
                while (retry);
            }
            catch (Exception ex)
            {
                // erreur inconnue
                await DisplayAlert("err", ex.ToString(), "cancel");
            }

            if (success)
            {
                // autorise la carte à lancer l'activité
                map.SetEnableStart(true);
                // géolocalise l'utilisateur sur la carte selon la dernière localisation connue
                map.TrackUserNow(await Geolocation.Default.GetLastKnownLocationAsync());
                // initialise l'objectif
                map.ObjectiveKm = _activity_engine.GetDistanceKm(_difficulty);
                // 
                map.StopTracking += Map_StopTracking;
            }
            else
            {
                // retourne au menu principal
                _ = Shell.Current.GoToAsync("//GamePage", true);
            }

            map.StopLoading();
        }
    }

    #endregion

    private async void Map_StopTracking()
    {
        await Shell.Current.GoToAsync("//AfterActivityPage");
        if (Shell.Current.CurrentPage is AfterActivityPage page)
        {
            page.SetEndActivity(_activity_engine, _difficulty, 
                map.DistanceKm, map.TotalSecActivity, map.TotalSecPause, map.GetSpeedKmHMean());
        }
    }
    
}