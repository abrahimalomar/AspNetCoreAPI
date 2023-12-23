using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Core.interfaces;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Authorization;

namespace FirstProjectAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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


        [HttpPatch("{id}")]
        public IActionResult UpdateByPatch(
            [FromBody] JsonPatchDocument<Category> categorymode, 
            [FromRoute] int id)
        {
            try
            {
                var category = _unitOfWork.categories.GetById(id);
                if (category == null)
                {
                    return BadRequest();
                }

                categorymode.ApplyTo(category, ModelState); // يتم تمرير ModelState هنا للتحقق من الأخطاء بعد تطبيق التغييرات

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState); // في حالة وجود أخطاء في Model، يتم إرجاعها كاستجابة BadRequest
                }

                _unitOfWork.save();
                return Ok(category);
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }


    }

}
