using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicMinkAppLayer.Models
{
    public class Result2
    {
        public int aid { get; set; }
        public string lrc { get; set; }
        public int sid { get; set; }
        public int artist_id { get; set; }
        public string song { get; set; }
    }


    public class Gecime
    {
        public int count { get; set; }
        public int code { get; set; }
        public Result2[] result { get; set; }
    }

}
