using System;

public class UserAddDeleteEventMediator
{
    public event Action<string> AddRequested;
    public event Action<Guid> DeleteRequested;

    public void RaiseAddRequested(string nickName)
    {
        AddRequested?.Invoke(nickName);
    }

    public void RaiseDeleteRequested(Guid guid)
    {
        DeleteRequested?.Invoke(guid);
    }
}
