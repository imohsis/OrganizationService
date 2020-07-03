using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrgDAL;

namespace OrgAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : Controller
    {
        OrganizationDbContext dbContext;
        public DepartmentsController(OrganizationDbContext _dbContext)
        {
            dbContext = _dbContext;
        }
        //[HttpGet]
        //public ActionResult<IEnumerable<Department>> Get()
        //{
        //    var depts = dbContext.Departments.ToList();
        //    return depts;
        //}
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var depts = await dbContext.Departments.Include(x => x.Employees)
                 .Select(x => new Department
                 {
                     Did = x.Did,
                     DName = x.DName,
                     Description = x.Description,
                     Employees = x.Employees.Select(y => new Employee
                     {
                         Eid = y.Eid,
                         Name = y.Name,
                         Gender = y.Gender
                     })
                 })
                 .ToListAsync();
                if (depts.Count != 0)
                    return Ok(depts);
                else
                    return NotFound();
            }
            catch (Exception e)
            {

                return StatusCode(500, e.Message);
            }

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var dept = await dbContext.Departments.Where(x => x.Did == id).FirstOrDefaultAsync();
            if (dept != null)
                return Ok(dept);
            else
                return NotFound();
            
        }

        [HttpPost]
        public async Task<IActionResult> Post(Department d)
        {
            if (ModelState.IsValid)
            {
                dbContext.Add<Department>(d);
                await dbContext.SaveChangesAsync();

                return  CreatedAtAction("Get",new { id =d.Did}, d); 
            }
            else
            {
                return BadRequest(ModelState);
            }
            
        }
        [HttpPut]
        public async Task<IActionResult> Put(Department d)
        {
            var dept = await dbContext.Departments.Where(x => x.Did ==d.Did).AsNoTracking().FirstOrDefaultAsync();
            if(dept != null)
            {
                if (ModelState.IsValid)
                {
                    dbContext.Update<Department>(d);
                    await dbContext.SaveChangesAsync();
                    return Ok(d);
                }
                else
                {
                   return BadRequest(ModelState);
                } 
            }
            else
            {
                return NotFound();
            }
           
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var dept = await dbContext.Departments.Where(x => x.Did == id).FirstOrDefaultAsync();
            if (dept != null)
            {
                dbContext.Remove<Department>(dept);
                await dbContext.SaveChangesAsync();
                return Ok(dept);
            }
            else
            {
                return NotFound();
            }            
          
        }

    }
}