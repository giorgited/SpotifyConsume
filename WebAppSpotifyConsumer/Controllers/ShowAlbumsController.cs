using System.Web.Mvc;
using System.Web.UI;
using SpotifyAPI;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using System;

namespace WebAppSpotifyConsumer.Controllers
{
    public class ShowAlbumController : Controller
    {

        public ActionResult Index(string firstName, string lastName)
        {
            

            return View();
        }


    }
}
