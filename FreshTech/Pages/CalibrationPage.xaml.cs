using AppCore.Models;
using AppCore.Services.GeneralMessage.Args;
using FreshTech.Views.Calibration;

namespace FreshTech.Pages;

public partial class CalibrationPage : ContentPage
{
	public CalibrationPage()
	{
		InitializeComponent();
    }

    private async void ContentPage_Loaded(object sender, EventArgs e)
    {
        StartLoading();

        // vérifie que l'objet stat est initialiser pour cette utilisateur
        Stat? stat = await App.client.SendAndGetResponse<Stat>(new EPGetStatByUserId());
        if(stat != null &&
            stat.ObjectifTempsSecMax != null && 
            stat.ObjectifPauseSecMax != null &&
            stat.ObjectifDistanceKm != null &&
            stat.ObjectifVitesseMoyenneKmH != null)
        {

            // maintenant on vérifie qu'on a les information nécessaire de l'utilisateur
            if(App.client.CurrentUser.PoidKg != null &&
                App.client.CurrentUser.TailleCm != null)
            {
                // tout est ok, l'application se lance normalement
                Exit();
            }
            else
            {
                // il manque encore quelque info, il faut basculer sur la page du formulaire
                GoToFormular(false);
            }
        }
        else
        {
            // s'il ne l'est pas, il faut demander un calibrage
            GoToMainPage();
        }

        StopLoading();
    }

    internal void GoToMainPage()
	{
        BorderContent.Content = new CalMain(this);
    }

	internal void GoToCalibrate()
	{
        BorderContent.Content = new CalTracking(this);
    }

    /// <summary>
    /// Ouvre la section formulaire
    /// </summary>
    /// <param name="withActivityEntry">fait apparaître le formulaire pour renseigner les objectifs</param>
    internal void GoToFormular(bool withActivityEntry)
    {
        BorderContent.Content = new CalFormular(this, withActivityEntry);
    }

    internal void GoToFinishCalibration()
    {
        BorderContent.Content = new CalFinish(this);
    }

    internal void Exit()
    {
        _ = Shell.Current.GoToAsync("//GamePage", true);
    }

    internal void ExitAndGoToObjective()
    {
        _ = Shell.Current.GoToAsync("//ProfilPage", true);
    }

    internal void StartLoading()
    {
        AI_Loading.IsRunning = true;
    }

    internal void StopLoading()
    {
        AI_Loading.IsRunning = false;
    }
}