using AppCore.Models;
using AppCore.Services.GeneralMessage.Args;

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
		Courses[]? courses = await App.client.SendAndGetResponse<Courses[]>(new EPGetCourses(0, 40));
        if(courses != null && courses.Length > 0)
		{
			barGraph.AddRange(courses);
        }
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

    private void Reload_Clicked(object sender, EventArgs e)
    {
        barGraph.Invalidate();

    }
}