namespace FreshTech.Pages;

public partial class PlusPage : ContentPage
{
	public PlusPage()
	{
		InitializeComponent();
	}
    
    private async void OnTitleLabelTapped(object sender, EventArgs e)
    {
        await Shell.Current.Navigation.PopAsync();

    }

    private async void OnInventaireClicked(object sender, EventArgs e)
    {

    }
    
    private async void OnReparationClicked(object sender, EventArgs e)
    {

    }
    
    private async void OnConstructionclicked(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new ConstructionPage());
    }
    
    private async void OnAmeliorationClicked(object sender, EventArgs e)
    {


    }
}