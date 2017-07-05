using Keebee.AAT.VideoConversion;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Keebee.AAT.ConvertVideos
{
    public class Engine
    {
        private const string AccesptedCodec = "h264";

        public void GenerateVideos(string PathRoot)
        {
            try
            {
#if DEBUG
                Console.WriteLine("----------------------");
                Console.WriteLine("Video Codec Converter");
                Console.WriteLine("----------------------");
                Console.WriteLine();
#endif
                var pathConvertedRoot = Path.Combine(PathRoot, "converted");

                // Data structure to hold names of subfolders to be examined for files
                var dirs = new Stack<string>(20);

                dirs.Push(PathRoot);

                while (dirs.Count > 0)
                {
                    var currentDir = dirs.Pop();
                    var surrentSubDir = currentDir.Replace($@"{PathRoot}\", string.Empty);
                    var subDirs = Directory.GetDirectories(currentDir);
                    var files = Directory.GetFiles(currentDir);

                    foreach (var file in files)
                    {
                        try
                        {
                            if (Path.GetExtension(file)?.ToLower() != ".mp4") continue;
                            var filename = Path.GetFileName(file);
                            if (filename == null) continue;
#if DEBUG
                            Console.WriteLine($@"File: {currentDir}\{filename}");
#endif
                            // generate
                            string errorMessage;

                            var codecName = VideoConverter.GetCodecName(Path.Combine(currentDir, filename), out errorMessage);
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
                            else // possibly needs converting
                            {
#if DEBUG
                                Console.WriteLine(" ---> UNACCEPTABLE");
#endif
                                var pathConverted = Path.Combine(pathConvertedRoot, surrentSubDir);

                                // see if it was already done
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
                                else // convert
                                {
#if DEBUG
                                    Console.Write("Converting...");
#endif
                                    var byteArray = VideoConverter.Convert(Path.Combine(currentDir, filename),
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
                    // Push the subdirectories onto the stack for traversal.
                    // This could also be done before handing the files.
                    foreach (var dir in subDirs.Select(x => x))
                        dirs.Push(dir);
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
