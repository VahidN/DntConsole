using Microsoft.EntityFrameworkCore;

namespace DntConsole.Entities;

[Index(nameof(UrlHash), IsUnique = true)]
public class BlogPost
{
    public int Id { set; get; }

    [MaxLength] public required string Url { set; get; }

    [MaxLength] public required string Content { set; get; }

    [StringLength(64)] public required string UrlHash { set; get; }
}