using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Core.interfaces;


namespace FirstProjectAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class CategoriesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoriesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        // GET: api
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var Result = _unitOfWork.categories.GetAll();
                return Ok(Result);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        
        }
  
        
        // GET api/CategoriesController/5
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var Result =_unitOfWork.categories.GetById(id);
                return Ok(Result);
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);

            }
        }
   
        // POST api/<CategoriesController>
        [HttpPost("AddCategory")]
        public IActionResult AddCategory([FromForm] Category model)
        {
            try
            {
               _unitOfWork.categories.Create(model);
                _unitOfWork.save();
                return Ok();
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
       
     // PUT api/<CategoriesController>/5
     //FromBody => Get Value in Export from Body Formats
     [HttpPut("Edit/{id}")]
     public IActionResult Edit(int id, [FromBody] Category model)
     {
         try
         {
             var category = _unitOfWork.categories.GetById(id);
             if (category!=null)
             {
                 category.Name = model.Name;
                 _unitOfWork.categories.Update(category);
                 _unitOfWork.save();

                 return Ok(category);
             }
             else
             {
                 return NotFound();
             }


         }catch(Exception ex)
         {
             return BadRequest(ex.Message);
         }
     }
      
    // DELETE api/<ValuesController>/5
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        try
        {
            if (id>0)
            {
                var category = _unitOfWork.categories.GetById(id);
                _unitOfWork.categories.Delete(category);
                _unitOfWork.save();

                return Ok();
            }
            else
            {
                throw new ArgumentException($"Entity with Id {id} not found.");
            }
        }
        catch(Exception ex)
        {

            return BadRequest(ex.Message);
        }
    
    }
     
     // DELETE api/categories/delete-multiple
  [HttpDelete("delete-deleteRange")]
  public IActionResult DeleteRange([FromBody] List<int> ids)
  {
      try
      {
          _unitOfWork.categories.DeleteRange(ids);
          _unitOfWork.save();

          return Ok();
      }
      catch (Exception ex)
      {
          return BadRequest(ex.Message);
      }
  }
    
    }

}
