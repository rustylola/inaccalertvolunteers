﻿using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inaccalertvolunteers.Adapter
{
    class ViewPagerAdapter : FragmentPagerAdapter
    {
        public List<Android.Support.V4.App.Fragment> fragments { get; set; }
        public List<string> fragmentNames { get; set; }
        //make constrac
        public ViewPagerAdapter(Android.Support.V4.App.FragmentManager fragmentManager) : base(fragmentManager)
        {
            fragments = new List<Android.Support.V4.App.Fragment>();
            fragmentNames = new List<string>();
        }

        public void Addfragment(Android.Support.V4.App.Fragment fragment, string name)
        {
            fragments.Add(fragment);
            fragmentNames.Add(name);
        }
        public override int Count
        {
            get
            {
                return fragments.Count;
            }
        }

        public override Android.Support.V4.App.Fragment GetItem(int position)
        {
            return fragments[position];
        }
    }
}