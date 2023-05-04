using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace Server {
	public static class RequestHelper {
		
		private static Dictionary<int, (Action<string>, Action<string>)> hash = new Dictionary<int, (Action<string>, Action<string>)>();
		private static CancellationTokenSource tokenSource;
		static RequestHelper() {
			tokenSource = new CancellationTokenSource();
		}
		public static void HttpGet(string url, Action<string> OnSuccess, Action<string> OnFail) {
		
			HttpGetAsync(url, OnSuccess, OnFail);
		}
		public static void HttpPost(string url, List<string> body, Action<string> OnSuccess, Action<string> OnFail) {
			HttpPostAsync(url, body, OnSuccess, OnFail);
		}
		private static async void HttpPostAsync(string url, List<string> body, Action<string> OnSuccess, Action<string> OnFail) {
			CancellationToken token = tokenSource.Token;
			string json = JsonConvert.SerializeObject(body);
			byte[] postData = System.Text.Encoding.UTF8.GetBytes(json);
			using (UnityWebRequest webRequest = UnityWebRequest.Post(url, "")) {
				hash.Add(webRequest.GetHashCode(), (OnSuccess, OnFail));
				webRequest.uploadHandler = new UploadHandlerRaw(postData);
				webRequest.SetRequestHeader("Content-Type", "application/json");
				var operation = webRequest.SendWebRequest();
				while (!operation.isDone && !token.IsCancellationRequested) {
					await Task.Yield();
				}
				switch (webRequest.result) {
					case UnityWebRequest.Result.ConnectionError:
					case UnityWebRequest.Result.DataProcessingError:
					case UnityWebRequest.Result.ProtocolError:
						hash[webRequest.GetHashCode()].Item2?.Invoke(webRequest.error);
						break;
					case UnityWebRequest.Result.Success:		
						hash[webRequest.GetHashCode()].Item1?.Invoke(webRequest.downloadHandler.text);
						break;
				}
				hash.Remove(webRequest.GetHashCode());
			}
		}
		private static async void HttpGetAsync(string url, Action<string> OnSuccess, Action<string> OnFail) {
			CancellationToken token = tokenSource.Token;

			using (UnityWebRequest webRequest = UnityWebRequest.Get(url)) {
				hash.Add(webRequest.GetHashCode(), (OnSuccess, OnFail));
				var operation = webRequest.SendWebRequest();
				while (!operation.isDone && !token.IsCancellationRequested) {
					await Task.Yield();
				}
				switch (webRequest.result) {
					case UnityWebRequest.Result.ConnectionError:
					case UnityWebRequest.Result.DataProcessingError:
					case UnityWebRequest.Result.ProtocolError:
						hash[webRequest.GetHashCode()].Item2?.Invoke(webRequest.error);
						break;
					case UnityWebRequest.Result.Success:						
						hash[webRequest.GetHashCode()].Item1?.Invoke(webRequest.downloadHandler.text);
						break;
				}
				hash.Remove(webRequest.GetHashCode());
			}
		}

		public static void Dispose() {
			tokenSource.Cancel();
			hash?.Clear();
			hash = null;
		}
	}

}