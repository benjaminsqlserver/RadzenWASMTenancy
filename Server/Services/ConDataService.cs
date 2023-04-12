using System;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Radzen;

using RadzenSchoolTenants.Server.Data;
using RadzenSchoolTenants.Server.Models.ConData;
using System.Net.Http.Headers;

namespace RadzenSchoolTenants.Server
{
    public partial class ConDataService
    {
        ConDataContext Context
        {
           get
           {
             return this.context;
           }
        }

        private readonly ConDataContext context;
        private readonly NavigationManager navigationManager;

        public ConDataService(ConDataContext context, NavigationManager navigationManager)
        {
            this.context = context;
            this.navigationManager = navigationManager;
        }

        public void Reset() => Context.ChangeTracker.Entries().Where(e => e.Entity != null).ToList().ForEach(e => e.State = EntityState.Detached);


        public async Task ExportSchoolsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/schools/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/schools/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportSchoolsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/schools/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/schools/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnSchoolsRead(ref IQueryable<RadzenSchoolTenants.Server.Models.ConData.School> items);

        public async Task<IQueryable<RadzenSchoolTenants.Server.Models.ConData.School>> GetSchools(Query query = null)
        {
            var items = Context.Schools.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                if (!string.IsNullOrEmpty(query.Filter))
                {
                    if (query.FilterParameters != null)
                    {
                        items = items.Where(query.Filter, query.FilterParameters);
                    }
                    else
                    {
                        items = items.Where(query.Filter);
                    }
                }

                if (!string.IsNullOrEmpty(query.OrderBy))
                {
                    items = items.OrderBy(query.OrderBy);
                }

                if (query.Skip.HasValue)
                {
                    items = items.Skip(query.Skip.Value);
                }

                if (query.Top.HasValue)
                {
                    items = items.Take(query.Top.Value);
                }
            }

            OnSchoolsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnSchoolGet(RadzenSchoolTenants.Server.Models.ConData.School item);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.School> GetSchoolBySchoolId(long schoolid)
        {
            var items = Context.Schools
                              .AsNoTracking()
                              .Where(i => i.SchoolID == schoolid);

  
            var itemToReturn = items.FirstOrDefault();

            OnSchoolGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnSchoolCreated(RadzenSchoolTenants.Server.Models.ConData.School item);
        partial void OnAfterSchoolCreated(RadzenSchoolTenants.Server.Models.ConData.School item);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.School> CreateSchool(RadzenSchoolTenants.Server.Models.ConData.School school)
        {
            OnSchoolCreated(school);

            var existingItem = Context.Schools
                              .Where(i => i.SchoolID == school.SchoolID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Schools.Add(school);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(school).State = EntityState.Detached;
                throw;
            }

            OnAfterSchoolCreated(school);

            return school;
        }

        public async Task<RadzenSchoolTenants.Server.Models.ConData.School> CancelSchoolChanges(RadzenSchoolTenants.Server.Models.ConData.School item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnSchoolUpdated(RadzenSchoolTenants.Server.Models.ConData.School item);
        partial void OnAfterSchoolUpdated(RadzenSchoolTenants.Server.Models.ConData.School item);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.School> UpdateSchool(long schoolid, RadzenSchoolTenants.Server.Models.ConData.School school)
        {
            OnSchoolUpdated(school);

            var itemToUpdate = Context.Schools
                              .Where(i => i.SchoolID == school.SchoolID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(school);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterSchoolUpdated(school);

            return school;
        }

        partial void OnSchoolDeleted(RadzenSchoolTenants.Server.Models.ConData.School item);
        partial void OnAfterSchoolDeleted(RadzenSchoolTenants.Server.Models.ConData.School item);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.School> DeleteSchool(long schoolid)
        {
            var itemToDelete = Context.Schools
                              .Where(i => i.SchoolID == schoolid)
                              .Include(i => i.Students)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnSchoolDeleted(itemToDelete);


            Context.Schools.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterSchoolDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportStudentsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/students/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/students/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportStudentsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/students/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/students/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnStudentsRead(ref IQueryable<RadzenSchoolTenants.Server.Models.ConData.Student> items);

        public async Task<IQueryable<RadzenSchoolTenants.Server.Models.ConData.Student>> GetStudents(Query query = null)
        {
            var items = Context.Students.AsQueryable();

            items = items.Include(i => i.School);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                if (!string.IsNullOrEmpty(query.Filter))
                {
                    if (query.FilterParameters != null)
                    {
                        items = items.Where(query.Filter, query.FilterParameters);
                    }
                    else
                    {
                        items = items.Where(query.Filter);
                    }
                }

                if (!string.IsNullOrEmpty(query.OrderBy))
                {
                    items = items.OrderBy(query.OrderBy);
                }

                if (query.Skip.HasValue)
                {
                    items = items.Skip(query.Skip.Value);
                }

                if (query.Top.HasValue)
                {
                    items = items.Take(query.Top.Value);
                }
            }

            OnStudentsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnStudentGet(RadzenSchoolTenants.Server.Models.ConData.Student item);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.Student> GetStudentByStudentId(long studentid)
        {
            var items = Context.Students
                              .AsNoTracking()
                              .Where(i => i.StudentID == studentid);

            items = items.Include(i => i.School);
  
            var itemToReturn = items.FirstOrDefault();

            OnStudentGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnStudentCreated(RadzenSchoolTenants.Server.Models.ConData.Student item);
        partial void OnAfterStudentCreated(RadzenSchoolTenants.Server.Models.ConData.Student item);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.Student> CreateStudent(RadzenSchoolTenants.Server.Models.ConData.Student student)
        {
            OnStudentCreated(student);

            var existingItem = Context.Students
                              .Where(i => i.StudentID == student.StudentID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Students.Add(student);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(student).State = EntityState.Detached;
                throw;
            }

            OnAfterStudentCreated(student);

            return student;
        }

        public async Task<RadzenSchoolTenants.Server.Models.ConData.Student> CancelStudentChanges(RadzenSchoolTenants.Server.Models.ConData.Student item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnStudentUpdated(RadzenSchoolTenants.Server.Models.ConData.Student item);
        partial void OnAfterStudentUpdated(RadzenSchoolTenants.Server.Models.ConData.Student item);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.Student> UpdateStudent(long studentid, RadzenSchoolTenants.Server.Models.ConData.Student student)
        {
            OnStudentUpdated(student);

            var itemToUpdate = Context.Students
                              .Where(i => i.StudentID == student.StudentID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(student);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterStudentUpdated(student);

            return student;
        }

        partial void OnStudentDeleted(RadzenSchoolTenants.Server.Models.ConData.Student item);
        partial void OnAfterStudentDeleted(RadzenSchoolTenants.Server.Models.ConData.Student item);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.Student> DeleteStudent(long studentid)
        {
            var itemToDelete = Context.Students
                              .Where(i => i.StudentID == studentid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnStudentDeleted(itemToDelete);


            Context.Students.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterStudentDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportAspNetRoleClaimsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/aspnetroleclaims/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/aspnetroleclaims/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportAspNetRoleClaimsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/aspnetroleclaims/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/aspnetroleclaims/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnAspNetRoleClaimsRead(ref IQueryable<RadzenSchoolTenants.Server.Models.ConData.AspNetRoleClaim> items);

        public async Task<IQueryable<RadzenSchoolTenants.Server.Models.ConData.AspNetRoleClaim>> GetAspNetRoleClaims(Query query = null)
        {
            var items = Context.AspNetRoleClaims.AsQueryable();

            items = items.Include(i => i.AspNetRole);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                if (!string.IsNullOrEmpty(query.Filter))
                {
                    if (query.FilterParameters != null)
                    {
                        items = items.Where(query.Filter, query.FilterParameters);
                    }
                    else
                    {
                        items = items.Where(query.Filter);
                    }
                }

                if (!string.IsNullOrEmpty(query.OrderBy))
                {
                    items = items.OrderBy(query.OrderBy);
                }

                if (query.Skip.HasValue)
                {
                    items = items.Skip(query.Skip.Value);
                }

                if (query.Top.HasValue)
                {
                    items = items.Take(query.Top.Value);
                }
            }

            OnAspNetRoleClaimsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnAspNetRoleClaimGet(RadzenSchoolTenants.Server.Models.ConData.AspNetRoleClaim item);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.AspNetRoleClaim> GetAspNetRoleClaimById(int id)
        {
            var items = Context.AspNetRoleClaims
                              .AsNoTracking()
                              .Where(i => i.Id == id);

            items = items.Include(i => i.AspNetRole);
  
            var itemToReturn = items.FirstOrDefault();

            OnAspNetRoleClaimGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnAspNetRoleClaimCreated(RadzenSchoolTenants.Server.Models.ConData.AspNetRoleClaim item);
        partial void OnAfterAspNetRoleClaimCreated(RadzenSchoolTenants.Server.Models.ConData.AspNetRoleClaim item);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.AspNetRoleClaim> CreateAspNetRoleClaim(RadzenSchoolTenants.Server.Models.ConData.AspNetRoleClaim aspnetroleclaim)
        {
            OnAspNetRoleClaimCreated(aspnetroleclaim);

            var existingItem = Context.AspNetRoleClaims
                              .Where(i => i.Id == aspnetroleclaim.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.AspNetRoleClaims.Add(aspnetroleclaim);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(aspnetroleclaim).State = EntityState.Detached;
                throw;
            }

            OnAfterAspNetRoleClaimCreated(aspnetroleclaim);

            return aspnetroleclaim;
        }

        public async Task<RadzenSchoolTenants.Server.Models.ConData.AspNetRoleClaim> CancelAspNetRoleClaimChanges(RadzenSchoolTenants.Server.Models.ConData.AspNetRoleClaim item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnAspNetRoleClaimUpdated(RadzenSchoolTenants.Server.Models.ConData.AspNetRoleClaim item);
        partial void OnAfterAspNetRoleClaimUpdated(RadzenSchoolTenants.Server.Models.ConData.AspNetRoleClaim item);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.AspNetRoleClaim> UpdateAspNetRoleClaim(int id, RadzenSchoolTenants.Server.Models.ConData.AspNetRoleClaim aspnetroleclaim)
        {
            OnAspNetRoleClaimUpdated(aspnetroleclaim);

            var itemToUpdate = Context.AspNetRoleClaims
                              .Where(i => i.Id == aspnetroleclaim.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(aspnetroleclaim);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterAspNetRoleClaimUpdated(aspnetroleclaim);

            return aspnetroleclaim;
        }

        partial void OnAspNetRoleClaimDeleted(RadzenSchoolTenants.Server.Models.ConData.AspNetRoleClaim item);
        partial void OnAfterAspNetRoleClaimDeleted(RadzenSchoolTenants.Server.Models.ConData.AspNetRoleClaim item);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.AspNetRoleClaim> DeleteAspNetRoleClaim(int id)
        {
            var itemToDelete = Context.AspNetRoleClaims
                              .Where(i => i.Id == id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnAspNetRoleClaimDeleted(itemToDelete);


            Context.AspNetRoleClaims.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterAspNetRoleClaimDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportAspNetRolesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/aspnetroles/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/aspnetroles/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportAspNetRolesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/aspnetroles/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/aspnetroles/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnAspNetRolesRead(ref IQueryable<RadzenSchoolTenants.Server.Models.ConData.AspNetRole> items);

        public async Task<IQueryable<RadzenSchoolTenants.Server.Models.ConData.AspNetRole>> GetAspNetRoles(Query query = null)
        {
            var items = Context.AspNetRoles.AsQueryable();

            items = items.Include(i => i.AspNetTenant);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                if (!string.IsNullOrEmpty(query.Filter))
                {
                    if (query.FilterParameters != null)
                    {
                        items = items.Where(query.Filter, query.FilterParameters);
                    }
                    else
                    {
                        items = items.Where(query.Filter);
                    }
                }

                if (!string.IsNullOrEmpty(query.OrderBy))
                {
                    items = items.OrderBy(query.OrderBy);
                }

                if (query.Skip.HasValue)
                {
                    items = items.Skip(query.Skip.Value);
                }

                if (query.Top.HasValue)
                {
                    items = items.Take(query.Top.Value);
                }
            }

            OnAspNetRolesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnAspNetRoleGet(RadzenSchoolTenants.Server.Models.ConData.AspNetRole item);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.AspNetRole> GetAspNetRoleById(string id)
        {
            var items = Context.AspNetRoles
                              .AsNoTracking()
                              .Where(i => i.Id == id);

            items = items.Include(i => i.AspNetTenant);
  
            var itemToReturn = items.FirstOrDefault();

            OnAspNetRoleGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnAspNetRoleCreated(RadzenSchoolTenants.Server.Models.ConData.AspNetRole item);
        partial void OnAfterAspNetRoleCreated(RadzenSchoolTenants.Server.Models.ConData.AspNetRole item);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.AspNetRole> CreateAspNetRole(RadzenSchoolTenants.Server.Models.ConData.AspNetRole aspnetrole)
        {
            OnAspNetRoleCreated(aspnetrole);

            var existingItem = Context.AspNetRoles
                              .Where(i => i.Id == aspnetrole.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.AspNetRoles.Add(aspnetrole);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(aspnetrole).State = EntityState.Detached;
                throw;
            }

            OnAfterAspNetRoleCreated(aspnetrole);

            return aspnetrole;
        }

        public async Task<RadzenSchoolTenants.Server.Models.ConData.AspNetRole> CancelAspNetRoleChanges(RadzenSchoolTenants.Server.Models.ConData.AspNetRole item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnAspNetRoleUpdated(RadzenSchoolTenants.Server.Models.ConData.AspNetRole item);
        partial void OnAfterAspNetRoleUpdated(RadzenSchoolTenants.Server.Models.ConData.AspNetRole item);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.AspNetRole> UpdateAspNetRole(string id, RadzenSchoolTenants.Server.Models.ConData.AspNetRole aspnetrole)
        {
            OnAspNetRoleUpdated(aspnetrole);

            var itemToUpdate = Context.AspNetRoles
                              .Where(i => i.Id == aspnetrole.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(aspnetrole);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterAspNetRoleUpdated(aspnetrole);

            return aspnetrole;
        }

        partial void OnAspNetRoleDeleted(RadzenSchoolTenants.Server.Models.ConData.AspNetRole item);
        partial void OnAfterAspNetRoleDeleted(RadzenSchoolTenants.Server.Models.ConData.AspNetRole item);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.AspNetRole> DeleteAspNetRole(string id)
        {
            var itemToDelete = Context.AspNetRoles
                              .Where(i => i.Id == id)
                              .Include(i => i.AspNetRoleClaims)
                              .Include(i => i.AspNetUserRoles)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnAspNetRoleDeleted(itemToDelete);


            Context.AspNetRoles.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterAspNetRoleDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportAspNetTenantsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/aspnettenants/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/aspnettenants/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportAspNetTenantsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/aspnettenants/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/aspnettenants/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnAspNetTenantsRead(ref IQueryable<RadzenSchoolTenants.Server.Models.ConData.AspNetTenant> items);

        public async Task<IQueryable<RadzenSchoolTenants.Server.Models.ConData.AspNetTenant>> GetAspNetTenants(Query query = null)
        {
            var items = Context.AspNetTenants.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                if (!string.IsNullOrEmpty(query.Filter))
                {
                    if (query.FilterParameters != null)
                    {
                        items = items.Where(query.Filter, query.FilterParameters);
                    }
                    else
                    {
                        items = items.Where(query.Filter);
                    }
                }

                if (!string.IsNullOrEmpty(query.OrderBy))
                {
                    items = items.OrderBy(query.OrderBy);
                }

                if (query.Skip.HasValue)
                {
                    items = items.Skip(query.Skip.Value);
                }

                if (query.Top.HasValue)
                {
                    items = items.Take(query.Top.Value);
                }
            }

            OnAspNetTenantsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnAspNetTenantGet(RadzenSchoolTenants.Server.Models.ConData.AspNetTenant item);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.AspNetTenant> GetAspNetTenantById(int id)
        {
            var items = Context.AspNetTenants
                              .AsNoTracking()
                              .Where(i => i.Id == id);

  
            var itemToReturn = items.FirstOrDefault();

            OnAspNetTenantGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnAspNetTenantCreated(RadzenSchoolTenants.Server.Models.ConData.AspNetTenant item);
        partial void OnAfterAspNetTenantCreated(RadzenSchoolTenants.Server.Models.ConData.AspNetTenant item);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.AspNetTenant> CreateAspNetTenant(RadzenSchoolTenants.Server.Models.ConData.AspNetTenant aspnettenant)
        {
            OnAspNetTenantCreated(aspnettenant);

            var existingItem = Context.AspNetTenants
                              .Where(i => i.Id == aspnettenant.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.AspNetTenants.Add(aspnettenant);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(aspnettenant).State = EntityState.Detached;
                throw;
            }

            OnAfterAspNetTenantCreated(aspnettenant);

            return aspnettenant;
        }

        public async Task<RadzenSchoolTenants.Server.Models.ConData.AspNetTenant> CancelAspNetTenantChanges(RadzenSchoolTenants.Server.Models.ConData.AspNetTenant item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnAspNetTenantUpdated(RadzenSchoolTenants.Server.Models.ConData.AspNetTenant item);
        partial void OnAfterAspNetTenantUpdated(RadzenSchoolTenants.Server.Models.ConData.AspNetTenant item);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.AspNetTenant> UpdateAspNetTenant(int id, RadzenSchoolTenants.Server.Models.ConData.AspNetTenant aspnettenant)
        {
            OnAspNetTenantUpdated(aspnettenant);

            var itemToUpdate = Context.AspNetTenants
                              .Where(i => i.Id == aspnettenant.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(aspnettenant);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterAspNetTenantUpdated(aspnettenant);

            return aspnettenant;
        }

        partial void OnAspNetTenantDeleted(RadzenSchoolTenants.Server.Models.ConData.AspNetTenant item);
        partial void OnAfterAspNetTenantDeleted(RadzenSchoolTenants.Server.Models.ConData.AspNetTenant item);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.AspNetTenant> DeleteAspNetTenant(int id)
        {
            var itemToDelete = Context.AspNetTenants
                              .Where(i => i.Id == id)
                              .Include(i => i.AspNetRoles)
                              .Include(i => i.AspNetUsers)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnAspNetTenantDeleted(itemToDelete);


            Context.AspNetTenants.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterAspNetTenantDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportAspNetUserClaimsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/aspnetuserclaims/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/aspnetuserclaims/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportAspNetUserClaimsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/aspnetuserclaims/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/aspnetuserclaims/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnAspNetUserClaimsRead(ref IQueryable<RadzenSchoolTenants.Server.Models.ConData.AspNetUserClaim> items);

        public async Task<IQueryable<RadzenSchoolTenants.Server.Models.ConData.AspNetUserClaim>> GetAspNetUserClaims(Query query = null)
        {
            var items = Context.AspNetUserClaims.AsQueryable();

            items = items.Include(i => i.AspNetUser);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                if (!string.IsNullOrEmpty(query.Filter))
                {
                    if (query.FilterParameters != null)
                    {
                        items = items.Where(query.Filter, query.FilterParameters);
                    }
                    else
                    {
                        items = items.Where(query.Filter);
                    }
                }

                if (!string.IsNullOrEmpty(query.OrderBy))
                {
                    items = items.OrderBy(query.OrderBy);
                }

                if (query.Skip.HasValue)
                {
                    items = items.Skip(query.Skip.Value);
                }

                if (query.Top.HasValue)
                {
                    items = items.Take(query.Top.Value);
                }
            }

            OnAspNetUserClaimsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnAspNetUserClaimGet(RadzenSchoolTenants.Server.Models.ConData.AspNetUserClaim item);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.AspNetUserClaim> GetAspNetUserClaimById(int id)
        {
            var items = Context.AspNetUserClaims
                              .AsNoTracking()
                              .Where(i => i.Id == id);

            items = items.Include(i => i.AspNetUser);
  
            var itemToReturn = items.FirstOrDefault();

            OnAspNetUserClaimGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnAspNetUserClaimCreated(RadzenSchoolTenants.Server.Models.ConData.AspNetUserClaim item);
        partial void OnAfterAspNetUserClaimCreated(RadzenSchoolTenants.Server.Models.ConData.AspNetUserClaim item);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.AspNetUserClaim> CreateAspNetUserClaim(RadzenSchoolTenants.Server.Models.ConData.AspNetUserClaim aspnetuserclaim)
        {
            OnAspNetUserClaimCreated(aspnetuserclaim);

            var existingItem = Context.AspNetUserClaims
                              .Where(i => i.Id == aspnetuserclaim.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.AspNetUserClaims.Add(aspnetuserclaim);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(aspnetuserclaim).State = EntityState.Detached;
                throw;
            }

            OnAfterAspNetUserClaimCreated(aspnetuserclaim);

            return aspnetuserclaim;
        }

        public async Task<RadzenSchoolTenants.Server.Models.ConData.AspNetUserClaim> CancelAspNetUserClaimChanges(RadzenSchoolTenants.Server.Models.ConData.AspNetUserClaim item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnAspNetUserClaimUpdated(RadzenSchoolTenants.Server.Models.ConData.AspNetUserClaim item);
        partial void OnAfterAspNetUserClaimUpdated(RadzenSchoolTenants.Server.Models.ConData.AspNetUserClaim item);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.AspNetUserClaim> UpdateAspNetUserClaim(int id, RadzenSchoolTenants.Server.Models.ConData.AspNetUserClaim aspnetuserclaim)
        {
            OnAspNetUserClaimUpdated(aspnetuserclaim);

            var itemToUpdate = Context.AspNetUserClaims
                              .Where(i => i.Id == aspnetuserclaim.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(aspnetuserclaim);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterAspNetUserClaimUpdated(aspnetuserclaim);

            return aspnetuserclaim;
        }

        partial void OnAspNetUserClaimDeleted(RadzenSchoolTenants.Server.Models.ConData.AspNetUserClaim item);
        partial void OnAfterAspNetUserClaimDeleted(RadzenSchoolTenants.Server.Models.ConData.AspNetUserClaim item);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.AspNetUserClaim> DeleteAspNetUserClaim(int id)
        {
            var itemToDelete = Context.AspNetUserClaims
                              .Where(i => i.Id == id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnAspNetUserClaimDeleted(itemToDelete);


            Context.AspNetUserClaims.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterAspNetUserClaimDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportAspNetUserLoginsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/aspnetuserlogins/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/aspnetuserlogins/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportAspNetUserLoginsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/aspnetuserlogins/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/aspnetuserlogins/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnAspNetUserLoginsRead(ref IQueryable<RadzenSchoolTenants.Server.Models.ConData.AspNetUserLogin> items);

        public async Task<IQueryable<RadzenSchoolTenants.Server.Models.ConData.AspNetUserLogin>> GetAspNetUserLogins(Query query = null)
        {
            var items = Context.AspNetUserLogins.AsQueryable();

            items = items.Include(i => i.AspNetUser);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                if (!string.IsNullOrEmpty(query.Filter))
                {
                    if (query.FilterParameters != null)
                    {
                        items = items.Where(query.Filter, query.FilterParameters);
                    }
                    else
                    {
                        items = items.Where(query.Filter);
                    }
                }

                if (!string.IsNullOrEmpty(query.OrderBy))
                {
                    items = items.OrderBy(query.OrderBy);
                }

                if (query.Skip.HasValue)
                {
                    items = items.Skip(query.Skip.Value);
                }

                if (query.Top.HasValue)
                {
                    items = items.Take(query.Top.Value);
                }
            }

            OnAspNetUserLoginsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnAspNetUserLoginGet(RadzenSchoolTenants.Server.Models.ConData.AspNetUserLogin item);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.AspNetUserLogin> GetAspNetUserLoginByLoginProviderAndProviderKey(string loginprovider, string providerkey)
        {
            var items = Context.AspNetUserLogins
                              .AsNoTracking()
                              .Where(i => i.LoginProvider == loginprovider && i.ProviderKey == providerkey);

            items = items.Include(i => i.AspNetUser);
  
            var itemToReturn = items.FirstOrDefault();

            OnAspNetUserLoginGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnAspNetUserLoginCreated(RadzenSchoolTenants.Server.Models.ConData.AspNetUserLogin item);
        partial void OnAfterAspNetUserLoginCreated(RadzenSchoolTenants.Server.Models.ConData.AspNetUserLogin item);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.AspNetUserLogin> CreateAspNetUserLogin(RadzenSchoolTenants.Server.Models.ConData.AspNetUserLogin aspnetuserlogin)
        {
            OnAspNetUserLoginCreated(aspnetuserlogin);

            var existingItem = Context.AspNetUserLogins
                              .Where(i => i.LoginProvider == aspnetuserlogin.LoginProvider && i.ProviderKey == aspnetuserlogin.ProviderKey)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.AspNetUserLogins.Add(aspnetuserlogin);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(aspnetuserlogin).State = EntityState.Detached;
                throw;
            }

            OnAfterAspNetUserLoginCreated(aspnetuserlogin);

            return aspnetuserlogin;
        }

        public async Task<RadzenSchoolTenants.Server.Models.ConData.AspNetUserLogin> CancelAspNetUserLoginChanges(RadzenSchoolTenants.Server.Models.ConData.AspNetUserLogin item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnAspNetUserLoginUpdated(RadzenSchoolTenants.Server.Models.ConData.AspNetUserLogin item);
        partial void OnAfterAspNetUserLoginUpdated(RadzenSchoolTenants.Server.Models.ConData.AspNetUserLogin item);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.AspNetUserLogin> UpdateAspNetUserLogin(string loginprovider, string providerkey, RadzenSchoolTenants.Server.Models.ConData.AspNetUserLogin aspnetuserlogin)
        {
            OnAspNetUserLoginUpdated(aspnetuserlogin);

            var itemToUpdate = Context.AspNetUserLogins
                              .Where(i => i.LoginProvider == aspnetuserlogin.LoginProvider && i.ProviderKey == aspnetuserlogin.ProviderKey)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(aspnetuserlogin);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterAspNetUserLoginUpdated(aspnetuserlogin);

            return aspnetuserlogin;
        }

        partial void OnAspNetUserLoginDeleted(RadzenSchoolTenants.Server.Models.ConData.AspNetUserLogin item);
        partial void OnAfterAspNetUserLoginDeleted(RadzenSchoolTenants.Server.Models.ConData.AspNetUserLogin item);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.AspNetUserLogin> DeleteAspNetUserLogin(string loginprovider, string providerkey)
        {
            var itemToDelete = Context.AspNetUserLogins
                              .Where(i => i.LoginProvider == loginprovider && i.ProviderKey == providerkey)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnAspNetUserLoginDeleted(itemToDelete);


            Context.AspNetUserLogins.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterAspNetUserLoginDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportAspNetUserRolesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/aspnetuserroles/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/aspnetuserroles/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportAspNetUserRolesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/aspnetuserroles/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/aspnetuserroles/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnAspNetUserRolesRead(ref IQueryable<RadzenSchoolTenants.Server.Models.ConData.AspNetUserRole> items);

        public async Task<IQueryable<RadzenSchoolTenants.Server.Models.ConData.AspNetUserRole>> GetAspNetUserRoles(Query query = null)
        {
            var items = Context.AspNetUserRoles.AsQueryable();

            items = items.Include(i => i.AspNetRole);
            items = items.Include(i => i.AspNetUser);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                if (!string.IsNullOrEmpty(query.Filter))
                {
                    if (query.FilterParameters != null)
                    {
                        items = items.Where(query.Filter, query.FilterParameters);
                    }
                    else
                    {
                        items = items.Where(query.Filter);
                    }
                }

                if (!string.IsNullOrEmpty(query.OrderBy))
                {
                    items = items.OrderBy(query.OrderBy);
                }

                if (query.Skip.HasValue)
                {
                    items = items.Skip(query.Skip.Value);
                }

                if (query.Top.HasValue)
                {
                    items = items.Take(query.Top.Value);
                }
            }

            OnAspNetUserRolesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnAspNetUserRoleGet(RadzenSchoolTenants.Server.Models.ConData.AspNetUserRole item);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.AspNetUserRole> GetAspNetUserRoleByUserIdAndRoleId(string userid, string roleid)
        {
            var items = Context.AspNetUserRoles
                              .AsNoTracking()
                              .Where(i => i.UserId == userid && i.RoleId == roleid);

            items = items.Include(i => i.AspNetRole);
            items = items.Include(i => i.AspNetUser);
  
            var itemToReturn = items.FirstOrDefault();

            OnAspNetUserRoleGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnAspNetUserRoleCreated(RadzenSchoolTenants.Server.Models.ConData.AspNetUserRole item);
        partial void OnAfterAspNetUserRoleCreated(RadzenSchoolTenants.Server.Models.ConData.AspNetUserRole item);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.AspNetUserRole> CreateAspNetUserRole(RadzenSchoolTenants.Server.Models.ConData.AspNetUserRole aspnetuserrole)
        {
            OnAspNetUserRoleCreated(aspnetuserrole);

            var existingItem = Context.AspNetUserRoles
                              .Where(i => i.UserId == aspnetuserrole.UserId && i.RoleId == aspnetuserrole.RoleId)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.AspNetUserRoles.Add(aspnetuserrole);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(aspnetuserrole).State = EntityState.Detached;
                throw;
            }

            OnAfterAspNetUserRoleCreated(aspnetuserrole);

            return aspnetuserrole;
        }

        public async Task<RadzenSchoolTenants.Server.Models.ConData.AspNetUserRole> CancelAspNetUserRoleChanges(RadzenSchoolTenants.Server.Models.ConData.AspNetUserRole item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnAspNetUserRoleUpdated(RadzenSchoolTenants.Server.Models.ConData.AspNetUserRole item);
        partial void OnAfterAspNetUserRoleUpdated(RadzenSchoolTenants.Server.Models.ConData.AspNetUserRole item);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.AspNetUserRole> UpdateAspNetUserRole(string userid, string roleid, RadzenSchoolTenants.Server.Models.ConData.AspNetUserRole aspnetuserrole)
        {
            OnAspNetUserRoleUpdated(aspnetuserrole);

            var itemToUpdate = Context.AspNetUserRoles
                              .Where(i => i.UserId == aspnetuserrole.UserId && i.RoleId == aspnetuserrole.RoleId)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(aspnetuserrole);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterAspNetUserRoleUpdated(aspnetuserrole);

            return aspnetuserrole;
        }

        partial void OnAspNetUserRoleDeleted(RadzenSchoolTenants.Server.Models.ConData.AspNetUserRole item);
        partial void OnAfterAspNetUserRoleDeleted(RadzenSchoolTenants.Server.Models.ConData.AspNetUserRole item);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.AspNetUserRole> DeleteAspNetUserRole(string userid, string roleid)
        {
            var itemToDelete = Context.AspNetUserRoles
                              .Where(i => i.UserId == userid && i.RoleId == roleid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnAspNetUserRoleDeleted(itemToDelete);


            Context.AspNetUserRoles.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterAspNetUserRoleDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportAspNetUsersToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/aspnetusers/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/aspnetusers/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportAspNetUsersToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/aspnetusers/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/aspnetusers/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnAspNetUsersRead(ref IQueryable<RadzenSchoolTenants.Server.Models.ConData.AspNetUser> items);

        public async Task<IQueryable<RadzenSchoolTenants.Server.Models.ConData.AspNetUser>> GetAspNetUsers(Query query = null)
        {
            var items = Context.AspNetUsers.AsQueryable();

            items = items.Include(i => i.AspNetTenant);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                if (!string.IsNullOrEmpty(query.Filter))
                {
                    if (query.FilterParameters != null)
                    {
                        items = items.Where(query.Filter, query.FilterParameters);
                    }
                    else
                    {
                        items = items.Where(query.Filter);
                    }
                }

                if (!string.IsNullOrEmpty(query.OrderBy))
                {
                    items = items.OrderBy(query.OrderBy);
                }

                if (query.Skip.HasValue)
                {
                    items = items.Skip(query.Skip.Value);
                }

                if (query.Top.HasValue)
                {
                    items = items.Take(query.Top.Value);
                }
            }

            OnAspNetUsersRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnAspNetUserGet(RadzenSchoolTenants.Server.Models.ConData.AspNetUser item);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.AspNetUser> GetAspNetUserById(string id)
        {
            var items = Context.AspNetUsers
                              .AsNoTracking()
                              .Where(i => i.Id == id);

            items = items.Include(i => i.AspNetTenant);
  
            var itemToReturn = items.FirstOrDefault();

            OnAspNetUserGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnAspNetUserCreated(RadzenSchoolTenants.Server.Models.ConData.AspNetUser item);
        partial void OnAfterAspNetUserCreated(RadzenSchoolTenants.Server.Models.ConData.AspNetUser item);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.AspNetUser> CreateAspNetUser(RadzenSchoolTenants.Server.Models.ConData.AspNetUser aspnetuser)
        {
            OnAspNetUserCreated(aspnetuser);

            var existingItem = Context.AspNetUsers
                              .Where(i => i.Id == aspnetuser.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.AspNetUsers.Add(aspnetuser);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(aspnetuser).State = EntityState.Detached;
                throw;
            }

            OnAfterAspNetUserCreated(aspnetuser);

            return aspnetuser;
        }

        public async Task<RadzenSchoolTenants.Server.Models.ConData.AspNetUser> CancelAspNetUserChanges(RadzenSchoolTenants.Server.Models.ConData.AspNetUser item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnAspNetUserUpdated(RadzenSchoolTenants.Server.Models.ConData.AspNetUser item);
        partial void OnAfterAspNetUserUpdated(RadzenSchoolTenants.Server.Models.ConData.AspNetUser item);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.AspNetUser> UpdateAspNetUser(string id, RadzenSchoolTenants.Server.Models.ConData.AspNetUser aspnetuser)
        {
            OnAspNetUserUpdated(aspnetuser);

            var itemToUpdate = Context.AspNetUsers
                              .Where(i => i.Id == aspnetuser.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(aspnetuser);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterAspNetUserUpdated(aspnetuser);

            return aspnetuser;
        }

        partial void OnAspNetUserDeleted(RadzenSchoolTenants.Server.Models.ConData.AspNetUser item);
        partial void OnAfterAspNetUserDeleted(RadzenSchoolTenants.Server.Models.ConData.AspNetUser item);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.AspNetUser> DeleteAspNetUser(string id)
        {
            var itemToDelete = Context.AspNetUsers
                              .Where(i => i.Id == id)
                              .Include(i => i.AspNetUserClaims)
                              .Include(i => i.AspNetUserLogins)
                              .Include(i => i.AspNetUserRoles)
                              .Include(i => i.AspNetUserTokens)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnAspNetUserDeleted(itemToDelete);


            Context.AspNetUsers.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterAspNetUserDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportAspNetUserTokensToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/aspnetusertokens/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/aspnetusertokens/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportAspNetUserTokensToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/aspnetusertokens/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/aspnetusertokens/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnAspNetUserTokensRead(ref IQueryable<RadzenSchoolTenants.Server.Models.ConData.AspNetUserToken> items);

        public async Task<IQueryable<RadzenSchoolTenants.Server.Models.ConData.AspNetUserToken>> GetAspNetUserTokens(Query query = null)
        {
            var items = Context.AspNetUserTokens.AsQueryable();

            items = items.Include(i => i.AspNetUser);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                if (!string.IsNullOrEmpty(query.Filter))
                {
                    if (query.FilterParameters != null)
                    {
                        items = items.Where(query.Filter, query.FilterParameters);
                    }
                    else
                    {
                        items = items.Where(query.Filter);
                    }
                }

                if (!string.IsNullOrEmpty(query.OrderBy))
                {
                    items = items.OrderBy(query.OrderBy);
                }

                if (query.Skip.HasValue)
                {
                    items = items.Skip(query.Skip.Value);
                }

                if (query.Top.HasValue)
                {
                    items = items.Take(query.Top.Value);
                }
            }

            OnAspNetUserTokensRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnAspNetUserTokenGet(RadzenSchoolTenants.Server.Models.ConData.AspNetUserToken item);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.AspNetUserToken> GetAspNetUserTokenByUserIdAndLoginProviderAndName(string userid, string loginprovider, string name)
        {
            var items = Context.AspNetUserTokens
                              .AsNoTracking()
                              .Where(i => i.UserId == userid && i.LoginProvider == loginprovider && i.Name == name);

            items = items.Include(i => i.AspNetUser);
  
            var itemToReturn = items.FirstOrDefault();

            OnAspNetUserTokenGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnAspNetUserTokenCreated(RadzenSchoolTenants.Server.Models.ConData.AspNetUserToken item);
        partial void OnAfterAspNetUserTokenCreated(RadzenSchoolTenants.Server.Models.ConData.AspNetUserToken item);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.AspNetUserToken> CreateAspNetUserToken(RadzenSchoolTenants.Server.Models.ConData.AspNetUserToken aspnetusertoken)
        {
            OnAspNetUserTokenCreated(aspnetusertoken);

            var existingItem = Context.AspNetUserTokens
                              .Where(i => i.UserId == aspnetusertoken.UserId && i.LoginProvider == aspnetusertoken.LoginProvider && i.Name == aspnetusertoken.Name)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.AspNetUserTokens.Add(aspnetusertoken);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(aspnetusertoken).State = EntityState.Detached;
                throw;
            }

            OnAfterAspNetUserTokenCreated(aspnetusertoken);

            return aspnetusertoken;
        }

        public async Task<RadzenSchoolTenants.Server.Models.ConData.AspNetUserToken> CancelAspNetUserTokenChanges(RadzenSchoolTenants.Server.Models.ConData.AspNetUserToken item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnAspNetUserTokenUpdated(RadzenSchoolTenants.Server.Models.ConData.AspNetUserToken item);
        partial void OnAfterAspNetUserTokenUpdated(RadzenSchoolTenants.Server.Models.ConData.AspNetUserToken item);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.AspNetUserToken> UpdateAspNetUserToken(string userid, string loginprovider, string name, RadzenSchoolTenants.Server.Models.ConData.AspNetUserToken aspnetusertoken)
        {
            OnAspNetUserTokenUpdated(aspnetusertoken);

            var itemToUpdate = Context.AspNetUserTokens
                              .Where(i => i.UserId == aspnetusertoken.UserId && i.LoginProvider == aspnetusertoken.LoginProvider && i.Name == aspnetusertoken.Name)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(aspnetusertoken);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterAspNetUserTokenUpdated(aspnetusertoken);

            return aspnetusertoken;
        }

        partial void OnAspNetUserTokenDeleted(RadzenSchoolTenants.Server.Models.ConData.AspNetUserToken item);
        partial void OnAfterAspNetUserTokenDeleted(RadzenSchoolTenants.Server.Models.ConData.AspNetUserToken item);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.AspNetUserToken> DeleteAspNetUserToken(string userid, string loginprovider, string name)
        {
            var itemToDelete = Context.AspNetUserTokens
                              .Where(i => i.UserId == userid && i.LoginProvider == loginprovider && i.Name == name)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnAspNetUserTokenDeleted(itemToDelete);


            Context.AspNetUserTokens.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterAspNetUserTokenDeleted(itemToDelete);

            return itemToDelete;
        }
          public async Task<int> AddNewUserToAppRoles(string UserId, string RoleId)
      {
          OnAddNewUserToAppRolesDefaultParams(ref UserId, ref RoleId);

          SqlParameter[] @params =
          {
              new SqlParameter("@returnVal", SqlDbType.Int) {Direction = ParameterDirection.Output},
              new SqlParameter("@UserId", SqlDbType.NVarChar) {Direction = ParameterDirection.Input, Value = UserId},
              new SqlParameter("@RoleId", SqlDbType.NVarChar) {Direction = ParameterDirection.Input, Value = RoleId},

          };

          foreach(var _p in @params)
          {
              if(_p.Direction == ParameterDirection.Input && _p.Value == null)
              {
                  _p.Value = DBNull.Value;
              }
          }

          Context.Database.ExecuteSqlRaw("EXEC @returnVal=[dbo].[AddNewUserToAppRole] @UserId, @RoleId", @params);

          int result = Convert.ToInt32(@params[0].Value);


          OnAddNewUserToAppRolesInvoke(ref result);

          return await Task.FromResult(result);
      }

      partial void OnAddNewUserToAppRolesDefaultParams(ref string UserId, ref string RoleId);
      partial void OnAddNewUserToAppRolesInvoke(ref int result);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.AspNetUser> GetAspNetUserByEmail(string email)
        {
            var user = new RadzenSchoolTenants.Server.Models.ConData.AspNetUser();

            var returnedUser = await Context.AspNetUsers.FirstOrDefaultAsync(p => p.Email == email);
            if(returnedUser!= null)
            {
                user = returnedUser;
            }

            return await Task.FromResult(user);
        }
    }
}