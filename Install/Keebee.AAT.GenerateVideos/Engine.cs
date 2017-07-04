using Keebee.AAT.VideoConversion;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;


namespace Keebee.AAT.GenerateVideos
{
    public class Engine
    {
        private const string PathRoot = @"C:\Media\SharedLibrary\videos";
        private const string PathConvertedRoot = @"C:\Media\SharedLibrary\videos\converted";
        private const string AccesptedCodec = "h264";

        public void GenerateVideos()
        {
            try
            {
#if DEBUG
                Console.WriteLine("----------------------");
                Console.WriteLine("Video Codec Converter");
                Console.WriteLine("----------------------");
                Console.WriteLine();
#endif
                IEnumerable<string> paths = new Collection<string> {"ambient", "cats", "tv-shows"};

                foreach (var path in paths)
                {
                    var pathFull = Path.Combine($@"{PathRoot}", path);
                    var files = Directory.GetFiles(pathFull);

                    foreach (var file in files)
                    {
                        try
                        {
                            if (Path.GetExtension(file)?.ToLower() != ".mp4") continue;
                            var filename = Path.GetFileName(file);
                            if (filename == null) continue;
#if DEBUG
                            Console.WriteLine($@"File: {path}\{filename}");
#endif
                            // generate
                            string errorMessage;

                            var codecName = VideoConverter.GetCodecName(Path.Combine(pathFull, filename), out errorMessage);
                            if (errorMessage != null) throw new Exception(errorMessage);

#if DEBUG
                            Console.Write($"Codec: {codecName}");
#endif
                            if (codecName == AccesptedCodec)
                            {
#if DEBUG
                                Console.WriteLine(" ---> GOOD");
#endif
                            }
                            else
                            {
#if DEBUG
                                Console.WriteLine(" ---> UNACCEPTABLE");
#endif
                                var pathConverted = Path.Combine(PathConvertedRoot, path);

                                if (File.Exists(Path.Combine(pathConverted, filename)))
                                {
#if DEBUG
                                    Console.WriteLine($"Already converted: {Path.Combine(pathConverted, filename)}");
                                    codecName = VideoConverter.GetCodecName(Path.Combine(pathConverted, filename), out errorMessage);

                                    Console.Write($"Converted codec: {codecName}");
                                    Console.WriteLine(codecName == AccesptedCodec
                                        ? " ---> GOOD"
                                        : " ---> UNACCEPTABLE (NEEDS INVESTIGATING)");
#endif
                                }
                                else
                                {
                                    // convert
#if DEBUG
                                    Console.Write("Converting...");
#endif
                                    var byteArray = VideoConverter.Convert(Path.Combine(pathFull, filename),
                                        out errorMessage);
                                    if (errorMessage != null) throw new Exception(errorMessage);

#if DEBUG
                                    Console.WriteLine("done.");
#endif
                                    // save
                                    errorMessage = VideoConverter.Save(pathConverted, filename, byteArray);
                                    if (errorMessage != null) throw new Exception(errorMessage);
#if DEBUG
                                    codecName = VideoConverter.GetCodecName(Path.Combine(pathConverted, filename), out errorMessage);
                                    Console.Write($"Converted codec: {codecName}");

                                    Console.WriteLine(codecName == AccesptedCodec
                                        ? " ---> GOOD"
                                        : " ---> UNACCEPTABLE (NEEDS INVESTIGATING)");
#endif
                                }
                            }
#if DEBUG
                            Console.WriteLine();
#endif
                        }
                        catch (Exception ex)
                        {
#if DEBUG
                            Console.WriteLine("error.");
                            Console.WriteLine($"Description: {ex.Message}");
                            Console.WriteLine();
#endif
                        }
                    }
                }
#if DEBUG
                Console.WriteLine("Completed.");
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
#endif
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine($"Error: {ex.Message}");
                Console.ReadKey();
#endif
            }
        }
    }
}
