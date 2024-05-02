using System.Text.Json.Serialization;
using GameStore.Domain.UserEntities;

namespace GameStore.Application.Dtos;

public class AccessPageDto
{
    public string? TargetID { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Permissions TargetPage { get; set; }
}
