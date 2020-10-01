using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using LeaveSample.Entities;

namespace LeaveSample.UI.Web
{
    public class UIHelper
    {
        public static List<ListItem> LoadEnumFilters(Type type)
        {
            int i = 0;
            List<ListItem> list = Enum.GetNames(type)
                .Select(v => new ListItem(v, i++.ToString())).ToList();

            list.Insert(0, new ListItem("- All -", ""));

            return list;
        }

    }
}