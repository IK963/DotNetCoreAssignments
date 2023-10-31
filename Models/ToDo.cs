namespace DotNetCoreAssignments.Models
{
    public class ToDo
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string? UserName { get; set; }
    }
}
