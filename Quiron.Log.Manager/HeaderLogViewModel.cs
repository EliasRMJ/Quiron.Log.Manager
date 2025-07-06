namespace Quiron.Log.Manager
{
    public struct HeaderLogViewModel
    {
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int ErrorCount { get; set; }
        public int CriticalCount { get; set; }
        public int WarningCount { get; set; }
        public int InformationCount { get; set; }
        public int UserCount { get; set; }

        public ViewLogViewModel[] Logs { get; set; }
    }
}