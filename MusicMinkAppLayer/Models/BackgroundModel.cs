using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicMinkAppLayer.Models
{
    public class BackgroundEntityModel
    {
        public BackgroundEntityModel(int id, string title, string name, string img, bool isSel, string ext)
        {
            Id = id;
            Title = title;
            Name = name;
            Img = img;
            IsSel = isSel;
            Ext = ext;
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public string Img { get; set; }
        public bool IsSel { get; set; }
        public string Ext { get; set; }
    }

    public class BackgroundModel
    {
        public List<BackgroundEntityModel> Backgrounds { get; set; }
        public string Ext { get; set; }

    }

}
