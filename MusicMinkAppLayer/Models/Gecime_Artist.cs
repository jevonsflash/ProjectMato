using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicMinkAppLayer.Models
{
    public class Result3
    {
        public string profile { get; set; }
        public string name { get; set; }
        public string area { get; set; }
        public string alias { get; set; }
        public string birthday { get; set; }
        public string constellation { get; set; }

    }
    public class Gecime_Artist
    {

        public int count { get; set; }
        public int code { get; set; }

        private Result3 result;

        public Result3 Result
        {
            get { return result; }
            set
            {
                result = value;
                
            }
        }
    }

}
