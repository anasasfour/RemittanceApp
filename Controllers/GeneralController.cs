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
using static RemittanceApp.Models.General;
using System.Data;
using static RemittanceApp.Utility.APIValidator;
using RemittanceApp.Models.Models.User;

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
        [HttpGet("ServiceCountry")]
        public IActionResult ServiceCountry(string userId, string password, string channelId)
        {
            logger.LogInformation("Request - GeneralController -> GetCountryList: " + JsonConvert.SerializeObject(new { userId = userId, password = password, channelId = channelId }));
            if (APIValidator.ValidateChannelId(channelId))
            {
                if (!ValidateUser(userId, password))
                {
                    var response = new ResponseMessage(Enum.GetName(ErrorCodes.invalid_user_password), (int)ErrorCodes.invalid_user_password, "");
                    logger.LogInformation("Response - GeneralController -> ValidateUser: " + JsonConvert.SerializeObject(response));
                    return BadRequest(response);
                }
                var result = new Helper().fmAdminF9_Get_Info("API_SERVICE","");

                List <Service> listCountry = new List<Service>();

                int? count = result.Tables[0].Rows.Count;
                foreach (DataRow row in result.Tables[0].Rows)
                {
                    bool same_bank = Convert.ToInt32(row["SAME_BANK"].ToString()) == 0 ? false : true;
                    bool other_bank = Convert.ToInt32(row["OTHER_BANK"].ToString()) == 0 ? false : true;
                    listCountry.Add(new Service
                    {

                        country_id = Convert.ToInt32(row["COUNTRY_ID"].ToString()),
                        country_name = (row["COUNTRY_DESC"].ToString()),
                        currency_id = Convert.ToInt32(row["currency_id"].ToString()),
                        currency_code = (row["CURRENCY_CODE"].ToString()),
                        cash_pickup = Convert.ToInt32(row["CASH_PICKUP"].ToString()) == 0 ? false : true,
                        bank_transfer = same_bank || other_bank
                    });  
                }


                response = new ResponseMessage(
                    Enum.GetName(ErrorCodes.SUCCESS_RESPONSE),
                    (int)ErrorCodes.SUCCESS_RESPONSE,
                  new { service = listCountry });




                logger.LogInformation("Response - GeneralController -> GetCountryList: " + JsonConvert.SerializeObject(response));
                return Ok(response);
            }
            else
            {
                var response = new ResponseMessage(Enum.GetName(ErrorCodes.INVALID_CHANNEL_ID), (int)ErrorCodes.INVALID_CHANNEL_ID, "");
                logger.LogInformation("Response - GeneralController -> GetCountryList: " + JsonConvert.SerializeObject(response));
                return BadRequest(response);
            }
        }

      

        [HttpGet("GetRateandFee")]
        public IActionResult GetRateandFee(string userId, string password, string profileNumber, string channelId,
            string countryId, string currencyId, double Amount, bool FCLCFlag)
        {
            RateandFee model = new RateandFee(userId, password, profileNumber, channelId, countryId, currencyId, Amount, FCLCFlag);
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
            logger.LogInformation("Request - GeneralController -> TramsactionPurpose: " + JsonConvert.SerializeObject(new { userId = userId, password = password, channelId = channelId }));
            if (APIValidator.ValidateChannelId(channelId))
            {
                if (!ValidateUser(userId, password))
                {
                    var response = new ResponseMessage(Enum.GetName(ErrorCodes.invalid_user_password), (int)ErrorCodes.invalid_user_password, "");
                    logger.LogInformation("Response - GeneralController -> ValidateUser: " + JsonConvert.SerializeObject(response));
                    return BadRequest(response);
                }

                #region start getting
                var result = new Helper().fmAdminF9_Get_Info("API_PURS", "");

                List<TransPurpose> PurposeList = new List<TransPurpose>();

                int? count = result.Tables[0].Rows.Count;
                foreach (DataRow row in result.Tables[0].Rows)
                {
                    PurposeList.Add(new TransPurpose
                    {
                        purpose_id = Convert.ToInt32(row["SOURCE_ID"].ToString()),
                        purpose_desc = (row["SOURCE_NAME_EN"].ToString()),
                       
                    });
                }


                response = new ResponseMessage(
                    Enum.GetName(ErrorCodes.SUCCESS_RESPONSE),
                    (int)ErrorCodes.SUCCESS_RESPONSE,
                   PurposeList );

                #endregion




                logger.LogInformation("Response - GeneralController -> TramsactionPurpose: " + JsonConvert.SerializeObject(response));
                return Ok(response);
            }
            else
            {
                var response = new ResponseMessage(Enum.GetName(ErrorCodes.INVALID_CHANNEL_ID), (int)ErrorCodes.INVALID_CHANNEL_ID, "");
                logger.LogInformation("Response - GeneralController -> TramsactionPurpose: " + JsonConvert.SerializeObject(response));
                return BadRequest(response);
            }
        }

        [HttpGet("SourceOfFund")]
        public IActionResult SourceOfFund(string userId, string password, string channelId)
        {
            logger.LogInformation("Request - GeneralController -> SourceOfFund: " + JsonConvert.SerializeObject(new { userId = userId, password = password, channelId = channelId }));
            if (APIValidator.ValidateChannelId(channelId))
            {
                if (!ValidateUser(userId, password))
                {
                    var response = new ResponseMessage(Enum.GetName(ErrorCodes.invalid_user_password), (int)ErrorCodes.invalid_user_password, "");
                    logger.LogInformation("Response - GeneralController -> ValidateUser: " + JsonConvert.SerializeObject(response));
                    return BadRequest(response);
                }

                #region start getting
                var result = new Helper().fmAdminF9_Get_Info("API_PURS", "");

                List<TransSource> SourceList = new List<TransSource>();

                int? count = result.Tables[0].Rows.Count;
                foreach (DataRow row in result.Tables[0].Rows)
                {
                    SourceList.Add(new TransSource
                    {
                        source_id = Convert.ToInt32(row["SOURCE_ID"].ToString()),
                        source_desc = (row["SOURCE_NAME_EN"].ToString()),

                    });
                }


                response = new ResponseMessage(
                    Enum.GetName(ErrorCodes.SUCCESS_RESPONSE),
                    (int)ErrorCodes.SUCCESS_RESPONSE,
                   SourceList);

                #endregion




                logger.LogInformation("Response - GeneralController -> SourceOfFund: " + JsonConvert.SerializeObject(response));
                return Ok(response);
            }
            else
            {
                var response = new ResponseMessage(Enum.GetName(ErrorCodes.INVALID_CHANNEL_ID), (int)ErrorCodes.INVALID_CHANNEL_ID, "");
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
                if (!ValidateUser(userId, password))
                {
                    var response = new ResponseMessage(Enum.GetName(ErrorCodes.invalid_user_password), (int)ErrorCodes.invalid_user_password, "");
                    logger.LogInformation("Response - GeneralController -> Relationship: " + JsonConvert.SerializeObject(response));
                    return BadRequest(response);
                }

                #region start getting
                var result = new Helper().fmAdminF9_Get_Info("API_RELATION", "");

                List<RelationShip> SourceList = new List<RelationShip>();

                int? count = result.Tables[0].Rows.Count;
                foreach (DataRow row in result.Tables[0].Rows)
                {
                    SourceList.Add(new RelationShip
                    {
                        Relation_id = Convert.ToInt32(row["REL_ID"].ToString()),
                        Relation_desc = (row["REL_NAME"].ToString()),

                    });
                }


                response = new ResponseMessage(
                    Enum.GetName(ErrorCodes.SUCCESS_RESPONSE),
                    (int)ErrorCodes.SUCCESS_RESPONSE,
                   SourceList);

                #endregion




                logger.LogInformation("Response - GeneralController -> Relationship: " + JsonConvert.SerializeObject(response));
                return Ok(response);
            }
            else
            {
                var response = new ResponseMessage(Enum.GetName(ErrorCodes.INVALID_CHANNEL_ID), (int)ErrorCodes.INVALID_CHANNEL_ID, "");
                logger.LogInformation("Response - GeneralController -> Relationship: " + JsonConvert.SerializeObject(response));
                return BadRequest(response);
            }
        }
        #endregion
    }
}
