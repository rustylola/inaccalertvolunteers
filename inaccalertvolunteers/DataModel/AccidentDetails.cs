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
    class AccidentDetails
    {
        public string accidentAddress { get; set; } //user address
        
        public string userName { get; set; }
        public string userPhone { get; set; }
        public double accidentLat { get; set; }
        public double accidentLng { get; set; }
        public string accidentID { get; set; }
    }
}