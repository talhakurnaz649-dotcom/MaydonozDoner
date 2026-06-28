namespace MaydonozDoner.Models
{
    public class Salary
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }
        public decimal Amount { get; set; }
    public DateTime Date {  get; set; }
    }
}
