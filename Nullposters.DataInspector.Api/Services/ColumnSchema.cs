namespace Nullposters.DataInspector.Api.Services
{
    public class ColumnSchema
    {
        public string? ColumnName { get; set; }
        public string? DataType { get; set; }
        public int? CharacterMaximumLength { get; set; }
        public string? IsNullable { get; set; }
        public string? ColumnDefault { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsForeignKey { get; set; }
    }
}
