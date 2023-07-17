using FreshTech.Views.Game;

namespace FreshTech.Pages;

public partial class InventairePage : ContentPage
{
    private readonly GameEngine _engine;

    #region TITLE
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create("Title", typeof(string), typeof(InventairePage), string.Empty, propertyChanged: OnTitleChanged);

    private static void OnTitleChanged(BindableObject view, object oldValue, object newValue)
    {
        ((InventairePage)view).TitleChanged();
    }

    private void TitleChanged()
    {
        L_Title.Text = Title;
    }
    #endregion
    public InventairePage(GameEngine gameEngine)
	{
        _engine = gameEngine;
        GetData();
        InitializeComponent();

    }

    private void GetData()
    {
        var objetPossedete = _engine.GetObjetsWithQuantity().Where(x => x.Value != 0).ToList();
        var test = objetPossedete.Select(x => x.Key.Nom).FirstOrDefault();
        //Title = test;
    }

    private async void TitleSpan_GoBack()
    {
        await Shell.Current.Navigation.PopAsync();
    }
}