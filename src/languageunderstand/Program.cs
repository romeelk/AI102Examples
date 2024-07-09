using Azure;
using Azure.AI.Language.Conversations;
using Azure.Core;
using Azure.Core.Serialization;
using Microsoft.Extensions.Configuration;

internal class Program
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine("Language understanding example!");
        Console.WriteLine("Gets the intent of language through a custom language understanding model trained on utterances");

        IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
        IConfigurationRoot configuration = builder.Build();
        string predictionEndpoint = configuration["predictionEndpoint"];
        string predictionKey = configuration["predictionKey"];


        Uri endpoint = new Uri(predictionEndpoint);
        AzureKeyCredential credential = new AzureKeyCredential(predictionKey);

        ConversationAnalysisClient client = new ConversationAnalysisClient(endpoint, credential);

        Console.WriteLine("Authentication to language understanding prediction endpoint");

        string projectName = "Clock";
        string deploymentName = "clock";

        var getDateData = new
        {
            analysisInput = new
            {
                conversationItem = new
                {
                    text = "what's the date on Weds?",
                    id = "1",
                    participantId = "1",
                }
            },
            parameters = new
            {
                projectName,
                deploymentName,

                // Use Utf16CodeUnit for strings in .NET.
                stringIndexType = "Utf16CodeUnit",
            },
            kind = "Conversation",
        };

        Response response = await client.AnalyzeConversationAsync(RequestContent.Create(getDateData));
        dynamic conversationalTaskResult = response.Content.ToDynamicFromJson(JsonPropertyNames.CamelCase);
        dynamic conversationPrediction = conversationalTaskResult.Result.Prediction;

        Console.WriteLine($"Top intent: {conversationPrediction.TopIntent}");
        foreach(var entity in conversationPrediction.Entities)
        {
            Console.WriteLine($"Entity {entity}");
        }
    }
}