namespace Parking;

public class Rowset
{
    public int Year { get; set; } = 0;
    public float Total { get; set; } = 0;
    public float Decline { get; set; } = 0;
    public float Remaining { get; set; } = 0;
    
    public Rowset NextYear(float percentage)
    {
        float newDecline = ((Remaining / 100) * percentage);
        return new Rowset()
        {
            Year = this.Year + 1,
            Total = this.Remaining,  
            Decline = newDecline,  
            Remaining = this.Remaining - newDecline 
        };
    }
}