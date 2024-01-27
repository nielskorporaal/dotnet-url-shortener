using Microsoft.EntityFrameworkCore;
using UrlShortener.Database;
using UrlShortener.Settings;

namespace UrlShortener.Services;

public class UrlShortenerService
{
    public UrlShortenerService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    private readonly Random _random = new();
    private readonly ApplicationDbContext _dbContext;

    public async Task<string> GenerateUniqueCode()
    {
        var codeChars = new char[ShortLinkSettings.Length];
        var maxValue = ShortLinkSettings.Alphabet.Length;

        while (true)
        {
            for (var i = 0; i < ShortLinkSettings.Length; i++)
            {
                var randomIndex = _random.Next(maxValue);

                codeChars[i] = ShortLinkSettings.Alphabet[randomIndex];
            }

            var code = new string(codeChars);

            if (!await _dbContext.ShortenedUrls.AnyAsync(s => s.Code == code))
            {
                return code;
            }
        }
    }
}