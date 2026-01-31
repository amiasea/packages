namespace Amiasea.Loom.Projection
{
    public static class ProjectionOutputTypeExtensions
    {
        public static IProjectionOutputType List(this IProjectionOutputType elementType)
            => new ProjectionOutputListType(elementType);

        public static IProjectionOutputType NonNull(this IProjectionOutputType innerType)
            => new ProjectionOutputNonNullType(innerType);
    }
}