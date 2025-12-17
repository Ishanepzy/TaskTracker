using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Models
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

        [Display(Name = "Assigned To")]
        [StringLength(256)]
        public string? AssignedTo { get; set; }

        /*[Required(ErrorMessage = "The Userid field is required.")]*/
        public string? UserId { get; internal set; }
        public ApplicationUser? User { get; set; }
    }
}
