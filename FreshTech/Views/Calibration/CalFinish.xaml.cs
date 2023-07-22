using FreshTech.Pages;

namespace FreshTech.Views.Calibration;

public partial class CalFinish : ContentView
{
    private readonly CalibrationPage _parent;

    public CalFinish(CalibrationPage parent)
	{
		InitializeComponent();
        _parent = parent;
	}

    private void Finish_Clicked(object sender, EventArgs e)
    {
        _parent.Exit();
    }

    private void CheckResult_Clicked(object sender, EventArgs e)
    {
        _parent.ExitAndGoToObjective();
    }
}