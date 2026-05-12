using System.Collections.Generic;

namespace Models
{
    public class ScError
    {
        public string? Message { get; set; }
        public Dictionary<string, List<string>>? ModelState { get; set; }
    
    }
}