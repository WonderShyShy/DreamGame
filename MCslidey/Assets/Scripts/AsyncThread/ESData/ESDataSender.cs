//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.IO.Compression;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Threading.Tasks;
//using Newtonsoft.Json.Linq;
//
//namespace BFF
//{
//    public class ESDataSender
//    {
//        private const string TAG = "ESDataSender----------";
//        private bool _isSending;
//        
//        private HttpClient _client;
//
//        public bool IsSending()
//        {
//            return _isSending;
//        }
//        
//        public async void SendESData(string esType, List<KeyValuePair<string, string>> postData, Action<string> successCallback = null, Action<string> failCallback = null)
//        {
//            try
//            {
//                _isSending = true;
//                if (_client == null)
//                {
//                    _client = new HttpClient();
//                }
//
//                _client.Timeout = TimeSpan.FromSeconds(10);
//                var encodedItems = postData.Select(i => WebUtility.UrlEncode(i.Key) + "=" + WebUtility.UrlEncode(i.Value));
//                var content = new StringContent(string.Join("&", encodedItems), null, "application/x-www-form-urlencoded");
//                var contentGzip = await GzipCompress(content);
//                var response = await _client.PostAsync(ESDataConfig.GetESUri(), contentGzip);
//                response.EnsureSuccessStatusCode();//用来抛异常的
//                var responseBody = await response.Content.ReadAsStringAsync();
//                Helper.Log(TAG, responseBody);
//                
//                if (response.IsSuccessStatusCode)
//                {
//                    var responseObj = JObject.Parse(responseBody);
//                    if ((int) responseObj["meta"]["code"] == 200)
//                    {
//                        successCallback?.Invoke(esType);
//                    }
//                    else
//                    {
//                        failCallback?.Invoke(esType);
//                    }
//                }
//                else
//                {
//                    failCallback?.Invoke(esType);
//                }
//                _isSending = false;
//            }
//            catch (Exception e)
//            {
//                Helper.LogError(e.Message + "\n" + e.StackTrace);
//                failCallback?.Invoke(esType);
//                _isSending = false;
//            }
//        }
//        
//        private async Task<HttpContent> GzipCompress(HttpContent content)
//        {
//            var ms = new MemoryStream();
//            using (var gzipStream = new GZipStream(ms, CompressionMode.Compress, true))
//            {
//                await content.CopyToAsync(gzipStream);
//                await gzipStream.FlushAsync();
//            }
//            ms.Position = 0;
//            var compressedStreamContent = new StreamContent(ms);
//            compressedStreamContent.Headers.ContentType = content.Headers.ContentType;
//            compressedStreamContent.Headers.Add("Content-Encoding", "gzip");
//            return compressedStreamContent;
//        }
//    }
//}
