namespace Quiron.Log.Manager
{
    public interface IViewLog
    {
        Task<ViewLogViewModel[]> Get(DateOnly begin, DateOnly end, string text = "", string Folder = "Logs");
    }
}