using System;

namespace Keebee.AAT.ApiClient.Models
{
    public class Thumbnail
    {
        public Guid StreamId { get; set; }
        public byte[] Image { get; set; }
    }
}
