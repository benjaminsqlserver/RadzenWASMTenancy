
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Web;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using Radzen;

namespace RadzenSchoolTenants.Client
{
    public partial class ConDataService
    {
        private readonly HttpClient httpClient;
        private readonly Uri baseUri;
        private readonly NavigationManager navigationManager;

        public ConDataService(NavigationManager navigationManager, HttpClient httpClient, IConfiguration configuration)
        {
            this.httpClient = httpClient;

            this.navigationManager = navigationManager;
            this.baseUri = new Uri($"{navigationManager.BaseUri}odata/ConData/");
        }


        public async System.Threading.Tasks.Task ExportSchoolsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/schools/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/schools/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async System.Threading.Tasks.Task ExportSchoolsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/schools/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/schools/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnGetSchools(HttpRequestMessage requestMessage);

        public async Task<Radzen.ODataServiceResult<RadzenSchoolTenants.Server.Models.ConData.School>> GetSchools(Query query)
        {
            return await GetSchools(filter:$"{query.Filter}", orderby:$"{query.OrderBy}", top:query.Top, skip:query.Skip, count:query.Top != null && query.Skip != null);
        }

        public async Task<Radzen.ODataServiceResult<RadzenSchoolTenants.Server.Models.ConData.School>> GetSchools(string filter = default(string), string orderby = default(string), string expand = default(string), int? top = default(int?), int? skip = default(int?), bool? count = default(bool?), string format = default(string), string select = default(string))
        {
            var uri = new Uri(baseUri, $"Schools");
            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:filter, top:top, skip:skip, orderby:orderby, expand:expand, select:select, count:count);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetSchools(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<Radzen.ODataServiceResult<RadzenSchoolTenants.Server.Models.ConData.School>>(response);
        }

        partial void OnCreateSchool(HttpRequestMessage requestMessage);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.School> CreateSchool(RadzenSchoolTenants.Server.Models.ConData.School school = default(RadzenSchoolTenants.Server.Models.ConData.School))
        {
            var uri = new Uri(baseUri, $"Schools");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(school), Encoding.UTF8, "application/json");

            OnCreateSchool(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<RadzenSchoolTenants.Server.Models.ConData.School>(response);
        }

        partial void OnDeleteSchool(HttpRequestMessage requestMessage);

