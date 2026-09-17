using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Api.Validation
{
	public sealed class ValidationFilter<T> : IEndpointFilter
	{
		public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
		{
			var argument = context.Arguments
				.OfType<T>()
				.FirstOrDefault();

			if (argument is null)
				return await next(context);

			var validationContext = new ValidationContext(argument);
			var validationResults = new List<ValidationResult>();

			var isValid = Validator.TryValidateObject(
				argument,
				validationContext,
				validationResults,
				validateAllProperties: true
			);

			if (isValid)
				return await next(context);

			var errors = validationResults
				.SelectMany(
					result => result.MemberNames.DefaultIfEmpty(string.Empty),
					(result, memberName) => new
					{
						MemberName = memberName,
						ErrorMessage = result.ErrorMessage!
					}
				)
				.GroupBy(x => x.MemberName)
				.ToDictionary(
					group => group.Key,
					group => group.Select(x => x.ErrorMessage).ToArray()
				);

			return Results.ValidationProblem(errors);
 		}
	}
}
