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
    [Route("odata/ConData/AspNetRoleClaims")]
    public partial class AspNetRoleClaimsController : ODataController
    {
        private RadzenSchoolTenants.Server.Data.ConDataContext context;

        public AspNetRoleClaimsController(RadzenSchoolTenants.Server.Data.ConDataContext context)
        {
            this.context = context;
        }

    
        [HttpGet]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IEnumerable<RadzenSchoolTenants.Server.Models.ConData.AspNetRoleClaim> GetAspNetRoleClaims()
        {
            var items = this.context.AspNetRoleClaims.AsQueryable<RadzenSchoolTenants.Server.Models.ConData.AspNetRoleClaim>();
            this.OnAspNetRoleClaimsRead(ref items);

            return items;
        }

        partial void OnAspNetRoleClaimsRead(ref IQueryable<RadzenSchoolTenants.Server.Models.ConData.AspNetRoleClaim> items);

        partial void OnAspNetRoleClaimGet(ref SingleResult<RadzenSchoolTenants.Server.Models.ConData.AspNetRoleClaim> item);

        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        [HttpGet("/odata/ConData/AspNetRoleClaims(Id={Id})")]
        public SingleResult<RadzenSchoolTenants.Server.Models.ConData.AspNetRoleClaim> GetAspNetRoleClaim(int key)
        {
            var items = this.context.AspNetRoleClaims.Where(i => i.Id == key);
            var result = SingleResult.Create(items);

            OnAspNetRoleClaimGet(ref result);

            return result;
        }
        partial void OnAspNetRoleClaimDeleted(RadzenSchoolTenants.Server.Models.ConData.AspNetRoleClaim item);
        partial void OnAfterAspNetRoleClaimDeleted(RadzenSchoolTenants.Server.Models.ConData.AspNetRoleClaim item);

        [HttpDelete("/odata/ConData/AspNetRoleClaims(Id={Id})")]
        public IActionResult DeleteAspNetRoleClaim(int key)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                var items = this.context.AspNetRoleClaims
                    .Where(i => i.Id == key)
                    .AsQueryable();

                items = Data.EntityPatch.ApplyTo<RadzenSchoolTenants.Server.Models.ConData.AspNetRoleClaim>(Request, items);

                var item = items.FirstOrDefault();

                if (item == null)
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed);
                }
                this.OnAspNetRoleClaimDeleted(item);
                this.context.AspNetRoleClaims.Remove(item);
                this.context.SaveChanges();
                this.OnAfterAspNetRoleClaimDeleted(item);

                return new NoContentResult();

            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnAspNetRoleClaimUpdated(RadzenSchoolTenants.Server.Models.ConData.AspNetRoleClaim item);
        partial void OnAfterAspNetRoleClaimUpdated(RadzenSchoolTenants.Server.Models.ConData.AspNetRoleClaim item);

        [HttpPut("/odata/ConData/AspNetRoleClaims(Id={Id})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PutAspNetRoleClaim(int key, [FromBody]RadzenSchoolTenants.Server.Models.ConData.AspNetRoleClaim item)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var items = this.context.AspNetRoleClaims
                    .Where(i => i.Id == key)
                    .AsQueryable();

                items = Data.EntityPatch.ApplyTo<RadzenSchoolTenants.Server.Models.ConData.AspNetRoleClaim>(Request, items);

                var firstItem = items.FirstOrDefault();

                if (firstItem == null)
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed);
                }
                this.OnAspNetRoleClaimUpdated(item);
                this.context.AspNetRoleClaims.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.AspNetRoleClaims.Where(i => i.Id == key);
                Request.QueryString = Request.QueryString.Add("$expand", "AspNetRole");
                this.OnAfterAspNetRoleClaimUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPatch("/odata/ConData/AspNetRoleClaims(Id={Id})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PatchAspNetRoleClaim(int key, [FromBody]Delta<RadzenSchoolTenants.Server.Models.ConData.AspNetRoleClaim> patch)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var items = this.context.AspNetRoleClaims
                    .Where(i => i.Id == key)
                    .AsQueryable();

                items = Data.EntityPatch.ApplyTo<RadzenSchoolTenants.Server.Models.ConData.AspNetRoleClaim>(Request, items);

                var item = items.FirstOrDefault();

                if (item == null)
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed);
                }
                patch.Patch(item);

                this.OnAspNetRoleClaimUpdated(item);
                this.context.AspNetRoleClaims.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.AspNetRoleClaims.Where(i => i.Id == key);
                Request.QueryString = Request.QueryString.Add("$expand", "AspNetRole");
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnAspNetRoleClaimCreated(RadzenSchoolTenants.Server.Models.ConData.AspNetRoleClaim item);
        partial void OnAfterAspNetRoleClaimCreated(RadzenSchoolTenants.Server.Models.ConData.AspNetRoleClaim item);

        [HttpPost]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult Post([FromBody] RadzenSchoolTenants.Server.Models.ConData.AspNetRoleClaim item)
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

                this.OnAspNetRoleClaimCreated(item);
                this.context.AspNetRoleClaims.Add(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.AspNetRoleClaims.Where(i => i.Id == item.Id);

                Request.QueryString = Request.QueryString.Add("$expand", "AspNetRole");

                this.OnAfterAspNetRoleClaimCreated(item);

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
