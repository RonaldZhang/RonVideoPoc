using System.Net;

namespace RonVideo.Models
{
    public class GetVideoResult
    {
        public GetVideoResult(HttpStatusCode hs,byte[] bs)
        {
            HttpStatus = hs;
            bytes = bs;
        }
        public HttpStatusCode  HttpStatus { get; set; }
        public byte[] bytes { get; set; }
    }
}
