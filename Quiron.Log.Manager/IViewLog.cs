namespace Quiron.Log.Manager
{
    public interface IViewLog
    {
        Task<HeaderLogViewModel> GetAsync(DateOnly begin, DateOnly end
            , string? text = "", string? eventName = "", string? type = ""
            , string folder = "Logs", int pageNumber = -1, int pageSize = -1
            , bool isParent = false);
    }
}