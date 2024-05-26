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
using Microsoft.Data.SqlClient;
using static _StreamingServer.Controllers.MusicStreamingController;

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
        [Route("api/get_songid")]
        public IActionResult GetSong([FromQuery] string titel, [FromQuery] string kuenstler)
        {
            try
            {
                Künstler kid = db.Künstlers.FirstOrDefault(x => x.Name == kuenstler);
                Lieder lieder = db.Lieders.FirstOrDefault(x => x.Titel == titel && x.KünstlerId == kid.KünstlerId);
                if (lieder == null)
                {
                    return NotFound();
                }
                return Ok(lieder.Id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("api/get_user")]
        public IActionResult GetUser([FromBody] int id)
        {
            try
            {
                Nutzer nutzer = db.Nutzers.SingleOrDefault(nutzer => nutzer.NutzerId == id);
                if (nutzer == null)
                {
                    return NotFound();
                }
                NutzerDTO nutzerdto = new NutzerDTO
                {
                    NutzerId = nutzer.NutzerId,
                    Name = nutzer.Name,
                    Vorname = nutzer.Vorname,
                    Email = nutzer.Email,
                };
                return Ok(nutzerdto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        public class NutzerDTO
        {
            public int NutzerId { get; set; }
            public string Name { get; set; }
            public string Vorname { get; set; }
            public string Email { get; set; }

        }
        [HttpGet]
        [Route("api/stream")]
        public IActionResult StreamAudio([FromQuery] string filetoget)
        {
            try
            {
                string folderpath = Path.Combine(Directory.GetCurrentDirectory(), "Lieder");

                if (!Directory.Exists(folderpath))
                {
                    return NotFound();
                }

                string[] audioFiles = Directory.GetFiles(folderpath, "*.mp3")
                .Where(file =>
                {
                    string fileName = Path.GetFileNameWithoutExtension(file);
                    return fileName.IndexOf(filetoget, StringComparison.OrdinalIgnoreCase) >= 0;
                })
                .ToArray();
                if (audioFiles.Length == 0)
                {
                    return NotFound("Audio file not found");
                }
                string filetostream = audioFiles[0];

                var fileStream = new FileStream(filetostream, FileMode.Open, FileAccess.Read, FileShare.Read);

                return File(fileStream, "audio/mpeg");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpPost]
        [Route("api/create_user")]
        public IActionResult CreateUser([FromBody] Nutzer neuerNutzer)
        {
            try
            {
                if (neuerNutzer == null)
                {
                    return BadRequest("Ungültige Nutzerdaten");
                }
                Nutzer nutzer = db.Nutzers.FirstOrDefault(x => x.Email == neuerNutzer.Email);
                if (nutzer == null)
                {
                    db.Nutzers.Add(neuerNutzer);
                    db.SaveChanges();
                    return Ok();
                }
                return BadRequest("Email bereits vergeben");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("api/update_user")]
        public IActionResult UpdateUser([FromBody] UpdateNutzerDTO updatenutzer)
        {
            try
            {
                Nutzer nutzer = db.Nutzers.FirstOrDefault(x => x.NutzerId == updatenutzer.NutzerId);
                if (nutzer == null)
                {
                    return NotFound();
                }

                var emailInUse = db.Nutzers.Any(x => x.Email == updatenutzer.Email && x.NutzerId != updatenutzer.NutzerId);
                if (emailInUse)
                {
                    return BadRequest("Email bereits vergeben.");
                }

                if (!string.IsNullOrEmpty(updatenutzer.OldPassword))
                {
                    if (nutzer.Passwort != updatenutzer.OldPassword)
                    {
                        return BadRequest("Passwort nicht korrekt!");
                    }
                    if (!string.IsNullOrEmpty(updatenutzer.NewPassword))
                    {
                        nutzer.Passwort = updatenutzer.NewPassword;
                    }
                }
                if (!string.IsNullOrEmpty(updatenutzer.Name))
                {
                    nutzer.Name = updatenutzer.Name;

                }
                if (!string.IsNullOrEmpty(updatenutzer.Vorname))
                {
                    nutzer.Vorname = updatenutzer.Vorname;

                }
                if (!string.IsNullOrEmpty(updatenutzer.Email))
                {
                    nutzer.Email = updatenutzer.Email;

                }
                db.SaveChanges();
                return Ok("Daten wurden erfolgreich geändert.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        public class UpdateNutzerDTO
        {
            public int NutzerId { get; set; }
            public string Name { get; set; }
            public string Vorname { get; set; }
            public string Email { get; set; }

            public string OldPassword { get; set; }
            public string NewPassword { get; set; }
        }
        [HttpPost]
        [Route("api/delete_user")]
        public IActionResult DeleteUser([FromBody] int id)
        {
            try
            {

                Nutzer delete = db.Nutzers.SingleOrDefault(x => x.NutzerId == id);
                if (delete == null)
                {
                    return NotFound();
                }
                db.Nutzers.Remove(delete);
                db.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("api/login")]
        public IActionResult Login([FromBody] LoginModel request)
        {
            try
            {


                Nutzer nutzerindb = db.Nutzers.SingleOrDefault(x => x.Email == request.Email);

                if (nutzerindb == null)
                {
                    return NotFound();
                }

                if (nutzerindb.Passwort == request.Passwort)
                {
                    return Ok(nutzerindb.NutzerId);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        public class LoginModel
        {
            public string? Email { get; set; }
            public string? Passwort { get; set; }
        }
        [HttpPost]
        [Route("api/add_favorite")]
        public IActionResult Favorite(int nutzerid, int liedid)
        {
            try
            {
                
                Nutzer nutzer = db.Nutzers.Include(x => x.Lieds).FirstOrDefault(x => x.NutzerId == nutzerid);

                Lieder lied = db.Lieders.Include(x => x.Nutzers).FirstOrDefault(x => x.Id == liedid);

                if (nutzer == null || lied == null)
                {
                    return NotFound();
                }

                if (!nutzer.Lieds.Contains(lied))
                {
                    nutzer.Lieds.Add(lied);
                }

                db.SaveChanges();
                return Ok("Favorit hinzugefügt");
            }
            
            catch (Exception ex)
            {
                Console.WriteLine("Exception Message: " + ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("api/del_favorite")]
        public IActionResult DelFavorite(int nutzerid, int liedid)
        {
            try
            {

                Nutzer nutzer = db.Nutzers.Include(x => x.Lieds).FirstOrDefault(x => x.NutzerId == nutzerid);

                Lieder lied = db.Lieders.Include(x => x.Nutzers).FirstOrDefault(x => x.Id == liedid);
                if (nutzer == null || lied == null)
                {
                    return NotFound();
                }
                if (nutzer.Lieds.Contains(lied))
                {
                    nutzer.Lieds.Remove(lied);

                }
               
                db.SaveChanges();
                return Ok("Favorit entfernt");


            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("api/is_favorite")]
        public IActionResult IsFavorite([FromQuery] int nutzerid, [FromQuery] int liedid)
        {
            try
            {
                bool isFavorite = false;
                Nutzer nutzer = db.Nutzers.Include(x => x.Lieds).FirstOrDefault(x => x.NutzerId == nutzerid);

                Lieder lied = db.Lieders.Include(x => x.Nutzers).FirstOrDefault(x => x.Id == liedid);

                if (nutzer == null || lied == null)
                {
                    return NotFound("User not found.");
                }
                if (nutzer.Lieds.Contains(lied))
                {
                    isFavorite = true;
                }
                return Ok(isFavorite);
              
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("api/get_favoritedb")]
        public IActionResult GetFavoritesInDb([FromQuery] int nutzerid)
        {
            try
            {
                Nutzer nutzer = db.Nutzers.Include(x => x.Lieds).ThenInclude(x=> x.Künstler).FirstOrDefault(x => x.NutzerId == nutzerid);

                if (nutzer == null)
                {
                    return NotFound();
                }
                var list = nutzer.Lieds.Select(x => new
                {
                    Titel = x.Titel,
                    Kuenstler = x.Künstler?.Name
                }).ToList();

                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        
        [HttpPost]
        [Route("api/create_playlist")]
        public IActionResult CreatePlaylist([FromBody] CreatePlaylistDTO playlist)
        {
            try
            {
                Nutzer nutzer = db.Nutzers.FirstOrDefault(x => x.NutzerId == playlist.NutzerID);
                if (nutzer == null)
                {
                    return NotFound("Nutzer nicht gefunden");
                }
               

                if (db.Playlists.Any(x=> x.Name == playlist.name && x.NutzerId == playlist.NutzerID))
                {
                    return BadRequest("Eine Playlist mit diesem Namen existiert bereits.");
                }
                Playlist neuePlaylist = new Playlist 
                {
                    Name = playlist.name,
                    NutzerId = playlist.NutzerID,
                };
                db.Playlists.Add(neuePlaylist);
                db.SaveChanges();
               
                return Ok("Playlist erstellt.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        public class CreatePlaylistDTO
        {
           public int NutzerID { get; set; }
            public string name { get; set; }
        }
       
        [HttpGet]
        [Route("api/get_playlists")]
        public IActionResult GetPlaylists([FromQuery] int nutzerid)
        {
            try
            {
                Nutzer nutzer = db.Nutzers.Include(x => x.Playlists).ThenInclude(x => x.Lieds).ThenInclude(x => x.Künstler).FirstOrDefault(x => x.NutzerId == nutzerid);

                if (nutzer == null)
                {
                    return NotFound("User not found");
                }

                var playlists = nutzer.Playlists.Select(x => new PlaylistDTO
                {
                    Id = x.Id,
                    Name = x.Name,
                    Lieder = x.Lieds.Select(x => new LiedDTO
                    {
                        Id = x.Id,
                        Titel = x.Titel,
                        Kuenstler = x.Künstler.Name
                    }).ToList()
                }).ToList();

                return Ok(playlists);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        public class PlaylistDTO
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public List<LiedDTO> Lieder { get; set; }
        }
        public class LiedDTO
        {
            public int Id { get; set; }
            public string Titel { get; set; }
            public string Kuenstler { get; set; }
        }
        [HttpPost]
        [Route("api/add_song_to_playlist")]
        public IActionResult AddSongToPlaylist([FromBody] ASTPRequest request)
        {
            try
            {
                

                Playlist playlist = db.Playlists.Include(x=>x.Lieds).FirstOrDefault(x => x.Id == request.PlaylistID);
                Lieder lied = db.Lieders.Include(x=> x.Playlists).FirstOrDefault(x => x.Id == request.LiedID);
                if(playlist == null || lied == null)
                {
                    return NotFound("Playlist oder Lied nicht gefunden");
                }
                if (playlist.Lieds.Contains(lied))
                {
                    return BadRequest("Lied bereits in Playlist");
                }
                playlist.Lieds.Add(lied);
                db.SaveChanges();
                return Ok("Lied wurde Playlist hinzugefügt");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        public class ASTPRequest
        {
            public int PlaylistID { get; set; }
            public int LiedID { get; set; }
        }


    }
}
