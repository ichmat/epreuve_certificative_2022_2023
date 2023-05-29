using FreshTech.Pages;
using FreshTech.Tools;

namespace FreshTech.Views.Calibration;

public partial class CalFormular : ContentView
{
	private readonly CalibrationPage _parent;
	private readonly bool _withActivityEntry;
	private readonly Color _standars_title_color;
	private readonly Color _standars_text_color;

    private double _last_sec_entry = 0;

    public CalFormular(CalibrationPage parent, bool withActivityEntry)
	{
		InitializeComponent();
		_parent = parent;
		_withActivityEntry = withActivityEntry;
		_standars_title_color = EntryWeight.TitleColor;
        _standars_text_color = EntryWeight.TextColor;
    }

	private async Task<bool> CheckEntry()
	{
		bool isOk = true;

        string errorList = string.Empty;

        bool weightParsed = EntryWeight.CheckIsNumber(out double weightKg);

        if (string.IsNullOrWhiteSpace(EntryWeight.Text) || !weightParsed || weightKg <= 50)
		{
			EntryWeight.TitleColor = ColorsTools.Danger;
			EntryWeight.TextColor = ColorsTools.DangerDark;
            isOk = false;
            if (weightParsed)
            {
                errorList += "Poids : insérez une valeur correct" + Environment.NewLine;
            }
            else if (weightKg <= 30)
            {
                errorList += "Poids : doit être supérieur à 30 Kg" + Environment.NewLine;
            }
        }
		else
		{
            EntryWeight.TitleColor = _standars_title_color;
            EntryWeight.TextColor = _standars_text_color;
        }

        bool sizeParsed = EntrySize.CheckIsNumber(out double sizeCm);

        if (string.IsNullOrWhiteSpace(EntrySize.Text) || !sizeParsed || sizeCm < 50)
        {
            EntrySize.TitleColor = ColorsTools.Danger;
            EntrySize.TextColor = ColorsTools.DangerDark;
            isOk = false;
            if (sizeParsed)
            {
                errorList += "Taille : insérez une valeur correct" + Environment.NewLine;
            }
            else if (sizeCm <= 50)
            {
                errorList += "Taille : doit être supérieur à 50 cm" + Environment.NewLine;
            }
        }
        else
        {
            EntrySize.TitleColor = _standars_title_color;
            EntrySize.TextColor = _standars_text_color;
        }

        bool distanceParsed = EntryDistance.CheckIsNumber(out double distanceKm);

        if (_withActivityEntry && (string.IsNullOrWhiteSpace(EntryDistance.Text) || !distanceParsed || distanceKm < 1))
        {
            EntryDistance.TitleColor = ColorsTools.Danger;
            EntryDistance.TextColor = ColorsTools.DangerDark;
            isOk = false;
            if (distanceParsed)
            {
                errorList += "Distance : insérez une valeur correct" + Environment.NewLine;
            }
            else if (distanceKm <= 50)
            {
                errorList += "Distance : doit être égal ou supérieur à 1 kg" + Environment.NewLine;
            }
        }
        else if(_withActivityEntry)
        {
            EntryDistance.TitleColor = _standars_title_color;
            EntryDistance.TextColor = _standars_text_color;
            
        }

        bool timeParsed = TimeSpan.TryParse(EntryTime.Text, System.Globalization.CultureInfo.InvariantCulture, out TimeSpan result);

        if (_withActivityEntry && (string.IsNullOrWhiteSpace(EntryTime.Text) || !timeParsed || result.TotalMinutes < 10))
        {
            EntryTime.TitleColor = ColorsTools.Danger;
            EntryTime.TextColor = ColorsTools.DangerDark;
            isOk = false;
            L_Time.IsVisible = false;
            if (timeParsed)
            {
                errorList += "Temps : insérez une valeur correct : 0.00:00:00 = J.HH.MM.SS" + Environment.NewLine;
            }
            else if (result.TotalMinutes < 10)
            {
                errorList += "Temps : doit être égal ou supérieur à 10 minutes" + Environment.NewLine;
            }
        }
        else if(_withActivityEntry)
        {
            EntryTime.TitleColor = _standars_title_color;
            EntryTime.TextColor = _standars_text_color;
            double sec = result.TotalSeconds;
            if(sec != _last_sec_entry)
            {
                _last_sec_entry = sec;
                isOk = false;
                ShowTimeResult(sec);
                Validate.Text = "TERMINER";
                Validate.BackgroundColor = ColorsTools.Success;
            }
        }

        if(errorList != string.Empty)
        {
            await _parent.DisplayAlert("Entrées incorrect", errorList, "Ok");
        }

        return isOk;
    }

    private void ShowTimeResult(double sec)
    {
        L_Time.IsVisible = true;

        int nbDay = 0;
        int nbHour = 0;
        int nbMin = 0;
        while (sec >= 86400)
        {
            sec -= 86400;
            nbDay++;
        }
        while (sec >= 3600)
        {
            sec -= 3600;
            nbHour++;
        }
        while (sec >= 60)
        {
            sec -= 60;
            nbMin++;
        }

        string txt = string.Empty;

        if(nbDay > 0)
        {
            txt += nbDay.ToString() + ' ';

            if (nbDay > 1)
                txt += "Jours";
            else
                txt += "Jour";
        }

        if (nbHour > 0)
        {
            txt += ' ' + nbHour.ToString() + ' ';

            if (nbHour > 1)
                txt += "Heures";
            else
                txt += "Heure";
        }

        if (nbMin > 0)
        {
            txt += ' ' + nbMin.ToString() + ' ';

            if (nbMin > 1)
                txt += "Minutes";
            else
                txt += "Minute";
        }

        if (sec > 0)
        {
            txt += ' ' + Math.Round(sec).ToString() + ' ';

            if (sec > 1)
                txt += "Secondes";
            else
                txt += "Seconde";
        }

        L_Time.Text = txt;
    }

    private async void Finish_Clicked(object sender, EventArgs e)
    {
		if (await CheckEntry())
		{

		}
    }

    private void ContentView_Loaded(object sender, EventArgs e)
    {
        L_Time.IsVisible = false;
    }
}