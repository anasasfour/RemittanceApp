using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RemittanceApp.Models;
using RemittanceApp.Models.Extensions;
using RemittanceApp.Models.Models.Beneficiary;
using RemittanceApp.Models.Models.LoggingModels;
using RemittanceApp.Services.Interfaces;
using RemittanceApp.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace RemittanceApp.Controllers
{
    public class BeneficiaryController : BaseController
    {
        #region Properties
        private BeneficiaryLog logModelBeneficiary;
        private ResponseMessage response;
        private readonly ILogger<BeneficiaryController> logger;
        private readonly IBeneficiaryService beneficiaryService;
        #endregion

        #region Constr
        public BeneficiaryController(ILogger<BeneficiaryController> _logger, IBeneficiaryService _beneficiaryService)
        {
            logger = _logger;
            beneficiaryService  = _beneficiaryService;
        }
        #endregion

        #region API Methods
        [HttpGet]
        public IActionResult Get(string userId, string password, string channelID)
        {
            logger.LogInformation("Request - BeneficiaryController -> Get: " + JsonConvert.SerializeObject(new { UserId = userId, Password = password, ChannelID = channelID }));
            if (APIValidator.ValidateChannelId(channelID))
            {
                response = new ResponseMessage(HttpStatusCode.OK.ToString(), (int)HttpStatusCode.OK, new BeneficiaryList());
                logger.LogInformation("Response - BeneficiaryController -> Get: " + JsonConvert.SerializeObject(response));
                return Ok(response);
            }
            else
            {
                var response = new ResponseMessage(HttpStatusCode.BadRequest.ToString(), (int)HttpStatusCode.BadRequest, JsonConvert.SerializeObject(new { UserId = userId, Password = password, ChannelID = channelID }));
                logger.LogInformation("Response - BeneficiaryController -> Get: " + JsonConvert.SerializeObject(response));
                return BadRequest(response);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Beneficiary model)
        {
            logModelBeneficiary = ConvertToLoggingModelBeneficiary(model);
            if (APIValidator.ValidateChannelId(model.ChannelID))
            {
                response = new ResponseMessage(HttpStatusCode.OK.ToString(), (int)HttpStatusCode.OK, new {
                    BeneficiaryID = "",
                    BeneficiarySubID = ""
                });
                logModelBeneficiary.Response = JsonConvert.SerializeObject(response);
                var res = await beneficiaryService.InsertBeneficiaryLog(logModelBeneficiary);
                return Ok(response);
            }
            else
            {
                var response = new ResponseMessage(HttpStatusCode.BadRequest.ToString(), (int)HttpStatusCode.BadRequest, "Invalid channel Id.");
                logModelBeneficiary.Response = JsonConvert.SerializeObject(response);
                var res = await beneficiaryService.InsertBeneficiaryLog(logModelBeneficiary);
                return BadRequest(response);
            }
        }

        #endregion

        #region Private Methods
        private BeneficiaryLog ConvertToLoggingModelBeneficiary(Beneficiary model)
        {
            BeneficiaryLog _model = new BeneficiaryLog();
            _model.UserId = model.userID;
            _model.ChannelId = model.ChannelID;
            _model.Ref_No = model.Releationship;
            _model.FiscalYear = DateTime.Now.GetFinancialYear();
            _model.LogDate = DateTime.Now;
            _model.LogTime = DateTime.Now;
            _model.Request = JsonConvert.SerializeObject(model);

            return _model;
        }
        #endregion
    }
}
