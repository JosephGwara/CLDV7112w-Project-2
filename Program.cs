using CLDV7112w_Project_2.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();

//Table Storage service
builder.Services.AddScoped<ITableStorageService, TableStorageService>();
//BlobStorage Service
builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();
//FileStorage Service
builder.Services.AddScoped<IFileStorageService, FileStorageService>();

var app = builder.Build();

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

app.Run();
