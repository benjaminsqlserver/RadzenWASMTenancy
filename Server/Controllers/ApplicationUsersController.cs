using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Deltas;

using RadzenSchoolTenants.Server.Data;
using RadzenSchoolTenants.Server.Models;
using RadzenSchoolTenants.Server.Models.ConData;

namespace RadzenSchoolTenants.Server.Controllers
{
    [Authorize]
    [Route("odata/Identity/ApplicationUsers")]
    public partial class ApplicationUsersController : ODataController
    {
        private readonly ApplicationIdentityDbContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ConDataService _conDataService;
        private readonly RoleManager<ApplicationRole> _roleManager;


        private readonly IWebHostEnvironment env;

        public ApplicationUsersController(IWebHostEnvironment env, ApplicationIdentityDbContext context, UserManager<ApplicationUser> userManager,ConDataService conDataService, RoleManager<ApplicationRole> roleManager)
        {
            this.context = context;
            this.userManager = userManager;

            this.env = env;
            _conDataService = conDataService;
            _roleManager = roleManager;
        }

        partial void OnUsersRead(ref IQueryable<ApplicationUser> users);

        [EnableQuery]
        [HttpGet]
        public IEnumerable<ApplicationUser> Get()
        {
            var users = userManager.Users;

            if (env.EnvironmentName != "Development")
            {
                var tenant = context.Tenants.ToList().Where(t => t.Hosts.Split(',').Where(h => h.Contains(HttpContext.Request.Host.Value)).Any()).FirstOrDefault();
                if (tenant != null)
                {
                    users = users.Where(r => r.TenantId == tenant.Id);
                }
            }
            OnUsersRead(ref users);

            return users;
        }

        [EnableQuery]
        [HttpGet("{Id}")]
        public SingleResult<ApplicationUser> GetApplicationUser(string key)
        {
            var user = context.Users.Where(i => i.Id == key);

            return SingleResult.Create(user);
        }

        partial void OnUserDeleted(ApplicationUser user);

        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(string key)
        {
            var user = await userManager.FindByIdAsync(key);

            if (user == null)
            {
                return NotFound();
            }

            OnUserDeleted(user);

            var result = await userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                return IdentityError(result);
            }

            return new NoContentResult();
        }

        partial void OnUserUpdated(ApplicationUser user);

        [HttpPatch("{Id}")]
        public async Task<IActionResult> Patch(string key, [FromBody]ApplicationUser data)
        {
            var user = await userManager.FindByIdAsync(key);

            if (user == null)
            {
                return NotFound();
            }

            OnUserUpdated(user);

            IdentityResult result = null;

            if (data.Roles != null)
            {
                var roles = await userManager.GetRolesAsync(user);

                result = await userManager.RemoveFromRolesAsync(user, roles);

                if (result.Succeeded)
                {
                    result = await userManager.AddToRolesAsync(user, data.Roles.Select(r => r.Name));
                }
            }

            if (!string.IsNullOrEmpty(data.Password))
            {
                result = await userManager.RemovePasswordAsync(user);

                if (result.Succeeded)
                {
                    result = await userManager.AddPasswordAsync(user, data.Password);
                }
            }

            if (result != null && !result.Succeeded)
            {
                return IdentityError(result);
            }

            return new NoContentResult();
        }

