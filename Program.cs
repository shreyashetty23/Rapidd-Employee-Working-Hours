
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Text.Json;
using System.Net.Http;
using System.Linq;
using ScottPlot;

HttpClient client = new HttpClient();
string url = "https://rc-vault-fap-live-1.azurewebsites.net/api/gettimeentries?code=vO17RnE8vuzXzPJo5eaLLjXjmRW07law99QTD90zat9FfOQJKKUcgQ==";
string response = await client.GetStringAsync(url);
List<TimeEntry>? employees =
    JsonSerializer.Deserialize<List<TimeEntry>>(response);
if (employees == null)
{
    Console.WriteLine("No records found.");
    return;
}
Console.WriteLine("Total records: " + employees.Count);
Dictionary<string, double> employeeHours = new Dictionary<string, double>();

foreach (var entry in employees)
{
    if (string.IsNullOrWhiteSpace(entry.EmployeeName))
    {
        continue;
    }

    if (entry.EndTimeUtc < entry.StarTimeUtc)
    {
        continue;
    }

    double hours = (entry.EndTimeUtc - entry.StarTimeUtc).TotalHours;
    string name = entry.EmployeeName;
    if (employeeHours.ContainsKey(name))
    {
        employeeHours[name] = employeeHours[name] + hours;
    }
    else
    {
        employeeHours[name] = hours;
    }
}
var totals = employeeHours
    .Select(item => new
    {
        Name = item.Key,
        TotalHours = item.Value
    })
    .OrderByDescending(item => item.TotalHours)
    .ToList();   
double grandTotal = totals.Sum(e => e.TotalHours);
Console.WriteLine("Total Working Hours: " + grandTotal);
foreach (var employee in totals)
{
    double percentage = employee.TotalHours / grandTotal * 100;

    Console.WriteLine(employee.Name + " - " +
                      percentage.ToString("F2") + "%");
}
foreach (var employee in totals)
{
    if (employee.TotalHours < 100)
    {
        Console.WriteLine("Less than 100 hours");
    }

    Console.WriteLine(employee.Name + " - " +
                      employee.TotalHours.ToString("F2") + " hours");
}
StringBuilder html = new StringBuilder();
html.AppendLine("<html>");
html.AppendLine("<head>");
html.AppendLine("<title>Employee Working Hours</title>");
html.AppendLine("<style>");
html.AppendLine("table { border-collapse: collapse; width: 70%; }");
html.AppendLine("th, td { border: 1px solid black; padding: 10px; }");
html.AppendLine("th { background-color: lightblue; }");
html.AppendLine(".low { background-color: #ffcccc; }");
html.AppendLine("</style>");
html.AppendLine("</head>");
html.AppendLine("<body>");
html.AppendLine("<h2>Employee Working Hours Report</h2>");
html.AppendLine("<table>");
html.AppendLine("<tr><th>Employee Name</th><th>Total Hours</th></tr>");
foreach (var employee in totals)
{
    string rowClass = "";
if (employee.TotalHours < 100)
{
    rowClass = "class='low'";
}
html.AppendLine("<tr " + rowClass + ">");

    html.AppendLine("<td>" + employee.Name + "</td>");
    html.AppendLine("<td>" + employee.TotalHours.ToString("F2") + "</td>");
    html.AppendLine("</tr>");
}
html.AppendLine("</table>");
html.AppendLine("</body>");
html.AppendLine("</html>");
File.WriteAllText("index.html", html.ToString());
Console.WriteLine("HTML report created successfully!");
var plot = new ScottPlot.Plot();
double[] values = totals.Select(e => e.TotalHours).ToArray();
var pie = plot.Add.Pie(values);
for (int i = 0; i < pie.Slices.Count; i++)
{
    double percentage = values[i] / grandTotal * 100;
     pie.Slices[i].Label = totals[i].Name + " " +  percentage.ToString("F2") + "%";
}
pie.SliceLabelDistance = 1.4;
plot.ShowLegend();
plot.Axes.Frameless();
plot.HideGrid();
plot.SavePng("piechart.png", 1000, 800);
Console.WriteLine("Pie chart created successfully!");


public class TimeEntry
{
    public string? EmployeeName { get; set; }
    public DateTime StarTimeUtc { get; set; }
    public DateTime EndTimeUtc { get; set; }
}
