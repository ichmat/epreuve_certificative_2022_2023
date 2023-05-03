using FreshTech.Pages;

namespace FreshTech.Views.Calibration;

public partial class CalTracking : ContentView
{
    private bool _is_init = false;
    private readonly CalibrationPage _parent;

	public CalTracking(CalibrationPage parent)
	{
		InitializeComponent();
        _parent = parent;
	}

    private void Start_Clicked(object sender, EventArgs e)
    {

    }

    private void Back_Clicked(object sender, EventArgs e)
    {
        _parent.GoToMainPage();
    }

    private void ContentView_SizeChanged(object sender, EventArgs e)
    {
        if (!_is_init)
        {
            _is_init = true;
            mapInstance.IsVisible = false;
        }
    }
}