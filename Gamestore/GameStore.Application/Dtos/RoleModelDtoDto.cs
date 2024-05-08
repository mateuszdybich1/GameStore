using System.Text.Json.Serialization;
using GameStore.Domain.UserEntities;

namespace GameStore.Application.Dtos;

public class RoleModelDtoDto
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public List<Permissions> Permissions { get; set; }

    public RoleModelDto Role { get; set; }
}