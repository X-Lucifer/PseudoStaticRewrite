using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PseudoStaticRewrite.Models;

namespace PseudoStaticRewrite.Controllers
{
    [AutoPseudo]
    public class OrderController : Controller
    {
        public IActionResult Index(int? state, int? page)
        {
            var xstate = state ?? 0;
            var xpage = page ?? 1;
            ViewData["xstate"] = xstate;
            ViewData["xpage"] = xpage;
            return View();
        }

        public IActionResult Info(int? oid,string orderno)
        {
            int xoid = oid ?? 0;
            ViewData["xoid"] = xoid;
            ViewData["xorderno"] = orderno ?? "";
            return View();
        }
    }
}
