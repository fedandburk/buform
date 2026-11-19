namespace Buform;

public static class FormTexts
{
    private const string DefaultClear = "Clear";
    private const string DefaultCancel = "Cancel";
    private const string DefaultDone = "Done";
    private const string DefaultSearch = "Search";
    private const string DefaultIncrease = "Increase";
    private const string DefaultDecrease = "Decrease";
    private const string DefaultSelected = "Selected";
    private const string DefaultLoading = "Loading…";
    private const string DefaultNoItems = "No items";
    private const string DefaultLoadingFailed = "Failed to load items";

    public static string Clear { get; set; }
    public static string Cancel { get; set; }
    public static string Done { get; set; }
    public static string Search { get; set; }
    public static string Increase { get; set; }
    public static string Decrease { get; set; }
    public static string Selected { get; set; }
    public static string Loading { get; set; }
    public static string NoItems { get; set; }
    public static string LoadingFailed { get; set; }

    static FormTexts()
    {
        Clear = DefaultClear;
        Cancel = DefaultCancel;
        Done = DefaultDone;
        Search = DefaultSearch;
        Increase = DefaultIncrease;
        Decrease = DefaultDecrease;
        Selected = DefaultSelected;
        Loading = DefaultLoading;
        NoItems = DefaultNoItems;
        LoadingFailed = DefaultLoadingFailed;
    }
}
