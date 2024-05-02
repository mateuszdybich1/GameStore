using GameStore.Domain.UserEntities;

namespace GameStore.Application.Dtos;

public class RoleModelDto
{
    public RoleModelDto()
    {
    }

    public RoleModelDto(RoleModel roleModel)
    {
        ID = roleModel.Id;
        Name = roleModel.Name.ToString();
    }

    public Guid? ID { get; set; }

    public string Name { get; set; }
}
