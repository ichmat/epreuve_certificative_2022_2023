namespace FreshTech.Views;

public partial class ContentWithClippedIcon : ContentView
{
	public ImageSource ClippedIcon
	{
		get => clippedImage.Source;
		set => clippedImage.Source = value;
	}

	public double ImgAnchorX
	{
		get => viewBox.AnchorX; set => viewBox.AnchorX = value;
    }

    public double ImgAnchorY
    {
        get => viewBox.AnchorY; set => viewBox.AnchorY = value;
    }

	public double ImgScale
	{
		get => viewBox.Scale; set => viewBox.Scale = value;
    }

	public string LabelTop
	{
		get => Label_Top.Text;
		set => Label_Top.Text = value;
	}

	public string LabelMiddle
	{
		get => Label_Middle.Text;
		set => Label_Middle.Text = value;
	}

	public string LabelBottom
	{
		get => Label_BottomClickable.Text;
		set => Label_BottomClickable.Text = value;
	}

	public delegate void OnLabelBottomClicked();

	public event OnLabelBottomClicked LabelBottomClicked;

    public ContentWithClippedIcon()
	{
		InitializeComponent();
	}

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
		LabelBottomClicked?.Invoke();
    }
}