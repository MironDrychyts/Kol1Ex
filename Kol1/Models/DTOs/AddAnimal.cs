namespace Kol1.Models.DTOs;

public class AddAnimal
{
    public string name { get; set;}
    
    public string type { get; set;}
    
    public DateTime admissionDate { get; set;}
    
    public int ownerId { get; set;}

    public IEnumerable<AddProcedure> procedures { get; set; } = new List<AddProcedure>();
}