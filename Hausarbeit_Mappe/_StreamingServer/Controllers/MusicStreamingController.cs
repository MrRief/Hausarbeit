using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Net.Http.Headers;
using System.IO;
using System.Linq;

namespace _StreamingServer.Controllers
{
    
    public class MusicStreamingController : ApiController
    {
        private readonly MusicStreamDbContext db;

        public MusicStreamingController(MusicStreamDbContext context)
        {
            db = context;
        }


        //localhost:44351/api
        [HttpGet]
        [Route("api/songs_in_db")]
        public IHttpActionResult GetSongsInDb()
        {
            var list = db.Lieders.ToList();
            return Ok(list);
        }
        [HttpGet]
        [Route("api/stream")]
        public async Task<IHttpActionResult> StreamSong(string name)
        {
            var filepath = Path.Combine(Directory.GetCurrentDirectory(), "api/Lieder", name);
            if (!System.IO.File.Exists(filepath))
            {
                return NotFound();
            }
            var memoryStream = new MemoryStream();
            using(var stream = new FileStream(filepath,FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                await stream.CopyToAsync(memoryStream);
            }
            memoryStream.Position = 0;
            return File(memoryStream, "audio/mpeg", enableRangeProcessing: true);
        }


        [HttpPost]
        [Route("api/user")]
        public IHttpActionResult CreateUser(string name, string vorname, string email, string passwort)
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

        public IHttpActionResult GetUser(int id)
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
        public IHttpActionResult Login(string email, string password)
        {
            Nutzer nutzerindb = db.Nutzers.SingleOrDefault(x =>x.Email == email);

            if(nutzerindb == null)
            {
                return NotFound();
            }

            if(nutzerindb.Passwort == password)
            {
                return Ok();
            }
            return NotFound();
        }

    }
}
