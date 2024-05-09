using Kol1.Models;
using Kol1.Models.DTOs;

namespace Kol1.Repositories;

public interface IAnimalRepository
{
    public bool AnimalExists(int AnimalId);
    public InfoAnimal GetAnimalInfo(int AnimalId);

    public void AddAnimal(AddAnimal animal);
}