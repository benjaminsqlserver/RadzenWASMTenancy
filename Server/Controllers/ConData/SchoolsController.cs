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
    [Route("odata/ConData/Schools")]
    public partial class SchoolsController : ODataController
    {
        private RadzenSchoolTenants.Server.Data.ConDataContext context;

        public SchoolsController(RadzenSchoolTenants.Server.Data.ConDataContext context)
        {
            this.context = context;
        }

    
        [HttpGet]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IEnumerable<RadzenSchoolTenants.Server.Models.ConData.School> GetSchools()
        {
            var items = this.context.Schools.AsQueryable<RadzenSchoolTenants.Server.Models.ConData.School>();
            this.OnSchoolsRead(ref items);

            return items;
        }

        partial void OnSchoolsRead(ref IQueryable<RadzenSchoolTenants.Server.Models.ConData.School> items);

        partial void OnSchoolGet(ref SingleResult<RadzenSchoolTenants.Server.Models.ConData.School> item);

        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        [HttpGet("/odata/ConData/Schools(SchoolID={SchoolID})")]
        public SingleResult<RadzenSchoolTenants.Server.Models.ConData.School> GetSchool(long key)
        {
            var items = this.context.Schools.Where(i => i.SchoolID == key);
            var result = SingleResult.Create(items);

            OnSchoolGet(ref result);

            return result;
        }
        partial void OnSchoolDeleted(RadzenSchoolTenants.Server.Models.ConData.School item);
        partial void OnAfterSchoolDeleted(RadzenSchoolTenants.Server.Models.ConData.School item);

        [HttpDelete("/odata/ConData/Schools(SchoolID={SchoolID})")]
        public IActionResult DeleteSchool(long key)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                var items = this.context.Schools
                    .Where(i => i.SchoolID == key)
                    .Include(i => i.Students)
                    .AsQueryable();

                items = Data.EntityPatch.ApplyTo<RadzenSchoolTenants.Server.Models.ConData.School>(Request, items);

                var item = items.FirstOrDefault();

                if (item == null)
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed);
                }
                this.OnSchoolDeleted(item);
                this.context.Schools.Remove(item);
                this.context.SaveChanges();
                this.OnAfterSchoolDeleted(item);

                return new NoContentResult();

            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnSchoolUpdated(RadzenSchoolTenants.Server.Models.ConData.School item);
        partial void OnAfterSchoolUpdated(RadzenSchoolTenants.Server.Models.ConData.School item);

        [HttpPut("/odata/ConData/Schools(SchoolID={SchoolID})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PutSchool(long key, [FromBody]RadzenSchoolTenants.Server.Models.ConData.School item)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var items = this.context.Schools
                    .Where(i => i.SchoolID == key)
                    .AsQueryable();

                items = Data.EntityPatch.ApplyTo<RadzenSchoolTenants.Server.Models.ConData.School>(Request, items);

                var firstItem = items.FirstOrDefault();

                if (firstItem == null)
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed);
                }
                this.OnSchoolUpdated(item);
                this.context.Schools.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Schools.Where(i => i.SchoolID == key);
                ;
                this.OnAfterSchoolUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPatch("/odata/ConData/Schools(SchoolID={SchoolID})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PatchSchool(long key, [FromBody]Delta<RadzenSchoolTenants.Server.Models.ConData.School> patch)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var items = this.context.Schools
                    .Where(i => i.SchoolID == key)
                    .AsQueryable();

                items = Data.EntityPatch.ApplyTo<RadzenSchoolTenants.Server.Models.ConData.School>(Request, items);

                var item = items.FirstOrDefault();

                if (item == null)
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed);
                }
                patch.Patch(item);

                this.OnSchoolUpdated(item);
                this.context.Schools.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Schools.Where(i => i.SchoolID == key);
                ;
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnSchoolCreated(RadzenSchoolTenants.Server.Models.ConData.School item);
        partial void OnAfterSchoolCreated(RadzenSchoolTenants.Server.Models.ConData.School item);

        [HttpPost]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult Post([FromBody] RadzenSchoolTenants.Server.Models.ConData.School item)
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

                this.OnSchoolCreated(item);
                this.context.Schools.Add(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Schools.Where(i => i.SchoolID == item.SchoolID);

                ;

                this.OnAfterSchoolCreated(item);

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
