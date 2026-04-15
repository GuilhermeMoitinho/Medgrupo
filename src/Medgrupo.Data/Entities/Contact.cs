namespace Medgrupo.Data.Entities;

public enum Gender
{
    Male = 1,
    Female = 2,
    Other = 3
}

public class Contact : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public DateTime BirthDate { get; private set; }
    public Gender Gender { get; private set; }
    public bool Active { get; private set; } = true;

    public int Age
    {
        get
        {
            var today = DateTime.Today;
            var age = today.Year - BirthDate.Year;
            if (BirthDate.Date > today.AddYears(-age)) age--;
            return age;
        }
    }

    protected Contact() { }

    public Contact(string name, DateTime birthDate, Gender gender)
    {
        Name = name;
        BirthDate = birthDate;
        Gender = gender;
        Active = true;
    }

    public void Update(string name, DateTime birthDate, Gender gender)
    {
        Name = name;
        BirthDate = birthDate;
        Gender = gender;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        Active = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        Active = true;
        UpdatedAt = DateTime.UtcNow;
    }
}
