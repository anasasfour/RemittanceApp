using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RemittanceApp.Models;
using RemittanceApp.Models.Extensions;
using RemittanceApp.Models.Models.LoggingModels;
using RemittanceApp.Models.StandingInstruction.Card;
using RemittanceApp.Services.Interfaces;
using RemittanceApp.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace RemittanceApp.Controllers
{
    public class StandingInstructionController : BaseController
    {
        #region Properties
        private SICreateLog logModel;
        private ResponseMessage response;
        private readonly ILogger<StandingInstructionController> logger;
        private readonly IStandingInstructionService standingInstructionService;
        #endregion

        #region Constr
        public StandingInstructionController(ILogger<StandingInstructionController> _logger, IStandingInstructionService _standingInstructionService)
        {
            logger = _logger;
            standingInstructionService = _standingInstructionService;
        }
        #endregion

        #region API Methods
        [HttpPost("SICreate")]
        public async Task<IActionResult> SICreate([FromBody] SICreate model)
        {
            logModel = ConvertToLoggingModel(model);
            if (APIValidator.ValidateChannelId(model.channelId))
            {
                response = new ResponseMessage(HttpStatusCode.OK.ToString(), (int)HttpStatusCode.OK, new { SIRefNumber = "" });

                logModel.Response = JsonConvert.SerializeObject(response);
                var res = await standingInstructionService.InsertSICreateLog(logModel);

                return Ok(response);
            }
            else
            {
                response = new ResponseMessage(HttpStatusCode.BadRequest.ToString(), (int)HttpStatusCode.BadRequest, "Invalid channel Id");

                logModel.Response = JsonConvert.SerializeObject(response);
                var res = await standingInstructionService.InsertSICreateLog(logModel);
                return BadRequest(response);
            }
        }
        #endregion

        #region Private Methods
        private SICreateLog ConvertToLoggingModel(SICreate model)
        {
            SICreateLog _model = new SICreateLog();
            _model.UserId = model.userId;
            _model.ChannelId = model.channelId;
            _model.ProfileNumber = model.profileNumber;
            _model.Ref_No = model.SIBenRef;
            _model.FiscalYear = DateTime.Now.GetFinancialYear();
            _model.LogDate = DateTime.Now;
            _model.LogTime = DateTime.Now;
            _model.Request = JsonConvert.SerializeObject(model);

            return _model;
        }
        #endregion
    }
}
