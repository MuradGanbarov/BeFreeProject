namespace BeFree.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string? ImageURL { get; set; }
        public int? PositionId { get; set; }
        public Position? Position { get; set; }
        public string? Facebook { get; set; }
        public string? Instagram { get; set; }
        public string? GooglePlus { get; set; }
    }
}
