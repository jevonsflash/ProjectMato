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

            DatabaseManager.Current.AddBackgroundEntry(new BackgroundTable("绿茵", "Grass", "/Img/Grass.png", false, "0x091000|0x122000"));
            DatabaseManager.Current.AddBackgroundEntry(new BackgroundTable("伞", "Umbrella", "/Img/Umbrella.png", false, "0x2f1f01|0x462e01"));
            DatabaseManager.Current.AddBackgroundEntry(new BackgroundTable("电吉他", "Guitar", "/Img/Guitar.jpg", false, "0x060829|0x090e44"));
            DatabaseManager.Current.AddBackgroundEntry(new BackgroundTable("女孩", "Girl", "/Img/Girl.jpg", false, "0x241418|0x3f232a"));

            DatabaseManager.Current.AddBackgroundEntry(new BackgroundTable("平静", "Peace", "/Img/Peace.png", false, "0x01102f|0x01194b"));
            DatabaseManager.Current.AddBackgroundEntry(new BackgroundTable("地下铁", "Metro", "/Img/Metro.png", false, "0x000000|0x2d2d2d"));
            DatabaseManager.Current.AddBackgroundEntry(new BackgroundTable("紫罗兰", "Violet", "/Img/Violet.png", false, "0x1d082d|0x2d0d47"));
            DatabaseManager.Current.AddBackgroundEntry(new BackgroundTable("童话", "Fairyland", "/Img/Fairyland.png", false, "0x14262d|0x213e4a"));
            DatabaseManager.Current.AddBackgroundEntry(new BackgroundTable("葡萄酒", "Wine", "/Img/Wine.png", true, "0x2d1d20|0x4a2f35"));
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
        public void ClearAll()
        {
            DatabaseManager.Current.ClearBackground();
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
