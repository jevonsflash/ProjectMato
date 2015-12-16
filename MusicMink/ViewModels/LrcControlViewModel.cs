using MusicMinkAppLayer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicMink.ViewModels
{
    public class LrcControlViewModel:INotifyPropertyChanged
    {
        private LRCItem lrcData;

        public LRCItem LrcData
        {
            get { return lrcData; }
            set
            {
                lrcData = value;
                NotifyPropertyChanged("LrcData");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
