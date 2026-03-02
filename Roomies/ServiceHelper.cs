using System;
using System.Collections.Generic;
using System.Text;

namespace Roomies
{
    public static class ServiceHelper
    {
        public static T GetService<T>() =>
            Current.GetService<T>();

        public static IServiceProvider Current =>
            Application.Current.Handler.MauiContext.Services;
    }
}
