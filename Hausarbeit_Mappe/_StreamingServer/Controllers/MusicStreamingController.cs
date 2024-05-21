using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _StreamingServer.Controllers
{
    [Route("api/")]
    [ApiController]
    public class MusicStreamingController : ControllerBase
    {
        private readonly MusicStreamDbContext db;

        public MusicStreamingController(MusicStreamDbContext context)
        {
            db = context;
        }


        //localhost:44351/api

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
            return CreatedAtAction("GetUser", new { id = nutzer.NutzerId }, nutzer); //Gibt die Methode zurück, die das erstellte Objekt findet.
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
