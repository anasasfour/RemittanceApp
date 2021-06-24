using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RemittanceApp.Models;
using RemittanceApp.Models.Extensions;
using RemittanceApp.Models.Models.Forex;
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
    public class ForexController : BaseController
    {
        #region Properties
        private FCRequestLog logModel;
        private ResponseMessage response;
        private readonly ILogger<ForexController> logger;
        private readonly IForexService forexService;
        #endregion

        #region Constr
        public ForexController(ILogger<ForexController> _logger, IForexService _forexService)
        {
            logger = _logger;
            forexService = _forexService;
        }
        #endregion

        #region API Methods
        [HttpPost("FCRequest")]
        public async Task<IActionResult> FCRequest([FromBody] FCRequest model)
        {
            logModel = ConvertToLoggingModel(model);
            if (APIValidator.ValidateChannelId(model.channelId))
            {
                response = new ResponseMessage(HttpStatusCode.OK.ToString(), (int)HttpStatusCode.OK, new { FCRefNumber = "" });

                logModel.Response = JsonConvert.SerializeObject(response);
                var res = await forexService.InsertFCRequestLog(logModel);

                return Ok(response);
            }
            else
            {
                response = new ResponseMessage(HttpStatusCode.BadRequest.ToString(), (int)HttpStatusCode.BadRequest, "Invalid channedl Id");
                logModel.Response = JsonConvert.SerializeObject(response);
                var res = await forexService.InsertFCRequestLog(logModel);

                return BadRequest(response);
            }
        }

        [HttpGet("ForexRate")]
        public IActionResult ForexRate(string userId, string password, string channelId)
        {
            logger.LogInformation("Request - ForexController -> ForexRate: " + JsonConvert.SerializeObject(new { userId = userId, password = password, channelId = channelId}));
            if (APIValidator.ValidateChannelId(channelId))
            {
                response = new ResponseMessage(HttpStatusCode.OK.ToString(), (int)HttpStatusCode.OK, new ForexRate());
                logger.LogInformation("Response - ForexController -> ForexRate: " + JsonConvert.SerializeObject(response));

                return Ok(response);
            }
            else
            {
                response = new ResponseMessage(HttpStatusCode.BadRequest.ToString(), (int)HttpStatusCode.BadRequest, "Invalid Channel Id");
                logger.LogInformation("Response - ForexController -> ForexRate: " + JsonConvert.SerializeObject(response));

                return BadRequest(response);
            }
        }
        #endregion

        #region Private Methods
        private FCRequestLog ConvertToLoggingModel(FCRequest model)
        {
            FCRequestLog _model = new FCRequestLog();
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
