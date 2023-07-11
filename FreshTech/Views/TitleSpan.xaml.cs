namespace FreshTech.Views;

public partial class TitleSpan : ContentView
{
    #region TITLE

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly BindableProperty TitleProperty =
		BindableProperty.Create("Title", typeof(string), typeof(TitleSpan), string.Empty, propertyChanged: OnTitleChanged);

    private static void OnTitleChanged(BindableObject view, object oldValue, object newValue)
    {
        ((TitleSpan)view).TitleChanged();
    }

    private void TitleChanged()
    {
        L_Title.Text = Title;
    }

    #endregion

    #region GOBACK

    public delegate void OnGoBack();

    public event OnGoBack GoBack;

    #endregion

    private bool _is_init = false;

    public TitleSpan()
	{
		InitializeComponent();
	}

    private void ContentView_SizeChanged(object sender, EventArgs e)
    {
        if (!_is_init)
        {
            _is_init = true;
            Dispatcher.Dispatch(Init);
        }
    }

    private void Init()
    {
        // s'il y a des évènements connectés, alors affiché la flèche de retour
        if (GoBack?.GetInvocationList().Length > 0)
        {
            imgBack.IsVisible = true;
        }
        else
        {
            imgBack.IsVisible = false;
        }
    }

    private void imgBack_Clicked(object sender, EventArgs e)
    {
        GoBack?.Invoke();
    }
}