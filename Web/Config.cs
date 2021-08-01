using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web
{
    public class Config
    {
        public static string ConnectionString { get; internal set; }
        public static string DocumentsFolderPath { get; internal set; }
        public static string ServerUrl { get; internal set; }
    }
}
