
using CommunityToolkit.Maui.Behaviors;

namespace FreshTech.Views;

public partial class CustomEntry : ContentView
{
	private bool is_init = false;
    private string _text = string.Empty;
    private static readonly char[] numeric = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

    public string Text
    {
        get => entry.Text;
        set => entry.Text = value;
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create("Title", typeof(string), typeof(CustomEntry), string.Empty, propertyChanged: OnTitleChanged);

    private static void OnTitleChanged(BindableObject view, object oldValue, object newValue)
    {
        ((CustomEntry)view).TitleChanged();
    }

    private void TitleChanged()
    {
        if (is_init)
        {
            L_Title.Text = Title;
            if (string.IsNullOrWhiteSpace(Title))
            {
                L_Title.IsVisible = false;
                L_Title.HeightRequest = 0;
            }
            else
            {
                L_Title.IsVisible = true;
                L_Title.HeightRequest = double.NaN;
            }
        }
    }

    public string ImgSource
    {
        get => (string)GetValue(ImgSourceProperty);
        set => SetValue(ImgSourceProperty, value);
    }

    public static readonly BindableProperty ImgSourceProperty =
        BindableProperty.Create("ImgSource", typeof(string), typeof(CustomEntry), string.Empty, propertyChanged: OnImgSourceChanged);

    private static void OnImgSourceChanged(BindableObject view, object oldValue, object newValue)
    {
        ((CustomEntry)view).ImgSourceChanged();
    }

    private void ImgSourceChanged()
    {
        if (is_init)
        {
            if (string.IsNullOrWhiteSpace(ImgSource))
            {
                imgIcon.WidthRequest = 0;
                imgIcon.IsVisible = false;
                imgIcon.Source = null;
                imgIcon.Margin = new Thickness(0);
            }
            else
            {
                imgIcon.WidthRequest = double.NaN;
                imgIcon.IsVisible = true;
                imgIcon.Source = ImgSource;
                imgIcon.Margin = new Thickness(0,0,10,0);
            }
        }
    }

    public bool NumberOnly
    {
        get => (bool)GetValue(NumberOnlyProperty);
        set => SetValue(NumberOnlyProperty, value);
    }

    public static readonly BindableProperty NumberOnlyProperty =
        BindableProperty.Create("NumberOnly", typeof(bool), typeof(CustomEntry), false, propertyChanged: OnNumberOnlyChanged);

    private static void OnNumberOnlyChanged(BindableObject view, object oldValue, object newValue)
    {
        ((CustomEntry)view).NumberOnlyChanged();
    }

    private void NumberOnlyChanged()
    {
        if (is_init)
        {
            if (NumberOnly)
            {
                if (!CheckIsNumber(entry.Text))
                {
                    entry.Text = string.Empty;
                }
                entry.Keyboard = Keyboard.Numeric;
            }
            else
            {
                entry.Keyboard = Keyboard.Default;
            }
        }
    }

    public CustomEntry()
	{
		InitializeComponent();
	}

    private void ContentView_Loaded(object sender, EventArgs e)
    {
        if (!is_init)
        {
            is_init = true;
            TitleChanged();
            ImgSourceChanged();
            NumberOnlyChanged();
        }
    }

    private bool CheckIsNumber(string text)
    {
        bool haveOneDot = false;
        for (int i = 0; i < text.Length; i++)
        {
            if (!numeric.Contains(text[i]))
            {
                if (!haveOneDot && 
                    (text[i] == '.' || text[i] == ','))
                {
                    haveOneDot = true;
                }
                else
                {
                    return false;
                }
            }
        }

        return true;
    }
    
}