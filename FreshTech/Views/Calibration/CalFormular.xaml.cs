using FreshTech.Pages;

namespace FreshTech.Views.Calibration;

public partial class CalFormular : ContentView
{
	private readonly CalibrationPage _parent;
	private readonly bool _withActivityEntry;

    public CalFormular(CalibrationPage parent, bool withActivityEntry)
	{
		InitializeComponent();
		_parent = parent;
		_withActivityEntry = withActivityEntry;
	}
}