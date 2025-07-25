﻿using System.Text;
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

        public async Task<HeaderLogViewModel> GetAsync(DateOnly begin, DateOnly end
            , string? text = "", string? eventName = "", string? type = ""
            , string folder = "logs", int pageNumber = -1, int pageSize = -1, bool isParent = false)
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var parentDirectory = Directory.GetParent(currentDirectory)!.FullName;
            var logDirectory = isParent ? Path.Combine(parentDirectory, folder) : Path.Combine(currentDirectory, folder);
          
            IEnumerable<string> allLines = [];
            ViewLogViewModel[] logs = [];

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

            if (!string.IsNullOrWhiteSpace(eventName))
                allLines = [.. allLines.Where(line => line.Contains(eventName, StringComparison.OrdinalIgnoreCase))];

            var viewLogViewModels = new ViewLogViewModel[allLines.Count()];
            for (int i = 0; i < allLines.Count(); i++)
            {
                try
                {
                    var line = allLines.ElementAt(i);
                    var newText = line.IndexOf(" -", line.IndexOf(" -") + 2) is int x && x != -1 ? line[(x + 2)..].TrimStart() : string.Empty;

                    viewLogViewModels[i].Index = (i + 1);
                    viewLogViewModels[i].Text = newText;
                    viewLogViewModels[i].Type = RegexType().Matches(line)[0].Value.Replace("[", string.Empty).Replace("]", string.Empty);
                    viewLogViewModels[i].Date = DateTime.Parse(line[..20]);

                    var events = RegexEvent().Matches(line)[0].Value.Split('|');

                    viewLogViewModels[i].EventCode = events[0].Replace("] -", string.Empty).Trim();
                    viewLogViewModels[i].EventName = events[1].Replace("-", string.Empty).Trim();
                    viewLogViewModels[i].UserName = events[2].Replace("-", string.Empty).Trim();
                    viewLogViewModels[i].Method = events[3].Replace("-", string.Empty).Trim();
                }
                catch { continue; }
            }

            if (!string.IsNullOrWhiteSpace(type))
                viewLogViewModels = [.. viewLogViewModels
                    .Where(vm => vm.Type.Equals(type, StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(order => order.Date)];

            logs = pageNumber > 0 ? 
                [.. viewLogViewModels.OrderByDescending(order => order.Date).Skip((pageNumber - 1) * pageSize).Take(pageSize)] 
              : [.. viewLogViewModels.OrderByDescending(order => order.Date)];

            var headerLogViewModel = new HeaderLogViewModel
            {
                TotalCount = viewLogViewModels.Length,
                ErrorCount = viewLogViewModels.Count(log => log.Type.Equals("Error", StringComparison.OrdinalIgnoreCase)),
                CriticalCount = viewLogViewModels.Count(log => log.Type.Equals("Critical", StringComparison.OrdinalIgnoreCase)),
                WarningCount = viewLogViewModels.Count(log => log.Type.Equals("Warning", StringComparison.OrdinalIgnoreCase)),
                InformationCount = viewLogViewModels.Count(log => log.Type.Equals("Information", StringComparison.OrdinalIgnoreCase)),
                UserCount = viewLogViewModels.GroupBy(group => group.UserName).Count(),
                Logs = [.. logs.OrderByDescending(order => order.Date)]
            };

            return headerLogViewModel;
        }

        static List<string> ExtractLogEntries(string[] lines)
        {
            var result = new List<string>();
            var builder = new StringBuilder();
            var regex = DateRegex();

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