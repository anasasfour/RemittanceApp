using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RemittanceApp.Utility;
using RemittanceApp.Models;
using RemittanceApp.Models.Models.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace RemittanceApp.Controllers
{
    public class GeneralController : BaseController
    {
        #region Properties
        private ResponseMessage response;
        private readonly ILogger<GeneralController> logger;
        #endregion

        #region Constr
        public GeneralController(ILogger<GeneralController> _logger)
        {
            logger = _logger;
        }
        #endregion

        #region API Methods
        [HttpGet("GetCountryList")]
        public IActionResult GetCountryList(string userId, string password, string channelId)
        {
            logger.LogInformation("Request - GeneralController -> GetCountryList: " + JsonConvert.SerializeObject(new { userId = userId, password = password, channelId = channelId }));
            if (APIValidator.ValidateChannelId(channelId))
            {
                var curr = new Helper().fmCurrency_Get_Info("23", "");
                response = new ResponseMessage(
                    HttpStatusCode.OK.ToString(),
                    (int)HttpStatusCode.OK,
                    new { Countries = JsonConvert.SerializeObject(curr.Tables[0]) });

                logger.LogInformation("Response - GeneralController -> GetCountryList: " + JsonConvert.SerializeObject(response));
                return Ok(response);
            }
            else
            {
                var response = new ResponseMessage(HttpStatusCode.BadRequest.ToString(), (int)HttpStatusCode.BadRequest, "Invalid channel Id.");
                logger.LogInformation("Response - GeneralController -> GetCountryList: " + JsonConvert.SerializeObject(response));
                return BadRequest(response);
            }
        }

        [HttpGet("GetServiceDetails")]
        public IActionResult GetServiceDetails(string userId, string password, string channelId, string countryCode)
        {
            logger.LogInformation("Request - GeneralController -> GetServiceDetails: " + JsonConvert.SerializeObject(new { userId = userId, password = password, channelId = channelId, countryCode = countryCode }));
            if (APIValidator.ValidateChannelId(channelId))
            {
                response = new ResponseMessage(HttpStatusCode.OK.ToString(), (int)HttpStatusCode.OK, new { CurrencyService = "" });
                logger.LogInformation("Response - GeneralController -> GetServiceDetails: " + JsonConvert.SerializeObject(response));
                return Ok(response);
            }
            else
            {
                var response = new ResponseMessage(HttpStatusCode.BadRequest.ToString(), (int)HttpStatusCode.BadRequest, "Invalid channel Id.");
                logger.LogInformation("Response - GeneralController -> GetServiceDetails: " + JsonConvert.SerializeObject(response));
                return BadRequest(response);
            }
        }

        [HttpGet("GetRateandFee")]
        public IActionResult GetRateandFee(string userId, string password, string profileNumber, string channelId,
            string countryCode, string currencyService, double Amount, bool FCLCFlag)
        {
            RateandFee model = new RateandFee(userId, password, profileNumber, channelId, countryCode, currencyService, Amount, FCLCFlag);
            logger.LogInformation("Request - GeneralController -> GetRateandFee: " + JsonConvert.SerializeObject(model));

            if (APIValidator.ValidateChannelId(channelId))
            {
                response = new ResponseMessage(HttpStatusCode.OK.ToString(), (int)HttpStatusCode.OK, new RateandFeeResponse());
                logger.LogInformation("Response - GeneralController -> GetRateandFee: " + JsonConvert.SerializeObject(response));
                return Ok(response);
            }
            else
            {
                var response = new ResponseMessage(HttpStatusCode.BadRequest.ToString(), (int)HttpStatusCode.BadRequest, "Invalid channel Id.");
                logger.LogInformation("Response - GeneralController -> GetRateandFee: " + JsonConvert.SerializeObject(response));
                return BadRequest(response);
            }
        }

        [HttpGet("TransactionPurpose")]
        public IActionResult TransactionPurpose(string userId, string password, string channelId)
        {
            logger.LogInformation("Request - GeneralController -> TransactionPurpose: " + JsonConvert.SerializeObject(new { userId = userId, password = password, channelId = channelId }));
            if (APIValidator.ValidateChannelId(channelId))
            {
                response = new ResponseMessage(HttpStatusCode.OK.ToString(), (int)HttpStatusCode.OK, new {
                    purpose_id = "",
                    purpose_name = ""
                });
                logger.LogInformation("Response - GeneralController -> TransactionPurpose: " + JsonConvert.SerializeObject(response));

                return Ok(response);
            }
            else
            {
                response = new ResponseMessage(HttpStatusCode.BadRequest.ToString(), (int)HttpStatusCode.BadRequest, "Invalid channel Id.");
                logger.LogInformation("Response - GeneralController -> TransactionPurpose: " + JsonConvert.SerializeObject(response));
                return BadRequest(response);
            }
        }

        [HttpGet("SourceOfFund")]
        public IActionResult SourceOfFund(string userId, string password, string channelId)
        {
            logger.LogInformation("Request - GeneralController -> SourceOfFund: " + JsonConvert.SerializeObject(new { userId = userId, password = password, channelId = channelId }));
            if (APIValidator.ValidateChannelId(channelId))
            {
                response = new ResponseMessage(HttpStatusCode.OK.ToString(), (int)HttpStatusCode.OK, new
                {
                    source_id = "",
                    source_name = ""
                });
                logger.LogInformation("Response - GeneralController -> SourceOfFund: " + JsonConvert.SerializeObject(response));

                return Ok(response);
            }
            else
            {
                response = new ResponseMessage(HttpStatusCode.BadRequest.ToString(), (int)HttpStatusCode.BadRequest, "Invalid channel Id.");
                logger.LogInformation("Response - GeneralController -> SourceOfFund: " + JsonConvert.SerializeObject(response));
                return BadRequest(response);
            }
        }

        [HttpGet("Relationship")]
        public IActionResult Relationship(string userId, string password, string channelId)
        {
            logger.LogInformation("Request - GeneralController -> Relationship: " + JsonConvert.SerializeObject(new { userId = userId, password = password, channelId = channelId }));
            if (APIValidator.ValidateChannelId(channelId))
            {
                response = new ResponseMessage(HttpStatusCode.OK.ToString(), (int)HttpStatusCode.OK, new
                {
                    relation_id = "",
                    relation_name = ""
                });
                logger.LogInformation("Response - GeneralController -> Relationship: " + JsonConvert.SerializeObject(response));

                return Ok(response);
            }
            else
            {
                response = new ResponseMessage(HttpStatusCode.BadRequest.ToString(), (int)HttpStatusCode.BadRequest, "Invalid channel Id.");
                logger.LogInformation("Response - GeneralController -> Relationship: " + JsonConvert.SerializeObject(response));
                return BadRequest(response);
            }
        }
        #endregion
    }
}
