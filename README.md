# Employee Working Hours Report

## About the Project

This is a C# project that gets employee time-entry details from a REST API and calculates how many hours each employee has worked.

Based on the results, it creates an HTML report and a pie chart to show the working hours of each employee.

## What the Project Does

- Fetches employee details from the API.
- Calculates the total working hours for each employee.
- Displays employees in descending order of working hours.
- Highlights employees who have worked less than 100 hours.
- Creates an HTML page with the employee details.
- Generates a pie chart showing each employee's share of the total working hours.

## Technologies Used

- C#
- .NET 10
- REST API
- HTML and CSS
- ScottPlot

## Generated Files

- `index.html` – Displays employee names and their total working hours.
- `piechart.png` – Shows the percentage of working hours for each employee.

## How to Run the Project

1. Open the project folder in VS Code.
2. Open the terminal.
3. Run `dotnet restore` to restore the required packages.
4. Run `dotnet run` to execute the project.

After running the project, the HTML report and pie chart will be created in the project folder.