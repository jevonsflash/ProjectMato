using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MusicMinkAppLayer.Helpers
{

    public class HttpHelper
    {
        public string postString = string.Empty;
        public static string result = string.Empty;

        //接下来创建类FileWatch。然后声明事件，注意事件的类型即为我们之前定义的委托。

        public delegate void FileWatchEventHander(object sender, CompleteEventArgs e);

        public event FileWatchEventHander FileWatchEvent;

        //现在创建方法OnFileChange()，当调用该方法时将触发事件：


        /// <summary>  
        /// 创建POST方式的HTTP请求  
        /// </summary>  
        public void CreatePostHttpResponse(string url)
        {
            HttpWebRequest request = null;
            request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "GET";
            //发送GET数据  
            request.BeginGetResponse(new AsyncCallback(ResponseReadySocket), request);


        }


        private void ResponseReadySocket(IAsyncResult asyncResult)
        {
            try
            {
                WebRequest request = asyncResult.AsyncState as WebRequest;
                WebResponse response = request.EndGetResponse(asyncResult) as HttpWebResponse;
                result = GetResponseString(response);
                string keyword = request.RequestUri.Segments.Last();

                CompleteEventArgs args = new CompleteEventArgs(result,keyword);
                FileWatchEvent(this, args);

            }
            catch (Exception e)
            {
            }

        }



        /// <summary>
        /// 获取请求的数据
        /// </summary>
        public string GetResponseString(WebResponse webresponse)
        {
            using (Stream s = webresponse.GetResponseStream())
            {
                StreamReader reader = new StreamReader(s, Encoding.UTF8);
                return reader.ReadToEnd();

            }
        }

    }
    public class CompleteEventArgs : EventArgs
    {
        public CompleteEventArgs()
        {
            _node = string.Empty;
            _node2 = string.Empty;
        }
        //需要传递的变量
        private string _node;

        public string Node
        {
            get { return _node; }
        }
        private string _node2;

        public string Node2
        {
            get { return _node2; }
        }
        public CompleteEventArgs(string node, string node2)
        {
            this._node = node;
            this._node2 = node2;
        }
    }
}