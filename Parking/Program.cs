using System.Resources;
using ScottPlot;
using Spectre.Console;

namespace Parking;
using Spectre;

class Program
{
    static void Main(string[] args)
    {
        if(args.Length < 3)
        {
            AnsiConsole.MarkupLine("[red]Usage: [/]Parking [green]total_parking_space_today[/] [blue]percent_yearly_decline[/] [yellow]/path/to/output.svg[/]");
            return;
        }

        if (!int.TryParse(args[0], out int total))
        {
            AnsiConsole.MarkupLine("[red]Error: [/]total_parking_space_today must be a number");
        };
        
        if (!float.TryParse(args[1], out float percent))
        {
            AnsiConsole.MarkupLine("[red]Error: [/]percent_yearly_decline must be a number");
        };
        
        try
        {
            using var fs = System.IO.File.Create(args[2]);
        }
        catch
        {
            AnsiConsole.MarkupLine("[red]Error: [/]output file path is invalid or not writable");
            return;
        }
        
        List<Rowset> data = new();
        
        data.Add(new Rowset()   // first line of data table
        {
            Year = DateTime.Now.Year,
            Total = total,
            Decline = 0,
            Remaining = total
        });
        
        
        do   // add new lines to data table until remaining parking spots are less than 1
        {
            AnsiConsole.Markup("[red].[/]");
            data.Add(data.Last().NextYear(percent));
        } 
        while (data.Last().Remaining > 1);

        AnsiConsole.MarkupLine($"\n\n\n[yellow] According to BILD newspaper, the city administration of Stuttgart managed around 15,000 parking spaces in [underline]the year 2014[/].[/]");
        AnsiConsole.MarkupLine($"[yellow] With a {percent} percent annual decrease of parking spots, it takes[/] [green]{data.FirstOrDefault(x => x.Total < total * 0.5).Year-DateTime.Now.Year} years[/][yellow] for the number of parking spaces to drop to 50 percent[/]");
        AnsiConsole.MarkupLine($"[yellow] and [/] [green]{data.Count} years[/][yellow] for the number of parking spaces to drop to 0.[/]\n\n\n");
        AnsiConsole.MarkupLine($"[italic]You can find a graph SVG file at {args[2]}[/]");
        
        double[] totals = data.Select(x => (double)x.Total).ToArray();
        double[] declines = data.Select(x => (double)x.Decline).ToArray();
        double[] remains = data.Select(x => (double)x.Remaining).ToArray();
        double[] x = data.Select(x => (double)x.Year).ToArray();
        
        var plt = new ScottPlot.Plot();
        
        var pp = plt.Add.Scatter(x, totals);
            pp.Color = Colors.Green.WithOpacity(.2);
            pp.LegendText = "available Parking";
            pp.FillY = false;
        
        var pd = plt.Add.Scatter(x, declines);
            pd.Color = Colors.DarkRed.WithOpacity(.2);
            pd.LegendText = "yearly Decline";
            pd.FillY = false;
            pd.Axes.YAxis = plt.Axes.Right;

        plt.Add.Palette = new ScottPlot.Palettes.SnowStorm();
            plt.XLabel("Year");
            plt.YLabel("Parking Spaces");
            plt.ShowLegend(Alignment.UpperRight);
            plt.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericFixedInterval(10);
            plt.Axes.Left.TickGenerator = new ScottPlot.TickGenerators.NumericAutomatic();
            plt.Title($"Development Stuttgart");
            plt.SaveSvg(args[2], 800, 600);
    }
}