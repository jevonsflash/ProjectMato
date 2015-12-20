using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Globalization.Collation;

namespace MusicMink.Common
{
    public static class HeaderCharacter
    {
        public static List<string> CreateDefaultGroups(CharacterGroupings slg)
        {
            List<string> list = new List<string>();
            foreach (CharacterGrouping cg in slg)
            {
                if (cg.Label == "") continue;
                else
                    list.Add(cg.Label);
            }
            return list;
        }

    }
}
