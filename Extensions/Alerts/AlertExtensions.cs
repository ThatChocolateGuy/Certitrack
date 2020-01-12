using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Certitrack.Extensions.Alerts
{
    public static class AlertExtensions
    {
        public static IActionResult WithSuccess(this IActionResult result, string title, string body)
        {
            return Alert(result, "alert-success", title, body);
        }

        public static IActionResult WithInfo(this IActionResult result, string title, string body)
        {
            return Alert(result, "alert-info", title, body);
        }

        public static IActionResult WithWarning(this IActionResult result, string title, string body)
        {
            return Alert(result, "alert-warning", title, body);
        }

        public static IActionResult WithDanger(this IActionResult result, string title, string body)
        {
            return Alert(result, "alert-danger", title, body);
        }

        private static IActionResult Alert(IActionResult result, string type, string title, string body) => 
            new AlertDecoratorResult(result, type, title, body);
    }
}
