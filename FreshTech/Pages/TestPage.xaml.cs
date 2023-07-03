namespace FreshTech.Pages;

public partial class TestPage : ContentPage
{
	public TestPage()
	{
		InitializeComponent();
		Test();
    }

	private async void Test()
	{
        /*map.StartLoading();
		map.QualityMode();
		try
		{
            if(await map.WaitStableLocalisation(5000))
			{
                map.SetEnableStart(true);
                map.TrackUserNow(await Geolocation.Default.GetLastKnownLocationAsync());
            }
            else
			{
				map.SetEnableStart(false);
                await DisplayAlert("Localisation", "la localisation du téléphone est difficile à récupérer. Veuillez évitez les espaces fermé.", "Ok");
            }
            map.ObjectiveKm = 5;
        }
		catch(Exception ex)
		{
			await DisplayAlert("err", ex.ToString(), "cancel");
		}
		finally { map.StopLoading(); }*/
    }

    private void ButtonBadge_Clicked()
    {

    }
}