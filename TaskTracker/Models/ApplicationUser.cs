using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using TaskTracker.Models;

namespace TaskTracker.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int UsernameChangeLimit { get; set; } = 10;
        public byte[]? ProfilePicture { get; set; }

        // Navigation property for related tasks
        public ICollection<UserTask> Tasks { get; set; } = new List<UserTask>();
    }
}