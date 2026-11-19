namespace Buform;

public class LabeledFormGroup : FormGroup<IFormItem>, ILabeledFormGroup
{
    public virtual string? HeaderLabel
    {
        get;
        set
        {
            field = value;

            NotifyPropertyChanged();
        }
    }

    public virtual string? FooterLabel
    {
        get;
        set
        {
            field = value;

            NotifyPropertyChanged();
        }
    }

    public LabeledFormGroup(string? headerLabel = null, string? footerLabel = null)
    {
        HeaderLabel = headerLabel;
        FooterLabel = footerLabel;
    }
}
