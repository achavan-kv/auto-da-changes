using FluentValidation;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unicomer.Cosacs.Model.Exceptions;
using Unicomer.Cosacs.Model.Interfaces;

namespace Unicomer.Cosacs.Business.Behaviors
{
    public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) { _validators = validators; }
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (!_validators.Any())
            {
                return await next();
            }
            var context = new ValidationContext<TRequest>(request);
            var errorsDictionary = _validators
                .Select(x => x.Validate(context))
                .SelectMany(x => x.Errors)
                .Where(x => x != null)
                .GroupBy(
                    x => x.PropertyName,
                    x => x.ErrorMessage,
                    (propertyName, errorMessages) => new
                    {
                        Key = propertyName,
                        Values = errorMessages.Distinct().ToArray()
                    })
                .ToDictionary(x => x.Key, x => x.Values);
            if (errorsDictionary.Any())
            {
                string id = string.Empty;

                if (request is IApiRequest)
                {
                    id = (request as IApiRequest).GetRequest().RequestId;
                }

                var errors = errorsDictionary.SelectMany(t => t.Value.Select(e => new ErrorMessage { Id = id, Detail = e })).ToArray();
                throw new Model.Exceptions.ValidationException(errors, new { request });
            }
            return await next();
        }
    }
}
