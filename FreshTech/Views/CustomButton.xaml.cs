
namespace FreshTech.Views;

public partial class CustomButton : ContentView
{
    #region TITLE

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create("Title", typeof(string), typeof(CustomButton), string.Empty, propertyChanged: OnTitleChanged);

    private static void OnTitleChanged(BindableObject view, object oldValue, object newValue)
    {
        ((CustomButton)view).TitleChanged();
    }

    private void TitleChanged()
    {
        if (!_is_init) return;
        L_Title.Text = Title;
    }

    #endregion

    #region IMG

    public string ImageSource
    {
        get => (string)GetValue(ImageSourceProperty);
        set => SetValue(ImageSourceProperty, value);
    }

    public static readonly BindableProperty ImageSourceProperty =
        BindableProperty.Create("ImageSource", typeof(string), typeof(CustomButton), string.Empty, propertyChanged: OnImageSourceChanged);

    private static void OnImageSourceChanged(BindableObject view, object oldValue, object newValue)
    {
        ((CustomButton)view).ImageSourceChanged();
    }

    private void ImageSourceChanged()
    {
        if (!_is_init) return;
        if (string.IsNullOrWhiteSpace(ImageSource))
        {
            img.IsVisible = false;
        }
        else
        {
            img.Source = ImageSource;
            img.IsVisible = true;
        }
    }

    #endregion

    #region PRESSED

    public delegate void OnClicked();

    public event OnClicked Clicked;

    #endregion

    #region COLOR

    public Color Color
    {
        get => L_Title.TextColor;
        set
        {
            L_Title.TextColor = value;
            imgForwardColor.TintColor = value;
            imgSourceColor.TintColor = value;
        }
    }

    #endregion

    private bool _is_init = false;

    public CustomButton()
	{
		InitializeComponent();
	}

    private void ContentView_SizeChanged(object sender, EventArgs e)
    {
        if (!_is_init)
        {
            _is_init = true;
            TitleChanged();
            ImageSourceChanged();
        }
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        Clicked?.Invoke();
    }
}