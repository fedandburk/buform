using System.Diagnostics.CodeAnalysis;

namespace Buform;

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
    private static List<Type> GetInterfacesTopDown(this Type? type)
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

    [UnconditionalSuppressMessage(
        "Trimming",
        "IL2075",
        Justification = "Registrations reference each registered interface with typeof, so the trimmer keeps it on every instantiated type."
    )]
    public static List<Type> GetRegistrationTypes(this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        var types = new List<Type>();

        for (
            var current = type;
            current != null && current != typeof(object);
            current = current.BaseType
        )
        {
            types.Add(current);
        }

        var interfaces = type.GetInterfacesTopDown();

        for (var index = 0; index < interfaces.Count; index++)
        {
            var parentInterfaces = interfaces[index]
                .GetInterfaces()
                .Where(parentInterface => !interfaces.Contains(parentInterface));

            interfaces.AddRange(parentInterfaces);
        }

        types.AddRange(interfaces);

        return types;
    }
}
