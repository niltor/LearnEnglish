using Microsoft.AspNetCore.Mvc;

namespace WebAPI;

public class API
{
    public static async Task<IResult> ChatAsync([FromServices] AIService service, IFormFile file)
    {
        // read file as stream 
        using var stream = file.OpenReadStream();
        var text = await service.SpeechToTextAsync(stream);
        if (text != null)
        {
            var result = await service.ChatResponseAsync(text);
            if (result != null)
            {
                return Results.Ok(result);
            }
        }
        return Results.Ok();
    }
}
