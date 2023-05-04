using FreshTech.Pages;

namespace FreshTech.Views.Calibration;

public partial class CalMain : ContentView
{
    private readonly CalibrationPage _parent;

	public CalMain(CalibrationPage parent)
	{
		InitializeComponent();
        _parent = parent;
	}

    private void Calibrate_Clicked(object sender, EventArgs e)
    {
        _parent.GoToCalibrate();
    }

    private void Formular_Clicked(object sender, EventArgs e)
    {
        _parent.GoToFormular(true);
    }
}