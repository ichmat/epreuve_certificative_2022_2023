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
    private void OnMenu1Clicked(object sender, EventArgs e)
    {
    //    Menu1Grid.IsVisible = true;
    //    Menu2Grid.IsVisible = false;
    //    Menu3Grid.IsVisible = false;
    }

    private void OnMenu2Clicked(object sender, EventArgs e)
    {
        //Menu1Grid.IsVisible = false;
        //Menu2Grid.IsVisible = true;
        //Menu3Grid.IsVisible = false;
    }

    private void OnMenu3Clicked(object sender, EventArgs e)
    {
        //Menu1Grid.IsVisible = false;
        //Menu2Grid.IsVisible = false;
        //Menu3Grid.IsVisible = true;
    }
   




}