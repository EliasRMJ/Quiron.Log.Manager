using System.Text;
using System.Text.RegularExpressions;

namespace Quiron.Log.Manager
{
    public partial class ViewLog : IViewLog
    {
        [GeneratedRegex(@"\[(.*?)\]")]
        private static partial Regex RegexType();

        [GeneratedRegex(@"\] (.*?) -")]
        private static partial Regex RegexEvent();

        [GeneratedRegex(@"^\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}")]
        private static partial Regex DateRegex();

        public async Task<ViewLogViewModel[]> GetAsync(DateOnly begin, DateOnly end, string? text = "", string folder = "Logs")
        {
            var logDirectory = Path.Combine(Directory.GetCurrentDirectory(), folder);
            IEnumerable<string> allLines = [];

            while (begin <= end)
            {
                var filePath = Path.Combine(logDirectory, $"log-{begin:yyyy-MM-dd}.txt");

                if (File.Exists(filePath))
                {
                    var lines = await File.ReadAllLinesAsync(filePath);
                    var logEntries = ExtractLogEntries(lines);

                    allLines = [.. allLines, .. logEntries];
                }

                begin = begin.AddDays(1);
            }

            if (!string.IsNullOrWhiteSpace(text))
                allLines = [.. allLines.Where(line => line.Contains(text, StringComparison.OrdinalIgnoreCase))];

            var viewLogViewModels = new ViewLogViewModel[allLines.Count()];
            for (int i = 0; i < allLines.Count(); i++)
            {
                try
                {
                    var line = allLines.ElementAt(i);
                    var newText = line.IndexOf(" -", line.IndexOf(" -") + 2) is int x && x != -1 ? line[(x + 2)..].TrimStart() : string.Empty;

                    viewLogViewModels[i].Text = newText;
                    viewLogViewModels[i].Type = RegexType().Matches(line)[0].Value.Replace("[", string.Empty).Replace("]", string.Empty);
                    viewLogViewModels[i].Date = DateTime.Parse(line[..20]);

                    var events = RegexEvent().Matches(line)[0].Value.Split('|');
                    viewLogViewModels[i].EventCode = events[0].Replace("] -", string.Empty).Trim();
                    viewLogViewModels[i].EventName = events[1].Replace("-", string.Empty).Trim();
                    viewLogViewModels[i].UserName = events.Length.Equals(3) ? events[2].Replace("-", string.Empty).Trim() : string.Empty;
                }
                catch { continue; }
            }

            return [.. viewLogViewModels.OrderByDescending(order => order.Date)];
        }

        static List<string> ExtractLogEntries(string[] lines)
        {
            var result = new List<string>();
            var regex = DateRegex();
            var builder = new StringBuilder();

            foreach (var line in lines)
            {
                if (regex.IsMatch(line) && builder.Length > 0)
                {
                    result.Add(builder.ToString().Trim());
                    builder.Clear();
                }

                if (builder.Length > 0)
                    builder.AppendLine();

                builder.Append(line);
            }

            if (builder.Length > 0)
                result.Add(builder.ToString().Trim());

            return result;
        }
    }
}