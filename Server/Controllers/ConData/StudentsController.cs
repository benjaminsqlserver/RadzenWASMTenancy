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
    [Route("odata/ConData/Students")]
    public partial class StudentsController : ODataController
    {
        private RadzenSchoolTenants.Server.Data.ConDataContext context;

        public StudentsController(RadzenSchoolTenants.Server.Data.ConDataContext context)
        {
            this.context = context;
        }

    
        [HttpGet]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IEnumerable<RadzenSchoolTenants.Server.Models.ConData.Student> GetStudents()
        {
            var items = this.context.Students.AsQueryable<RadzenSchoolTenants.Server.Models.ConData.Student>();
            this.OnStudentsRead(ref items);

            return items;
        }

        partial void OnStudentsRead(ref IQueryable<RadzenSchoolTenants.Server.Models.ConData.Student> items);

        partial void OnStudentGet(ref SingleResult<RadzenSchoolTenants.Server.Models.ConData.Student> item);

        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        [HttpGet("/odata/ConData/Students(StudentID={StudentID})")]
        public SingleResult<RadzenSchoolTenants.Server.Models.ConData.Student> GetStudent(long key)
        {
            var items = this.context.Students.Where(i => i.StudentID == key);
            var result = SingleResult.Create(items);

            OnStudentGet(ref result);

            return result;
        }
        partial void OnStudentDeleted(RadzenSchoolTenants.Server.Models.ConData.Student item);
        partial void OnAfterStudentDeleted(RadzenSchoolTenants.Server.Models.ConData.Student item);

        [HttpDelete("/odata/ConData/Students(StudentID={StudentID})")]
        public IActionResult DeleteStudent(long key)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                var items = this.context.Students
                    .Where(i => i.StudentID == key)
                    .AsQueryable();

                items = Data.EntityPatch.ApplyTo<RadzenSchoolTenants.Server.Models.ConData.Student>(Request, items);

                var item = items.FirstOrDefault();

                if (item == null)
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed);
                }
                this.OnStudentDeleted(item);
                this.context.Students.Remove(item);
                this.context.SaveChanges();
                this.OnAfterStudentDeleted(item);

                return new NoContentResult();

            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnStudentUpdated(RadzenSchoolTenants.Server.Models.ConData.Student item);
        partial void OnAfterStudentUpdated(RadzenSchoolTenants.Server.Models.ConData.Student item);

        [HttpPut("/odata/ConData/Students(StudentID={StudentID})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PutStudent(long key, [FromBody]RadzenSchoolTenants.Server.Models.ConData.Student item)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var items = this.context.Students
                    .Where(i => i.StudentID == key)
                    .AsQueryable();

                items = Data.EntityPatch.ApplyTo<RadzenSchoolTenants.Server.Models.ConData.Student>(Request, items);

                var firstItem = items.FirstOrDefault();

                if (firstItem == null)
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed);
                }
                this.OnStudentUpdated(item);
                this.context.Students.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Students.Where(i => i.StudentID == key);
                Request.QueryString = Request.QueryString.Add("$expand", "School");
                this.OnAfterStudentUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPatch("/odata/ConData/Students(StudentID={StudentID})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PatchStudent(long key, [FromBody]Delta<RadzenSchoolTenants.Server.Models.ConData.Student> patch)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var items = this.context.Students
                    .Where(i => i.StudentID == key)
                    .AsQueryable();

                items = Data.EntityPatch.ApplyTo<RadzenSchoolTenants.Server.Models.ConData.Student>(Request, items);

                var item = items.FirstOrDefault();

                if (item == null)
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed);
                }
                patch.Patch(item);

                this.OnStudentUpdated(item);
                this.context.Students.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Students.Where(i => i.StudentID == key);
                Request.QueryString = Request.QueryString.Add("$expand", "School");
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnStudentCreated(RadzenSchoolTenants.Server.Models.ConData.Student item);
        partial void OnAfterStudentCreated(RadzenSchoolTenants.Server.Models.ConData.Student item);

        [HttpPost]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult Post([FromBody] RadzenSchoolTenants.Server.Models.ConData.Student item)
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

                this.OnStudentCreated(item);
                this.context.Students.Add(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Students.Where(i => i.StudentID == item.StudentID);

                Request.QueryString = Request.QueryString.Add("$expand", "School");

                this.OnAfterStudentCreated(item);

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
