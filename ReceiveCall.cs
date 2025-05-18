// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}

using System;
using Azure.Messaging;
using Azure.Messaging.EventGrid;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Azure.Communication.CallAutomation;
using Azure.Messaging.EventGrid.SystemEvents;


namespace Contoso.Functions;

public class ReceiveCall
{
    private readonly ILogger<ReceiveCall> _logger;

    public ReceiveCall(ILogger<ReceiveCall> logger)
    {
        _logger = logger;
    }

    [Function(nameof(ReceiveCall))]
    public async Task Run([EventGridTrigger] EventGridEvent callEvent)
    {
        Console.WriteLine($"Event Grid Function hit at {DateTime.UtcNow}");


        var connectionString = Environment.GetEnvironmentVariable("ACSConnectionString");
        var callAutomationClient = new CallAutomationClient(connectionString);


        var binaryEvent = BinaryData.FromString(callEvent.ToString());
        var incomingCallEvent = CallAutomationEventParser.Parse(binaryEvent);

        if (incomingCallEvent != null)
        {
            Console.WriteLine("☎️ Accepting incoming call...");

            var acceptCallResult = await callAutomationClient.AcceptCallAsync(incomingCallEvent.IncomingCallContext);
            var acceptCallResult = await callAutomationClient.AnswerCallAsync(incomingCallEvent.IncomingCallContext);

            var answerCallOptions = new AnswerCallOptions("<Incoming call context once call is connected>", new Uri("<https://sample-callback-uri>"))
            {
                CallIntelligenceOptions = new CallIntelligenceOptions() { CognitiveServicesEndpoint = new Uri("<Azure Cognitive Services Endpoint>") }
            };

            var answerCallResult = await callAutomationClient.AnswerCallAsync(answerCallOptions);
            Console.WriteLine("✅ Call accepted.");
        }

    }
}