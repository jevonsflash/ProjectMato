using System;
using System.Collections.Generic;
using System.ComponentModel;
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


    public class Gecime_Lyric : INotifyPropertyChanged
    {
        public int count { get; set; }
        public int code { get; set; }

        private Result2[] result;

        public Result2[] Result
        {
            get { return result; }
            set
            {
                result = value;
                NotifyPropertyChanged("Result");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string value)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(value));
            }
        }

    }

    public class Result2ForShow : Result2
    {

        public string artist { get; set; }
    }

}
