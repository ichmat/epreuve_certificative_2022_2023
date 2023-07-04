using Microsoft.Maui.Controls.PlatformConfiguration;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net;
using CommunityToolkit.Maui.Views;


namespace FreshTech.Pages;

public partial class ProfilePage : ContentPage
{
	public ProfilePage()
	{
        AvatarView avatarView = new()
        {
            ImageSource = "dotnet_bot.png"
        };
        Button button = new Button
        {
            Text = "Objecifs",
            ImageSource = new FileImageSource
            {
                File = "ionic-ios-information-circle-outline.svg"
            },
            ContentLayout = new Button.ButtonContentLayout(Button.ButtonContentLayout.ImagePosition.Left, 20)
        };
        Content = avatarView;
        InitializeComponent();

    }
    private async void OnObjectifsClicked(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new ObjectifPage());
    }

    private async void OnConfidentialiteClicked(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new ConfidentialitePage());
    }

    private void OnCompteClicked(object sender, EventArgs e)
    {
        DisplayAlert("click", "OnCompteClicked", "canceml");
        
    } 
    private async void OnproposClicked(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new ProposPage());
    } 

    private async void OnLanguageClicked(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new LangagePage());

    }

   




}