using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Infrastructures.UnitOfWork;
using Core.interfaces;
using Infrastructures.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace FirstProjectAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _Context;
        protected IUnitOfWork _unitOfWork;

        public ProductsController(IUnitOfWork unitOfWork,AppDbContext appDbContext)
        {
            _unitOfWork = unitOfWork;
            _Context = appDbContext;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
               
                var product = _unitOfWork.products.GetAllWithIncludes(c => c.Category);
                return Ok(product);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetProduct/{id}")]
        public IActionResult GetProduct(int id)
        {
            var product = _unitOfWork.products.GetById(id);
            if (product==null)
            {
                return NotFound($"Product Id {id} Not Found");
            }
            return Ok(product);
        }

        [HttpPost]
        public IActionResult AddProduct([FromForm] Product product)
        {
            try
            {
                _unitOfWork.products.Create(product);
                _unitOfWork.save();
                return Ok();
            }
            catch (Exception ex)
            {
               
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public IActionResult Edit(int id, [FromBody] Product updatedProduct)
        {
            try
            {
                var existingProduct = _unitOfWork.products.GetById(id);

                if (existingProduct == null)
                {
                    return NotFound($"Product with ID {id} not found.");
                }


                existingProduct.Name = updatedProduct.Name;
                existingProduct.Price = updatedProduct.Price;
                existingProduct.Discount = updatedProduct.Discount;
                existingProduct.Quantity = updatedProduct.Quantity;
                existingProduct.CategoryId = updatedProduct.CategoryId;
                existingProduct.Price = updatedProduct.Price;


                _unitOfWork.products.Update(existingProduct);
                _unitOfWork.save();

                return Ok("Product updated successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to update product. Error: {ex.Message}");
            }
        }


        [HttpDelete("{Id}")]
        public IActionResult Delete(int Id)
        {
            try
            {
                if (Id>0)
                {
                    var product = _unitOfWork.products.GetById(Id);
                    _unitOfWork.products.Delete(product);
                    _unitOfWork.save();
                }
                return Ok("Product delete successfully");

            }catch(Exception ex)
            {
                return BadRequest($"Failed to delete product. Error: {ex.Message}");
            }
        }
    }
}
