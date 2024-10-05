namespace Nullposters.DataInspector.Api.Services
{
    public class TableSchema
    {
        public string TableName { get; set; } = string.Empty;
        public List<ColumnSchema> Columns { get; set; } = string.Empty;
    }
}
