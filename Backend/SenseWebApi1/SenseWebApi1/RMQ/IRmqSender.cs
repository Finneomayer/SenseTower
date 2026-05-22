namespace SenseWebApi1.RMQ;

public interface IRmqSender
{
    Task SendDeleteEvent(Guid id);
}