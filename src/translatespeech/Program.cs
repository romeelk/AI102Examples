using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.Translation;
using Microsoft.Extensions.Configuration;

IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
IConfigurationRoot configuration = builder.Build();
string aiSvcKey = configuration["SpeechKey"];
string aiSvcRegion = configuration["SpeechRegion"];

try
{
    // congiure Azure AI speech
    // Configure speech service
    var speechTranslationConfig = SpeechTranslationConfig.FromSubscription(aiSvcKey, aiSvcRegion);
    var speechConfig = SpeechConfig.FromSubscription(aiSvcKey, aiSvcRegion);

    speechTranslationConfig.SpeechRecognitionLanguage = "en-US";
    speechTranslationConfig.AddTargetLanguage("fr");
    var translation = await TranslateFromSpeechFromMicrophone(speechTranslationConfig);
    await SynthesizeTranslation(speechConfig, translation );
    await TranslateFromFile(speechTranslationConfig);
}
catch (Exception e)
{
    Console.WriteLine($"Woops an error occured: {e.ToString()}");
}

async Task SynthesizeTranslation(SpeechConfig speechConfig, string ? translation)
{
    var languages = new Dictionary<string, string>()
    {
        ["fr"] = "fr-FR-HenriNeural",
        ["es"] = "es-ES-ElviraNeural",
        ["hi"] = "hi-IN-MadhurNeural"
    };
    speechConfig.SpeechSynthesisVoiceName = languages["fr"];

    using SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer(speechConfig);

    var speak = await speechSynthesizer.SpeakTextAsync(translation);
}

async Task TranslateFromFile(SpeechTranslationConfig?  speechTranslationConfig)
{
    var audioConfig = AudioConfig.FromWavFileInput("station.wav");

    var speechRecognizer = new TranslationRecognizer(speechTranslationConfig, audioConfig);

    var result = await speechRecognizer.RecognizeOnceAsync();

    Console.WriteLine($"Translating input:{result.Text}");
    var translation = result.Translations["fr"];
    Console.WriteLine($"French translation from station.wav:{translation}");
}
static async Task<string> TranslateFromSpeechFromMicrophone(SpeechTranslationConfig speechTranslationConfig)
{
    string translation = "";
    Console.WriteLine("Ready to use speech service for speech(audio) translation " + speechTranslationConfig.Region);

    Console.WriteLine("Speak into microphone");
    var audioConfig = AudioConfig.FromDefaultMicrophoneInput();

    var speechRecognizer = new TranslationRecognizer(speechTranslationConfig, audioConfig);

    var result = await speechRecognizer.RecognizeOnceAsync();

    Console.WriteLine($"Translating input:{result.Text}");
    translation = result.Translations["fr"];
    Console.WriteLine($"French translation:{translation}");

    return translation;
}
