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
    [Route("odata/ConData/AspNetTenants")]
    public partial class AspNetTenantsController : ODataController
    {
        private RadzenSchoolTenants.Server.Data.ConDataContext context;

        public AspNetTenantsController(RadzenSchoolTenants.Server.Data.ConDataContext context)
        {
            this.context = context;
        }

    
        [HttpGet]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IEnumerable<RadzenSchoolTenants.Server.Models.ConData.AspNetTenant> GetAspNetTenants()
        {
            var items = this.context.AspNetTenants.AsQueryable<RadzenSchoolTenants.Server.Models.ConData.AspNetTenant>();
            this.OnAspNetTenantsRead(ref items);

            return items;
        }

        partial void OnAspNetTenantsRead(ref IQueryable<RadzenSchoolTenants.Server.Models.ConData.AspNetTenant> items);

        partial void OnAspNetTenantGet(ref SingleResult<RadzenSchoolTenants.Server.Models.ConData.AspNetTenant> item);

        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        [HttpGet("/odata/ConData/AspNetTenants(Id={Id})")]
        public SingleResult<RadzenSchoolTenants.Server.Models.ConData.AspNetTenant> GetAspNetTenant(int key)
        {
            var items = this.context.AspNetTenants.Where(i => i.Id == key);
            var result = SingleResult.Create(items);

            OnAspNetTenantGet(ref result);

            return result;
        }
        partial void OnAspNetTenantDeleted(RadzenSchoolTenants.Server.Models.ConData.AspNetTenant item);
        partial void OnAfterAspNetTenantDeleted(RadzenSchoolTenants.Server.Models.ConData.AspNetTenant item);

        [HttpDelete("/odata/ConData/AspNetTenants(Id={Id})")]
        public IActionResult DeleteAspNetTenant(int key)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                var items = this.context.AspNetTenants
                    .Where(i => i.Id == key)
                    .Include(i => i.AspNetRoles)
                    .Include(i => i.AspNetUsers)
                    .AsQueryable();

                items = Data.EntityPatch.ApplyTo<RadzenSchoolTenants.Server.Models.ConData.AspNetTenant>(Request, items);

                var item = items.FirstOrDefault();

                if (item == null)
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed);
                }
                this.OnAspNetTenantDeleted(item);
                this.context.AspNetTenants.Remove(item);
                this.context.SaveChanges();
                this.OnAfterAspNetTenantDeleted(item);

                return new NoContentResult();

            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnAspNetTenantUpdated(RadzenSchoolTenants.Server.Models.ConData.AspNetTenant item);
        partial void OnAfterAspNetTenantUpdated(RadzenSchoolTenants.Server.Models.ConData.AspNetTenant item);

        [HttpPut("/odata/ConData/AspNetTenants(Id={Id})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PutAspNetTenant(int key, [FromBody]RadzenSchoolTenants.Server.Models.ConData.AspNetTenant item)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var items = this.context.AspNetTenants
                    .Where(i => i.Id == key)
                    .AsQueryable();

                items = Data.EntityPatch.ApplyTo<RadzenSchoolTenants.Server.Models.ConData.AspNetTenant>(Request, items);

                var firstItem = items.FirstOrDefault();

                if (firstItem == null)
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed);
                }
                this.OnAspNetTenantUpdated(item);
                this.context.AspNetTenants.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.AspNetTenants.Where(i => i.Id == key);
                ;
                this.OnAfterAspNetTenantUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPatch("/odata/ConData/AspNetTenants(Id={Id})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PatchAspNetTenant(int key, [FromBody]Delta<RadzenSchoolTenants.Server.Models.ConData.AspNetTenant> patch)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var items = this.context.AspNetTenants
                    .Where(i => i.Id == key)
                    .AsQueryable();

                items = Data.EntityPatch.ApplyTo<RadzenSchoolTenants.Server.Models.ConData.AspNetTenant>(Request, items);

                var item = items.FirstOrDefault();

                if (item == null)
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed);
                }
                patch.Patch(item);

                this.OnAspNetTenantUpdated(item);
                this.context.AspNetTenants.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.AspNetTenants.Where(i => i.Id == key);
                ;
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnAspNetTenantCreated(RadzenSchoolTenants.Server.Models.ConData.AspNetTenant item);
        partial void OnAfterAspNetTenantCreated(RadzenSchoolTenants.Server.Models.ConData.AspNetTenant item);

        [HttpPost]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult Post([FromBody] RadzenSchoolTenants.Server.Models.ConData.AspNetTenant item)
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

                this.OnAspNetTenantCreated(item);
                this.context.AspNetTenants.Add(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.AspNetTenants.Where(i => i.Id == item.Id);

                ;

                this.OnAfterAspNetTenantCreated(item);

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
