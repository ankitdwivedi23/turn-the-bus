using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using NAudio.Lame;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace AzureTTS
{
    class Program
    {
        public static async Task SynthesisToAudioFileAsync(string ssmlFile, string outputAudioFile, string subscriptionKey, string region)
        {
            // Replace with your own subscription key and region identifier from here: https://aka.ms/speech/sdkregion
            // The default language is "en-us".
            var config = SpeechConfig.FromSubscription(subscriptionKey, region);

            var outputDirectory = Path.GetDirectoryName(outputAudioFile);
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            using (var fileOutput = AudioConfig.FromWavFileOutput(outputAudioFile))
            {
                using (var synthesizer = new SpeechSynthesizer(config, fileOutput))
                {
                    var ssml = File.ReadAllText(ssmlFile);
                    var result = await synthesizer.SpeakSsmlAsync(ssml);

                    if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                    {
                        Console.WriteLine($"Speech synthesized to [{Path.GetFullPath(outputAudioFile)}]");
                    }
                    else if (result.Reason == ResultReason.Canceled)
                    {
                        var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                        Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                        if (cancellation.Reason == CancellationReason.Error)
                        {
                            Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                            Console.WriteLine($"CANCELED: ErrorDetails=[{cancellation.ErrorDetails}]");
                            Console.WriteLine($"CANCELED: Did you update the subscription info?");
                        }
                    }
                }
            }            
        }       

        static void ConcatAudio(string audioFile1, string audioFile2, string outputAudioFile)
        {
            using (var reader1 = new AudioFileReader(audioFile1))
            using (var reader2 = new AudioFileReader(audioFile2))
            {
                var mixer = new ConcatenatingSampleProvider(new[] { reader1, reader2 });
                WaveFileWriter.CreateWaveFile16("mixed.wav", mixer);
            }
        }

        static void Main(string[] args)
        {
            string subscriptionKey = args[0];
            string region = args[1];
            string outputAudioFile = "../../../Audio/hi-IN-Kalpana.wav";
            string ssmlFile = "../../../Text/Digant-Text.xml";

            SynthesisToAudioFileAsync(ssmlFile, outputAudioFile, subscriptionKey, region).Wait();
        }
    }
}
