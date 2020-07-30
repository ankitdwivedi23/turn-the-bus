using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AzureTTS
{
    class Program
    {
        private static Dictionary<string, List<string>> chapterAudioFiles = new Dictionary<string, List<string>>();

        public static async Task SynthesisToAudioFileAsync(string ssmlFile, string outputAudioFile, string subscriptionKey, string region)
        {
            Console.WriteLine($"Processing [{Path.GetFullPath(ssmlFile)}]");

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

        static void ConcatAudio(List<string> audioFiles, string outputAudioFile)
        {
            List<AudioFileReader> audioFileReaders = new List<AudioFileReader>();
            audioFiles.ForEach(f => audioFileReaders.Add(new AudioFileReader(f)));
            var mixer = new ConcatenatingSampleProvider(audioFileReaders);
            var outputDirectory = Path.GetDirectoryName(outputAudioFile);
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }
            WaveFileWriter.CreateWaveFile16(outputAudioFile, mixer);
        }

        static void Main(string[] args)
        {
            /*string subscriptionKey = args[0];
            string region = args[1];

            foreach (var ssmlFile in Directory.GetFiles("../../../Digant/", "*.xml"))
            {
                string filename = Path.GetFileNameWithoutExtension(ssmlFile);
                string[] filenameParts = filename.Split('_');
                string chapterKey = string.Concat(filenameParts[0], "_", filenameParts[1]);
                if (!chapterAudioFiles.ContainsKey(chapterKey))
                {
                    chapterAudioFiles[chapterKey] = new List<string>();
                }

                string outputAudioFile = Path.Combine("../../../Audio/", string.Concat(filename, ".wav"));
                chapterAudioFiles[chapterKey].Add(outputAudioFile);
                SynthesisToAudioFileAsync(ssmlFile, outputAudioFile, subscriptionKey, region).Wait();
            }

            foreach (var chapterKey in chapterAudioFiles.Keys)
            {
                Console.WriteLine($"Concatenating audio files for {chapterKey}");
                ConcatAudio(chapterAudioFiles[chapterKey], Path.Combine("../../../Audio/Final", string.Concat(chapterKey, ".wav")));
            }*/

            string batchFileContent = string.Empty;
            string cmdTemplate = "ffmpeg -loop 1 -i ./Digant/Digant_Cover.jpg -i ./Audio/Final/{0}.wav -c:v libx264 -tune stillimage -c:a aac -b:a 192k -pix_fmt yuv420p -vf \"pad = ceil(iw / 2) * 2:ceil(ih / 2) * 2\" -shortest {0}.mp4";
            foreach (var file in Directory.GetFiles("../../../Audio/Final", "*.wav"))
            {
                string filename = Path.GetFileNameWithoutExtension(file);
                batchFileContent = string.Concat(batchFileContent, string.Format(cmdTemplate, filename), Environment.NewLine);
            }

            File.WriteAllText("../../../wav2Mp4.sh", batchFileContent);
        }
    }
}
