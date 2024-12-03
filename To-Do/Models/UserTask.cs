using System.ComponentModel.DataAnnotations;

namespace To_Do.Models
{
    public class UserTask
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime Deadline { get; set; }
        public int Priority { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime CompletedDate { get; set; }

        /*[Required(ErrorMessage = "The Userid field is required.")]*/
        public string? UserId { get; internal set; }
        public ApplicationUser? User { get; set; }
    }
}
