using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using System.Net.Http;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Net.Http.Headers;

namespace TestStringParameter.Controllers
{
    public class MusicController : ApiController
    {
        [HttpGet]
        [Route("api/music/")]
        public HttpResponseMessage Stream([FromUri] string filename)
        {
            string filePath = HttpContext.Current.Server.MapPath(filename);

            if (!File.Exists(filePath))
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

            HttpResponseMessage response =
                Request.CreateResponse(HttpStatusCode.PartialContent);

            if (Request.Headers.Range != null)
            {
                response.Content = new ByteRangeStreamContent(stream,
                    Request.Headers.Range,
                    "audio/mpeg"
                    );
            }
            else
            {
                RangeHeaderValue rhv = new RangeHeaderValue();
                rhv.Ranges.Add(new RangeItemHeaderValue(0, 1));

                response.Content
                    = new ByteRangeStreamContent(stream, rhv, "audio/mpeg");
            }

            return response;

        }
    }
}
