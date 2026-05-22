using FluentValidation;
using MediatR;
using SenseWebApi1.domain.Exceptions;

namespace SenseWebApi1.Common;

public class ValidatorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidatorBehavior(IEnumerable<IValidator<TRequest>> validators)
        => _validators = validators;



    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {

        // Invoke the validators
        var errors = await Task.WhenAll(_validators
            .Select(async p => await p.ValidateAsync(request, cancellationToken)));

        var errorsDictionary = errors.SelectMany(x => x.Errors)
            .Where(x => x != null)
            .GroupBy(
                x => x.PropertyName,
                x => x.ErrorMessage,
                (propertyName, errorMessages) => new
                {
                    Key = propertyName,
                    Values = errorMessages.Distinct().ToList()
                })
            .ToDictionary(x => x.Key, x => x.Values);




        if (errorsDictionary.Count > 0)
        {
            // Map the validation failures and throw an error,
            // this stops the execution of the request
                

            throw  new ExceptionsAdapter(errorsDictionary);
        }

        // Invoke the next handler
        // (can be another pipeline behavior or the request handler)
        return await next();
    }
}