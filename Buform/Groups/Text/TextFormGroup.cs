namespace Buform;

public sealed class TextFormGroup : FormGroup<IFormItem>
{
    public string? HeaderLabel
    {
        get;
        set
        {
            field = value;

            NotifyPropertyChanged();
        }
    }

    public string? FooterLabel
    {
        get;
        set
        {
            field = value;

            NotifyPropertyChanged();
        }
    }

    public TextFormGroup(string? headerLabel = null, string? footerLabel = null)
    {
        HeaderLabel = headerLabel;
        FooterLabel = footerLabel;
    }
}
