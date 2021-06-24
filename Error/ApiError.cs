using System;
using System.Text.Json;

namespace RemittanceApp.Error
{
    public class ApiError
    {
        #region Properties
        public int ErrorCode { get; set; }
        public string Status { get; set; }
        public string ErrorDetail { get; set; }
        #endregion

        #region Constr
        public ApiError(int errocode, string errormsg, string errordetail = null)
        {
            this.ErrorCode = errocode;
            this.Status = errormsg;
            this.ErrorDetail = errordetail;
        }
        #endregion

        #region Override Methods
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
        #endregion
    }
}
