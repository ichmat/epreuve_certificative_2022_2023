using AppCore.Models;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using System.Collections.ObjectModel;

namespace FreshTech.Views.Dashboard;

public partial class CustomGraph : ContentView
{
	public ObservableCollection<Courses> Courses { get; set; } = new ObservableCollection<Courses>();

    private ObservedData _observed_data = ObservedData.Distance;
    public ObservedData DataObserved
    {
        get => _observed_data;
        set
        {
            if(value != _observed_data)
            {
                _observed_data = value;
                ResetView();
            }
        }
    }

    private bool _is_init = false;

    public CustomGraph()
	{
		InitializeComponent();
	}

    public void AddRange(IEnumerable<Courses> courses)
    {
        Courses.CollectionChanged -= Courses_CollectionChanged;
        foreach(Courses course in courses)
        {
            Courses.Add(course);
        }
        Courses.CollectionChanged += Courses_CollectionChanged;
        ResetView();
    }

    private void InitView()
    {
        _is_init = true;
    }

    private void ResetView()
    {
        if (_is_init)
        {
            ClearData();

            DateTime lastDateTime = DateTime.MinValue;
            double value = 0;
            foreach (Courses c in Courses.OrderBy(x => x.DateDebut))
            {
                if(SameDate(lastDateTime, c.DateDebut))
                {
                    value = AddValue(value, c);
                }
                else if (lastDateTime != DateTime.MinValue)
                {
                    AddData(lastDateTime, Math.Round(value, 3 ));
                    lastDateTime = c.DateDebut;
                    value = AddValue(0, c);
                }
                else
                {
                    lastDateTime = c.DateDebut;
                    value = AddValue(value, c);
                }
            }

            if (lastDateTime != DateTime.MinValue)
            {
                AddData(lastDateTime, Math.Round(value, 3));
            }

            Invalidate();
        }
    }

    private void ClearData()
    {
        ((GraphicsDrawableGraph)graph.Drawable).Values.Clear();
    }

    private void AddData(DateTime date, double value)
    {
        ((GraphicsDrawableGraph)graph.Drawable).Values.Add(date.ToString("dd/MM/yy"), value);
    }

    private double AddValue(double value, Courses courses) =>
        DataObserved switch
        {
            ObservedData.Distance => value + courses.DistanceKm,
            ObservedData.MeanSpeed => (value + courses.VitesseMoyenKmH) / 2,
            ObservedData.TotalTimeActivity => value + courses.TempsSec,
            _ => throw new NotSupportedException("enum ObservedData not supported")
        };

    private bool SameDate(DateTime first, DateTime second) => 
        first.Year == second.Year && 
        first.Month == second.Month &&
        first.Day == second.Day;

    private void ContentView_Unloaded(object sender, EventArgs e)
    {
        Courses.CollectionChanged -= Courses_CollectionChanged;
    }

    private void Courses_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        Dispatcher.Dispatch(ResetView);
    }

    private void ContentView_SizeChanged(object sender, EventArgs e)
    {
        if (!_is_init)
        {
            InitView();
            ResetView();
            Courses.CollectionChanged += Courses_CollectionChanged;
        }
        else
        {
            Invalidate();
        }
    }

    public void Invalidate() => graph.Invalidate();
}

public enum ObservedData
{
    MeanSpeed = 0,
    Distance = 1,
    TotalTimeActivity = 2
}