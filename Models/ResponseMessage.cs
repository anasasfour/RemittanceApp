using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemittanceApp.Models
{
    public class ResponseMessage
    {
        #region Properties
      
        public int ErrorCode { get; set; }
        public string ErrorDesc { get; set; }
        public Object Data { get; set; }
        #endregion

        #region Constr
        public ResponseMessage()
        {

        }

        public ResponseMessage(string ErrorDesc, int errorcode, object data)
        {
           
            this.ErrorCode = errorcode;
            this.ErrorDesc = ErrorDesc;
            this.Data = data;
        }
        #endregion
    }
}
