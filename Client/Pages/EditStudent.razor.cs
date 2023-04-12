using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace RadzenSchoolTenants.Client.Pages
{
    public partial class EditStudent
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }
        [Inject]
        public ConDataService ConDataService { get; set; }

        [Parameter]
        public long StudentID { get; set; }

        protected override async Task OnInitializedAsync()
        {
            student = await ConDataService.GetStudentByStudentId(studentId:StudentID);
        }
        protected bool errorVisible;
        protected RadzenSchoolTenants.Server.Models.ConData.Student student;

        protected IEnumerable<RadzenSchoolTenants.Server.Models.ConData.School> schoolsForSchoolID;


        protected int schoolsForSchoolIDCount;
        protected RadzenSchoolTenants.Server.Models.ConData.School schoolsForSchoolIDValue;
        protected async Task schoolsForSchoolIDLoadData(LoadDataArgs args)
        {
            try
            {
                var result = await ConDataService.GetSchools(top: args.Top, skip: args.Skip, count:args.Top != null && args.Skip != null, filter: $"{args.Filter}", orderby: $"{args.OrderBy}");
                schoolsForSchoolID = result.Value.AsODataEnumerable();
                schoolsForSchoolIDCount = result.Count;

            }
            catch (System.Exception ex)
            {
                NotificationService.Notify(new NotificationMessage(){ Severity = NotificationSeverity.Error, Summary = $"Error", Detail = $"Unable to load School" });
            }
        }
        protected async Task FormSubmit()
        {
            try
            {
                var result = await ConDataService.UpdateStudent(studentId:StudentID, student);
                if (result.StatusCode == System.Net.HttpStatusCode.PreconditionFailed)
                {
                     hasChanges = true;
                     canEdit = false;
                     return;
                }
                DialogService.Close(student);
            }
            catch (Exception ex)
            {
                errorVisible = true;
            }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }


        protected bool hasChanges = false;
        protected bool canEdit = true;

        [Inject]
        protected SecurityService Security { get; set; }


        protected async Task ReloadButtonClick(MouseEventArgs args)
        {
            hasChanges = false;
            canEdit = true;

            student = await ConDataService.GetStudentByStudentId(studentId:StudentID);
        }
    }
}