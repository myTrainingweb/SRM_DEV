using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IP.ActionFilters;
using PlusCP.Models;

namespace PlusCP.Controllers
{
    [OutputCache(Duration = 0)]
    [SessionTimeout]
    public class PhotosController : Controller
    {
        Photos oPhotos;
        // GET: Photos
        public ActionResult Option()
        {
            oPhotos = new Photos();
            return View(oPhotos);
        }
        public ActionResult Index()
        {
            return View();
        }
    }
}