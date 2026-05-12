using System.Collections.Generic;
using Assets.Scripts.Space;

namespace API.Models
{
    public class LocalSpacesResponse
    {
        public int id { get; set; }
        public List<LocalSpace> result;
        public string exception{ get; set; }
        public int status { get; set; }
        public bool isCanseled { get; set; }
        public bool isCompleted { get; set; }
        public bool isCompletedSuccessfully { get; set; }
    }
}