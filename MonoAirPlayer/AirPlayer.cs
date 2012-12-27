using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Mono.Zeroconf;
using System.IO;
using System.Collections.Generic;

namespace AirPlay
{
	public class AirPlayer
	{
		private AppleTv tv = null;
		async public Task<AppleTv> TV()  
		{ 
			if (tv == null)
				tv = await FindAppleTv();

			return tv;
		}

		public Task<AppleTv> FindAppleTv()
		{
			var tcs = new TaskCompletionSource<AppleTv>(); 

			var domainBrowser = new Mono.Zeroconf.ServiceBrowser();
			domainBrowser.ServiceAdded += (o, args) => 
			{ 
				args.Service.Resolved +=  (orsv,  arsv) => {

					if(arsv == null) tcs.SetException(new ArgumentNullException("Service not found"));

					var s = (IResolvableService)args.Service;
					var tv = new AppleTv 
					{ 
						Model = s.TxtRecord["model"].ValueString, 
						Version = s.TxtRecord["srcvers"].ValueString, 
						Features = int.Parse(s.TxtRecord["features"].ValueString.Substring(2), System.Globalization.NumberStyles.HexNumber),
						DeviceID = s.TxtRecord["deviceid"].ValueString, 
						IPAddress = s.HostEntry.AddressList[0].ToString ()
					};

					tcs.SetResult(tv);
				};

				args.Service.Resolve ();		
			};

			domainBrowser.Browse("_airplay._tcp", "local");
			return tcs.Task;
		}

		/// <summary>
		/// Sends one picture to the device
		/// </summary>
		/// <returns>The picture.</returns>
		/// <param name="fileName">File name.</param>
		public async Task PlayPicture(string fileName)
		{
			using (var wc = await CreateClient()) {
				wc.Headers.Add ("X-Apple-AssetKey", "F92F9B91-954E-4D63-BB9A-EEC771ADE6E8");
				wc.Headers.Add ("User-Agent", "AirPlay/160.4 (Photos)");
				wc.Headers.Add ("X-Apple-Transition: Dissolve");
				var data = File.ReadAllBytes (fileName);

				await wc.UploadDataTaskAsync("/photo", "PUT", data);
			}
		}

		/// <summary>
		/// Sends a list of pictures to the device with a given delay in seconds
		/// </summary>
		/// <returns>The pictures.</returns>
		/// <param name="fileNames">list of path and names to JPEG files</param>
		/// <param name="delay">Delay in seconds</param>
		public async Task PlayPictures(IEnumerable<string> fileNames, int delay = 1)
		{
			foreach (var fileName in fileNames) 
			{
				await PlayPicture(fileName);
				await Task.Delay(delay * 1000);
			}
		}

		public async Task<IEnumerable<string>> AvailableSlideshowFeatures()
		{
			using (var wc = await CreateClient()) {
				wc.Headers.Add ("User-Agent", "MediaControl/1.0");

				var xml = await wc.DownloadStringTaskAsync("/slideshow-features");

				// We get a dictionary with one item called "themes"
				var pl = (Dictionary<string,object>)PList.readPlistSource(xml);

				// That item holds a list of objects, which are actually dictionaries
//				var themes = pl["themes"] as List<Dictionary<string,object>>;
				var themes = pl["themes"] as List<object>;

				// Each dictionary has 2 entries: one with key "key" and one with key "name"
				return themes.Select(t => (t as Dictionary<string,object>)["name"].ToString()).ToList();
			}
		}

		public async Task StartSlideshow(string transitions)
		{
			using (var wc = await CreateClient()) {
				wc.Headers.Add ("User-Agent", "MediaControl/1.0");
				wc.Headers.Add("Content-Type", "text/x-apple-plist+xml");
				wc.Headers.Add("Content-Length", "0");

				var plistData = new Dictionary<string,object>
				{
					{"settings",
						new Dictionary<string,object>
						{
							{ "slideDuration", 1 },
							{ "theme", transitions },
						}
					},
					{ "state", "playing"}
				};

				var pl = PList.writeXml(plistData);
				var result = await wc.UploadStringTaskAsync("/slideshows/1", "PUT", pl);
				Console.WriteLine ("TV returned: {0}", result);
			}
		}


		private async Task<WebClient> CreateClient()
		{
			var wc = new WebClient();
			wc.Headers.Add ("Accept-Language", "English");
			wc.Headers.Add ("X-Apple-Session-ID", new Guid().ToString());
			var url = string.Format("http://{0}:7000",  (await this.TV()).IPAddress);
			wc.BaseAddress = url;

			return wc;
		}
	}

	public class AppleTv
	{
		public string DeviceID {
			get;
			set;
		}

		public string Model {
			get;
			set;
		}

		public string IPAddress {
			get;
			set;
		}

		public int Features {
			get;
			set;
		}

		public string Version {
			get;
			set;
		}

		public bool SupportsVideo  { 	get { return (Features & 1) == 1;} }
		public bool SupportsPhoto  { 	get { return (Features & 2) == 2;} }
		public bool SupportsVideoFairPlay  { 	get { return (Features & 4) == 4;} }
		public bool SupportsVideoVolumeControl  { 	get { return (Features & 8) == 8;} }
		public bool SupportsVideoHTTPLiveStreams  { 	get { return (Features & 16) == 16;} }
		public bool SupportsSlideshow  { 	get { return (Features & 32) == 32;} }
		public bool SupportsScreenMirroring  { 	get { return (Features & 64) == 64;} }
		public bool SupportdScreenRotation  { 	get { return (Features & 128) == 128;} }
		public bool SupportsAudio  { 	get { return (Features & 256) == 256;} }
		public bool SupportsAudioRedundant  { 	get { return (Features & 512) == 512;} }
		public bool SupportsFairplayAuth  { 	get { return (Features & 1204) == 1024;} }
		public bool SupportsPhotoCaching  { 	get { return (Features & 2048) == 2048;} }
	}
}

