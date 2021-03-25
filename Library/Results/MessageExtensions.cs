using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.Results
{

    public static class MessageExtensions
    {
        public static IEnumerable<string> ExtractMessages(this Exception ex)
        {
            var messages = new List<string>();
            while (ex != null)
            {
                messages.Add(ex.Message);
                ex = ex.InnerException;
            }
            return messages;
        }

        public static IEnumerable<string> ExtractMessages(this IEnumerable<ValidationFailure> validationFailures)
        {
            return validationFailures is null
                ? new string[] { }
                : validationFailures.Select(x => x.ErrorMessage);
        }
    }
}
