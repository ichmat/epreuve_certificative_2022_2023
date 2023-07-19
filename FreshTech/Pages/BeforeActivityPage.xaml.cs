using AppCore.Models;
using AppCore.Property;
using AppCore.Services.GeneralMessage.Args;
using AppCore.Services;
using FreshTech.Views.Activity;
using System.Data;

namespace FreshTech.Pages;

public partial class BeforeActivityPage : ContentPage
{
    private ActivityEngine _activity_engine;

	public BeforeActivityPage()
	{
		InitializeComponent();
	}

    private void TitleSpan_GoBack()
    {
        _ = Shell.Current.Navigation.PopAsync();
    }

    private async void ContentPage_Loaded(object sender, EventArgs e)
    {
        StartLoading();
        if(!await TryGenerateActivityEngine())
        {
            await DisplayAlert("Connexion réseaux", 
                "Impossible de communiquer avec le serveur. Veuillez ressayez.", "Revenir");
            await Shell.Current.Navigation.PopAsync();
            return;
        }
        ResetView();
        StopLoading();
    }

    internal async Task<bool> TryGenerateActivityEngine()
    {
        ActivityEngine? engine = await App.client.SendAndGetResponse<ActivityEngine>(new EPGenerateActivityEngine());
        if(engine == null)
        {
            return false;
        }
        else
        {
            _activity_engine = engine;
            return true;
        }
    }

    private void ResetView()
    {
        // objectifs
        DC_Distance.Title = RoundStr(_activity_engine.GetDistanceKm(difficultySelector.Level), 2) + " Km";
        DC_Time.Title = TimeSpan.FromSeconds(_activity_engine.GetTotalSecActivity(difficultySelector.Level)).ToString(@"hh\:mm\:ss");
        DC_Pause.Title = TimeSpan.FromSeconds(_activity_engine.GetTotalSecPauseActivity(difficultySelector.Level)).ToString(@"hh\:mm\:ss");
        DC_MeanSpeed.Title = RoundStr(_activity_engine.GetMeanSpeedKmH(difficultySelector.Level), 2) + " Km/H";

        // récompenses
        DC_Award_Bois.Title = _activity_engine.AwardWood.ToString();
        DC_Award_Ferraille.Title = _activity_engine.AwardScapMetal.ToString();
        DC_Obj_Common.Title = RoundStr(_activity_engine.AwardCommonObject, 4);
        DC_Obj_Common.SubTitle = Percent(_activity_engine.AwardCommonObject, 2);
        DC_Obj_Rare.Title = RoundStr(_activity_engine.AwardRareObject,4);
        DC_Obj_Rare.SubTitle = Percent(_activity_engine.AwardRareObject, 2);
        DC_Obj_Epic.Title = RoundStr(_activity_engine.AwardEpicObject, 4);
        DC_Obj_Epic.SubTitle = Percent(_activity_engine.AwardEpicObject, 2);
        DC_Obj_Legendary.Title = RoundStr(_activity_engine.AwardLegendaryObject, 4);
        DC_Obj_Legendary.SubTitle = Percent(_activity_engine.AwardLegendaryObject, 2);

        // TODO : ajouter les vues affichants les bonus
    }

    private string RoundStr(double d, int digit) => Math.Round(d, digit).ToString(System.Globalization.CultureInfo.InvariantCulture);

    private string Percent(double d, int digit) => "Soit " + 
        Math.Round(d * 100, digit).ToString(System.Globalization.CultureInfo.InvariantCulture)
        + " % de chance";

    public void StartLoading()
    {
        BorderLoading.IsVisible = true;
        AI_Loading.IsRunning = true;
        scollViews.IsVisible = false;
    }

    public void StopLoading()
    {
        BorderLoading.IsVisible = false;
        AI_Loading.IsRunning = false;
        scollViews.IsVisible = true;
    }

    private void difficultySelector_DifficultyChanged(DifficulteCourse newLevel)
    {
        ResetView();
    }

    private async void Start_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//CurrentActivityPage");
        if(Shell.Current.CurrentPage is CurrentActivityPage page)
        {
            page.SetActivity(_activity_engine, difficultySelector.Level);
        }
    }
}