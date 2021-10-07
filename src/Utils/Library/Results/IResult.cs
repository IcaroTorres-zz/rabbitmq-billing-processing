using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Library.Results
{
    /// <summary>
    /// Abstraction for any execution result with methods to check status, success and
    /// the presence of errors instead of throwing exceptions
    /// </summary>
    public interface IResult : IActionResult
    {
        bool IsSuccess();
        int GetStatus();
        object GetData();
        object Value { get; }
        IReadOnlyList<string> Errors { get; }
    }
}
