using ToastNotification.Abstractions;

namespace ToastNotification.Containers
{
    public interface IMessageContainerFactory
    {
        IToastNotificationContainer<TMessage> Create<TMessage>() where TMessage : class;
    }
}
