using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LudoGame;

internal class Application : IApplication
{
    private readonly ILogger<Application> _logger;
    private readonly IConfiguration _config;

    public Application(ILogger<Application> logger, IConfiguration config)
    {
        _logger = logger;
        _config = config;
    }

    public void Run()
    {

    }
}
