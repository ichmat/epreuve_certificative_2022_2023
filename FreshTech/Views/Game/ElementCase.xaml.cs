using AppCore.Services;

namespace FreshTech.Views.Game;

public partial class ElementCase : ContentView
{
	private bool _is_init = false;

	public IConstruction Construction { get; private set; }

	public ElementCase(IConstruction construction)
	{
		InitializeComponent();
		this.Construction = construction;
	}

    private void ContentView_SizeChanged(object sender, EventArgs e)
    {
		if (!_is_init)
		{
			_is_init = true;
            LoadImg();
            ReloadState();
            if(Clicked?.GetInvocationList().Length > 0)
            {
                TapGestureRecognizer tap = new TapGestureRecognizer();
                tap.Tapped += TapGestureRecognizer_Tapped;
                MainBorder.GestureRecognizers.Add(tap);
            }
        }
    }

    private void LoadImg()
    {
        mainImg.Source = IconConstruction.GetIconByConsInfoId(Construction.GetConsInfoId());
    }

	private void ReloadState()
	{
		double percent = (double)Construction.GetVie() / Construction.GetVieMax();
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

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        Clicked?.Invoke(this);
    }

    public delegate void OnClicked(ElementCase clicked);

    public event OnClicked Clicked;
}