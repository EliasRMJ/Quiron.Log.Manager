namespace Quiron.Log.Manager
{
    public record struct ViewLogViewModel(
          string Text
        , string Type
        , DateTime Date
        , string EventCode = ""
        , string EventName = ""
        , string UserName= ""
     );
}