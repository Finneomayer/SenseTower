using System;
using static Data.Enumenators.Server;

namespace API.Models
{
    [Serializable]
    public class Assembly
    {
        public Profile AssemblyType;
        //public ServerType ServerType;
        //public Port port;
    }
}