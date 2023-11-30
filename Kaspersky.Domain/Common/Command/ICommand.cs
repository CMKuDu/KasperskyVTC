using MediatR;

namespace Kaspersky.Domain.Common.Command
{
    public interface ICommand :IRequest
    {
    }
    public interface  ICommand<out TResult> : IRequest<TResult> { }
}
