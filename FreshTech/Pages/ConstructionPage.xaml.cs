using AppCore.Models;
using AppCore.Services.GeneralMessage.Args;
using AppCore.Services.GeneralMessage.Response;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using static AppCore.Services.NecessaryData;

namespace FreshTech.Pages;

public partial class ConstructionPage : ContentPage, INotifyPropertyChanged
{
    StackLayout stackLayout = new StackLayout();
    ScrollView scrollView = new ScrollView();

    public ConstructionPage()
    {
        InitializeComponent();
        BindingContext = this;
        Content = CreateMainLayout();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        GetData();
    }

    private async void OnTitleLabelTapped(object sender, EventArgs e)
    {
        await Shell.Current.Navigation.PopAsync();
    }
    private async void OnTitleLabelTapped(object obj)
    {
        // Ajoutez ici la logique de navigation pour revenir au menu précédent
        await Shell.Current.Navigation.PopAsync();
    }

    private async void GetData()
    {
        ResponseGetNecessaryDataVillage infoTown = await App.client.SendAndGetResponse<ResponseGetNecessaryDataVillage>(new EPGetNecessaryDataVillage());
        var infoTownconsrtuction = infoTown.ConstructionInfos.ToList();

        stackLayout.Children.Clear(); // Effacer les anciennes cartes (frames) du stackLayout

        foreach (ConstructionInfo construction in infoTownconsrtuction)
        {
            Frame frame = CreateRessourceFrame(construction);
            stackLayout.Children.Add(frame);
        }
    }

    private View CreateMainLayout()
    {
        StackLayout mainStackLayout = new StackLayout { Margin = new Thickness(20) };

        StackLayout headerStackLayout = new StackLayout { Orientation = StackOrientation.Horizontal };

        Button backButton = new Button { Text = "<", Margin = new Thickness(0, 0, 20, 0) };
        backButton.Clicked += OnTitleLabelTapped;
        headerStackLayout.Children.Add(backButton);

        Label titleLabel = new Label { Text = "Constructions", FontSize = 30, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };
        titleLabel.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(OnTitleLabelTapped) });
        headerStackLayout.Children.Add(titleLabel);

        mainStackLayout.Children.Add(headerStackLayout);

        scrollView.Content = stackLayout;
        scrollView.VerticalOptions = LayoutOptions.FillAndExpand; // Occupe toute la hauteur de la page
        scrollView.HorizontalOptions = LayoutOptions.FillAndExpand; // Occupe toute la largeur de la page
        scrollView.Margin = new Thickness(10); // Ajoutez une marge de 10 autour de la ScrollView
        mainStackLayout.Children.Add(scrollView);

        return mainStackLayout;
    }

    private Frame CreateRessourceFrame(ConstructionInfo constructionInfo)
    {
        Frame frame = new Frame
        {
            BackgroundColor = Color.FromHex("#EFEFEF"),
            Margin = new Thickness(0, 0, 0, 10),
            Padding = new Thickness(10)
        };

        StackLayout stackLayout = new StackLayout();

        Label titleLabel = new Label
        {
            Text = constructionInfo.Nom,
            FontSize = 24,
            //TextColor = Color.Black,
            Margin = new Thickness(0, 0, 0, 10)
        };
        stackLayout.Children.Add(titleLabel);

        //Image image = new Image
        //{
        //    Source = constructionInfo.ImagePath,
        //    WidthRequest = 100,
        //    HeightRequest = 100,
        //    Margin = new Thickness(0, 0, 0, 10)
        //};
        //stackLayout.Children.Add(image);

        //Label quantityLabel = new Label
        //{
        //    Text = $"Quantité nécessaire: {constructionInfo.}",
        //    FontSize = 16,
        //    TextColor = Color.Gray,
        //    Margin = new Thickness(0, 0, 0, 5)
        //};
        //stackLayout.Children.Add(quantityLabel);

        Label descriptionLabel = new Label
        {
            Text = constructionInfo.Type.ToString(),
            FontSize = 16,
            Margin = new Thickness(0, 0, 0, 5)
        };
        stackLayout.Children.Add(descriptionLabel);

        Label timeLabel = new Label
        {
            Text = $"Temps de contruction: {constructionInfo.TempsSecConstruction} secondes",
            FontSize = 16,
            //TextColor = Color.Gray,
            Margin = new Thickness(0, 0, 0, 5)
        };
        stackLayout.Children.Add(timeLabel);

        ProgressBar progressBar = new ProgressBar
        {
            Progress = constructionInfo.TempsSecConstruction / 187200,
            HeightRequest = 10,
            Margin = new Thickness(0, 0, 0, 10)
        };
        stackLayout.Children.Add(progressBar);

        Label timerLabel = new Label
        {
            Text = constructionInfo.TempsSecConstruction.ToString("hh\\:mm"),
            FontSize = 20,
            //TextColor = Color.Black,
            HorizontalOptions = LayoutOptions.Center
        };
        stackLayout.Children.Add(timerLabel);

        frame.Content = stackLayout;

        return frame;
    }
}