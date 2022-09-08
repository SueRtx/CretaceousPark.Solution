using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CretaceousPark.Models;

namespace CretaceousPark.Controllers
{
  [Produces("application/json")]
  [Route("api/[controller]")]
  [ApiController]
  public class AnimalsController : ControllerBase
  {
    private readonly CretaceousParkContext _db;

    public AnimalsController(CretaceousParkContext db)
    {
      _db = db;
    }


    /// <summary>
    /// Animal List
    /// </summary>
    /// <remarks>
    ///
    /// Sample request:
    /// GET /api/animals
    ///     
    /// </remarks>
    /// 
    /// <returns>Animal List</returns>
    /// <response code="200">Returns Animal List</response>
    /// <response code="400">If the animal is null</response> 
    
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesDefaultResponseType]
    [HttpGet]
    public async Task<List<Animal>> Get(string species, string name, int minimumAge)
    {
      IQueryable<Animal> query = _db.Animals.AsQueryable();

      if (species != null)
      {
        query = query.Where(entry => entry.Species == species);
      }

      if (name != null)
      {
        query = query.Where(entry => entry.Name == name);
      }

      if (minimumAge > 0)
      {
        query = query.Where(entry => entry.Age >= minimumAge);
      }

      return await query.ToListAsync();
    }

    
    /// <summary>
    /// Return individual animal base by Id
    /// </summary>
    /// <remarks>
    ///
    /// Sample request:
    /// GET /api/animals/1
    ///     
    /// </remarks>
    /// 
    /// <returns>Return animal base by Id</returns>
    /// <response code="200">Returns Animal</response>
    /// <response code="400">If the animal is null</response> 
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesDefaultResponseType]
    [HttpGet("{id}")]
    public async Task<ActionResult<Animal>> GetAnimal(int id)
    {
        var animal = await _db.Animals.FindAsync(id);

        if (animal == null)
        {
            return NotFound();
        }

        return animal;
    }


    /// <summary>
    /// Update Animal 
    /// </summary>
    /// <remarks>
    ///
    /// Sample request:
    /// DELETE /api/animals/1 
    ///     
    /// </remarks>
    /// 
    /// <returns>Delete animal in API</returns>
    /// <response code="201">Animal Deleted Successfully</response>
    /// <response code="400">If the animal is null</response> 
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesDefaultResponseType]
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, Animal animal)
    {
      if (id != animal.AnimalId)
      {
        return BadRequest();
      }

      _db.Entry(animal).State = EntityState.Modified;

      try
      {
        await _db.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException)
      {
        if (!AnimalExists(id))
        {
          return NotFound();
        }
        else
        {
          throw;
        }
      }

      return NoContent();
    }


    /// <summary>
    /// Creates animal.
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /Todo
    ///     {
    ///        "id": 1,
    ///        "name": "Animal1",
    ///        "Species": "Species1"
    ///     }
    ///
    /// </remarks>
    /// 
    /// <returns>A newly created Animal</returns>
    /// <response code="201">Returns the newly created animal</response>
    /// <response code="400">If the animal is null</response> 
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Animal>> Post(Animal animal)
    {
      _db.Animals.Add(animal);
      await _db.SaveChangesAsync();

      return CreatedAtAction(nameof(GetAnimal), new { id = animal.AnimalId }, animal);
    }



    /// <summary>
    /// Delete Animal 
    /// </summary>
    /// <remarks>
    ///
    /// 
    ///     
    /// </remarks>
    /// 
    /// <returns>Animal List</returns>
    /// <response code="201">Animal Updated successfully</response>
    /// <response code="400">If the animal is null</response>    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAnimal(int id)
    {
      var animal = await _db.Animals.FindAsync(id);
      if (animal == null)
      {
        return NotFound();
      }

      _db.Animals.Remove(animal);
      await _db.SaveChangesAsync();

      return NoContent();
    }

    private bool AnimalExists(int id)
    {
      return _db.Animals.Any(e => e.AnimalId == id);
    }
  }
}