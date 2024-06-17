using System.Linq.Expressions;
using CommunityToolkit.Mvvm.Input;

namespace Buform;

public partial class RandomNumberGeneratorItem : TextFormItem<int>
{
    private const int MinValue = 0;
    private const int MaxValue = 1000;

    private readonly Random _random;

    public RandomNumberGeneratorItem(Expression<Func<int>> property)
        : base(property)
    {
        _random = new Random();
    }

    [RelayCommand]
    private void Generate()
    {
        Value = _random.Next(MinValue, MaxValue);
    }
}
