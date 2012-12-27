Client.cs
	using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Tedd.Demo.HttpServer.Server
{
	class Client
	{
		private readonly Socket _socket;
		private readonly NetworkStream _networkStream;
		private readonly MemoryStream _memoryStream = new MemoryStream();
		private readonly StreamReader _streamReader;
		private readonly string _serverName = "Tedd.Demo.HttpServer";
		
		public Client(Socket socket)
		{
			_socket = socket;
			_networkStream = new NetworkStream(socket, true);
			_streamReader = new StreamReader(_memoryStream);
		}
		
		public async void Do()
		{
			// We are executed on a separate thread from listener, but will release this back to the threadpool as often as we can.
			byte[] buffer = new byte[4096];
			for (; ; )
			{
				// Read a chunk of data
				int bytesRead = await _networkStream.ReadAsync(buffer, 0, buffer.Length);
				
				// If Read returns with no data then the connection is closed.
				if (bytesRead == 0)
					return;
				
				// Write to buffer and process
				_memoryStream.Seek(0, SeekOrigin.End);
				_memoryStream.Write(buffer, 0, bytesRead);
				bool done = ProcessHeader();
				if (done)
					break;
			}
			
		}
		
		private bool ProcessHeader()
		{
			// Our task is to find when full HTTP header has been received, then send reply.
			for (; ; )
			{
				_memoryStream.Seek(0, SeekOrigin.Begin);
				var line = _streamReader.ReadLine();
				if (line == null)
					break;
				
				if (line.ToUpperInvariant().StartsWith("GET "))
				{
					// We got a request: GET /file HTTP/1.1
					var file = line.Split(' ')[1].TrimStart('/');
					// Default document is index.html
					if (string.IsNullOrWhiteSpace(file))
						file = "index.html";
					// Send header+file
					SendFile(file);
					// Close connection (we don't support keep-alive)
					_socket.Close();
					return true;
				}
				
			}
			return false;
		}
		
		private void SendFile(string file)
		{
			// Get info and assemble header
			byte[] data;
			string responseCode = "";
			string contentType = "";
			try
			{
				if (File.Exists(file))
				{
					// Read file
					data = File.ReadAllBytes(file);
					contentType = GetContentType(Path.GetExtension(file).TrimStart(".".ToCharArray()));
					responseCode = "200 OK";
				}
				else
				{
					data = System.Text.Encoding.ASCII.GetBytes("<html><body><h1>404 File Not Found</h1></body></html>");
					contentType = GetContentType("html");
					responseCode = "404 Not found";
				}
			}
			catch (Exception exception)
			{
				// In case of error dump exception to client.
				data = System.Text.Encoding.ASCII.GetBytes("<html><body><h1>500 Internal server error</h1><pre>" + exception.ToString() + "</pre></body></html>");
				responseCode = "500 Internal server error";
			}
			
			string header = string.Format("HTTP/1.1 {0}\r\n"
			                              + "Server: {1}\r\n"
			                              + "Content-Length: {2}\r\n"
			                              + "Content-Type: {3}\r\n"
			                              + "Keep-Alive: Close\r\n"
			                              + "\r\n",
			                              responseCode, _serverName, data.Length, contentType);
			// Send header & data
			_socket.Send(System.Text.Encoding.ASCII.GetBytes(header));
			_socket.Send(data);
			
		}
		
		/// <summary>
		/// Get mime type from a file extension
		/// </summary>
		/// <param name="extension">File extension without starting dot (html, not .html)</param>
		/// <returns>Mime type or default mime type "application/octet-stream" if not found.</returns>
		private string GetContentType(string extension)
		{
			// We are accessing the registry with data received from third party, so we need to have a strict security test. We only allow letters and numbers.
			if (Regex.IsMatch(extension, "^[a-z0-9]+$", RegexOptions.IgnoreCase | RegexOptions.Compiled))
				return (Registry.GetValue(@"HKEY_CLASSES_ROOT\." + extension, "Content Type", null) as string) ?? "application/octet-stream";
			return "application/octet-stream";
			
		}
	}
}