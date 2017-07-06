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

        public void GenerateVideos(string pathRoot)
        {
            try
            {
                const string logFilename = "Conversion.log";
                var pathConvertedRoot = Path.Combine(pathRoot, "converted");
                var pathLogFile = Path.Combine(pathConvertedRoot, logFilename);
                var text = "Video Codec Converter";   // for the accumulated messages

#if DEBUG
                Console.WriteLine("----------------------");
                Console.WriteLine(text);
                Console.WriteLine("----------------------");
                Console.WriteLine();
#elif !DEBUG
                Console.Write("Converting...");
#endif
                InitializeLogFile(pathConvertedRoot, logFilename);

                using (var w = File.AppendText(pathLogFile))
                {
                    w.WriteLine("-----------------------");
                    w.WriteLine(text);
                    w.WriteLine("-----------------------");
                    w.WriteLine();

                    // Data structure to hold names of subfolders to be examined for files
                    var dirs = new Stack<string>(20);

                    dirs.Push(pathRoot);

                    while (dirs.Count > 0)
                    {
                        var currentDir = dirs.Pop();
                        var surrentSubDir = currentDir.Replace($@"{pathRoot}\", string.Empty);
                        var subDirs = Directory.GetDirectories(currentDir);
                        var files = Directory.GetFiles(currentDir);

                        foreach (var file in files)
                        {
                            try
                            {
                                if (Path.GetExtension(file)?.ToLower() != ".mp4") continue;
                                var filename = Path.GetFileName(file);
                                if (filename == null) continue;

                                text = $@"File: {currentDir}\{filename}";
#if DEBUG
                                Console.WriteLine(text);
#endif
                                w.WriteLine(text);

                                // generate
                                string errorMessage;

                                var codecName = VideoConverter.GetCodecName(Path.Combine(currentDir, filename),
                                    out errorMessage);
                                if (errorMessage != null) throw new Exception(errorMessage);

                                text = $"Codec: {codecName}";
#if DEBUG
                                Console.Write(text);
#endif
                                w.Write(text);
                                if (codecName == AccesptedCodec)
                                {
                                    text = " ---> GOOD";
#if DEBUG
                                    Console.WriteLine(text);
#endif
                                    w.WriteLine(text);
                                }
                                else // possibly needs converting
                                {
                                    text = " ---> UNACCEPTABLE";
#if DEBUG
                                    Console.WriteLine(text);
#endif
                                    w.WriteLine(text);
                                    var pathConverted = Path.Combine(pathConvertedRoot, surrentSubDir);

                                    // see if it was already done
                                    if (File.Exists(Path.Combine(pathConverted, filename)))
                                    {
                                        text = $"Already converted: {Path.Combine(pathConverted, filename)}";
#if DEBUG
                                        Console.WriteLine(text);
#endif
                                        w.WriteLine(text);

                                        codecName = VideoConverter.GetCodecName(Path.Combine(pathConverted, filename),
                                            out errorMessage);

                                        text = $"Converted codec: {codecName}";
#if DEBUG
                                        Console.Write(text);
#endif
                                        w.Write(text);
                                        text = codecName == AccesptedCodec
                                            ? " ---> GOOD"
                                            : " ---> UNACCEPTABLE (NEEDS INVESTIGATING)";
#if DEBUG
                                        Console.WriteLine(text);
#endif
                                        w.WriteLine(text);
                                    }
                                    else // convert
                                    {
                                        text = "Converting...";
#if DEBUG
                                        Console.Write(text);
#endif
                                        w.Write(text);
                                        var byteArray = VideoConverter.Convert(Path.Combine(currentDir, filename),
                                            out errorMessage);
                                        if (errorMessage != null) throw new Exception(errorMessage);

                                        text = "done.";
#if DEBUG
                                        Console.WriteLine(text);
#endif
                                        w.WriteLine(text);

                                        // save
                                        errorMessage = VideoConverter.Save(pathConverted, filename, byteArray);
                                        if (errorMessage != null) throw new Exception(errorMessage);

                                        codecName = VideoConverter.GetCodecName(Path.Combine(pathConverted, filename), out errorMessage);
                                        if (errorMessage != null) throw new Exception(errorMessage);
                                        text = $"Converted codec: {codecName}";
#if DEBUG

                                        Console.Write(text);
#endif
                                        w.Write(text);
                                        text = codecName == AccesptedCodec
                                            ? " ---> GOOD"
                                            : " ---> UNACCEPTABLE (NEEDS INVESTIGATING)";
#if DEBUG
                                        Console.WriteLine(text);
#endif
                                        w.WriteLine(text);
                                    }
                                }
#if DEBUG
                                Console.WriteLine();
#endif
                                w.WriteLine();
                            }
                            catch (Exception ex)
                            {
#if DEBUG
                                Console.WriteLine("error.");
                                Console.WriteLine($"Description: {ex.Message}");
                                Console.WriteLine();
#endif
                                w.WriteLine();
                                w.WriteLine($"Error: {ex.Message}");
                            }
                        }
                        // Push the subdirectories onto the stack for traversal.
                        // This could also be done before handing the files.
                        foreach (var dir in subDirs.Select(x => x))
                            dirs.Push(dir);
                    }
                    text = "Completed successfully.";

                    Console.WriteLine(text);
                    Console.WriteLine();
                    Console.WriteLine("Press any key to exit");
                    Console.ReadKey();

                    w.WriteLine(text);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine($"Error: {ex.Message}");
                Console.ReadKey();
#endif
            }
        }

        private static void InitializeLogFile(string path, string filename)
        {
            var directory = new DirectoryInfo(path);
            if (!directory.Exists)
                directory.Create();

            var filePath =Path.Combine(path, filename);
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }
}
