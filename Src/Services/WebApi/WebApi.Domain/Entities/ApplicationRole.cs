using Microsoft.AspNetCore.Identity;

namespace WebApi.Domain.Entities;
public class ApplicationRole : IdentityRole<Guid>
{
    public string? Description { get; set; }
}
