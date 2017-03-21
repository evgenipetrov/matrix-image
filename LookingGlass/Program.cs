﻿// This is free and unencumbered software released into the public domain.
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;

namespace LookingGlass
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
                QueryDownloadUrl(@"http://reflector.red-gate.com/Reflector.version");
            else
                Download(args[0]);
        }

        static void QueryDownloadUrl(string versionUrl)
        {
            Console.WriteLine("Phase 1: Getting download URL...");

            HttpWebRequest request = GetHttpWebRequest(versionUrl);
            string downloadUrl = null;
            string data;

            using (WebResponse response = request.GetResponse())
            {
                data = ReadResponse(response);

                foreach (string line in data.Split('\n'))
                {
                    if (line.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                    {
                        // If this version number stops working, search for other known .NET Reflector version numbers; Red Gate's own forums mention a few
                        downloadUrl = line.Trim('\r').Replace("{Client}", "Reflector").Replace("{Version}", "6.1.0.11");
                        break;
                    }
                }
            }

            if (downloadUrl == null)
            {
                Console.WriteLine("Failed to find download URL. Returned data was:");
                Console.WriteLine(data);
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("Launching download process...");
                Process.Start(Process.GetCurrentProcess().MainModule.FileName, downloadUrl);
                Console.WriteLine("Phase 1 complete.");
            }
        }

        static void Download(string downloadUrl)
        {
            Console.WriteLine("Phase 2: Downloading...");

            HttpWebRequest request = GetHttpWebRequest(downloadUrl);
            string path;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                path = GetDownloadFilename(response);

                if (response.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(path))
                {
                    Console.WriteLine("Saving to {0}...", path);
                    SaveResponse(response, path);
                    Console.WriteLine("Done!");
                }
                else
                {
                    Console.WriteLine("Download failed; server returned status code {0}. Returned data was:", response.StatusCode);
                    Console.WriteLine(ReadResponse(response));
                    Console.ReadLine();
                }
            }
        }

        static HttpWebRequest GetHttpWebRequest(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Headers.Add("Cache-Control: no-cache, no-store, max-age=0");
            // Emulate the manner in which the headers are handled
            FieldInfo usesProxySemantics = typeof(ServicePoint).GetField("m_ProxyServicePoint", BindingFlags.NonPublic | BindingFlags.Instance);
            usesProxySemantics.SetValue(request.ServicePoint, true);
            return request;
        }

        static string GetDownloadFilename(HttpWebResponse response)
        {
            string contentDisposition = response.Headers["content-disposition"];

            if (string.IsNullOrEmpty(contentDisposition))
                return null;

            string filenameField = "filename=";
            int index = contentDisposition.IndexOf(filenameField, StringComparison.CurrentCultureIgnoreCase);

            if (index < 0)
                return null;

            return contentDisposition.Substring(index + filenameField.Length).Trim('"');
        }

        static string ReadResponse(WebResponse response)
        {
            using (Stream netStream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(netStream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        static void SaveResponse(WebResponse response, string path)
        {
            byte[] buffer = new byte[4 * 1024];
            int bytesRead;

            using (Stream netStream = response.GetResponseStream())
            {
                if (File.Exists(path))
                    File.Delete(path);

                using (FileStream fileStream = File.Create(path))
                {
                    while ((bytesRead = netStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        Console.Write(".");
                        fileStream.Write(buffer, 0, bytesRead);
                    }
                }
            }

            Console.WriteLine();
        }
    }
}