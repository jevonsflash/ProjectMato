using SQLite;

namespace MusicMinkAppLayer.Tables
{
    /// <summary>
    /// Defines the structure of the Background table for the SQLite database 
    /// </summary>
    [Table("BackgroundTable")]
    public class BackgroundTable : BaseTable
    {
        [PrimaryKey, AutoIncrement]
        public int BackgroundId { get; set; }

        public static class Properties
        {
            public const string BackgroundId = "BackgroundId";
            public const string Title = "Title";
            public const string Name = "Name";
            public const string Img = "Img";
            public const string IsSel = "IsSel";
            public const string Ext = "Ext";
        }

        public BackgroundTable()
        {
            Title = string.Empty;
            Name = string.Empty;
            Img = string.Empty;
            IsSel = false;
            Ext = string.Empty;
        }
        public BackgroundTable(string title, string name, string img, bool isSel, string ext)
        {
            Title = title;
            Name = name;
            Img = img;
            IsSel = isSel;
            Ext = ext;
        }

        public string Title { get; set; }
        public string Name { get; set; }
        public string Img { get; set; }
        public bool IsSel { get; set; }
        public string Ext { get; set; }
    }
}