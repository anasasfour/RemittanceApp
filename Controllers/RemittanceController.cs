using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RemittanceApp.Models;
using RemittanceApp.Models.Extensions;
using RemittanceApp.Models.Models.LoggingModels;
using RemittanceApp.Models.Models.Remittance;
using RemittanceApp.Services.Interfaces;
using RemittanceApp.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace RemittanceApp.Controllers
{
    [Authorize]
    public class RemittanceController : BaseController
    {
        #region Properties
        private RemittanceLog logModelRemittance;
        private RemittanceApproveLog logModelRemittanceApprov;
        private InwardIBRPaymentLog logModelInwardIBRPayment;
        private TransactionAmmendmentLog logModelTransactionAmmendment;
        private TransactionCancellationLog logModelTransactionCancellation;
        private RecordWUTxnLog logModelRecordWUTxn;
        private ResponseMessage response;
        private readonly ILogger<RemittanceController> logger;
        private readonly IRemittanceService remittanceService;
        #endregion

        #region Constr
        public RemittanceController(ILogger<RemittanceController> _logger, IRemittanceService _remittanceService)
        {
            logger = _logger;
            remittanceService = _remittanceService;
        }
        #endregion

        #region API
        [HttpPost("Remittance")]
        public async Task<IActionResult> Remittance([FromBody] Remittance model)
        {
            logModelRemittance = ConvertToLoggingModelRemittance(model);
            if (ModelState.IsValid && APIValidator.ValidateChannelId(model.channelId))
            {
                response = new ResponseMessage(HttpStatusCode.OK.ToString(), (int)HttpStatusCode.OK, new { RemittanceRefNumber = "" });
                logModelRemittance.Response = JsonConvert.SerializeObject(response);
                var res = await remittanceService.InsertRemittanceLog(logModelRemittance);

                return Ok(response);
            }
            else
            {
                var message = string.Join(" | ", ModelState.Values
                                    .SelectMany(v => v.Errors)
                                    .Select(e => e.ErrorMessage));

                response = new ResponseMessage(HttpStatusCode.BadRequest.ToString(), (int)HttpStatusCode.BadRequest, message);
                logModelRemittance.Response = JsonConvert.SerializeObject(response);
                var res = await remittanceService.InsertRemittanceLog(logModelRemittance);

                return BadRequest(response);
            }
        }

        [HttpPost("RemittanceApprove")]
        public async Task<IActionResult> RemittanceApprove([FromBody] RemittanceApprove model)
        {
            logModelRemittanceApprov = ConvertToLoggingModelRemittanceApprove(model);
            if (ModelState.IsValid && APIValidator.ValidateChannelId(model.channelID))
            {
                response = new ResponseMessage(HttpStatusCode.OK.ToString(), (int)HttpStatusCode.OK, null);
                logModelRemittanceApprov.Response = JsonConvert.SerializeObject(response);
                var res = await remittanceService.InsertRemittanceApproveLog(logModelRemittanceApprov);

                return Ok(response);
            }
            else
            {
                var message = string.Join(" | ", ModelState.Values
                                    .SelectMany(v => v.Errors)
                                    .Select(e => e.ErrorMessage));

                response = new ResponseMessage(HttpStatusCode.BadRequest.ToString(), (int)HttpStatusCode.BadRequest, message);
                logModelRemittanceApprov.Response = JsonConvert.SerializeObject(response);
                var res = await remittanceService.InsertRemittanceApproveLog(logModelRemittanceApprov);
                return BadRequest(response);
            }
        }

        [HttpGet("GetRemittanceDetails")]
        public IActionResult GetRemittanceDetails(string userId, string password, string channelId, string profileNumber, string lariCardNumber)
        {
            logger.LogInformation("Request - RemittanceController -> GetRemittanceDetails: " + JsonConvert.SerializeObject(new { userId = userId, password = password, channelId = channelId, profileNumber = profileNumber, lariCardNumber = lariCardNumber }));
            if (APIValidator.ValidateChannelId(channelId) && int.TryParse(profileNumber,out int n))
            {
                response = new ResponseMessage(HttpStatusCode.OK.ToString(), (int)HttpStatusCode.OK, new RemittanceDetailsResponse());
                logger.LogInformation("Response - RemittanceController -> GetRemittanceDetails: " + JsonConvert.SerializeObject(response));

                return Ok(response);
            }
            else
            {
                response = new ResponseMessage(HttpStatusCode.BadRequest.ToString(), (int)HttpStatusCode.BadRequest, "Invalid channel Id OR Invalid Profile Number");
                logger.LogInformation("Response - RemittanceController -> GetRemittanceDetails: " + JsonConvert.SerializeObject(response));
                return BadRequest(response);
            }
        }

        [HttpGet("InwardIBRInquiry")]
        public IActionResult InwardIBRInquiry(string userId, string password, string channelId, string InwardTxnRef, double amount, string sendername, string OTP)
        {
            logger.LogInformation("Request - RemittanceController -> InwardIBRInquiry: " + JsonConvert.SerializeObject(new { userId = userId, password = password, channelId = channelId, InwardTxnRef = InwardTxnRef, amount = amount, sendername= sendername, OTP = OTP }));
            if (APIValidator.ValidateChannelId(channelId))
            {
                response = new ResponseMessage(HttpStatusCode.OK.ToString(), (int)HttpStatusCode.OK, new { linkRefNo = "" });
                logger.LogInformation("Response - RemittanceController -> InwardIBRInquiry: " + JsonConvert.SerializeObject(response));

                return Ok(response);
            }
            else
            {
                response = new ResponseMessage(HttpStatusCode.BadRequest.ToString(), (int)HttpStatusCode.BadRequest, "Invalid channel Id.");
                logger.LogInformation("Response - RemittanceController -> InwardIBRInquiry: " + JsonConvert.SerializeObject(response));
                return BadRequest(response);
            }
        }

        [HttpPost("InwardIBRPayment")]
        public async Task<IActionResult> InwardIBRPayment([FromBody] InwardIBRPayment model)
        {
            logModelInwardIBRPayment = ConvertToLoggingModelInwardIBRPayment(model);
            if (APIValidator.ValidateChannelId(model.ChannelID))
            {
                response = new ResponseMessage(HttpStatusCode.OK.ToString(), (int)HttpStatusCode.OK, new { creditRefno = "" });
                logModelInwardIBRPayment.Response = JsonConvert.SerializeObject(response);
                var res = await remittanceService.InsertInwardIBRPaymentLog(logModelInwardIBRPayment);
                return Ok(response);
            }
            else
            {
                response = new ResponseMessage(HttpStatusCode.BadRequest.ToString(), (int)HttpStatusCode.BadRequest, "Invalid Channel Id.");
                logModelInwardIBRPayment.Response = JsonConvert.SerializeObject(response);
                var res = await remittanceService.InsertInwardIBRPaymentLog(logModelInwardIBRPayment);
                return BadRequest(response);
            }
        }

        [HttpPost("TransactionAmmendment")]
        public async Task<IActionResult> TransactionAmmendment([FromBody] TransactionAmmendment model)
        {
            logModelTransactionAmmendment = ConvertToLoggingModelTransactionAmmendment(model);
            if (APIValidator.ValidateChannelId(model.ChannelId))
            {
                response = new ResponseMessage(HttpStatusCode.OK.ToString(), (int)HttpStatusCode.OK, new
                {
                    AmendRef = "",
                    AmendStatus = ""
                });
                logModelTransactionAmmendment.Response = JsonConvert.SerializeObject(response);
                var res = await remittanceService.InsertTransactionAmmendmentLog(logModelTransactionAmmendment);
                return Ok(response);
            }
            else
            {
                var response = new ResponseMessage(HttpStatusCode.BadRequest.ToString(), (int)HttpStatusCode.BadRequest, "Invalid channel Id.");
                logModelTransactionAmmendment.Response = JsonConvert.SerializeObject(response);
                var res = await remittanceService.InsertTransactionAmmendmentLog(logModelTransactionAmmendment);
                return BadRequest(response);
            }
        }

        [HttpPost("TransactionCancellation")]
        public async Task<IActionResult> TransactionCancellation([FromBody] TransactionCancellation model)
        {
            logModelTransactionCancellation = ConvertToLoggingModelTransactionCancellation(model);
            if (APIValidator.ValidateChannelId(model.ChannelId))
            {
                response = new ResponseMessage(HttpStatusCode.OK.ToString(), (int)HttpStatusCode.OK, new {
                    CancelRef = "",
                    CancelStatus = ""
                });
                logModelTransactionCancellation.Response = JsonConvert.SerializeObject(response);
                var res = await remittanceService.InsertTransactionCancellationLog(logModelTransactionCancellation);
                return Ok(response);
            }
            else
            {
                response = new ResponseMessage(HttpStatusCode.BadRequest.ToString(), (int)HttpStatusCode.BadRequest, "Invalid Channel Id.");
                logModelTransactionCancellation.Response = JsonConvert.SerializeObject(response);
                var res = await remittanceService.InsertTransactionCancellationLog(logModelTransactionCancellation);
                return BadRequest(response);
            }
        }

        [HttpPost("RecordWUTxn")]
        public async Task<IActionResult> RecordWUTxn([FromBody] RecordWUTxn model)
        {
            logModelRecordWUTxn = ConvertToLoggingModelTransactionCancellation(model);
            if (APIValidator.ValidateChannelId(model.ChannelId))
            {
                response = new ResponseMessage(HttpStatusCode.OK.ToString(), (int)HttpStatusCode.OK, null);
                logModelRecordWUTxn.Response = JsonConvert.SerializeObject(response);
                var res = await remittanceService.InsertRecordWUTxnLog(logModelRecordWUTxn);
                return Ok(response);
            }
            else
            {
                response = new ResponseMessage(HttpStatusCode.BadRequest.ToString(), (int)HttpStatusCode.BadRequest, "Invalid Channel Id.");
                logModelRecordWUTxn.Response = JsonConvert.SerializeObject(response);
                var res = await remittanceService.InsertRecordWUTxnLog(logModelRecordWUTxn);
                return BadRequest(response);
            }
        }
        #endregion

        #region Private Methods
        private RemittanceLog ConvertToLoggingModelRemittance(Remittance model)
        {
            RemittanceLog _model = new RemittanceLog();
            _model.UserId = model.userId;
            _model.ChannelId = model.channelId;
            _model.ProfileNumber = model.profileNumber;
            _model.FiscalYear = DateTime.Now.GetFinancialYear();
            _model.LogDate = DateTime.Now;
            _model.LogTime = DateTime.Now;
            _model.Request = JsonConvert.SerializeObject(model);

            return _model;
        }

        private RemittanceApproveLog ConvertToLoggingModelRemittanceApprove(RemittanceApprove model)
        {
            RemittanceApproveLog _model = new RemittanceApproveLog();
            _model.UserId = model.userId;
            _model.ChannelId = model.channelID;
            _model.Ref_No = model.remittanceRefNumber;
            _model.FiscalYear = DateTime.Now.GetFinancialYear();
            _model.LogDate = DateTime.Now;
            _model.LogTime = DateTime.Now;
            _model.Request = JsonConvert.SerializeObject(model);

            return _model;
        }

        private InwardIBRPaymentLog ConvertToLoggingModelInwardIBRPayment(InwardIBRPayment model)
        {
            InwardIBRPaymentLog _model = new InwardIBRPaymentLog();
            _model.UserId = model.userID;
            _model.ChannelId = model.ChannelID;
            _model.Ref_No = model.linkRefNo;
            _model.FiscalYear = DateTime.Now.GetFinancialYear();
            _model.LogDate = DateTime.Now;
            _model.LogTime = DateTime.Now;
            _model.Request = JsonConvert.SerializeObject(model);

            return _model;
        }

        private TransactionAmmendmentLog ConvertToLoggingModelTransactionAmmendment(TransactionAmmendment model)
        {
            TransactionAmmendmentLog _model = new TransactionAmmendmentLog();
            _model.UserId = model.userId;
            _model.ChannelId = model.ChannelId;
            _model.Ref_No = model.TxnRefNo;
            _model.FiscalYear = DateTime.Now.GetFinancialYear();
            _model.LogDate = DateTime.Now;
            _model.LogTime = DateTime.Now;
            _model.Request = JsonConvert.SerializeObject(model);

            return _model;
        }

        private TransactionCancellationLog ConvertToLoggingModelTransactionCancellation(TransactionCancellation model)
        {
            TransactionCancellationLog _model = new TransactionCancellationLog();
            _model.UserId = model.userId;
            _model.ChannelId = model.ChannelId;
            _model.Ref_No = model.TxnRefNo;
            _model.FiscalYear = DateTime.Now.GetFinancialYear();
            _model.LogDate = DateTime.Now;
            _model.LogTime = DateTime.Now;
            _model.Request = JsonConvert.SerializeObject(model);

            return _model;
        }

        private RecordWUTxnLog ConvertToLoggingModelTransactionCancellation(RecordWUTxn model)
        {
            RecordWUTxnLog _model = new RecordWUTxnLog();
            _model.UserId = model.userId;
            _model.ChannelId = model.ChannelId;
            _model.Ref_No = model.WURefNumber;
            _model.FiscalYear = DateTime.Now.GetFinancialYear();
            _model.LogDate = DateTime.Now;
            _model.LogTime = DateTime.Now;
            _model.Request = JsonConvert.SerializeObject(model);

            return _model;
        }
        #endregion 
    }
}
