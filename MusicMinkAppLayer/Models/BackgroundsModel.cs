using MusicMinkAppLayer.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicMinkAppLayer.Models
{
    public class BackgroundsModel
    {
        private static BackgroundsModel _current;
        public static BackgroundsModel Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new BackgroundsModel();
                }

                return _current;
            }
        }
        private BackgroundsModel()
        {

        }

        private void Init()
        {

            DatabaseManager.Current.AddBackgroundEntry(new BackgroundTable("绿茵", "Grass", "/Img/Grass.png", true, ""));
            DatabaseManager.Current.AddBackgroundEntry(new BackgroundTable("经典", "Mato", "/Img/Mato.png", false, ""));
            DatabaseManager.Current.AddBackgroundEntry(new BackgroundTable("平静", "Peace", "/Img/Peace.png", false, ""));
            DatabaseManager.Current.AddBackgroundEntry(new BackgroundTable("地下铁", "Metro", "/Img/Metro.png", false, ""));

        }
        public BackgroundModel GetSelected()
        {
            BackgroundTable table = DatabaseManager.Current.LookupSelectedBackground();
            BackgroundModel backgroundItem = new BackgroundModel(table);
            return backgroundItem;
        }
        public void UpdateBackground(List<BackgroundModel> source)
        {
            foreach (var item in source)
            {
                BackgroundTable table = new BackgroundTable(item.Title, item.Name, item.Img, item.IsSel, item.Ext);
                DatabaseManager.Current.Update(table);
            }
        }
        public List<BackgroundModel> GetBackgroundList()
        {
            List<BackgroundModel> backgroundList = new List<BackgroundModel>();
            IEnumerable<BackgroundTable> backgroundTableList = DatabaseManager.Current.FetchBackgroundItems();
            if (backgroundTableList.Count() == 0)
            {
                Init();
                backgroundTableList = DatabaseManager.Current.FetchBackgroundItems();
            }
            foreach (BackgroundTable item in backgroundTableList)
            {
                BackgroundModel backgroundItem = new BackgroundModel(item);
                backgroundList.Add(backgroundItem);
            }
            return backgroundList;
        }
        private List<BackgroundModel> _backgrounds;
        public List<BackgroundModel> Backgrounds
        {
            get
            {
                return _backgrounds;
            }
            private set
            {
                _backgrounds = value;
            }
        }


    }
}
