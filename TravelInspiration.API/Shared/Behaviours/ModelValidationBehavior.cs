using FluentValidation;
using MediatR;

namespace TravelInspiration.API.Shared.Behaviours
{
    public class ModelValidationBehavior<TRequest, IResult> : IPipelineBehavior<TRequest, IResult> where TRequest : IRequest<IResult>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ModelValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<IResult> Handle(TRequest request, RequestHandlerDelegate<IResult> next, CancellationToken cancellationToken)
        {
            if (!_validators.Any())
                return await next();

            var context = new ValidationContext<TRequest>(request);
            var validationResults = _validators.Select(v => v.Validate(context)).ToList();
            var groupedValidationFailures = validationResults
                .SelectMany(vr => vr.Errors)
                .GroupBy(vf => vf.PropertyName)
                .Select(g => new
                {
                    PropertyName = g.Key,
                    ValidationFailure = g.Select(v => new { v.ErrorMessage })
                }).ToList();

            if (groupedValidationFailures.Count != 0)
            {
                var validationProblemsDicitionary = new Dictionary<string, string[]>();
                foreach (var group in groupedValidationFailures)
                {
                    var errorMessage = group.ValidationFailure.Select(vf => vf.ErrorMessage);
                    validationProblemsDicitionary.Add(group.PropertyName, errorMessage.ToArray());
                }
                return (IResult)Results.ValidationProblem(validationProblemsDicitionary);
            }

            return await next();
        }
    }
}   
