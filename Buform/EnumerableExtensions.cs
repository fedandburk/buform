using System.Collections;

namespace Buform;

public static class EnumerableExtensions
{
    public static int IndexOf(this IEnumerable enumerable, object value)
    {
        if (enumerable == null)
        {
            throw new ArgumentNullException(nameof(enumerable));
        }

        return enumerable.IndexOf(value, EqualityComparer<object>.Default);
    }

    public static int IndexOf(this IEnumerable enumerable, object value, IEqualityComparer comparer)
    {
        if (enumerable == null)
        {
            throw new ArgumentNullException(nameof(enumerable));
        }

        if (comparer == null)
        {
            throw new ArgumentNullException(nameof(comparer));
        }

        var enumerator = enumerable.GetEnumerator();

        using var disposable = enumerator as IDisposable;

        for (var index = 0;; index++)
        {
            if (!enumerator.MoveNext())
            {
                return -1;
            }

            if (enumerator.Current == null)
            {
                return index;
            }

            if (comparer.Equals(enumerator.Current, value))
            {
                return index;
            }
        }
    }

    public static int IndexOf<T>(this IEnumerable<T> enumerable, T value)
    {
        if (enumerable == null)
        {
            throw new ArgumentNullException(nameof(enumerable));
        }

        return enumerable.IndexOf(value, EqualityComparer<T>.Default);
    }

    public static int IndexOf<T>(this IEnumerable<T> enumerable, T value, IEqualityComparer<T> comparer)
    {
        if (enumerable == null)
        {
            throw new ArgumentNullException(nameof(enumerable));
        }

        if (comparer == null)
        {
            throw new ArgumentNullException(nameof(comparer));
        }

        return enumerable
            .Select((item, index) => new { Item = item, Index = index })
            .FirstOrDefault(item => comparer.Equals(item.Item, value))?.Index ?? -1;
    }
}