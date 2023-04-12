using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using RadzenSchoolTenants.Server.Models;

namespace RadzenSchoolTenants.Server.Controllers
{

    public class MultiTenancyRoleValidator : RoleValidator<ApplicationRole>
    {
        public override Task<IdentityResult> ValidateAsync(RoleManager<ApplicationRole> manager, ApplicationRole role)
        {
            return Task.FromResult(IdentityResult.Success);
        }
    }
    [Authorize]
    [Route("odata/Identity/ApplicationRoles")]
    public partial class ApplicationRolesController : ODataController
    {
       private readonly RoleManager<ApplicationRole> roleManager;

       private readonly IWebHostEnvironment env;
       private readonly Data.ApplicationIdentityDbContext context;

       public ApplicationRolesController(IWebHostEnvironment env, Data.ApplicationIdentityDbContext context, RoleManager<ApplicationRole> roleManager)
       {
           this.roleManager = roleManager;

           this.env = env;
           this.context = context;
           this.roleManager.RoleValidators.Clear();
           this.roleManager.RoleValidators.Add(new MultiTenancyRoleValidator());
       }

       partial void OnRolesRead(ref IQueryable<ApplicationRole> roles);

       [EnableQuery]
       [HttpGet]
       public IEnumerable<ApplicationRole> Get()
       {
           var roles = roleManager.Roles;

           if (env.EnvironmentName != "Development")
           {
               var tenant = context.Tenants.ToList().Where(t => t.Hosts.Split(',').Where(h => h.Contains(HttpContext.Request.Host.Value)).Any()).FirstOrDefault();
               if (tenant != null)
               {
                   roles = roles.Where(r => r.TenantId == tenant.Id);
               }

           }
           OnRolesRead(ref roles);

           return roles;
       }

       partial void OnRoleCreated(ApplicationRole role);

       [HttpPost]
       public async Task<IActionResult> Post([FromBody] ApplicationRole role)
       {
           if (role == null)
           {
               return BadRequest();
           }

           OnRoleCreated(role);

           var result = await roleManager.CreateAsync(role);

           if (!result.Succeeded)
           {
               var message = string.Join(", ", result.Errors.Select(error => error.Description));

               return BadRequest(new { error = new { message }});
           }

           return Created($"odata/Identity/Roles('{role.Id}')", role);
       }

       partial void OnRoleDeleted(ApplicationRole role);

       [HttpDelete("{Id}")]
       public async Task<IActionResult> Delete(string key)
       {
           var role = await roleManager.FindByIdAsync(key);

           if (role == null)
           {
               return NotFound();
           }

           OnRoleDeleted(role);

           var result = await roleManager.DeleteAsync(role);

           if (!result.Succeeded)
           {
               var message = string.Join(", ", result.Errors.Select(error => error.Description));

               return BadRequest(new { error = new { message }});
           }

           return new NoContentResult();
       }
    }
}