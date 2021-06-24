using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemittanceApp.Models
{
    public class ResponseMessage
    {
        #region Properties
        public string Status { get; set; }
        public int ErrorCode { get; set; }
        public Object Data { get; set; }
        #endregion

        #region Constr
        public ResponseMessage()
        {

        }

        public ResponseMessage(string status,int errorcode, object data)
        {
            this.Status = status;
            this.ErrorCode = errorcode;
            this.Data = data;
        }
        #endregion
    }
}
