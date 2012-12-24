using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using Mono.Zeroconf;
using System.Threading.Tasks;

namespace AirPlay {
class AirPic {
	
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
		Console.WriteLine ("Usage: airpic delai repertoire");
		Console.WriteLine ("  delai        Nombre, en secondes, entre les images");
		Console.WriteLine ("  repertoire   Emplacement des images (extension .jpg)");
		Environment.Exit (1);
	}
	
	public static void Main (string [] args)
	{
		if (args.Length < 2)
			Help ();

		var player = new AirPlayer();
		var features = player.AvailableSlideshowFeatures();
		foreach (var f in features.Result)
		{
			Console.WriteLine (f);
		}

		int delay = -1;
		if (!Int32.TryParse (args [0], out delay))
			Help ("ERREUR: Delai invalide.");
		
		if (!Directory.Exists (args [1]))
			Help ("ERREUR: Repertoire inexistant.");
		
		while (true) {
//			while (true) {
//				int c = files.Count;
//				if (c == 0)
//					c = FindJpeg (args [1]);
//				
//				int n = rnd.Next (0, c);
//				file = files [n];
//				files.Remove (file);
//				// file could have been [re]moved
//				if (File.Exists (file))
//					break;
//			}

			var files = FindJpeg (args [1]);
			
			Console.WriteLine ("Sending {0} files to TV", files.Count());
			Task.WaitAll(player.PlayPictures(files, delay));
		}
	}
}
}