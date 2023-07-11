using AppCore.Models;
using AppCore.Services.GeneralMessage.Args;

namespace FreshTech.Pages;

public partial class ObjectifPage : ContentPage
{
    public ObjectifPage()
	{
		InitializeComponent();
    }

    private async void TitleSpan_GoBack()
    {
        await Shell.Current.Navigation.PopAsync();
    }

    private void Save_Clicked(object sender, EventArgs e)
    {
    }

    private async void ContentPage_Loaded(object sender, EventArgs e)
    {
        Stat? stat = await App.client.SendAndGetResponse<Stat>(new EPGetStatByUserId());
        if(stat != null)
        {
            EntryDistance.Text = stat.ObjectifDistanceKm.Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
            EntryTime.Text = TimeSpan.FromSeconds(stat.ObjectifTempsSecMax.Value).ToString(@"hh\:mm\:ss");
        }
        else
        {
            await DisplayAlert("R�cup�ration Impossible","Veuillez v�rifier votre connexion internet et r�essayez","Ok");
            await Shell.Current.Navigation.PopAsync();
        }
    }
}