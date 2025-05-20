using System.Text.RegularExpressions;

namespace Quiron.Log.Manager
{
    public partial class ViewLog: IViewLog
    {
        [GeneratedRegex(@"\[(.*?)\]")]
        private static partial Regex RegexType();

        [GeneratedRegex(@"\] (.*?) -")]
        private static partial Regex RegexEvent();

        public async Task<ViewLogViewModel[]> GetAsync(DateOnly begin, DateOnly end, string text = "", string folder = "Logs")
        {
            var logDirectory = Path.Combine(Directory.GetCurrentDirectory(), folder);
            IEnumerable<string> allLines = [];

            while (begin <= end)
            {
                var filePath = Path.Combine(logDirectory, $"log-{begin:yyyy-MM-dd}.txt");

                if (File.Exists(filePath))
                {
                    var lines = await File.ReadAllLinesAsync(filePath);
                    allLines = [.. allLines, .. lines];
                }

                begin = begin.AddDays(1);
            }

            if (!string.IsNullOrWhiteSpace(text))
                allLines = [.. allLines.Where(line => line.Contains(text, StringComparison.OrdinalIgnoreCase))];

            var viewLogViewModels = new ViewLogViewModel[allLines.Count()];
            for (int i = 0; i < allLines.Count(); i++)
            {
                var newText = allLines.ElementAt(i).IndexOf(" -", allLines.ElementAt(i).IndexOf(" -") + 2) is int x && x != -1 ? allLines.ElementAt(i)[(x + 2)..].TrimStart() : string.Empty;

                viewLogViewModels[i].Text = newText;
                viewLogViewModels[i].Type = RegexType().Matches(allLines.ElementAt(i))[0].Value.Replace("[", string.Empty).Replace("]", string.Empty);
                viewLogViewModels[i].Date = DateTime.Parse(allLines.ElementAt(i)[..20]);

                var events = RegexEvent().Matches(allLines.ElementAt(i))[0].Value.Split('|');
                viewLogViewModels[i].EventCode = events[0].Replace("] -", string.Empty).Trim();
                viewLogViewModels[i].EventName = events[1].Replace("-", string.Empty).Trim();
                viewLogViewModels[i].UserName = events[2].Replace("-", string.Empty).Trim();
            }

            return [.. viewLogViewModels.OrderByDescending(order => order.Date)];
        }
    }
}