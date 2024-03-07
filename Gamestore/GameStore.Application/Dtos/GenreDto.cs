using System.ComponentModel.DataAnnotations;
using GameStore.Domain.Entities;

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
        Description = genre.Description;
        Picture = genre.Picture;
        ParentGenreId = genre.ParentGenre?.Id;
    }

    public Guid? Id { get; set; }

    [Required]
    public string Name { get; set; }

    public string? Description { get; set; }

    public string? Picture { get; set; }

    public Guid? ParentGenreId { get; set; }
}
