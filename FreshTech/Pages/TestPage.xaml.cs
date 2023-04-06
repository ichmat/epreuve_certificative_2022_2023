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
		await Task.Delay(4000);
		map.QualityMode();
		await map.WaitStableLocalisation();
		await map.TrackUserNow();
    }
}