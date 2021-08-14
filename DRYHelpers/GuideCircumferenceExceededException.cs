using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRYHelpers
{
    [Serializable]
    public class GuideCircumferenceExceededException : ApplicationException
    {
        public GuideCircumferenceExceededException()
        {
        }

        public GuideCircumferenceExceededException(string message) : base(message)
        {
        }

        public GuideCircumferenceExceededException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected GuideCircumferenceExceededException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
