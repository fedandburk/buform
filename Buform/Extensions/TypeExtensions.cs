using System.Diagnostics.CodeAnalysis;

namespace Buform.Extensions;

internal static class TypeExtensions
{
    [UnconditionalSuppressMessage(
        "Trimming",
        "IL2070",
        Justification = "Registrations reference each registered interface with typeof, so the trimmer keeps it on every instantiated type."
    )]
    [UnconditionalSuppressMessage(
        "Trimming",
        "IL2075",
        Justification = "Registrations reference each registered interface with typeof, so the trimmer keeps it on every instantiated type."
    )]
    public static List<Type> GetInterfacesTopDown(this Type? type)
    {
        var interfaces = new List<Type>();

        while (type != null)
        {
            var all = type.GetInterfaces();

            interfaces.AddRange(
                all.Except(all.SelectMany(i => i.GetInterfaces()))
                    .Except(type.BaseType?.GetInterfaces() ?? [])
            );

            type = type.BaseType;
        }

        return interfaces;
    }
}
