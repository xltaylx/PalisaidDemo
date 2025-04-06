public class Appointment 
{
    public int Id {get;set;}
    public int PatientId {get;set;}
    public int LocationId {get;set;}
    public DateTime Date {get;set;}
    public string Reason {get;set;} = default!;
}