
using CommunityToolkit.Maui.Behaviors;

namespace FreshTech.Views;

public partial class CustomEntry : ContentView
{
    private bool _cancel_txt_changed_event = false;
	private bool is_init = false;
    private static readonly char[] numeric = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

    public string Text
    {
        get => entry.Text;
        set => entry.Text = value;
    }

    public Color TextColor
    {
        get => entry.TextColor;
        set => entry.TextColor = value;
    }

    public Color TitleColor
    {
        get => L_Title.TextColor; 
        set => L_Title.TextColor = value;
    }

    public string Placeholder
    {
        get => entry.Placeholder;
        set => entry.Placeholder = value;
    }

    public Color PlaceholderColor
    {
        get => entry.PlaceholderColor;
        set => entry.PlaceholderColor = value;
    }

    #region TITLE

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

    #endregion

    #region IMGSOURCE

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

    #endregion

    #region NUMBERONLY

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

    #endregion

    #region TIMERONLY

    public bool TimerOnly
    {
        get => (bool)GetValue(TimerOnlyProperty);
        set => SetValue(TimerOnlyProperty, value);
    }

    public static readonly BindableProperty TimerOnlyProperty =
        BindableProperty.Create("TimerOnly", typeof(bool), typeof(CustomEntry), false, propertyChanged: OnTimerOnlyChanged);

    private static void OnTimerOnlyChanged(BindableObject view, object oldValue, object newValue)
    {
        ((CustomEntry)view).TimerOnlyChanged();
    }

    private void TimerOnlyChanged()
    {
        if (is_init)
        {
            if (TimerOnly)
            {
                if (NumberOnly) NumberOnly = false;

                entry.Keyboard = Keyboard.Numeric;
                entry.Text = "00:00:00";
            }
            else
            {
                if(!NumberOnly)
                    entry.Keyboard = Keyboard.Default;
            }
        }
    }

    #endregion

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
            TimerOnlyChanged();
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
    
    public bool CheckIsNumber(out double result)
    {
        if (!string.IsNullOrWhiteSpace(Text) && CheckIsNumber(Text))
        {
            result = double.Parse(Text, System.Globalization.CultureInfo.InvariantCulture);
            return true;
        }
        result = double.NaN;
        return false;
    }

    private void entry_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_cancel_txt_changed_event)
        {
            _cancel_txt_changed_event = false;
            return;
        }

        if (TimerOnly)
        {
            if (!CheckTimer(e.NewTextValue))
            {
                entry.Text = e.OldTextValue;
            }
        }
        else if (NumberOnly)
        {
            // Rien
        }
    }

    private bool CheckTimer(string newtxt)
    {
        string[] datas = newtxt.Split(':');
        if (datas.Length == 3) { 

            return (datas.FirstOrDefault(x => x.Length == 0) == null);
            /*Func<int, string> getString = (int val) =>
            {
                string res = val.ToString();
                if(res.Length == 1)
                {
                    res = '0' + res;
                }
                return res;
            };

            int sec = Convert.ToInt32(datas[0]);
            int min = Convert.ToInt32(datas[1]);
            int hour = Convert.ToInt32(datas[2]);
            while(sec >= 60)
            {
                sec -= 60;
                min++;
            }
            while (min >= 60)
            {
                min -= 60;
                hour++;
            }

            toBind = getString(hour) + ':' + getString(min) + ':' + getString(sec);*/
        }
        else
        {
            return false;
            //toBind = oldtxt;

        }
    }
}