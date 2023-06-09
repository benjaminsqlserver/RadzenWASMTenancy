using System;
using System.Net;
using System.Data;
using System.Linq;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace RadzenSchoolTenants.Server.Controllers.ConData
{
    [Route("odata/ConData/AspNetUsers")]
    public partial class AspNetUsersController : ODataController
    {
        private RadzenSchoolTenants.Server.Data.ConDataContext context;

        public AspNetUsersController(RadzenSchoolTenants.Server.Data.ConDataContext context)
        {
            this.context = context;
        }

    
        [HttpGet]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IEnumerable<RadzenSchoolTenants.Server.Models.ConData.AspNetUser> GetAspNetUsers()
        {
            var items = this.context.AspNetUsers.AsQueryable<RadzenSchoolTenants.Server.Models.ConData.AspNetUser>();
            this.OnAspNetUsersRead(ref items);

            return items;
        }

        partial void OnAspNetUsersRead(ref IQueryable<RadzenSchoolTenants.Server.Models.ConData.AspNetUser> items);

        partial void OnAspNetUserGet(ref SingleResult<RadzenSchoolTenants.Server.Models.ConData.AspNetUser> item);

        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        [HttpGet("/odata/ConData/AspNetUsers(Id={Id})")]
        public SingleResult<RadzenSchoolTenants.Server.Models.ConData.AspNetUser> GetAspNetUser(string key)
        {
            var items = this.context.AspNetUsers.Where(i => i.Id == Uri.UnescapeDataString(key));
            var result = SingleResult.Create(items);

            OnAspNetUserGet(ref result);

            return result;
        }
        partial void OnAspNetUserDeleted(RadzenSchoolTenants.Server.Models.ConData.AspNetUser item);
        partial void OnAfterAspNetUserDeleted(RadzenSchoolTenants.Server.Models.ConData.AspNetUser item);

        [HttpDelete("/odata/ConData/AspNetUsers(Id={Id})")]
        public IActionResult DeleteAspNetUser(string key)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                var items = this.context.AspNetUsers
                    .Where(i => i.Id == Uri.UnescapeDataString(key))
                    .Include(i => i.AspNetUserClaims)
                    .Include(i => i.AspNetUserLogins)
                    .Include(i => i.AspNetUserRoles)
                    .Include(i => i.AspNetUserTokens)
                    .AsQueryable();

                items = Data.EntityPatch.ApplyTo<RadzenSchoolTenants.Server.Models.ConData.AspNetUser>(Request, items);

                var item = items.FirstOrDefault();

                if (item == null)
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed);
                }
                this.OnAspNetUserDeleted(item);
                this.context.AspNetUsers.Remove(item);
                this.context.SaveChanges();
                this.OnAfterAspNetUserDeleted(item);

                return new NoContentResult();

            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnAspNetUserUpdated(RadzenSchoolTenants.Server.Models.ConData.AspNetUser item);
        partial void OnAfterAspNetUserUpdated(RadzenSchoolTenants.Server.Models.ConData.AspNetUser item);

        [HttpPut("/odata/ConData/AspNetUsers(Id={Id})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PutAspNetUser(string key, [FromBody]RadzenSchoolTenants.Server.Models.ConData.AspNetUser item)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var items = this.context.AspNetUsers
                    .Where(i => i.Id == Uri.UnescapeDataString(key))
                    .AsQueryable();

                items = Data.EntityPatch.ApplyTo<RadzenSchoolTenants.Server.Models.ConData.AspNetUser>(Request, items);

                var firstItem = items.FirstOrDefault();

                if (firstItem == null)
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed);
                }
                this.OnAspNetUserUpdated(item);
                this.context.AspNetUsers.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.AspNetUsers.Where(i => i.Id == Uri.UnescapeDataString(key));
                Request.QueryString = Request.QueryString.Add("$expand", "AspNetTenant");
                this.OnAfterAspNetUserUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPatch("/odata/ConData/AspNetUsers(Id={Id})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PatchAspNetUser(string key, [FromBody]Delta<RadzenSchoolTenants.Server.Models.ConData.AspNetUser> patch)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var items = this.context.AspNetUsers
                    .Where(i => i.Id == Uri.UnescapeDataString(key))
                    .AsQueryable();

                items = Data.EntityPatch.ApplyTo<RadzenSchoolTenants.Server.Models.ConData.AspNetUser>(Request, items);

                var item = items.FirstOrDefault();

                if (item == null)
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed);
                }
                patch.Patch(item);

                this.OnAspNetUserUpdated(item);
                this.context.AspNetUsers.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.AspNetUsers.Where(i => i.Id == Uri.UnescapeDataString(key));
                Request.QueryString = Request.QueryString.Add("$expand", "AspNetTenant");
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnAspNetUserCreated(RadzenSchoolTenants.Server.Models.ConData.AspNetUser item);
        partial void OnAfterAspNetUserCreated(RadzenSchoolTenants.Server.Models.ConData.AspNetUser item);

        [HttpPost]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult Post([FromBody] RadzenSchoolTenants.Server.Models.ConData.AspNetUser item)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (item == null)
                {
                    return BadRequest();
                }

                this.OnAspNetUserCreated(item);
                this.context.AspNetUsers.Add(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.AspNetUsers.Where(i => i.Id == item.Id);

                Request.QueryString = Request.QueryString.Add("$expand", "AspNetTenant");

                this.OnAfterAspNetUserCreated(item);

                return new ObjectResult(SingleResult.Create(itemToReturn))
                {
                    StatusCode = 201
                };
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }
    }
}
