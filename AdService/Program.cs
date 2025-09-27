using System.Text;

var builder = WebApplication.CreateBuilder(args);

// singleton репозиторий в памяти
builder.Services.AddSingleton<AdPlatformRepository>();

var app = builder.Build();

// POST /upload
// Поддерживает: raw text в теле (text/plain) или multipart/form-data с полем file
app.MapPost("/upload", async (HttpRequest request, AdPlatformRepository repo) =>
{
    string content;
    if (request.HasFormContentType && request.Form.Files.Count > 0)
    {
        var file = request.Form.Files[0];
        using var sr = new StreamReader(file.OpenReadStream(), Encoding.UTF8);
        content = await sr.ReadToEndAsync();
    }
    else
    {
        using var sr = new StreamReader(request.Body, Encoding.UTF8);
        content = await sr.ReadToEndAsync();
    }

    try
    {
        repo.LoadFromText(content);
        return Results.Ok(new { message = "Data loaded" });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

// GET /search?location=/ru/svrd/revda
app.MapGet("/search", (string location, AdPlatformRepository repo) =>
{
    if (string.IsNullOrWhiteSpace(location))
        return Results.BadRequest(new { error = "location is required, e.g. /ru/svrd" });

    var platforms = repo.FindPlatforms(location.Trim());
    return Results.Ok(platforms);
});

app.Run();