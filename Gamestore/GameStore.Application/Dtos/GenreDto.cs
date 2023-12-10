using System.ComponentModel.DataAnnotations;
using GameStore.Infrastructure.Entities;

namespace GameStore.Application.Dtos;
public class GenreDto
{
    public GenreDto()
    {
    }

    public GenreDto(Genre genre)
    {
        Id = genre.Id;
        Name = genre.Name;
        ParentGerneId = genre.ParentGerneId;
    }

    public Guid Id { get; private set; }

    [Required]
    public string Name { get; set; }

    public Guid ParentGerneId { get; set; }
}
