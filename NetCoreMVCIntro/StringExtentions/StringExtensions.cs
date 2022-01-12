using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreMVCIntro.StringExtentions
{
    // extension sınfıları static olur. static sınıfların içerisinde zaten static üyeler yazabiliriz.
    public static class StringExtensions
    {

        //public static void Run(this IApplicationBuilder app, RequestDelegate handler);
        // this keyword ile c# extent edeceğimiz tipi belirtebiliriz.
        // this IApplicationBuilder app  IApplicationBuilder interfaceden türeyen tüm nesneleri extend etmiş
        public static string ToUpperCase(this string value)
        {
            return value.ToUpper();
        }

        public static string GetPrettyDate(this DateTime date)
        {
            return date.ToShortDateString();

        }
    }
}
