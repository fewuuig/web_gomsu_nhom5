using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace QuanLyBanDoGomSu.Exception
{
    /// <summary>
    /// Global Exception Filter tự động bắt mọi Exception và bọc về form ApiResponse<object>
    /// </summary>
    public class GlobalExceptionHandlerFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            int statusCode = (int)HttpStatusCode.InternalServerError;
            string message = "Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau!";
            string errorDetails = context.Exception.Message;

            if (context.Exception is NotFoundException || context.Exception is KeyNotFoundException)
            {
                statusCode = (int)HttpStatusCode.NotFound;
                message = context.Exception.Message;
            }
            else if (context.Exception is ValidationCustomException || context.Exception is ArgumentException || context.Exception is ArgumentNullException)
            {
                statusCode = (int)HttpStatusCode.BadRequest;
                message = context.Exception.Message;
            }
            else if (context.Exception is UnauthorizedCustomException)
            {
                statusCode = (int)HttpStatusCode.Unauthorized;
                message = context.Exception.Message;
            }
            else if (context.Exception is BusinessRuleException)
            {
                statusCode = (int)HttpStatusCode.UnprocessableEntity;
                message = context.Exception.Message;
            }

            var response = ApiResponse<object>.ErrorResponse(statusCode, message, errorDetails);

            context.Result = new ObjectResult(response)
            {
                StatusCode = statusCode
            };
            context.ExceptionHandled = true;
        }
    }
}
