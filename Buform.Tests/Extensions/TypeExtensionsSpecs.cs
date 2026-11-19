using AwesomeAssertions;
using Xunit;

namespace Buform.Tests;

public static class TypeExtensionsSpecs
{
    public interface IFirst;

    public interface IThird;

    public interface ISecond : IThird;

    public class Base : IFirst;

    public sealed class Derived : Base, ISecond;

    public static class GetRegistrationTypes
    {
        public sealed class DerivedType
        {
            private readonly IReadOnlyList<Type> _types;

            public DerivedType()
            {
                _types = typeof(Derived).GetRegistrationTypes();
            }

            [Fact]
            public void ListsTypeThenBaseClassesThenInterfacesThenTheirParents()
            {
                _types
                    .Should()
                    .Equal(
                        typeof(Derived),
                        typeof(Base),
                        typeof(ISecond),
                        typeof(IFirst),
                        typeof(IThird)
                    );
            }
        }
    }
}
