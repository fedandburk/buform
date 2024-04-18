namespace Buform.Example.MvvmCross.iOS;

// ReSharper disable UnusedParameter.Local
// ReSharper disable UnusedMember.Global
// ReSharper disable once UnusedType.Global

[Preserve(AllMembers = true)]
public class Linker
{
    public static void Include(UIBarButtonItem barButtonItem)
    {
        barButtonItem.Clicked += (sender, args) => { };
    }
}
