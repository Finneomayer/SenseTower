using System;

public enum ClientType
{
    VrClient = 1,
    WinClient = 2
}

public class ClientInSpaceData
{
    public Guid ClientId { get; set; }
    public ClientType ClientType { get; set; }
}


public class RegisterUsersInSpaceData
{
    public Guid SpaceId { get; set; }
    public ClientInSpaceData[] ClientsData { get; set; }
}
