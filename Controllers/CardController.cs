using log4net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RemittanceApp.Models;
using RemittanceApp.Models.Extensions;
using RemittanceApp.Models.Models.Card;
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
    public class CardController : BaseController
    {
        #region Properties
        private CardRequestLog logModelCardReq;
        private CardTopupLog logModelCardTopup;
        private ResponseMessage response;
        private readonly ILogger<CardController> logger;
        private readonly ICardService cardService;
        #endregion

        #region Constr
        public CardController(ILogger<CardController> _logger, ICardService _cardService)
        {
            logger = _logger;
            cardService = _cardService;
        }
        #endregion

        #region API Methods 
        [HttpPost("CardRequest")]
        public async Task<IActionResult> CardRequest([FromBody] CardRequest model)
        {
            if (APIValidator.ValidateChannelId(model.channelId)) {
                logModelCardReq = ConvertToLoggingModelCardRequest(model);

                response = new ResponseMessage(HttpStatusCode.OK.ToString(), (int)HttpStatusCode.OK, new { OrderNumber = "" });

                logModelCardReq.Response = JsonConvert.SerializeObject(response);
                var res = await cardService.InsertCardRequestLog(logModelCardReq);

                return Ok(response);
            }
            else
            {
                logModelCardReq = ConvertToLoggingModelCardRequest(model);
                response = new ResponseMessage(HttpStatusCode.BadRequest.ToString(), (int)HttpStatusCode.BadRequest, "Invalid channel id.");
                logModelCardReq.Response = JsonConvert.SerializeObject(response);
                var res = await cardService.InsertCardRequestLog(logModelCardReq);

                return BadRequest(response);
            }
        }

        [HttpPost("CardTopup")]
        public async Task<IActionResult> CardTopup([FromBody] CardTopup model)
        {
            if (APIValidator.ValidateChannelId(model.channeId))
            {
                logModelCardTopup = ConvertToLoggingModelCardTopup(model);

                response = new ResponseMessage(HttpStatusCode.OK.ToString(), (int)HttpStatusCode.OK, new { TopupRefNumber = "" });

                logModelCardTopup.Response = JsonConvert.SerializeObject(response);
                var res = await cardService.InsertCardTopupLog(logModelCardTopup);

                return Ok(response);
            }
            else
            {
                logModelCardTopup = ConvertToLoggingModelCardTopup(model);
                response = new ResponseMessage(HttpStatusCode.BadRequest.ToString(), (int)HttpStatusCode.BadRequest, "Invalid channel id.");
                logModelCardTopup.Response = JsonConvert.SerializeObject(response);
                var res = await cardService.InsertCardTopupLog(logModelCardTopup);

                return BadRequest(response);
            }
        }

        #endregion

        #region Private Methods
        private CardRequestLog ConvertToLoggingModelCardRequest(CardRequest model)
        {
            CardRequestLog _model = new CardRequestLog();
            _model.UserId = model.userId;
            _model.ChannelId = model.channelId;
            _model.ProfileNumber = model.profileNumber;
            _model.FiscalYear = DateTime.Now.GetFinancialYear();
            _model.LogDate = DateTime.Now;
            _model.LogTime = DateTime.Now;
            _model.Request = JsonConvert.SerializeObject(model);

            return _model;
        }

        private CardTopupLog ConvertToLoggingModelCardTopup(CardTopup model)
        {
            CardTopupLog _model = new CardTopupLog();
            _model.UserId = model.userId;
            _model.ChannelId = model.channeId;
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
