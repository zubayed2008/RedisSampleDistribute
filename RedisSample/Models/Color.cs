using System.ComponentModel.DataAnnotations;

namespace RedisSample.Models
{
    public class Color
    {
        public long Id { get; set; }
        public string? ColorName { get; set; }
        public bool UpToDate { get; set; }
    }
}
