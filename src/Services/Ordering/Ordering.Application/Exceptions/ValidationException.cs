﻿using FluentValidation.Results;
using Ordering.Domain.Exceptions;

namespace Ordering.Application.Exceptions
{
    public class ValidationException : OrderingException
    {
        public IDictionary<string, string[]> Errors { get; }
        public ValidationException() : base("One or more validation error(s) occurred.")
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(IEnumerable<ValidationFailure> failures) : this()
        {
            Errors = failures
                .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                .ToDictionary(failure => failure.Key, failure => failure.ToArray());
        }
    }
}
