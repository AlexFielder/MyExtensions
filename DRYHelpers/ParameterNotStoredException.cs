using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRYHelpers
{
    [Serializable]
    public class ParameterNotStoredException : ApplicationException
    {
        private string parameterName;

        public string ParameterName
        {
            get
            {
                return this.parameterName;
            }
            set
            {
                this.parameterName = value;
            }
        }

        public ParameterNotStoredException()
        {

        }

        public ParameterNotStoredException(string message)
        : base(message)
        {

        }

        public ParameterNotStoredException(string message, Exception innerException) : base(message, innerException)
        {

        }

        protected ParameterNotStoredException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            base.GetObjectData(info, context);

            if (info != null)
            {
                this.parameterName = info.GetString("parameterName");
            }

        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            if (info != null)
            {
                info.AddValue("fUserName", this.parameterName);
            }
        }
    }
}
