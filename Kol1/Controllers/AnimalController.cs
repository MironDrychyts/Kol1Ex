using Kol1.Models.DTOs;
using Kol1.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Kol1.Controllers;

[ApiController]
// [Route("/api/animals")]
[Route("/api/[controller]")]
public class AnimalController : ControllerBase
{
    private readonly IAnimalRepository _animalRepository;

    public AnimalController(IAnimalRepository animalRepository)
    {
        _animalRepository = animalRepository;
    }

    [HttpGet]
    public IActionResult GetAnimal(int id)
    {
        if (!_animalRepository.AnimalExists(id))
        {
            return NotFound("This animal does not exists");
        }
        
        var animal = _animalRepository.GetAnimalInfo(id);
        
        return Ok(animal);
    }
    
    [HttpPost]
    public IActionResult AddAnimal(AddAnimal animal)
    {
        _animalRepository.AddAnimal(animal);
        
       
        return Created("/api/animals", null);
    
    }
}