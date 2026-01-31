namespace Amiasea.Loom.Schema
{
    public sealed class FieldDependency
    {
        public string TargetField { get; private set; }
        public string WhenField { get; private set; }
        public string WhenEquals { get; private set; }

        public FieldDependency(string targetField, string whenField, string whenEquals)
        {
            TargetField = targetField;
            WhenField = whenField;
            WhenEquals = whenEquals;
        }
    }
}