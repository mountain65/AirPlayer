using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using Mono.Zeroconf;
using System.Threading.Tasks;

namespace AirPlay 
{
	class Program 
	{
		static Queue<string> dirs = new Queue<string> ();
		
		static IEnumerable<string> FindJpeg (string dir)
		{
			var files = new List<string> ();
			dirs.Enqueue (dir);
			while (dirs.Count > 0) {
				dir = dirs.Dequeue ();
				foreach (string subdir in Directory.GetDirectories (dir))
					dirs.Enqueue (subdir);
				foreach (string file in Directory.GetFiles (dir, "*.jpg"))
					files.Add (file);
				foreach (string file in Directory.GetFiles (dir, "*.JPG"))
					files.Add (file);
			}
			return files;
		}
		
		static void Help (string message = null)
		{
			if (message != null) {
				Console.WriteLine (message);
				Console.WriteLine ();
			}
			Console.WriteLine ("Usage: monoairplayer delay directory");
			Console.WriteLine ("  delay\t\tin seconds");
			Console.WriteLine ("  directory\tdirectory holding JPEGs");
			Environment.Exit (1);
		}
		
		public static void Main (string [] args)
		{
			if (args.Length < 2)
				Help ();

			int delay = -1;
			if (!Int32.TryParse (args [0], out delay))
				Help ("ERROR: Delay invalid");
			
			if (!Directory.Exists (args [1]))
				Help ("ERROR: Directory does not exist.");
			
			var player = new AirPlayer();

			Console.WriteLine ("Available transitions:");
			var features = player.AvailableSlideshowFeatures().Result;
			foreach (var f in features)
				Console.WriteLine ("\t{0}", f);
				

			var files = FindJpeg (args [1]);
			
			Console.WriteLine ("Sending {0} files to TV", files.Count());
				player.PlayPictures(files, delay).Wait();
		}
	}
}