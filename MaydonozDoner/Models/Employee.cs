namespace MaydonozDoner.Models
{
    public class Employee
    {
        public int Id {  get; set; }
        public string Name { get; set; }=string.Empty;
        public string State { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public int DepartmentId    { get; set; }
    public Department? Department { get; set; }
    }
}
