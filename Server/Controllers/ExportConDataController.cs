using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

using RadzenSchoolTenants.Server.Data;

namespace RadzenSchoolTenants.Server.Controllers
{
    public partial class ExportConDataController : ExportController
    {
        private readonly ConDataContext context;
        private readonly ConDataService service;

        public ExportConDataController(ConDataContext context, ConDataService service)
        {
            this.service = service;
            this.context = context;
        }

        [HttpGet("/export/ConData/schools/csv")]
        [HttpGet("/export/ConData/schools/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportSchoolsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetSchools(), Request.Query), fileName);
        }

        [HttpGet("/export/ConData/schools/excel")]
        [HttpGet("/export/ConData/schools/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportSchoolsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetSchools(), Request.Query), fileName);
        }

        [HttpGet("/export/ConData/students/csv")]
        [HttpGet("/export/ConData/students/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportStudentsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetStudents(), Request.Query), fileName);
        }

        [HttpGet("/export/ConData/students/excel")]
        [HttpGet("/export/ConData/students/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportStudentsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetStudents(), Request.Query), fileName);
        }

        [HttpGet("/export/ConData/aspnetroleclaims/csv")]
        [HttpGet("/export/ConData/aspnetroleclaims/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspNetRoleClaimsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAspNetRoleClaims(), Request.Query), fileName);
        }

        [HttpGet("/export/ConData/aspnetroleclaims/excel")]
        [HttpGet("/export/ConData/aspnetroleclaims/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspNetRoleClaimsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAspNetRoleClaims(), Request.Query), fileName);
        }

        [HttpGet("/export/ConData/aspnetroles/csv")]
        [HttpGet("/export/ConData/aspnetroles/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspNetRolesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAspNetRoles(), Request.Query), fileName);
        }

        [HttpGet("/export/ConData/aspnetroles/excel")]
        [HttpGet("/export/ConData/aspnetroles/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspNetRolesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAspNetRoles(), Request.Query), fileName);
        }

        [HttpGet("/export/ConData/aspnettenants/csv")]
        [HttpGet("/export/ConData/aspnettenants/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspNetTenantsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAspNetTenants(), Request.Query), fileName);
        }

        [HttpGet("/export/ConData/aspnettenants/excel")]
        [HttpGet("/export/ConData/aspnettenants/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspNetTenantsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAspNetTenants(), Request.Query), fileName);
        }

        [HttpGet("/export/ConData/aspnetuserclaims/csv")]
        [HttpGet("/export/ConData/aspnetuserclaims/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspNetUserClaimsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAspNetUserClaims(), Request.Query), fileName);
        }

        [HttpGet("/export/ConData/aspnetuserclaims/excel")]
        [HttpGet("/export/ConData/aspnetuserclaims/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspNetUserClaimsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAspNetUserClaims(), Request.Query), fileName);
        }

        [HttpGet("/export/ConData/aspnetuserlogins/csv")]
        [HttpGet("/export/ConData/aspnetuserlogins/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspNetUserLoginsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAspNetUserLogins(), Request.Query), fileName);
        }

        [HttpGet("/export/ConData/aspnetuserlogins/excel")]
        [HttpGet("/export/ConData/aspnetuserlogins/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspNetUserLoginsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAspNetUserLogins(), Request.Query), fileName);
        }

        [HttpGet("/export/ConData/aspnetuserroles/csv")]
        [HttpGet("/export/ConData/aspnetuserroles/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspNetUserRolesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAspNetUserRoles(), Request.Query), fileName);
        }

        [HttpGet("/export/ConData/aspnetuserroles/excel")]
        [HttpGet("/export/ConData/aspnetuserroles/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspNetUserRolesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAspNetUserRoles(), Request.Query), fileName);
        }

        [HttpGet("/export/ConData/aspnetusers/csv")]
        [HttpGet("/export/ConData/aspnetusers/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspNetUsersToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAspNetUsers(), Request.Query), fileName);
        }

        [HttpGet("/export/ConData/aspnetusers/excel")]
        [HttpGet("/export/ConData/aspnetusers/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspNetUsersToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAspNetUsers(), Request.Query), fileName);
        }

        [HttpGet("/export/ConData/aspnetusertokens/csv")]
        [HttpGet("/export/ConData/aspnetusertokens/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspNetUserTokensToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAspNetUserTokens(), Request.Query), fileName);
        }

        [HttpGet("/export/ConData/aspnetusertokens/excel")]
        [HttpGet("/export/ConData/aspnetusertokens/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspNetUserTokensToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAspNetUserTokens(), Request.Query), fileName);
        }
    }
}
