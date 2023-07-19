namespace FreshTech.Views;

public partial class DataContent : ContentView
{
	public string Title
	{
		get => L_Title.Text;
		set => L_Title.Text = value;
	}

    public ImageSource ImageSource
    {
        get => img.Source; set => img.Source = value;
    }

    public double ScaleImageSource
    {
        get => img.Scale;
        set
        {
            Dispatcher.Dispatch(() => img.Scale = value);
        }
    }

    #region COLOR_LEFT_ICON

    public Color ColorLeftIcon
    {
        get => imgSourceColor.TintColor;
        set => imgSourceColor.TintColor = value;
    }

    #endregion

    #region COLOR_TEXT

    public Color ColorText
    {
        get => L_Title.TextColor;
        set
        {
            L_Title.TextColor = value;
            L_SubTitle.TextColor = value;
            L_RightText.TextColor = value;
        }
    }

    #endregion

    #region COLOR

    public Color Color
    {
        get => L_Title.TextColor;
        set
        {
            L_Title.TextColor = value;
            L_SubTitle.TextColor = value;
            L_RightText.TextColor = value;
            imgRightSourceColor.TintColor = value;
            imgSourceColor.TintColor = value;
        }
    }

    #endregion

    #region SUBTITLE

    public string SubTitle
    {
        get => (string)GetValue(SubTitleProperty);
        set => SetValue(SubTitleProperty, value);
    }

    public static readonly BindableProperty SubTitleProperty =
        BindableProperty.Create("SubTitle", typeof(string), typeof(DataContent), string.Empty, propertyChanged: OnSubTitleChanged);

    private static void OnSubTitleChanged(BindableObject view, object oldValue, object newValue)
    {
        ((DataContent)view).SubTitleChanged();
    }

    private void SubTitleChanged()
    {
        if (!_is_init) return;

        Dispatcher.Dispatch(() =>
        {
            L_SubTitle.Text = SubTitle;
            if (string.IsNullOrWhiteSpace(SubTitle))
            {
                L_SubTitle.IsVisible = false;
            }
            else
            {
                L_SubTitle.IsVisible = true;
            }
        });

    }

    #endregion

    #region RIGHT_TEXT

    public string RightText
    {
        get => (string)GetValue(RightTextProperty);
        set => SetValue(RightTextProperty, value);
    }

    public static readonly BindableProperty RightTextProperty =
        BindableProperty.Create("RightText", typeof(string), typeof(DataContent), string.Empty, propertyChanged: OnRightTextChanged);

    private static void OnRightTextChanged(BindableObject view, object oldValue, object newValue)
    {
         ((DataContent)view).RightTextChanged();
    }

    private void RightTextChanged()
    {
        if (!_is_init) return;
        Dispatcher.Dispatch(() =>
        {
            if (string.IsNullOrWhiteSpace(RightText))
            {
                colRightTxt.Width = new GridLength(0, GridUnitType.Absolute);
                L_RightText.IsVisible = false;
            }
            else
            {
                colRightTxt.Width = new GridLength(0, GridUnitType.Auto);
                L_RightText.Text = RightText;
                L_RightText.IsVisible = true;
            }
        });
    }

    #endregion

    #region IMG_SOURCE_RIGHT

    public string ImageSourceRight
    {
        get => (string)GetValue(ImageSourceRightProperty);
        set => SetValue(ImageSourceRightProperty, value);
    }

    public static readonly BindableProperty ImageSourceRightProperty =
        BindableProperty.Create("ImageSourceRight", typeof(string), typeof(DataContent), string.Empty, propertyChanged: OnImageSourceRightChanged);

    private static void OnImageSourceRightChanged(BindableObject view, object oldValue, object newValue)
    {
        ((DataContent)view).ImageSourceRightChanged();
    }

    private void ImageSourceRightChanged()
    {
        if (!_is_init) return;
        Dispatcher.Dispatch(() =>
        {
            if (string.IsNullOrWhiteSpace(ImageSourceRight))
            {
                colRightIcon.Width = new GridLength(25, GridUnitType.Absolute);
                imgRight.IsVisible = false;
            }
            else
            {
                colRightIcon.Width = new GridLength(75, GridUnitType.Absolute);
                imgRight.Source = ImageSourceRight;
                imgRight.IsVisible = true;
            }
        });
    }

    #endregion

    #region PRESSED

    public delegate void OnClicked();

    public event OnClicked Clicked;

    #endregion

    private bool _is_init = false;

    public DataContent()
	{
		InitializeComponent();
	}

    private void ContentView_SizeChanged(object sender, EventArgs e)
    {
        if (!_is_init)
        {
            _is_init = true;
            SubTitleChanged();
            RightTextChanged();
            ImageSourceRightChanged();
        }
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        Clicked?.Invoke();
    }
}