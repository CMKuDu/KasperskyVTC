using MediatR;

namespace Kaspersky.Domain.Common.Queries
{
    public interface IQuery<out TResult> : IRequest<TResult>
    {
    }
}
