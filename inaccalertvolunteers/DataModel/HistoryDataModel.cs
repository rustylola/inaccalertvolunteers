using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inaccalertvolunteers.DataModel
{
    public class HistoryDataModel
    {
        public string uID { get; set; }
        public string UsersName { get; set; }
        public string VolunteerName { get; set; }
        public string AccidentAddress { get; set; }
        public string AccidentCateg { get; set; }
        public string AccidentDescription { get; set; }

    }
}