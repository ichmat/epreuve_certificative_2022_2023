using AppCore.Models;

namespace FreshTech.Views.Activity;

public partial class ContentActivity : ContentView
{
	public Courses Course { get; private set; }

    public ContentActivity(Courses course)
	{
		InitializeComponent();
		Course = course;
		content.LabelTop = Course.DateDebut.ToString("dd/MM/yyyy");
		content.LabelMiddle = Math.Round(Course.DistanceKm, 2).ToString() + " Km";
	}

    private void content_LabelBottomClicked()
    {
		// TODO : mettre en place la consultation de l'activité.
		// ⚠ Pas de design prévue pour le moment
    }
}