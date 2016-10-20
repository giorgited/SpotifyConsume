using System.Web.Mvc;
using System.Web.UI;
using SpotifyAPI;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using System;

namespace WebAppSpotifyConsumer.Controllers
{
    public class HomeController : Controller
    {
        
        public ActionResult Index(string artistName)
        {
            if (artistName != null )
            {
                 ShowAlbums(artistName);
                 return View("ShowAlbums"); 

            }
                 
            return View();
        }
        
        //show albums
        public ActionResult ShowAlbums(string artistName)
        {
            //search the artist
            string fullName = artistName;
            //if full name has space replace it with %20 for searching
            if(fullName.Contains(" "))
            {
                int index = fullName.IndexOf(' ');
                fullName = fullName.Substring(0, index) + "%20" + fullName.Substring(index + 1);
                Response.Write(fullName);
            }
            //search the artist
            string url = $"https://api.spotify.com/v1/search?q={fullName}&type=artist";
            SpotifyAPI.Web.Models.SearchItem t = new SpotifyAPI.Web.Models.SearchItem();
            t = GetFromSpotify<SpotifyAPI.Web.Models.SearchItem>(url);

            try
            {
                string artistID = t.Artists.Items[0].Id;
                //search artist's albums
                string urlArtist = $"https://api.spotify.com/v1/artists/{artistID}/albums";
                SpotifyAPI.Web.Models.Paging<SpotifyAPI.Web.Models.SimpleAlbum> allAlbumsSimplified = new SpotifyAPI.Web.Models.Paging<SpotifyAPI.Web.Models.SimpleAlbum>();
                allAlbumsSimplified = GetFromSpotify<SpotifyAPI.Web.Models.Paging<SpotifyAPI.Web.Models.SimpleAlbum>>(urlArtist);
                //get only 5 albums
                int count; 
                if (allAlbumsSimplified.Items.Count > 5) { count = 5; }
                else { count = allAlbumsSimplified.Items.Count; }
                //use the herf to get the album's full details
                SpotifyAPI.Web.Models.FullAlbum[] fullAlbum = new SpotifyAPI.Web.Models.FullAlbum[count];
                for (int i = 0; i < count; i++)
                {
                    fullAlbum[i] = GetFromSpotify<SpotifyAPI.Web.Models.FullAlbum>(allAlbumsSimplified.Items[i].Href);
                }
                //declare all the returned albums information
                string[] albumNames = new string[count];
                string[] albumReleaseDate = new string[count];
                string[] albumArtist = new string[count];
                string[] NumberOfTracks = new string[count];
                string[] Popularity = new string[count];
                //store the returned albums information
                for (int i=0; i<fullAlbum.Length; i++)
                {
                    albumNames[i]= fullAlbum[i].Name;
                    albumReleaseDate[i] = fullAlbum[i].ReleaseDate;
                    albumArtist[i] = fullAlbum[i].Artists[0].Name;
                    NumberOfTracks[i] = (fullAlbum[i].Tracks.Total).ToString();
                    Popularity[i] = (fullAlbum[i].Popularity).ToString();
                }
                //store it in ViewBag to send it to View
                ViewBag.AlbumNames = albumNames;
                ViewBag.AlbumReleaseDate = albumReleaseDate;
                ViewBag.albumArtist = albumArtist;
                ViewBag.NumberOfTracks = NumberOfTracks;
                ViewBag.Popularity = Popularity;
               
           } catch (ArgumentOutOfRangeException e){
                Response.Write("<H1>Artist Not Found</H1>");
           }
            return View();
        }

        //get data from spotify server
        public T GetFromSpotify<T>(string url)
        {
            try
            {
                WebRequest request = WebRequest.Create(url);
                request.Method = "GET";

                request.ContentType = "application/json; charset=utf-8";

                T type = default(T);

                using (WebResponse response = request.GetResponse())
                {
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(dataStream))
                        {
                            string responseFromServer = reader.ReadToEnd();
                            type = JsonConvert.DeserializeObject<T>(responseFromServer); //Json return
                        }
                    }
                }
                return type;
            }
            catch (WebException ex)
            {
                return default(T);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        


    }
}
