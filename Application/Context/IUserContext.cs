    namespace Application.Context;

    public interface IUserContext
    {
        string UserIdentifier { get; }
        string RequestPath { get; }
        string RequestMethod { get; }
    }