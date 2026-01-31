namespace Amiasea.Loom.Projection
{

    public static class ProjectionInputTypeExtensions
    {
        public static IProjectionInputType List(this IProjectionInputType elementType)
            => new ProjectionListInputType(elementType);

        public static IProjectionInputType NonNull(this IProjectionInputType innerType)
            => new ProjectionNonNullInputType(innerType);
    }
}