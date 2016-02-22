using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MusicMinkAppLayer.Models
{



    public class Album
    {

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("artist")]
        public string Artist { get; set; }

        [XmlElement("url")]
        public string Url { get; set; }

        [XmlElement("image")]
        public string[] Image { get; set; }

        [XmlElement("listeners")]
        public string Listeners { get; set; }

        [XmlElement("playcount")]
        public string Playcount { get; set; }
    }

    public class lfm
    {

        [XmlElement("album")]
        public Album Album { get; set; }
    }

}

