namespace FreshTech.Views.Game;

public partial class ElementCase : ContentView
{
	private bool _is_init = false;

	private IConstruction construction;

	public ElementCase(IConstruction construction)
	{
		InitializeComponent();
		this.construction = construction;
	}

    private void ContentView_SizeChanged(object sender, EventArgs e)
    {
		if (!_is_init)
		{
			_is_init = true;
			ReloadState();
        }
    }

	private void ReloadState()
	{
		double percent = (double)construction.GetVie() / construction.GetVieMax();
		if (percent == 1)
		{
            imgState.IsVisible = false;
        }
        else if (percent > 40)
		{
            imgState.IsVisible = true;
			imgState.Source = "repaire.svg";
        }
        else if (percent > 0)
        {
            imgState.IsVisible = true;
            imgState.Source = "critical_repaire.svg";
        }
        else
        {
            imgState.IsVisible = true;
            imgState.Source = "broken.svg";
        }
    }
}