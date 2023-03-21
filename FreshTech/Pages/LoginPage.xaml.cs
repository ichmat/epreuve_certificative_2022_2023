namespace FreshTech.Pages;

public partial class LoginPage : ContentPage
{
    int count = 0;
    public LoginPage()
	{
        InitializeComponent();


        var usernameEntry = new Entry
        {
            Placeholder = "Nom d'utilisateur"
        };
    }
    private void OnCounterClicked(object sender, EventArgs e)
    {

    }
}