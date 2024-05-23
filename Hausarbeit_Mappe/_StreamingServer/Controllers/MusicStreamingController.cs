using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Net;
using System.Web;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.IO;
using Azure;

namespace _StreamingServer.Controllers
{

    public class MusicStreamingController : ControllerBase
    {
        private readonly MusicStreamDbContext db;


        public MusicStreamingController(MusicStreamDbContext context)
        {
            db = context;

        }


        //localhost:44351/api
        [HttpGet]
        [Route("api/songs_in_db")]
        public IActionResult GetSongsInDb()
        {
            try
            {
                var list = db.Lieders.Select(lied => new 
                {
                    Titel = lied.Titel,
                    Kuenstler = lied.Künstler.Name
                })
                .ToList();

                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpGet]
        [Route("api/stream")]
        public IActionResult StreamAudio([FromQuery] string filetoget)
        {
            
            string folderpath = Path.Combine(Directory.GetCurrentDirectory(), "/Lieder");

            string[] audioFiles = Directory.GetFiles(folderpath, "*.mp3")
            .Where(file =>
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                return fileName.IndexOf(filetoget, StringComparison.OrdinalIgnoreCase) >= 0;
            })
            .ToArray();
            if(audioFiles.Length == 0)
            {
                return NotFound("Audio file not found");
            }
            string filetostream = audioFiles[0];

            var fileStream = new FileStream(filetostream,FileMode.Open,FileAccess.Read,FileShare.Read);

            return File(fileStream,"audio/mpeg");
            }

        private IActionResult RangeNotSatisfiable(long totalLength)
        {
            Response.StatusCode = (int)HttpStatusCode.RequestedRangeNotSatisfiable;
            Response.Headers[HeaderNames.ContentRange] = new ContentRangeHeaderValue(totalLength).ToString();
            return new EmptyResult();
        }



        [HttpPost]
        [Route("api/user")]
        public IActionResult CreateUser(string name, string vorname, string email, string passwort)
        {
            Nutzer nutzer = new Nutzer();
            nutzer.Name = name;
            nutzer.Vorname = vorname;
            nutzer.Email = email;
            nutzer.Passwort = passwort;

            db.Nutzers.Add(nutzer);
            db.SaveChanges();
            return Ok(nutzer.NutzerId);
        }
        [HttpGet]
        [Route("api/user")]

        public IActionResult GetUser(int id)
        {
            Nutzer nutzerindb = db.Nutzers.SingleOrDefault(x => x.NutzerId == id);

            if (nutzerindb == null)
            {
                return NotFound();
            }

            return Ok(nutzerindb);
        }
        [HttpGet]
        [Route("api/login")]
        public IActionResult Login(string email, string password)
        {
            Nutzer nutzerindb = db.Nutzers.SingleOrDefault(x => x.Email == email);

            if (nutzerindb == null)
            {
                return NotFound();
            }

            if (nutzerindb.Passwort == password)
            {
                return Ok();
            }
            return NotFound();
        }

    }
}
