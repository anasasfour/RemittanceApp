using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RemittanceApp.Models;
using RemittanceApp.Models.Extensions;
using RemittanceApp.Models.Models.LoggingModels;
using RemittanceApp.Models.Models.OTP;
using RemittanceApp.Services.Interfaces;
using RemittanceApp.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace RemittanceApp.Controllers
{
    public class OTPController : BaseController
    {
        #region Properties
        private OTPSendLog logModelOTPSend;
        private OTPValidateLog logModelOTPValidate;
        private readonly IAuthService authService;
        private ResponseMessage response;
        private readonly ILogger<OTPController> logger;
        private readonly IOTPService oTPService;
        #endregion

        #region Constr
        public OTPController(IAuthService auth, ILogger<OTPController> _logger, IOTPService _oTPService)
        {
            authService = auth;
            logger = _logger;
            oTPService = _oTPService;
        }
        #endregion

        #region API Methods
        [HttpPost("OTPSend")]
        public async Task<IActionResult> OTPSend([FromBody] OTPSend model)
        {
            logModelOTPSend = ConvertToLoggingModelOTPSend(model);
            if (APIValidator.ValidateChannelId(model.channelId))
            {
                response = new ResponseMessage(HttpStatusCode.OK.ToString(), (int)HttpStatusCode.OK, new { Countries = "" });

                logModelOTPSend.Response = JsonConvert.SerializeObject(response);
                var res = await oTPService.InsertOTPSendLog(logModelOTPSend);

                return Ok(response);
            }
            else
            {
                response = new ResponseMessage(HttpStatusCode.BadRequest.ToString(), (int)HttpStatusCode.BadRequest, "Invalid Channel Id");
                logModelOTPSend.Response = JsonConvert.SerializeObject(response);
                var res = await oTPService.InsertOTPSendLog(logModelOTPSend);
                return BadRequest(response);
            }
        }

        [HttpPost("OTPValidate")]
        public async Task<IActionResult> OTPValidate([FromBody] OTPValidate model)
        {
            logModelOTPValidate = ConvertToLoggingModelOTPValidate(model);
            if (APIValidator.ValidateChannelId(model.channelId))
            {
                OTPValidateResponse _response = new OTPValidateResponse();
                _response.JWTToken = this.authService.CreateJWT(model);

                response = new ResponseMessage(HttpStatusCode.OK.ToString(), (int)HttpStatusCode.OK, _response);

                logModelOTPValidate.Response = JsonConvert.SerializeObject(response);
                var res = await oTPService.InsertOTPValidateLog(logModelOTPValidate);

                return Ok(response);
            }
            else
            {
                response = new ResponseMessage(HttpStatusCode.BadRequest.ToString(), (int)HttpStatusCode.BadRequest, "Invalid Channel Id");

                logModelOTPValidate.Response = JsonConvert.SerializeObject(response);
                var res = await oTPService.InsertOTPValidateLog(logModelOTPValidate);
                return BadRequest(response);
            }
        }
        #endregion

        #region Private Methods
        private OTPSendLog ConvertToLoggingModelOTPSend(OTPSend model)
        {
            OTPSendLog _model = new OTPSendLog();
            _model.UserId = model.userId;
            _model.ChannelId = model.channelId;
            _model.ProfileNumber = model.profileNumber;
            _model.FiscalYear = DateTime.Now.GetFinancialYear();
            _model.LogDate = DateTime.Now;
            _model.LogTime = DateTime.Now;
            _model.Request = JsonConvert.SerializeObject(model);

            return _model;
        }

        private OTPValidateLog ConvertToLoggingModelOTPValidate(OTPValidate model)
        {
            OTPValidateLog _model = new OTPValidateLog();
            _model.UserId = model.userId;
            _model.ChannelId = model.channelId;
            _model.ProfileNumber = model.profileNumber;
            _model.FiscalYear = DateTime.Now.GetFinancialYear();
            _model.LogDate = DateTime.Now;
            _model.LogTime = DateTime.Now;
            _model.Request = JsonConvert.SerializeObject(model);

            return _model;
        }
        #endregion
    }
}
