using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace RedmondIntegrationService
{
	public class ProcessingIntegrationProvider
	{
		private string _login;
		private string _password;
		private string _uri;
		private string _token;
		private string _tokenCacheName;
		//private CookieContainer _bpmCookieContainer;
		//private string _csrf;
		//private bool _useLocalCookie;

		public ProcessingIntegrationProvider(bool isPA)
		{
			if (isPA)
			{
				GlobalCacheReader.GetValue(GlobalCacheReader.CacheKeys.PersonalAreaUri, out _uri);
			}
			else
			{
				GlobalCacheReader.GetValue(GlobalCacheReader.CacheKeys.ProcessingUri, out _uri);
			}

			/*if (String.IsNullOrEmpty(_token))
			{
				Authorize();
			}*/
		}

		public RequestResult Request(string method, string body, bool isNeedRepeat = true)
		{
			var req = (HttpWebRequest)WebRequest.Create(String.Format("{0}/Entity/{1}", _uri, method));
			req.Method = "POST";
			req.ContentType = "application/json";
			req.Accept = "application/json";
			req.Credentials = System.Net.CredentialCache.DefaultCredentials;
			req.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
			req.Timeout = 10 * 1000 * 60;
			//req.Headers.Add("Authorization", $"Bearer {_token}");

			using (var requestStream = req.GetRequestStream())
			{
				using (var streamWriter = new StreamWriter(requestStream))
				{
					streamWriter.Write(body);
					streamWriter.Flush();
					streamWriter.Close();
				}
			}

			try
			{
				using (var response = req.GetResponse())
				{
					using (var responseStream = response.GetResponseStream())
					{
						using (var streamReader = new StreamReader(responseStream))
						{
							return new RequestResult() { IsSuccess = true, ResponseStr = streamReader.ReadToEnd() };
						}
					}
				}
			}
			catch (WebException e)
			{
				if (e.Status == WebExceptionStatus.Timeout)
				{
					Logger.LogError(method, e);
					return new RequestResult() { IsSuccess = false, ResponseStr = e.Message, IsTimeout = true };
				}

				if (e.Response == null)
				{
					Logger.LogError(method, e);
					return new RequestResult() { IsSuccess = false, ResponseStr = e.Message };
				}

				/*if (((HttpWebResponse)e.Response).StatusCode == HttpStatusCode.Unauthorized)
				{
					if (Authorize())
					{
						if (isNeedRepeat) return Request(method, body, false);
					}
				}*/

				using (var streamReader = new StreamReader(e.Response.GetResponseStream()))
				{
					var res = streamReader.ReadToEnd();
					Logger.LogError($"{method} {res} {_token} {body}", e);
					return new RequestResult() { IsSuccess = false, ResponseStr = e.Message + " " + res };
				}
			}
			catch (Exception e)
			{
				return new RequestResult() { IsSuccess = false, ResponseStr = e.Message };
			}
		}

		public RequestResult RequestMethod(string method, string body, bool isNeedRepeat = true)
		{
			var req = (HttpWebRequest)WebRequest.Create(String.Format("{0}/{1}", _uri, method));
			req.Method = "POST";
			req.ContentType = "application/json";
			req.Accept = "application/json";
			req.Credentials = System.Net.CredentialCache.DefaultCredentials;
			req.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
			req.Timeout = 10 * 1000 * 60;
			//req.Headers.Add("Authorization", $"Bearer {_token}");

			using (var requestStream = req.GetRequestStream())
			{
				using (var streamWriter = new StreamWriter(requestStream))
				{
					streamWriter.Write(body);
					streamWriter.Flush();
					streamWriter.Close();
				}
			}

			try
			{
				using (var response = req.GetResponse())
				{
					using (var responseStream = response.GetResponseStream())
					{
						using (var streamReader = new StreamReader(responseStream))
						{
							return new RequestResult() { IsSuccess = true, ResponseStr = streamReader.ReadToEnd() };
						}
					}
				}
			}
			catch (WebException e)
			{
				if (e.Status == WebExceptionStatus.Timeout)
				{
					return new RequestResult() { IsSuccess = false, ResponseStr = e.Message, IsTimeout = true };
				}

				if (e.Response == null)
				{
					Logger.LogError($"{method}", e);
					return new RequestResult() { IsSuccess = false, ResponseStr = e.Message };
				}

				/*if (((HttpWebResponse)e.Response).StatusCode == HttpStatusCode.Unauthorized)
				{
					if (Authorize())
					{
						if (isNeedRepeat) return Request(method, body, false);
					}
				}*/

				using (var streamReader = new StreamReader(e.Response.GetResponseStream()))
				{
					var res = streamReader.ReadToEnd();
					Logger.LogError($"{method} {res}", e);
					return new RequestResult() { IsSuccess = false, ResponseStr = e.Message + " " + res };
				}
			}
			catch (Exception e)
			{
				return new RequestResult() { IsSuccess = false, ResponseStr = e.Message };
			}
		}
	}

}
