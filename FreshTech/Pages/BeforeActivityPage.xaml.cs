using AppCore.Models;
using AppCore.Property;
using AppCore.Services.GeneralMessage.Args;
using AppCore.Services;
using FreshTech.Views.Activity;
using System.Data;
using FreshTech.Views;
using System.Globalization;

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
        // TODO : ⚠ ATTENTION, l'utilisateur peut démarrer une activité alors qu'il n'a pas de village
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
        DC_Award_Bois.Title = _activity_engine.GetAwardWood(difficultySelector.Level).ToString();
        DC_Award_Ferraille.Title = _activity_engine.GetAwardScrapMetal(difficultySelector.Level).ToString();
        DC_Obj_Common.Title = RoundStr(_activity_engine.GetAwardCommonObj(difficultySelector.Level), 4);
        DC_Obj_Common.SubTitle = Percent(_activity_engine.GetAwardCommonObj(difficultySelector.Level), 2);
        DC_Obj_Rare.Title = RoundStr(_activity_engine.GetAwardRareObj(difficultySelector.Level), 4);
        DC_Obj_Rare.SubTitle = Percent(_activity_engine.GetAwardRareObj(difficultySelector.Level), 2);
        DC_Obj_Epic.Title = RoundStr(_activity_engine.GetAwardEpicObj(difficultySelector.Level), 4);
        DC_Obj_Epic.SubTitle = Percent(_activity_engine.GetAwardEpicObj(difficultySelector.Level), 2);
        DC_Obj_Legendary.Title = RoundStr(_activity_engine.GetAwardLegendaryObj(difficultySelector.Level), 4);
        DC_Obj_Legendary.SubTitle = Percent(_activity_engine.GetAwardLegendaryObj(difficultySelector.Level), 2);

        // bonus 
        FillBonusContent(DC_Bonus_Bois, _activity_engine.BonusWood);
        FillBonusContent(DC_Bonus_Ferraille, _activity_engine.BonusScrapMetal);
        FillBonusContent(DC_Bonus_Obj_Common, _activity_engine.BonusCommonObject);
        FillBonusContent(DC_Bonus_Obj_Rare, _activity_engine.BonusRareObject);
        FillBonusContent(DC_Bonus_Obj_Epic, _activity_engine.BonusEpicObject);
        FillBonusContent(DC_Bonus_Obj_Legendary, _activity_engine.BonusLegendaryObject);
    }

    private static void FillBonusContent(DataContent view, SuccessBonus data)
    {
        view.Title = "x " + data.Multiply.ToString(CultureInfo.InvariantCulture);
        switch(data.Type)
        {
            case TypeSuccessBonus.PerObjective:
                view.SubTitle = "Bonus par objectif rempli";
                view.ImageSourceRight = "round_checked.svg";
                break;
            case TypeSuccessBonus.AllObjective:
                view.SubTitle = "Bonus si tout objectifs rempli";
                view.ImageSourceRight = "all_checked.svg";
                break;
        }
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
        CurrentActivityPage page = new CurrentActivityPage();
        await Shell.Current.Navigation.PushModalAsync(page, true);
        page.SetActivity(_activity_engine, difficultySelector.Level);
    }
}