using Microsoft.EntityFrameworkCore;
using UrlShortener.Database;
using UrlShortener.Models;
using UrlShortener.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("app-settings.json", optional: false, reloadOnChange: true);

var databaseSettings = builder.Configuration.GetSection("Database");
var connectionString = $"Host={databaseSettings["Host"]};Port={databaseSettings["Port"]};Database={databaseSettings["DatabaseName"]};Username={databaseSettings["Username"]};Password={databaseSettings["Password"]}";

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<UrlShortenerService>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("shorten", async (
    ShortenUrlRequest urlRequest,
    UrlShortenerService urlShortenerService,
    ApplicationDbContext dbContext,
    HttpContext httpContext) =>
{
    if (!Uri.TryCreate(urlRequest.Url, UriKind.Absolute, out _))
    {
        return Results.BadRequest("The specified URL is invalid.");
    }

    var code = await urlShortenerService.GenerateUniqueCode();

    var request = httpContext.Request;

    var shortenedUrl = new ShortenedUrl()
    {
        Id = Guid.NewGuid(),
        LongUrl = urlRequest.Url,
        Code = code,
        ShortUrl = $"{request.Scheme}://{request.Host}/{code}",
        CreatedOnUtc = DateTime.UtcNow
    };

    dbContext.ShortenedUrls.Add(shortenedUrl);
    await dbContext.SaveChangesAsync();

    return Results.Ok(shortenedUrl.ShortUrl);
});

app.MapGet("{code}", async (
    string code,
    ApplicationDbContext dbContext
) =>
{
    var shortenedUrl = await dbContext.ShortenedUrls.SingleOrDefaultAsync(s => s.Code == code);

    if (shortenedUrl is null) return Results.NotFound();

    return Results.Redirect(shortenedUrl.LongUrl);
});

app.Run();