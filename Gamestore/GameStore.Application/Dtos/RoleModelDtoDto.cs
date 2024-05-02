using System.Text.Json.Serialization;
using GameStore.Domain.UserEntities;

namespace GameStore.Application.Dtos;

public class RoleModelDtoDto
{
    public RoleModelDto Role { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public List<Permissions> Permissions { get; set; }
}