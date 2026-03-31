using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Maui;

namespace Roomies
{
    public static class ServiceHelper
    {
        public static T GetService<T>() =>
            IPlatformApplication.Current.Services.GetService<T>();
    }
}


