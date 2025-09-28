using Gradution_Project_G5.BLL.Interfaces;
using Gradution_Project_G5.BLL.Services;
using Gradution_Project_G5.BLL.Validation;
using Gradution_Project_G5.DAL.Models.Data;
using Gradution_Project_G5.DAL.Repositories;
using Gradution_Project_G5.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Repositories
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICourseServices, CourseServices>();
builder.Services.AddScoped<IGradeServices, GradeServices>();
builder.Services.AddScoped<IInstructorServices, InstructorService>();
builder.Services.AddScoped<ISessionServices, SessionServices>();
builder.Services.AddScoped<IUserServices, UserServices>();

// Register Validation Services
builder.Services.AddScoped<DateValidation>();
// Add other services...
builder.Services.AddControllersWithViews()
    .AddViewOptions(options =>
    {
        options.HtmlHelperOptions.ClientValidationEnabled = true;
    });

var app = builder.Build();

// Configure pipeline...
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
}
app.Run();