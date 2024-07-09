// Import namespaces
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Extensions.Configuration;
// See https://aka.ms/new-console-template for more information
Console.WriteLine("Cognitive speech examples!");

IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
IConfigurationRoot configuration = builder.Build();
string aiSvcKey = configuration["SpeechKey"];
string aiSvcRegion = configuration["SpeechRegion"];

try
{
    // congiure Azure AI speech
    // Configure speech service
    var speechConfig = SpeechConfig.FromSubscription(aiSvcKey, aiSvcRegion);
    Console.WriteLine("Ready to use speech service in " + speechConfig.Region);
    var speechResult = await RecognizeSpeech(speechConfig);

    if(speechResult.Reason == ResultReason.RecognizedSpeech)
    {
        Console.WriteLine("Speech recognized");
        Console.WriteLine($"Text={speechResult.Text}");
    }
    else
    {
        Console.WriteLine("Failed to recognize speech!!!");
    }

}
catch (Exception e)
{
    Console.WriteLine("Woops an error occured")
}

async Task<SpeechRecognitionResult> RecognizeSpeech(SpeechConfig speechConfig)
{
    // Configure speech recognition
    using AudioConfig audioConfig = AudioConfig.FromDefaultMicrophoneInput();
    using SpeechRecognizer speechRecognizer = new SpeechRecognizer(speechConfig, audioConfig);
    Console.WriteLine("Speak now...");
    var speechRecognitionResult = await speechRecognizer.RecognizeOnceAsync();

    return speechRecognitionResult;
}