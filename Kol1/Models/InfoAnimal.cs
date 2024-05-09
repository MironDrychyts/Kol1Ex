namespace Kol1.Models;

public class InfoAnimal
{
    public int id { get; set;}
    
    public string name { get; set;}
    
    public string type { get; set;}
    
    public DateTime admissionDate { get; set;}
    
    public Owner owner { get; set;}

    public List<Procedure> procedures { get; set;}
}