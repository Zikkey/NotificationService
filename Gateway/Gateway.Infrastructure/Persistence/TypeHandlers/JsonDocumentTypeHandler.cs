using System.Data;
using System.Text.Json;
using Dapper;

namespace Gateway.Infrastructure.Persistence.TypeHandlers;

public class JsonDocumentTypeHandler : SqlMapper.ITypeHandler
{
    public JsonSerializerOptions SerializerOptions { get; set; }

    public void SetValue(IDbDataParameter parameter, object value)
    {
        parameter.Value = value == null ? DBNull.Value : JsonSerializer.Serialize(value, SerializerOptions);
        parameter.DbType = DbType.String;
    }

    public object Parse(Type destinationType, object value)
    {
        return JsonSerializer.Deserialize(value.ToString(), destinationType, SerializerOptions);
    }
}
