using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Radzen;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder;
using Microsoft.AspNetCore.OData;
using RadzenSchoolTenants.Server.Data;
using Microsoft.AspNetCore.Identity;
using RadzenSchoolTenants.Server.Models;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TooltipService>();
builder.Services.AddScoped<ContextMenuService>();
builder.Services.AddSingleton(sp =>
{
    // Get the address that the app is currently running at
    var server = sp.GetRequiredService<IServer>();
    var addressFeature = server.Features.Get<IServerAddressesFeature>();
    string baseAddress = addressFeature.Addresses.First();
    return new HttpClient{BaseAddress = new Uri(baseAddress), Timeout = TimeSpan.FromMinutes(20)};
});
builder.Services.AddScoped<RadzenSchoolTenants.Server.ConDataService>();
builder.Services.AddDbContext<RadzenSchoolTenants.Server.Data.ConDataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConDataConnection"));
});
builder.Services.AddControllers().AddOData(opt =>
{
    var oDataBuilderConData = new ODataConventionModelBuilder();
    oDataBuilderConData.EntitySet<RadzenSchoolTenants.Server.Models.ConData.School>("Schools");
    oDataBuilderConData.EntitySet<RadzenSchoolTenants.Server.Models.ConData.Student>("Students");
    oDataBuilderConData.EntitySet<RadzenSchoolTenants.Server.Models.ConData.AspNetRoleClaim>("AspNetRoleClaims");
    oDataBuilderConData.EntitySet<RadzenSchoolTenants.Server.Models.ConData.AspNetRole>("AspNetRoles");
    oDataBuilderConData.EntitySet<RadzenSchoolTenants.Server.Models.ConData.AspNetTenant>("AspNetTenants");
    oDataBuilderConData.EntitySet<RadzenSchoolTenants.Server.Models.ConData.AspNetUserClaim>("AspNetUserClaims");
    oDataBuilderConData.EntitySet<RadzenSchoolTenants.Server.Models.ConData.AspNetUserLogin>("AspNetUserLogins").EntityType.HasKey(entity => new
    {
    entity.LoginProvider, entity.ProviderKey
    });
    oDataBuilderConData.EntitySet<RadzenSchoolTenants.Server.Models.ConData.AspNetUserRole>("AspNetUserRoles").EntityType.HasKey(entity => new
    {
    entity.UserId, entity.RoleId
    });
    oDataBuilderConData.EntitySet<RadzenSchoolTenants.Server.Models.ConData.AspNetUser>("AspNetUsers");
    oDataBuilderConData.EntitySet<RadzenSchoolTenants.Server.Models.ConData.AspNetUserToken>("AspNetUserTokens").EntityType.HasKey(entity => new
    {
    entity.UserId, entity.LoginProvider, entity.Name
    });
    var conDataAddNewUserToAppRole = oDataBuilderConData.Function("AddNewUserToAppRolesFunc");
    conDataAddNewUserToAppRole.Parameter<string>("UserId");
    conDataAddNewUserToAppRole.Parameter<string>("RoleId");
    conDataAddNewUserToAppRole.Returns(typeof(int));
    opt.AddRouteComponents("odata/ConData", oDataBuilderConData.GetEdmModel()).Count().Filter().OrderBy().Expand().Select().SetMaxTop(null).TimeZone = TimeZoneInfo.Utc;
});
builder.Services.AddScoped<RadzenSchoolTenants.Client.ConDataService>();
builder.Services.AddHttpClient("RadzenSchoolTenants.Server").AddHeaderPropagation(o => o.Headers.Add("Cookie"));
builder.Services.AddHeaderPropagation(o => o.Headers.Add("Cookie"));
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddScoped<RadzenSchoolTenants.Client.SecurityService>();
builder.Services.AddDbContext<ApplicationIdentityDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConDataConnection"));
});
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>().AddEntityFrameworkStores<ApplicationIdentityDbContext>().AddDefaultTokenProviders();
builder.Services.AddTransient<IUserStore<ApplicationUser>, MultiTenancyUserStore>();
builder.Services.AddControllers().AddOData(o =>
{
    var oDataBuilder = new ODataConventionModelBuilder();
    oDataBuilder.EntitySet<ApplicationUser>("ApplicationUsers");
    var usersType = oDataBuilder.StructuralTypes.First(x => x.ClrType == typeof(ApplicationUser));
    usersType.AddProperty(typeof(ApplicationUser).GetProperty(nameof(ApplicationUser.Password)));
    usersType.AddProperty(typeof(ApplicationUser).GetProperty(nameof(ApplicationUser.ConfirmPassword)));
    oDataBuilder.EntitySet<ApplicationRole>("ApplicationRoles");
    oDataBuilder.EntitySet<ApplicationTenant>("ApplicationTenants");
    o.AddRouteComponents("odata/Identity", oDataBuilder.GetEdmModel()).Count().Filter().OrderBy().Expand().Select().SetMaxTop(null).TimeZone = TimeZoneInfo.Utc;
});
builder.Services.AddScoped<AuthenticationStateProvider, RadzenSchoolTenants.Client.ApplicationAuthenticationStateProvider>();
var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseHeaderPropagation();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.MapControllers();
app.MapFallbackToPage("/_Host");
app.Services.CreateScope().ServiceProvider.GetRequiredService<ApplicationIdentityDbContext>().Database.Migrate();
app.Services.CreateScope().ServiceProvider.GetRequiredService<ApplicationIdentityDbContext>().SeedTenantsAdmin().Wait();
app.Run();