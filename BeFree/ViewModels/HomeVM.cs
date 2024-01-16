using BeFree.Models;

namespace BeFree.ViewModels
{
    public class HomeVM
    {
        public List<Employee>? Employees { get; set; }
        public List<Position>? Positions { get; set; }
        public List<Product>? Products { get; set; }
        public List<Category>? Categories { get; set; }

    }
}
