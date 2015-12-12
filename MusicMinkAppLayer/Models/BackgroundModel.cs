using MusicMinkAppLayer.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicMinkAppLayer.Models
{
    public class BackgroundModel : BaseModel<BackgroundTable>
    {
        public static class Properties
        {
            public const string BackgroundId = "BackgroundId";
            public const string Title = "Title";
            public const string Name = "Name";
            public const string Img = "Img";
            public const string IsSel = "IsSel";
            public const string Ext = "Ext";
        }

        public BackgroundModel(BackgroundTable table) : base(table)
        {

        }

        public int BackgroundId
        {
            get
            {
                return GetTableField<int>(BackgroundTable.Properties.BackgroundId);
            }
        }

        public string Title
        {
            get
            {
                return GetTableField<string>(BackgroundTable.Properties.Title);
            }
            set
            {
                SetTableField<string>(BackgroundTable.Properties.Title, value, Properties.Title);
            }
        }

        public string Img
        {
            get
            {
                return GetTableField<string>(BackgroundTable.Properties.Img);
            }
            set
            {
                SetTableField<string>(BackgroundTable.Properties.Img, value, Properties.Img);
            }
        }

        public string Name
        {
            get
            {
                return GetTableField<string>(BackgroundTable.Properties.Name);
            }
            set
            {
                SetTableField<string>(BackgroundTable.Properties.Name, value, Properties.Name);
            }
        }
        public string Ext
        {
            get
            {
                return GetTableField<string>(BackgroundTable.Properties.Ext);
            }
            set
            {
                SetTableField<string>(BackgroundTable.Properties.Ext, value, Properties.Ext);
            }
        }

        public bool IsSel
        {
            get
            {
                return GetTableField<bool>(BackgroundTable.Properties.IsSel);
            }
            set
            {
                SetTableField<bool>(BackgroundTable.Properties.IsSel, value, Properties.IsSel);
            }
        }
    }
}
