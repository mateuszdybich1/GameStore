using GameStore.Domain.UserEntities;

namespace GameStore.Application.Dtos;

public class UserModelDto
{
    public UserModelDto()
    {
    }

    public UserModelDto(string name)
    {
        Name = name;
    }

    public UserModelDto(PersonModel personModel)
    {
        ID = personModel.Id;
        Name = personModel.Name;
    }

    public UserModelDto(UserModelMicroserviceDto userModelMicroserviceDto)
    {
        Name = userModelMicroserviceDto.Email;
    }

    public Guid? ID { get; set; }

    public string Name { get; set; }
}
