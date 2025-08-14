using System.Threading.RateLimiting;
using Core;
using Datalayer.Repositories;
using Datalayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using Core.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.MaxAge = TimeSpan.FromMinutes(60);
});
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetSlidingWindowLimiter(
            partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
            factory: partition => new SlidingWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 200,
                QueueLimit = 50,
                Window = TimeSpan.FromMinutes(1),
                SegmentsPerWindow = 4
            }));

    options.OnRejected = (context, cancellationToken) =>
    {
        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
        {
            context.HttpContext.Response.Headers.RetryAfter = retryAfter.TotalSeconds.ToString();
        }

        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.");

        return new ValueTask();
    };
});

builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<TheImageRepository>();

builder.Services.AddScoped<IImageFilterService,ImageFiterService>();
builder.Services.AddScoped<IUserService,UserService>();
builder.Services.AddScoped<ITheImageService,TheImageService>();
builder.Services.AddScoped<IEncryptionAndDecryptionService,EncryptionAndDecryptionService>();

//builder.Services.AddScoped<UnitOfWork>();

//builder.Services.AddScoped<IUserService,UserService>();

/*builder.Services.AddDbContext<ImageGalleryContext>(
       options => options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionString"), b => b.MigrationsAssembly("Image Gallery")));*/

var app = builder.Build();

//app.MapGet("/", () => "Hello World!");
app.UseRateLimiter();
app.UseAntiforgery();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCookiePolicy();
//app.UseAuthentication();
app.UseRouting();
//app.UseAuthorization();
app.UseSession();
app.MapControllers();
app.Run();