        public async Task<HttpResponseMessage> DeleteSchool(long schoolId = default(long))
        {
            var uri = new Uri(baseUri, $"Schools({schoolId})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            OnDeleteSchool(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        partial void OnGetSchoolBySchoolId(HttpRequestMessage requestMessage);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.School> GetSchoolBySchoolId(string expand = default(string), long schoolId = default(long))
        {
            var uri = new Uri(baseUri, $"Schools({schoolId})");

            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:null, top:null, skip:null, orderby:null, expand:expand, select:null, count:null);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetSchoolBySchoolId(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<RadzenSchoolTenants.Server.Models.ConData.School>(response);
        }

        partial void OnUpdateSchool(HttpRequestMessage requestMessage);
        
        public async Task<HttpResponseMessage> UpdateSchool(long schoolId = default(long), RadzenSchoolTenants.Server.Models.ConData.School school = default(RadzenSchoolTenants.Server.Models.ConData.School))
        {
            var uri = new Uri(baseUri, $"Schools({schoolId})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);

            httpRequestMessage.Headers.Add("If-Match", school.ETag);    

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(school), Encoding.UTF8, "application/json");

            OnUpdateSchool(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        public async System.Threading.Tasks.Task ExportStudentsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/students/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/students/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async System.Threading.Tasks.Task ExportStudentsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/students/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/students/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnGetStudents(HttpRequestMessage requestMessage);

        public async Task<Radzen.ODataServiceResult<RadzenSchoolTenants.Server.Models.ConData.Student>> GetStudents(Query query)
        {
            return await GetStudents(filter:$"{query.Filter}", orderby:$"{query.OrderBy}", top:query.Top, skip:query.Skip, count:query.Top != null && query.Skip != null);
        }

        public async Task<Radzen.ODataServiceResult<RadzenSchoolTenants.Server.Models.ConData.Student>> GetStudents(string filter = default(string), string orderby = default(string), string expand = default(string), int? top = default(int?), int? skip = default(int?), bool? count = default(bool?), string format = default(string), string select = default(string))
        {
            var uri = new Uri(baseUri, $"Students");
            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:filter, top:top, skip:skip, orderby:orderby, expand:expand, select:select, count:count);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetStudents(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<Radzen.ODataServiceResult<RadzenSchoolTenants.Server.Models.ConData.Student>>(response);
        }

        partial void OnCreateStudent(HttpRequestMessage requestMessage);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.Student> CreateStudent(RadzenSchoolTenants.Server.Models.ConData.Student student = default(RadzenSchoolTenants.Server.Models.ConData.Student))
        {
            var uri = new Uri(baseUri, $"Students");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(student), Encoding.UTF8, "application/json");

            OnCreateStudent(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<RadzenSchoolTenants.Server.Models.ConData.Student>(response);
        }

        partial void OnDeleteStudent(HttpRequestMessage requestMessage);

        public async Task<HttpResponseMessage> DeleteStudent(long studentId = default(long))
        {
            var uri = new Uri(baseUri, $"Students({studentId})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            OnDeleteStudent(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        partial void OnGetStudentByStudentId(HttpRequestMessage requestMessage);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.Student> GetStudentByStudentId(string expand = default(string), long studentId = default(long))
        {
            var uri = new Uri(baseUri, $"Students({studentId})");

            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:null, top:null, skip:null, orderby:null, expand:expand, select:null, count:null);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetStudentByStudentId(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<RadzenSchoolTenants.Server.Models.ConData.Student>(response);
        }

        partial void OnUpdateStudent(HttpRequestMessage requestMessage);
        
        public async Task<HttpResponseMessage> UpdateStudent(long studentId = default(long), RadzenSchoolTenants.Server.Models.ConData.Student student = default(RadzenSchoolTenants.Server.Models.ConData.Student))
        {
            var uri = new Uri(baseUri, $"Students({studentId})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);

            httpRequestMessage.Headers.Add("If-Match", student.ETag);    

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(student), Encoding.UTF8, "application/json");

            OnUpdateStudent(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        public async System.Threading.Tasks.Task ExportAspNetRoleClaimsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/aspnetroleclaims/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/aspnetroleclaims/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async System.Threading.Tasks.Task ExportAspNetRoleClaimsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/aspnetroleclaims/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/aspnetroleclaims/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnGetAspNetRoleClaims(HttpRequestMessage requestMessage);

        public async Task<Radzen.ODataServiceResult<RadzenSchoolTenants.Server.Models.ConData.AspNetRoleClaim>> GetAspNetRoleClaims(Query query)
        {
            return await GetAspNetRoleClaims(filter:$"{query.Filter}", orderby:$"{query.OrderBy}", top:query.Top, skip:query.Skip, count:query.Top != null && query.Skip != null);
        }

        public async Task<Radzen.ODataServiceResult<RadzenSchoolTenants.Server.Models.ConData.AspNetRoleClaim>> GetAspNetRoleClaims(string filter = default(string), string orderby = default(string), string expand = default(string), int? top = default(int?), int? skip = default(int?), bool? count = default(bool?), string format = default(string), string select = default(string))
        {
            var uri = new Uri(baseUri, $"AspNetRoleClaims");
            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:filter, top:top, skip:skip, orderby:orderby, expand:expand, select:select, count:count);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetAspNetRoleClaims(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<Radzen.ODataServiceResult<RadzenSchoolTenants.Server.Models.ConData.AspNetRoleClaim>>(response);
        }

        partial void OnCreateAspNetRoleClaim(HttpRequestMessage requestMessage);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.AspNetRoleClaim> CreateAspNetRoleClaim(RadzenSchoolTenants.Server.Models.ConData.AspNetRoleClaim aspNetRoleClaim = default(RadzenSchoolTenants.Server.Models.ConData.AspNetRoleClaim))
        {
            var uri = new Uri(baseUri, $"AspNetRoleClaims");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(aspNetRoleClaim), Encoding.UTF8, "application/json");

            OnCreateAspNetRoleClaim(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<RadzenSchoolTenants.Server.Models.ConData.AspNetRoleClaim>(response);
        }

        partial void OnDeleteAspNetRoleClaim(HttpRequestMessage requestMessage);

        public async Task<HttpResponseMessage> DeleteAspNetRoleClaim(int id = default(int))
        {
            var uri = new Uri(baseUri, $"AspNetRoleClaims({id})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            OnDeleteAspNetRoleClaim(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        partial void OnGetAspNetRoleClaimById(HttpRequestMessage requestMessage);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.AspNetRoleClaim> GetAspNetRoleClaimById(string expand = default(string), int id = default(int))
        {
            var uri = new Uri(baseUri, $"AspNetRoleClaims({id})");

            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:null, top:null, skip:null, orderby:null, expand:expand, select:null, count:null);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetAspNetRoleClaimById(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<RadzenSchoolTenants.Server.Models.ConData.AspNetRoleClaim>(response);
        }

        partial void OnUpdateAspNetRoleClaim(HttpRequestMessage requestMessage);
        
        public async Task<HttpResponseMessage> UpdateAspNetRoleClaim(int id = default(int), RadzenSchoolTenants.Server.Models.ConData.AspNetRoleClaim aspNetRoleClaim = default(RadzenSchoolTenants.Server.Models.ConData.AspNetRoleClaim))
        {
            var uri = new Uri(baseUri, $"AspNetRoleClaims({id})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);

            httpRequestMessage.Headers.Add("If-Match", aspNetRoleClaim.ETag);    

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(aspNetRoleClaim), Encoding.UTF8, "application/json");

            OnUpdateAspNetRoleClaim(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        public async System.Threading.Tasks.Task ExportAspNetRolesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/aspnetroles/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/aspnetroles/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async System.Threading.Tasks.Task ExportAspNetRolesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/aspnetroles/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/aspnetroles/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnGetAspNetRoles(HttpRequestMessage requestMessage);

        public async Task<Radzen.ODataServiceResult<RadzenSchoolTenants.Server.Models.ConData.AspNetRole>> GetAspNetRoles(Query query)
        {
            return await GetAspNetRoles(filter:$"{query.Filter}", orderby:$"{query.OrderBy}", top:query.Top, skip:query.Skip, count:query.Top != null && query.Skip != null);
        }

        public async Task<Radzen.ODataServiceResult<RadzenSchoolTenants.Server.Models.ConData.AspNetRole>> GetAspNetRoles(string filter = default(string), string orderby = default(string), string expand = default(string), int? top = default(int?), int? skip = default(int?), bool? count = default(bool?), string format = default(string), string select = default(string))
        {
            var uri = new Uri(baseUri, $"AspNetRoles");
            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:filter, top:top, skip:skip, orderby:orderby, expand:expand, select:select, count:count);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetAspNetRoles(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<Radzen.ODataServiceResult<RadzenSchoolTenants.Server.Models.ConData.AspNetRole>>(response);
        }

        partial void OnCreateAspNetRole(HttpRequestMessage requestMessage);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.AspNetRole> CreateAspNetRole(RadzenSchoolTenants.Server.Models.ConData.AspNetRole aspNetRole = default(RadzenSchoolTenants.Server.Models.ConData.AspNetRole))
        {
            var uri = new Uri(baseUri, $"AspNetRoles");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(aspNetRole), Encoding.UTF8, "application/json");

            OnCreateAspNetRole(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<RadzenSchoolTenants.Server.Models.ConData.AspNetRole>(response);
        }

        partial void OnDeleteAspNetRole(HttpRequestMessage requestMessage);

        public async Task<HttpResponseMessage> DeleteAspNetRole(string id = default(string))
        {
            var uri = new Uri(baseUri, $"AspNetRoles('{HttpUtility.UrlEncode(id.Trim().Replace("'", "''").Replace(" ","%20"))}')");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            OnDeleteAspNetRole(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        partial void OnGetAspNetRoleById(HttpRequestMessage requestMessage);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.AspNetRole> GetAspNetRoleById(string expand = default(string), string id = default(string))
        {
            var uri = new Uri(baseUri, $"AspNetRoles('{HttpUtility.UrlEncode(id.Trim().Replace("'", "''").Replace(" ","%20"))}')");

            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:null, top:null, skip:null, orderby:null, expand:expand, select:null, count:null);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetAspNetRoleById(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<RadzenSchoolTenants.Server.Models.ConData.AspNetRole>(response);
        }

        partial void OnUpdateAspNetRole(HttpRequestMessage requestMessage);
        
        public async Task<HttpResponseMessage> UpdateAspNetRole(string id = default(string), RadzenSchoolTenants.Server.Models.ConData.AspNetRole aspNetRole = default(RadzenSchoolTenants.Server.Models.ConData.AspNetRole))
        {
            var uri = new Uri(baseUri, $"AspNetRoles('{HttpUtility.UrlEncode(id.Trim().Replace("'", "''").Replace(" ","%20"))}')");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);

            httpRequestMessage.Headers.Add("If-Match", aspNetRole.ETag);    

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(aspNetRole), Encoding.UTF8, "application/json");

            OnUpdateAspNetRole(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        public async System.Threading.Tasks.Task ExportAspNetTenantsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/aspnettenants/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/aspnettenants/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async System.Threading.Tasks.Task ExportAspNetTenantsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/aspnettenants/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/aspnettenants/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnGetAspNetTenants(HttpRequestMessage requestMessage);

        public async Task<Radzen.ODataServiceResult<RadzenSchoolTenants.Server.Models.ConData.AspNetTenant>> GetAspNetTenants(Query query)
        {
            return await GetAspNetTenants(filter:$"{query.Filter}", orderby:$"{query.OrderBy}", top:query.Top, skip:query.Skip, count:query.Top != null && query.Skip != null);
        }

        public async Task<Radzen.ODataServiceResult<RadzenSchoolTenants.Server.Models.ConData.AspNetTenant>> GetAspNetTenants(string filter = default(string), string orderby = default(string), string expand = default(string), int? top = default(int?), int? skip = default(int?), bool? count = default(bool?), string format = default(string), string select = default(string))
        {
            var uri = new Uri(baseUri, $"AspNetTenants");
            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:filter, top:top, skip:skip, orderby:orderby, expand:expand, select:select, count:count);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetAspNetTenants(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<Radzen.ODataServiceResult<RadzenSchoolTenants.Server.Models.ConData.AspNetTenant>>(response);
        }

        partial void OnCreateAspNetTenant(HttpRequestMessage requestMessage);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.AspNetTenant> CreateAspNetTenant(RadzenSchoolTenants.Server.Models.ConData.AspNetTenant aspNetTenant = default(RadzenSchoolTenants.Server.Models.ConData.AspNetTenant))
        {
            var uri = new Uri(baseUri, $"AspNetTenants");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(aspNetTenant), Encoding.UTF8, "application/json");

            OnCreateAspNetTenant(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<RadzenSchoolTenants.Server.Models.ConData.AspNetTenant>(response);
        }

        partial void OnDeleteAspNetTenant(HttpRequestMessage requestMessage);

        public async Task<HttpResponseMessage> DeleteAspNetTenant(int id = default(int))
        {
            var uri = new Uri(baseUri, $"AspNetTenants({id})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            OnDeleteAspNetTenant(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        partial void OnGetAspNetTenantById(HttpRequestMessage requestMessage);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.AspNetTenant> GetAspNetTenantById(string expand = default(string), int id = default(int))
        {
            var uri = new Uri(baseUri, $"AspNetTenants({id})");

            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:null, top:null, skip:null, orderby:null, expand:expand, select:null, count:null);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetAspNetTenantById(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<RadzenSchoolTenants.Server.Models.ConData.AspNetTenant>(response);
        }

        partial void OnUpdateAspNetTenant(HttpRequestMessage requestMessage);
        
        public async Task<HttpResponseMessage> UpdateAspNetTenant(int id = default(int), RadzenSchoolTenants.Server.Models.ConData.AspNetTenant aspNetTenant = default(RadzenSchoolTenants.Server.Models.ConData.AspNetTenant))
        {
            var uri = new Uri(baseUri, $"AspNetTenants({id})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);

            httpRequestMessage.Headers.Add("If-Match", aspNetTenant.ETag);    

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(aspNetTenant), Encoding.UTF8, "application/json");

            OnUpdateAspNetTenant(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        public async System.Threading.Tasks.Task ExportAspNetUserClaimsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/aspnetuserclaims/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/aspnetuserclaims/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async System.Threading.Tasks.Task ExportAspNetUserClaimsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/aspnetuserclaims/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/aspnetuserclaims/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnGetAspNetUserClaims(HttpRequestMessage requestMessage);

        public async Task<Radzen.ODataServiceResult<RadzenSchoolTenants.Server.Models.ConData.AspNetUserClaim>> GetAspNetUserClaims(Query query)
        {
            return await GetAspNetUserClaims(filter:$"{query.Filter}", orderby:$"{query.OrderBy}", top:query.Top, skip:query.Skip, count:query.Top != null && query.Skip != null);
        }

        public async Task<Radzen.ODataServiceResult<RadzenSchoolTenants.Server.Models.ConData.AspNetUserClaim>> GetAspNetUserClaims(string filter = default(string), string orderby = default(string), string expand = default(string), int? top = default(int?), int? skip = default(int?), bool? count = default(bool?), string format = default(string), string select = default(string))
        {
            var uri = new Uri(baseUri, $"AspNetUserClaims");
            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:filter, top:top, skip:skip, orderby:orderby, expand:expand, select:select, count:count);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetAspNetUserClaims(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<Radzen.ODataServiceResult<RadzenSchoolTenants.Server.Models.ConData.AspNetUserClaim>>(response);
        }

        partial void OnCreateAspNetUserClaim(HttpRequestMessage requestMessage);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.AspNetUserClaim> CreateAspNetUserClaim(RadzenSchoolTenants.Server.Models.ConData.AspNetUserClaim aspNetUserClaim = default(RadzenSchoolTenants.Server.Models.ConData.AspNetUserClaim))
        {
            var uri = new Uri(baseUri, $"AspNetUserClaims");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(aspNetUserClaim), Encoding.UTF8, "application/json");

            OnCreateAspNetUserClaim(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<RadzenSchoolTenants.Server.Models.ConData.AspNetUserClaim>(response);
        }

        partial void OnDeleteAspNetUserClaim(HttpRequestMessage requestMessage);

        public async Task<HttpResponseMessage> DeleteAspNetUserClaim(int id = default(int))
        {
            var uri = new Uri(baseUri, $"AspNetUserClaims({id})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            OnDeleteAspNetUserClaim(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        partial void OnGetAspNetUserClaimById(HttpRequestMessage requestMessage);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.AspNetUserClaim> GetAspNetUserClaimById(string expand = default(string), int id = default(int))
        {
            var uri = new Uri(baseUri, $"AspNetUserClaims({id})");

            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:null, top:null, skip:null, orderby:null, expand:expand, select:null, count:null);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetAspNetUserClaimById(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<RadzenSchoolTenants.Server.Models.ConData.AspNetUserClaim>(response);
        }

        partial void OnUpdateAspNetUserClaim(HttpRequestMessage requestMessage);
        
        public async Task<HttpResponseMessage> UpdateAspNetUserClaim(int id = default(int), RadzenSchoolTenants.Server.Models.ConData.AspNetUserClaim aspNetUserClaim = default(RadzenSchoolTenants.Server.Models.ConData.AspNetUserClaim))
        {
            var uri = new Uri(baseUri, $"AspNetUserClaims({id})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);

            httpRequestMessage.Headers.Add("If-Match", aspNetUserClaim.ETag);    

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(aspNetUserClaim), Encoding.UTF8, "application/json");

            OnUpdateAspNetUserClaim(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        public async System.Threading.Tasks.Task ExportAspNetUserLoginsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/aspnetuserlogins/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/aspnetuserlogins/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async System.Threading.Tasks.Task ExportAspNetUserLoginsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/aspnetuserlogins/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/aspnetuserlogins/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnGetAspNetUserLogins(HttpRequestMessage requestMessage);

        public async Task<Radzen.ODataServiceResult<RadzenSchoolTenants.Server.Models.ConData.AspNetUserLogin>> GetAspNetUserLogins(Query query)
        {
            return await GetAspNetUserLogins(filter:$"{query.Filter}", orderby:$"{query.OrderBy}", top:query.Top, skip:query.Skip, count:query.Top != null && query.Skip != null);
        }

        public async Task<Radzen.ODataServiceResult<RadzenSchoolTenants.Server.Models.ConData.AspNetUserLogin>> GetAspNetUserLogins(string filter = default(string), string orderby = default(string), string expand = default(string), int? top = default(int?), int? skip = default(int?), bool? count = default(bool?), string format = default(string), string select = default(string))
        {
            var uri = new Uri(baseUri, $"AspNetUserLogins");
            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:filter, top:top, skip:skip, orderby:orderby, expand:expand, select:select, count:count);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetAspNetUserLogins(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<Radzen.ODataServiceResult<RadzenSchoolTenants.Server.Models.ConData.AspNetUserLogin>>(response);
        }

        partial void OnCreateAspNetUserLogin(HttpRequestMessage requestMessage);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.AspNetUserLogin> CreateAspNetUserLogin(RadzenSchoolTenants.Server.Models.ConData.AspNetUserLogin aspNetUserLogin = default(RadzenSchoolTenants.Server.Models.ConData.AspNetUserLogin))
        {
            var uri = new Uri(baseUri, $"AspNetUserLogins");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(aspNetUserLogin), Encoding.UTF8, "application/json");

            OnCreateAspNetUserLogin(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<RadzenSchoolTenants.Server.Models.ConData.AspNetUserLogin>(response);
        }

        partial void OnDeleteAspNetUserLogin(HttpRequestMessage requestMessage);

        public async Task<HttpResponseMessage> DeleteAspNetUserLogin(string loginProvider = default(string), string providerKey = default(string))
        {
            var uri = new Uri(baseUri, $"AspNetUserLogins(LoginProvider='{HttpUtility.UrlEncode(loginProvider.Trim().Replace("'", "''").Replace(" ","%20"))}',ProviderKey='{HttpUtility.UrlEncode(providerKey.Trim().Replace("'", "''").Replace(" ","%20"))}')");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            OnDeleteAspNetUserLogin(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        partial void OnGetAspNetUserLoginByLoginProviderAndProviderKey(HttpRequestMessage requestMessage);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.AspNetUserLogin> GetAspNetUserLoginByLoginProviderAndProviderKey(string expand = default(string), string loginProvider = default(string), string providerKey = default(string))
        {
            var uri = new Uri(baseUri, $"AspNetUserLogins(LoginProvider='{HttpUtility.UrlEncode(loginProvider.Trim().Replace("'", "''").Replace(" ","%20"))}',ProviderKey='{HttpUtility.UrlEncode(providerKey.Trim().Replace("'", "''").Replace(" ","%20"))}')");

            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:null, top:null, skip:null, orderby:null, expand:expand, select:null, count:null);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetAspNetUserLoginByLoginProviderAndProviderKey(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<RadzenSchoolTenants.Server.Models.ConData.AspNetUserLogin>(response);
        }

        partial void OnUpdateAspNetUserLogin(HttpRequestMessage requestMessage);
        
        public async Task<HttpResponseMessage> UpdateAspNetUserLogin(string loginProvider = default(string), string providerKey = default(string), RadzenSchoolTenants.Server.Models.ConData.AspNetUserLogin aspNetUserLogin = default(RadzenSchoolTenants.Server.Models.ConData.AspNetUserLogin))
        {
            var uri = new Uri(baseUri, $"AspNetUserLogins(LoginProvider='{HttpUtility.UrlEncode(loginProvider.Trim().Replace("'", "''").Replace(" ","%20"))}',ProviderKey='{HttpUtility.UrlEncode(providerKey.Trim().Replace("'", "''").Replace(" ","%20"))}')");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);

            httpRequestMessage.Headers.Add("If-Match", aspNetUserLogin.ETag);    

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(aspNetUserLogin), Encoding.UTF8, "application/json");

            OnUpdateAspNetUserLogin(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        public async System.Threading.Tasks.Task ExportAspNetUserRolesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/aspnetuserroles/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/aspnetuserroles/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async System.Threading.Tasks.Task ExportAspNetUserRolesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/aspnetuserroles/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/aspnetuserroles/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnGetAspNetUserRoles(HttpRequestMessage requestMessage);

        public async Task<Radzen.ODataServiceResult<RadzenSchoolTenants.Server.Models.ConData.AspNetUserRole>> GetAspNetUserRoles(Query query)
        {
            return await GetAspNetUserRoles(filter:$"{query.Filter}", orderby:$"{query.OrderBy}", top:query.Top, skip:query.Skip, count:query.Top != null && query.Skip != null);
        }

        public async Task<Radzen.ODataServiceResult<RadzenSchoolTenants.Server.Models.ConData.AspNetUserRole>> GetAspNetUserRoles(string filter = default(string), string orderby = default(string), string expand = default(string), int? top = default(int?), int? skip = default(int?), bool? count = default(bool?), string format = default(string), string select = default(string))
        {
            var uri = new Uri(baseUri, $"AspNetUserRoles");
            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:filter, top:top, skip:skip, orderby:orderby, expand:expand, select:select, count:count);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetAspNetUserRoles(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<Radzen.ODataServiceResult<RadzenSchoolTenants.Server.Models.ConData.AspNetUserRole>>(response);
        }

        partial void OnCreateAspNetUserRole(HttpRequestMessage requestMessage);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.AspNetUserRole> CreateAspNetUserRole(RadzenSchoolTenants.Server.Models.ConData.AspNetUserRole aspNetUserRole = default(RadzenSchoolTenants.Server.Models.ConData.AspNetUserRole))
        {
            var uri = new Uri(baseUri, $"AspNetUserRoles");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(aspNetUserRole), Encoding.UTF8, "application/json");

            OnCreateAspNetUserRole(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<RadzenSchoolTenants.Server.Models.ConData.AspNetUserRole>(response);
        }

        partial void OnDeleteAspNetUserRole(HttpRequestMessage requestMessage);

        public async Task<HttpResponseMessage> DeleteAspNetUserRole(string userId = default(string), string roleId = default(string))
        {
            var uri = new Uri(baseUri, $"AspNetUserRoles(UserId='{HttpUtility.UrlEncode(userId.Trim().Replace("'", "''").Replace(" ","%20"))}',RoleId='{HttpUtility.UrlEncode(roleId.Trim().Replace("'", "''").Replace(" ","%20"))}')");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            OnDeleteAspNetUserRole(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        partial void OnGetAspNetUserRoleByUserIdAndRoleId(HttpRequestMessage requestMessage);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.AspNetUserRole> GetAspNetUserRoleByUserIdAndRoleId(string expand = default(string), string userId = default(string), string roleId = default(string))
        {
            var uri = new Uri(baseUri, $"AspNetUserRoles(UserId='{HttpUtility.UrlEncode(userId.Trim().Replace("'", "''").Replace(" ","%20"))}',RoleId='{HttpUtility.UrlEncode(roleId.Trim().Replace("'", "''").Replace(" ","%20"))}')");

            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:null, top:null, skip:null, orderby:null, expand:expand, select:null, count:null);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetAspNetUserRoleByUserIdAndRoleId(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<RadzenSchoolTenants.Server.Models.ConData.AspNetUserRole>(response);
        }

        partial void OnUpdateAspNetUserRole(HttpRequestMessage requestMessage);
        
        public async Task<HttpResponseMessage> UpdateAspNetUserRole(string userId = default(string), string roleId = default(string), RadzenSchoolTenants.Server.Models.ConData.AspNetUserRole aspNetUserRole = default(RadzenSchoolTenants.Server.Models.ConData.AspNetUserRole))
        {
            var uri = new Uri(baseUri, $"AspNetUserRoles(UserId='{HttpUtility.UrlEncode(userId.Trim().Replace("'", "''").Replace(" ","%20"))}',RoleId='{HttpUtility.UrlEncode(roleId.Trim().Replace("'", "''").Replace(" ","%20"))}')");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);

            httpRequestMessage.Headers.Add("If-Match", aspNetUserRole.ETag);    

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(aspNetUserRole), Encoding.UTF8, "application/json");

            OnUpdateAspNetUserRole(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        public async System.Threading.Tasks.Task ExportAspNetUsersToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/aspnetusers/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/aspnetusers/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async System.Threading.Tasks.Task ExportAspNetUsersToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/aspnetusers/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/aspnetusers/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnGetAspNetUsers(HttpRequestMessage requestMessage);

        public async Task<Radzen.ODataServiceResult<RadzenSchoolTenants.Server.Models.ConData.AspNetUser>> GetAspNetUsers(Query query)
        {
            return await GetAspNetUsers(filter:$"{query.Filter}", orderby:$"{query.OrderBy}", top:query.Top, skip:query.Skip, count:query.Top != null && query.Skip != null);
        }

        public async Task<Radzen.ODataServiceResult<RadzenSchoolTenants.Server.Models.ConData.AspNetUser>> GetAspNetUsers(string filter = default(string), string orderby = default(string), string expand = default(string), int? top = default(int?), int? skip = default(int?), bool? count = default(bool?), string format = default(string), string select = default(string))
        {
            var uri = new Uri(baseUri, $"AspNetUsers");
            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:filter, top:top, skip:skip, orderby:orderby, expand:expand, select:select, count:count);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetAspNetUsers(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<Radzen.ODataServiceResult<RadzenSchoolTenants.Server.Models.ConData.AspNetUser>>(response);
        }

        partial void OnCreateAspNetUser(HttpRequestMessage requestMessage);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.AspNetUser> CreateAspNetUser(RadzenSchoolTenants.Server.Models.ConData.AspNetUser aspNetUser = default(RadzenSchoolTenants.Server.Models.ConData.AspNetUser))
        {
            var uri = new Uri(baseUri, $"AspNetUsers");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(aspNetUser), Encoding.UTF8, "application/json");

            OnCreateAspNetUser(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<RadzenSchoolTenants.Server.Models.ConData.AspNetUser>(response);
        }

        partial void OnDeleteAspNetUser(HttpRequestMessage requestMessage);

        public async Task<HttpResponseMessage> DeleteAspNetUser(string id = default(string))
        {
            var uri = new Uri(baseUri, $"AspNetUsers('{HttpUtility.UrlEncode(id.Trim().Replace("'", "''").Replace(" ","%20"))}')");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            OnDeleteAspNetUser(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        partial void OnGetAspNetUserById(HttpRequestMessage requestMessage);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.AspNetUser> GetAspNetUserById(string expand = default(string), string id = default(string))
        {
            var uri = new Uri(baseUri, $"AspNetUsers('{HttpUtility.UrlEncode(id.Trim().Replace("'", "''").Replace(" ","%20"))}')");

            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:null, top:null, skip:null, orderby:null, expand:expand, select:null, count:null);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetAspNetUserById(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<RadzenSchoolTenants.Server.Models.ConData.AspNetUser>(response);
        }

        partial void OnUpdateAspNetUser(HttpRequestMessage requestMessage);
        
        public async Task<HttpResponseMessage> UpdateAspNetUser(string id = default(string), RadzenSchoolTenants.Server.Models.ConData.AspNetUser aspNetUser = default(RadzenSchoolTenants.Server.Models.ConData.AspNetUser))
        {
            var uri = new Uri(baseUri, $"AspNetUsers('{HttpUtility.UrlEncode(id.Trim().Replace("'", "''").Replace(" ","%20"))}')");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);

            httpRequestMessage.Headers.Add("If-Match", aspNetUser.ETag);    

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(aspNetUser), Encoding.UTF8, "application/json");

            OnUpdateAspNetUser(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        public async System.Threading.Tasks.Task ExportAspNetUserTokensToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/aspnetusertokens/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/aspnetusertokens/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async System.Threading.Tasks.Task ExportAspNetUserTokensToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/aspnetusertokens/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/aspnetusertokens/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnGetAspNetUserTokens(HttpRequestMessage requestMessage);

        public async Task<Radzen.ODataServiceResult<RadzenSchoolTenants.Server.Models.ConData.AspNetUserToken>> GetAspNetUserTokens(Query query)
        {
            return await GetAspNetUserTokens(filter:$"{query.Filter}", orderby:$"{query.OrderBy}", top:query.Top, skip:query.Skip, count:query.Top != null && query.Skip != null);
        }

        public async Task<Radzen.ODataServiceResult<RadzenSchoolTenants.Server.Models.ConData.AspNetUserToken>> GetAspNetUserTokens(string filter = default(string), string orderby = default(string), string expand = default(string), int? top = default(int?), int? skip = default(int?), bool? count = default(bool?), string format = default(string), string select = default(string))
        {
            var uri = new Uri(baseUri, $"AspNetUserTokens");
            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:filter, top:top, skip:skip, orderby:orderby, expand:expand, select:select, count:count);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetAspNetUserTokens(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<Radzen.ODataServiceResult<RadzenSchoolTenants.Server.Models.ConData.AspNetUserToken>>(response);
        }

        partial void OnCreateAspNetUserToken(HttpRequestMessage requestMessage);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.AspNetUserToken> CreateAspNetUserToken(RadzenSchoolTenants.Server.Models.ConData.AspNetUserToken aspNetUserToken = default(RadzenSchoolTenants.Server.Models.ConData.AspNetUserToken))
        {
            var uri = new Uri(baseUri, $"AspNetUserTokens");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(aspNetUserToken), Encoding.UTF8, "application/json");

            OnCreateAspNetUserToken(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<RadzenSchoolTenants.Server.Models.ConData.AspNetUserToken>(response);
        }

        partial void OnDeleteAspNetUserToken(HttpRequestMessage requestMessage);

        public async Task<HttpResponseMessage> DeleteAspNetUserToken(string userId = default(string), string loginProvider = default(string), string name = default(string))
        {
            var uri = new Uri(baseUri, $"AspNetUserTokens(UserId='{HttpUtility.UrlEncode(userId.Trim().Replace("'", "''").Replace(" ","%20"))}',LoginProvider='{HttpUtility.UrlEncode(loginProvider.Trim().Replace("'", "''").Replace(" ","%20"))}',Name='{HttpUtility.UrlEncode(name.Trim().Replace("'", "''").Replace(" ","%20"))}')");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            OnDeleteAspNetUserToken(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        partial void OnGetAspNetUserTokenByUserIdAndLoginProviderAndName(HttpRequestMessage requestMessage);

        public async Task<RadzenSchoolTenants.Server.Models.ConData.AspNetUserToken> GetAspNetUserTokenByUserIdAndLoginProviderAndName(string expand = default(string), string userId = default(string), string loginProvider = default(string), string name = default(string))
        {
            var uri = new Uri(baseUri, $"AspNetUserTokens(UserId='{HttpUtility.UrlEncode(userId.Trim().Replace("'", "''").Replace(" ","%20"))}',LoginProvider='{HttpUtility.UrlEncode(loginProvider.Trim().Replace("'", "''").Replace(" ","%20"))}',Name='{HttpUtility.UrlEncode(name.Trim().Replace("'", "''").Replace(" ","%20"))}')");

            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:null, top:null, skip:null, orderby:null, expand:expand, select:null, count:null);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetAspNetUserTokenByUserIdAndLoginProviderAndName(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<RadzenSchoolTenants.Server.Models.ConData.AspNetUserToken>(response);
        }

        partial void OnUpdateAspNetUserToken(HttpRequestMessage requestMessage);
        
        public async Task<HttpResponseMessage> UpdateAspNetUserToken(string userId = default(string), string loginProvider = default(string), string name = default(string), RadzenSchoolTenants.Server.Models.ConData.AspNetUserToken aspNetUserToken = default(RadzenSchoolTenants.Server.Models.ConData.AspNetUserToken))
        {
            var uri = new Uri(baseUri, $"AspNetUserTokens(UserId='{HttpUtility.UrlEncode(userId.Trim().Replace("'", "''").Replace(" ","%20"))}',LoginProvider='{HttpUtility.UrlEncode(loginProvider.Trim().Replace("'", "''").Replace(" ","%20"))}',Name='{HttpUtility.UrlEncode(name.Trim().Replace("'", "''").Replace(" ","%20"))}')");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);

            httpRequestMessage.Headers.Add("If-Match", aspNetUserToken.ETag);    

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(aspNetUserToken), Encoding.UTF8, "application/json");

            OnUpdateAspNetUserToken(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        partial void OnAddNewUserToAppRoles(HttpRequestMessage requestMessage);

        public async Task<HttpResponseMessage> AddNewUserToAppRoles(string userId = default(string), string roleId = default(string))
        {
            var uri = new Uri(baseUri, $"AddNewUserToAppRolesFunc(UserId='{HttpUtility.UrlEncode(userId.Trim().Replace("'", "''").Replace(" ","%20"))}',RoleId='{HttpUtility.UrlEncode(roleId.Trim().Replace("'", "''").Replace(" ","%20"))}')");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnAddNewUserToAppRoles(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }
    }
}