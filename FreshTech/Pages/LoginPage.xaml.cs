namespace FreshTech.Pages;

using AppCore.Services;
using Microsoft.Maui.Controls;
//namespace FreshTech.Pages;

public partial class LoginPage : ContentPage
{
    int count = 0;
    public LoginPage()
	{
        InitializeComponent();
        TestConnect();
    }

    [Obsolete("test only")]
    private async void TestConnect()
    {
        await Task.Delay(5000);
        FTMClientManager clientManager = new FTMClientManager();
        bool r = await clientManager.EstablishConnection();
    }

    private void OnLoginClicked(object sender, System.EventArgs e)
    {
        // TODO: Add login logic here
    }

    private async void OnRegisterClicked(object sender, System.EventArgs e)
    {
       await Navigation.PushAsync(new RegisterPage());
    }
}