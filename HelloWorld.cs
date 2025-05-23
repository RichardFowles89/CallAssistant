using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

public class HelloWorldFunction
{
    private readonly ILogger _logger;

    public HelloWorldFunction(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<HelloWorldFunction>();
    }

    [Function("HelloWorldFunction")]
    public HttpResponseData Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
    {
        _logger.LogInformation("HelloWorldFunction triggered");
        Console.WriteLine("Hello, World!");
        var response = req.CreateResponse(HttpStatusCode.OK);
        // response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
        // response.WriteString("Hello, world!");

        return response;
    }
}
