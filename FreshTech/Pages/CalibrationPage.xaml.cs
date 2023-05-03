using FreshTech.Views.Calibration;

namespace FreshTech.Pages;

public partial class CalibrationPage : ContentPage
{
	public CalibrationPage()
	{
		InitializeComponent();
		GoToMainPage();
    }

	internal void GoToMainPage()
	{
        BorderContent.Content = new CalMain(this);
    }

	internal void GoToCalibrate()
	{
        BorderContent.Content = new CalTracking(this);
    }

    internal void GoToFormular()
    {

    }
}