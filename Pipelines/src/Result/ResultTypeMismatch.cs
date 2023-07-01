using System.Runtime.Serialization;

namespace Pipelines
{
    [Serializable]
    internal class ResultTypeMismatch : Exception
    {
        public ResultTypeMismatch()
        {
        }
        
        public ResultTypeMismatch(Type expected, Type actual) : base($"Expected type {expected.Name} but received {actual.Name}")
        {            
        }


        public ResultTypeMismatch(string? message) : base(message)
        {
        }

        public ResultTypeMismatch(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected ResultTypeMismatch(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}