        partial void OnUserCreated(ApplicationUser user);

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ApplicationUser data)
        {
            var email = data.Email;
            var password = data.Password;

            var user = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = true };

            user.TenantId = data.TenantId;

            userManager.UserValidators.Clear();

            if (context.Users.Any(u => u.TenantId == user.TenantId && u.UserName == user.Name))
            {
                ModelState.AddModelError("", "User with the same name already exist for this tenant.");
                return BadRequest(ModelState);
            }
            IdentityResult result = await userManager.CreateAsync(user, password);

            if (result.Succeeded && data.Roles != null)
            {
                //we might not need to resort to my last comment yet
                //get newly created user
                AspNetUser newUser = await _conDataService.GetAspNetUserByEmail(user.Email);
                if(data.Roles.Any())
                {
                    foreach(var role in data.Roles)
                    {
                        IdentityRole selectedRole = await _roleManager.FindByNameAsync(role.Name);
                        if(selectedRole!=null)
                        {
                            var result1=await _conDataService.CreateAspNetUserRole(new AspNetUserRole { UserId=newUser.Id, RoleId=selectedRole.Id });
                        }
                       // result = await userManager.AddToRolesAsync(user, data.Roles.Select(r => r.Name));
                    }
                }
               
            }

            if (result.Succeeded)
            {
                OnUserCreated(user);

                return Created($"odata/Identity/Users('{user.Id}')", user);
            }
            else
            {
                return IdentityError(result);
            }
        }

        private IActionResult IdentityError(IdentityResult result)
        {
            var message = string.Join(", ", result.Errors.Select(error => error.Description));

            return BadRequest(new { error = new { message } });
        }
    }

    [Authorize]
    [Route("odata/Identity/ApplicationTenants")]
    public partial class ApplicationTenantsController : ODataController
    {
        private ApplicationIdentityDbContext context;
        private IWebHostEnvironment env;

        public ApplicationTenantsController(IWebHostEnvironment env, ApplicationIdentityDbContext context)
        {
          this.context = context;
          this.env = env;
        }

        partial void OnTenantsRead(ref IQueryable<ApplicationTenant> tenants);

        [EnableQuery]
        [HttpGet]
        public IEnumerable<ApplicationTenant> Get()
        {
            if (HttpContext.User.Identity.Name != "tenantsadmin")
            {
                return Enumerable.Empty<ApplicationTenant>();
            }
            
            var items = this.context.Tenants.AsQueryable<ApplicationTenant>();

            OnTenantsRead(ref items);

            return items;
        }

        [EnableQuery]
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetApplicationTenant(int key)
        {
            var item = this.context.Tenants.Where(i=>i.Id == key).FirstOrDefault();

            return new ObjectResult(item);
        }

        partial void OnTenantDeleted(ApplicationTenant tenant);

        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(int key)
        {
            try
            {
                if (HttpContext.User.Identity.Name != "tenantsadmin")
                {
                    return new UnauthorizedResult();
                }

                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var item = this.context.Tenants
                    .Where(i => i.Id == key)
                    .FirstOrDefault();

                if (item == null)
                {
                    ModelState.AddModelError("", "Item no longer available");
                    return BadRequest(ModelState);
                }

                this.OnTenantDeleted(item);
                this.context.Tenants.Remove(item);
                this.context.SaveChanges();

                return new NoContentResult();
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnTenantUpdated(ApplicationTenant tenant);

        [HttpPatch("{Id}")]
        public async Task<IActionResult> Patch(int key, [FromBody]Delta<ApplicationTenant> patch)
        {
            if (HttpContext.User.Identity.Name != "tenantsadmin")
            {
                return new UnauthorizedResult();
            }

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var item = this.context.Tenants.Where(i => i.Id == key).FirstOrDefault();

            if (item == null)
            {
                ModelState.AddModelError("", "Item no longer available");
                return BadRequest(ModelState);
            }

            if (env.EnvironmentName != "Development")
            {
                if (context.Tenants.ToList().Where(t => t.Hosts.Split(',').Where(h => item.Hosts.Split(',').Contains(h)).Any()).Any())
                {
                    ModelState.AddModelError("", "Tenant with the same host already exist.");
                    return BadRequest(ModelState);
                }
            }

            patch.Patch(item);

            this.OnTenantUpdated(item);
            this.context.Tenants.Update(item);
            this.context.SaveChanges();

            var itemToReturn = this.context.Tenants.Where(i => i.Id == key);
            return new ObjectResult(SingleResult.Create(itemToReturn));
        }

        partial void OnTenantCreated(ApplicationTenant tenant);

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ApplicationTenant item)
        {
            if (HttpContext.User.Identity.Name != "tenantsadmin")
            {
                return new UnauthorizedResult();
            }

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (item == null)
            {
                return BadRequest();
            }

            if (env.EnvironmentName != "Development")
            {
                if (context.Tenants.ToList().Where(t => t.Hosts.Split(',').Where(h => item.Hosts.Split(',').Contains(h)).Any()).Any())
                {
                    ModelState.AddModelError("", "Tenant with the same host already exist.");
                    return BadRequest(ModelState);
                }
            }
            
            this.OnTenantCreated(item);
            this.context.Tenants.Add(item);
            this.context.SaveChanges();

            var key = item.Id;

            var itemToReturn = this.context.Tenants.Where(i => i.Id == key).FirstOrDefault();

            return Created($"auth/ApplicationTenants({itemToReturn.Id})", itemToReturn);
        }
    }
}