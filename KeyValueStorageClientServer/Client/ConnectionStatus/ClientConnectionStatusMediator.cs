using Common.ConnectionStatusUtils;

namespace Client.ConnectionStatus
{
    // Client Connection Status Mediator Singleton
    // Generics: https://docs.microsoft.com/en-us/archive/blogs/kirillosenkov/how-i-started-to-really-understand-generics
    public sealed class ClientConnectionStatusMediator : ConnectionStatusMediator<ClientConnectionStatusMediator> { }
}