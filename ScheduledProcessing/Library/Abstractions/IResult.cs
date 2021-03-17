using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Library.Abstractions
{
    public interface IResult : IActionResult
    {
        bool IsSuccess();
        int GetStatus();
        object GetData();
        object Value { get; }
        IReadOnlyList<string> Errors { get; }
    }
}
