
namespace FreshTech.Views;

public partial class ButtonBadge : ContentView
{
    #region TITLE

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create("Title", typeof(string), typeof(ButtonBadge), string.Empty, propertyChanged: OnTitleChanged);

    private static void OnTitleChanged(BindableObject view, object oldValue, object newValue)
    {
        ((ButtonBadge)view).TitleChanged();
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
        BindableProperty.Create("ImageSource", typeof(string), typeof(ButtonBadge), string.Empty, propertyChanged: OnImageSourceChanged);

    private static void OnImageSourceChanged(BindableObject view, object oldValue, object newValue)
    {
        ((ButtonBadge)view).ImageSourceChanged();
    }

    private void ImageSourceChanged()
    {
        if (!_is_init) return;
        if (string.IsNullOrWhiteSpace(ImageSource))
        {
            colImg.Width = new GridLength(0);
            img.IsVisible = false;
        }
        else
        {
            colImg.Width = new GridLength(40, GridUnitType.Absolute);
            img.Source = ImageSource;
            img.IsVisible = true;
        }
    }

    #endregion

    #region BADGE

    public string Badge
    {
        get => (string)GetValue(BadgeProperty);
        set => SetValue(BadgeProperty, value);
    }

    public static readonly BindableProperty BadgeProperty =
        BindableProperty.Create("Badge", typeof(string), typeof(ButtonBadge), string.Empty, propertyChanged: OnBadgeChanged);

    private static void OnBadgeChanged(BindableObject view, object oldValue, object newValue)
    {
        ((ButtonBadge)view).BadgeChanged();
    }

    private void BadgeChanged()
    {
        if (!_is_init) return;
        if (string.IsNullOrWhiteSpace(Badge))
        {
            colBadge.Width = new GridLength(0);
            B_Badge.IsVisible = false;
        }
        else
        {
            colBadge.Width = new GridLength(25, GridUnitType.Absolute);
            L_Badge.Text = Badge;
            B_Badge.IsVisible = true;
        }
    }

    #endregion

    #region PRESSED

    public delegate void OnClicked();

    public event OnClicked Clicked;

    #endregion

    private bool _is_init = false;

    public ButtonBadge()
	{
		InitializeComponent();
	}

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        Clicked?.Invoke();
    }

    private void ContentView_SizeChanged(object sender, EventArgs e)
    {
        if (!_is_init)
        {
            _is_init = true;
            TitleChanged();
            ImageSourceChanged();
            BadgeChanged();
        }
    }
}