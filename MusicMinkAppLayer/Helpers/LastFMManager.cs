using MusicMinkAppLayer.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace MusicMinkAppLayer.Helpers
{
    public enum LastfmStatusCode
    {
        Success,
        RetryableFailure,
        Failure,
    }

    /// <summary>
    /// Helper for interfacing with LastFM
    /// </summary>
    public class LastFMManager
    {
        private const string API_KEY = ApiKeys.LASTFM_API_KEY;
        private const string API_SECRET = ApiKeys.LASTFM_API_SECRET;
        private const string API_PATH = "https://ws.audioscrobbler.com/2.0/";

        private HttpClient LocalClient = new HttpClient();

        private bool Initalized = false;

        private static LastFMManager _current;
        public static LastFMManager Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new LastFMManager();
                }

                return _current;
            }
        }

        private LastFMManager() { }

        private async Task<JObject> getInfo(string method, Dictionary<string, string> parameters, bool isSigned = false)
        {
            if (!Initalized)
            {
                LocalClient.BaseAddress = new Uri(API_PATH);
                LocalClient.DefaultRequestHeaders.Accept.Clear();
                LocalClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                Initalized = true;
            }

            parameters.Add("api_key", API_KEY);
            parameters.Add("method", method);

            if (isSigned)
            {
                string api_sig = GenerateSignature(parameters);

                parameters.Add("api_sig", api_sig);
            }

            parameters.Add("format", "JSON");

            HttpContent content = new FormUrlEncodedContent(parameters);

            HttpResponseMessage response = await LocalClient.PostAsync("", content);

            string responseAsString = await response.Content.ReadAsStringAsync();

            if (responseAsString == string.Empty) return new JObject();

            return (JObject) JsonConvert.DeserializeObject(responseAsString);
        }

        public async Task<String> GetAlbumArt(string artist, string album)
        {
            JObject result = await getInfo("album.getinfo",
                new Dictionary<string, string>() { { "artist", artist }, { "album", album } });

            if (result["album"] != null)
            {
                if (result["album"]["image"] != null)
                {
                    return result["album"]["image"].Last["#text"].ToString();
                }
            }

            return string.Empty;
        }




        private string GenerateSignature(Dictionary<string, string> parameters)
        {
            List<string> signatureParts = new List<string>();

            List<string> keyList = new List<string>(parameters.Keys);

            keyList.Sort();

            StringBuilder b = new StringBuilder();

            foreach (string s in keyList)
            {
                b.Append(s);
                b.Append(parameters[s]);
            }

            b.Append(API_SECRET);


                var Md5 = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
                var buffer = CryptographicBuffer.ConvertStringToBinary(b.ToString(), BinaryStringEncoding.Utf8);
                IBuffer buffHash = Md5.HashData(buffer);


                return CryptographicBuffer.EncodeToHexString(buffHash);
        }
    }
}
