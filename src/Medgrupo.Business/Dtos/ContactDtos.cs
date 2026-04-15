using Medgrupo.Data.Entities;

namespace Medgrupo.Business.Dtos;

public interface IContactInput
{
    string Name { get; }
    DateTime BirthDate { get; }
}

public record ContactCreateDto(string Name, DateTime BirthDate, Gender Gender) : IContactInput;

public record ContactUpdateDto(string Name, DateTime BirthDate, Gender Gender) : IContactInput;

public record ContactResponseDto(
    Guid Id,
    string Name,
    DateTime BirthDate,
    Gender Gender,
    int Age,
    bool Active,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
