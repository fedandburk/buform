namespace Buform.Extensions;

internal static class TypeExtensions
{
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
