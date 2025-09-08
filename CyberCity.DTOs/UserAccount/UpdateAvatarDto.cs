using Microsoft.AspNetCore.Http;

public class UpdateAvatarDto
{
    public IFormFile Avatar { get; set; }
    // Add other properties if needed
}