namespace SenseWebApi1.RMQ;

public class RmqDto
{ 
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public Guid Id { get; set; }
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public RmqType Type { get; set; }
}
public enum RmqType
{
        ImageDeleteEvent=2,
        SpaceDeleteEvent=1
}