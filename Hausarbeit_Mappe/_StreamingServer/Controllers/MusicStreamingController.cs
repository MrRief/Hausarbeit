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
using Microsoft.AspNetCore.Identity.Data;

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
        [Route("api/create_user")]
        public IActionResult CreateUser([FromBody] Nutzer neuerNutzer)
        {
           if(neuerNutzer == null)
            {
                return BadRequest("Ungültige Nutzerdaten");
            }

            db.Nutzers.Add(neuerNutzer);
            db.SaveChanges();
            return Ok();
        }
       
        [HttpPost]
        [Route("api/login")]
        public IActionResult Login([FromBody]LoginModel request)
        {
            Nutzer nutzerindb = db.Nutzers.SingleOrDefault(x => x.Email == request.Email);

            if (nutzerindb == null)
            {
                return NotFound();
            }

            if (nutzerindb.Passwort == request.Passwort)
            {
                return Ok();
            }
            return NotFound();
        }
        public class LoginModel 
        {
            public string? Email { get; set; }
            public string? Passwort { get; set;}
        }

    }
}
