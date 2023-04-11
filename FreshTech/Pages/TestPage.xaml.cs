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
        map.StartLoading();
		map.QualityMode();
		await map.WaitStableLocalisation();
		await map.TrackUserNow();
        map.StopLoading();
        map.ObjectiveKm = 5;
    }
}