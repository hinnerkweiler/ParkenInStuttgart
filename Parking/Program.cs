using System.Resources;
using ScottPlot;
using Spectre.Console;

namespace Parking;
using Spectre;

class Program
{
    static void Main(string[] args)
    {
        int.TryParse(args[0], out int total);
        float.TryParse(args[1], out float percent);
        List<Rowset> data = new();
        
        data.Add(new Rowset()
        {
            Year = 2024,
            Total = total,
            Decline = 0,
            Remaining = total
        });
        
        
        do
        {
            AnsiConsole.Markup("[red].[/]");
            data.Add(data.Last().NextYear(percent));
        } 
        while (data.Last().Remaining > 1);

        AnsiConsole.MarkupLine($"\n\n\n[yellow] Laut BILD-Zeitung bewitschaftete die Stadtverwaltung Stuttgart im [underline]Jahr 2014[/] rund 15.000 Parkplätze.[/]");
        AnsiConsole.MarkupLine($"[yellow] Bei {percent} Prozent jährlicher Abnahme dauert es[/] [green]{data.FirstOrDefault(x => x.Total < total * 0.5).Year-2024} Jahre[/][yellow], bis die Anzahl der Parkplätze auf 50 Prozent gesunken ist[/]");
        AnsiConsole.MarkupLine($"[yellow] und [/] [green]{data.Count} Jahre[/][yellow], bis die Anzahl der Parkplätze auf 0 sinkt.[/]\n\n\n");

        double[] totals = data.Select(x => (double)x.Total).ToArray();
        double[] declines = data.Select(x => (double)x.Decline).ToArray();
        double[] remains = data.Select(x => (double)x.Remaining).ToArray();
        double[] x = data.Select(x => (double)x.Year).ToArray();
        
        var plt = new ScottPlot.Plot();
        
        var pp = plt.Add.Scatter(x, totals);
        pp.Color = Colors.Green.WithOpacity(.2);
        pp.LegendText = "verfügb. Parkplätze";
        pp.FillY = false;
        
        plt.Add.Palette = new ScottPlot.Palettes.SnowStorm();
        plt.XLabel("Jahre");
        plt.YLabel("Parkplätze");
        plt.ShowLegend(Alignment.UpperRight);
        plt.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericFixedInterval(10);
        plt.Axes.Left.TickGenerator = new ScottPlot.TickGenerators.NumericAutomatic();
        plt.Title($"Entwicklung Stuttgart");
        plt.SaveSvg(args[2], 800, 600);
    }
}