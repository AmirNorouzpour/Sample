
namespace Common.Exception
{
    public class NotFoundException : AppException
    {
        public NotFoundException()
            : base(ApiResultStatusCode.NotFound)
        {
        }

        public NotFoundException(string message)
            : base(ApiResultStatusCode.NotFound, message)
        {
        }

        public NotFoundException(object additionalData)
            : base(ApiResultStatusCode.NotFound, additionalData)
        {
        }

        public NotFoundException(string message, object additionalData)
            : base(ApiResultStatusCode.NotFound, message, additionalData)
        {
        }

        public NotFoundException(string message, System.Exception exception)
            : base(ApiResultStatusCode.NotFound, message, exception)
        {
        }

        public NotFoundException(string message, System.Exception exception, object additionalData)
            : base(ApiResultStatusCode.NotFound, message, exception, additionalData)
        {
        }
    }
}
