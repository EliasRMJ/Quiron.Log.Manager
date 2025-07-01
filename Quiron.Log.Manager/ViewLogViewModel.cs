namespace Quiron.Log.Manager
{
    public record struct ViewLogViewModel(
          int Index
        , string Text
        , string Type
        , DateTime Date
        , string EventCode = ""
        , string EventName = ""
        , string UserName= ""
     );
}