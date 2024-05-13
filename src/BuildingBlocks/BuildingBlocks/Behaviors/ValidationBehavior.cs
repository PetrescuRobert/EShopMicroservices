
using BuildingBlocks.CQRS;
using FluentValidation;
using MediatR;

namespace BuildingBlocks.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommand<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TRequest>(request);

        var validationResult = await Task.WhenAll(validators.Select(_ => _.ValidateAsync(context, cancellationToken)));

        var errors = validationResult.Where(result => result.Errors.Any()).SelectMany(result => result.Errors).ToList();

        if (errors.Count() != 0)
        {
            throw new ValidationException(errors);
        }

        return await next();
    }
}

