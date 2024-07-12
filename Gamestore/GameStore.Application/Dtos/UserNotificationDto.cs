using System.Text.Json.Serialization;
using GameStore.Domain.UserEntities;

namespace GameStore.Application.Dtos;

public class UserNotificationDto
{
    public Guid? UserId { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public List<UserNotificationType> Notifications { get; set; }
}
