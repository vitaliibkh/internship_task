namespace project.Models;

public class Account {
    public long Id { get; set; }
    public decimal CurrentBalance { get; set; }
    public DateTime LastTransaction { get; set; }
    public int CustomerId { get; set; }
}
