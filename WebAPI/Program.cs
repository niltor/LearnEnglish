
using System.Diagnostics;
using Application.Services;
using WebAPI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient<OpenAIClient>();
builder.Services.AddScoped<AIService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapPost("/test", async (AIService service, IFormFile file)
    =>
{
    Stopwatch sw = new Stopwatch();
    sw.Start();
    using var stream = file.OpenReadStream();
    var text = await service.SpeechToTextAsync(stream);
    await Console.Out.WriteLineAsync($"speech:{sw.ElapsedMilliseconds} ms");

    if (text != null)
    {
        var result = await service.ChatResponseAsync(text);
        if (result != null)
        {

            sw.Stop();
            Console.WriteLine($"Time elapsed: {sw.ElapsedMilliseconds} ms");
            return Results.Ok(result);


        }
    }

    return Results.Ok();

});

app.Run();
