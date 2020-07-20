using LoyaltyMiddleware.Cache;
using LoyaltyMiddleware.Loyalty;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LoyaltyMiddleware.Creatio
{
	public class CRMIntegrationProvider
	{
		private readonly string _login;
		private readonly string _password;
		private readonly string _uri;
		private readonly string _cookieLifetime;
		private CookieContainer _bpmCookieContainer;
		private string _csrf;
		private readonly bool _useLocalCookie;

		public CRMIntegrationProvider(bool useLocalCookie = false)
		{ 
			_useLocalCookie = useLocalCookie;
			GlobalCacheReader.GetValue(GlobalCacheReader.CacheKeys.BPMLogin, out _login);
			GlobalCacheReader.GetValue(GlobalCacheReader.CacheKeys.BPMPassword, out _password);
			GlobalCacheReader.GetValue(GlobalCacheReader.CacheKeys.BPMUri, out _uri);
			GlobalCacheReader.GetValue(GlobalCacheReader.CacheKeys.BPMAuthCookieLifetimeMinutes, out _cookieLifetime);
		}

		public RequestResult MakeRequest(string bpmServiceUri, string body)
		{
			CookieContainer bpmCookieContainer = null;

			if (_useLocalCookie)
			{
				if (_bpmCookieContainer == null)
				{
					_csrf = Authorize(out _bpmCookieContainer);
					bpmCookieContainer = _bpmCookieContainer;
				}
			}
			else
			{
				if (!GlobalCacheReader.GetValue(GlobalCacheReader.CacheKeys.BPMCookie, out bpmCookieContainer))
				{
					var csrf = Authorize( out bpmCookieContainer);
					GlobalCacheReader.SetTemporaryValue(GlobalCacheReader.CacheKeys.BPMCookie, bpmCookieContainer, TimeSpan.FromMinutes(Convert.ToInt32(_cookieLifetime)));
					GlobalCacheReader.SetTemporaryValue(GlobalCacheReader.CacheKeys.BPMCSRF, csrf, TimeSpan.FromMinutes(Convert.ToInt32(_cookieLifetime)));
				}
			}

			return Request(bpmServiceUri, body, out _, bpmCookieContainer);
		}

		private string Authorize(out CookieContainer bpmCookieContainer)
		{
			//Вызов сервиса с авторизацией    
			var req = (HttpWebRequest)WebRequest.Create(String.Format("{0}/ServiceModel/AuthService.svc/Login", _uri));

			req.Method = "POST";
			req.ContentType = "application/json";
			req.Accept = "application/json";
			bpmCookieContainer = new CookieContainer();
			req.CookieContainer = bpmCookieContainer;

			req.Credentials =
				   System.Net.CredentialCache.DefaultCredentials;

			req.Proxy.Credentials =
				   System.Net.CredentialCache.DefaultCredentials;

			using (var streamWriter = new StreamWriter(req.GetRequestStream()))
			{
				string json = "{\"UserName\":\"" + _login + "\",\"UserPassword\":\"" + _password + "\"}";

				streamWriter.Write(json);
				streamWriter.Flush();
				streamWriter.Close();
			}

			req.GetResponse();
			var cookies = bpmCookieContainer.GetCookies(new Uri(_uri));
			try
			{
				return cookies["BPMCSRF"].Value;
			}
			catch
			{
				return String.Empty;
			}
		}

		private RequestResult Request(string bpmServiceUri, string body, out HttpWebRequest req, CookieContainer bpmCookieContainer)
		{
			req = (HttpWebRequest)WebRequest.Create(String.Format("{0}/0/rest/{1}", _uri, bpmServiceUri));
			req.Method = "POST";
			req.ContentType = "application/json";
			req.Accept = "application/json";
			req.CookieContainer = bpmCookieContainer;
			req.Credentials = System.Net.CredentialCache.DefaultCredentials;
			req.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
			req.Timeout = 10 * 1000 * 60;

			string csrfToken;
			if (_useLocalCookie)
			{
				csrfToken = _csrf;
				req.CookieContainer = _bpmCookieContainer;
			}
			else GlobalCacheReader.GetValue(GlobalCacheReader.CacheKeys.BPMCSRF, out csrfToken);

			if (!String.IsNullOrEmpty(csrfToken))
			{
				req.Headers.Add("BPMCSRF", csrfToken);
			}

			using (var requestStream = req.GetRequestStream())
			{
				using var streamWriter = new StreamWriter(requestStream);
				streamWriter.Write(body);
				streamWriter.Flush();
				streamWriter.Close();
			}

			try
			{
				using var response = req.GetResponse();
				using var responseStream = response.GetResponseStream();
				using var streamReader = new StreamReader(responseStream);
				return new RequestResult() { IsSuccess = true, ResponseStr = streamReader.ReadToEnd() };
			}
			catch (WebException e)
			{
				if (e.Response == null)
				{
					Logger.LogError($"{bpmServiceUri} error: ", e);
					return new RequestResult() { IsSuccess = false, ResponseStr = $"{e.Message} null response" };
				}
				using var streamReader = new StreamReader(e.Response.GetResponseStream());
				var res = streamReader.ReadToEnd();
				Logger.LogError($"{bpmServiceUri} error. {res}", e);
				return new RequestResult() { IsSuccess = false, ResponseStr = $"{e.Message} {res}" };
			}
		}
	}
}
