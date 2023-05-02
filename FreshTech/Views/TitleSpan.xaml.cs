namespace FreshTech.Views;

public partial class TitleSpan : ContentView
{
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

    public TitleSpan()
	{
		InitializeComponent();
	}
}