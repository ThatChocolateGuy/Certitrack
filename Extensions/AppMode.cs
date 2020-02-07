using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Certitrack.Extensions
{
    public static class AppMode
    {
    #if DEBUG
        public const bool isDebug = true;
    #else
        public const bool isDebug = false;
    #endif
    }
}
