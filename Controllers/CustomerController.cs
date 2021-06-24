using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RemittanceApp.Models;
using RemittanceApp.Models.Extensions;
using RemittanceApp.Models.Models.Customer;
using RemittanceApp.Models.Models.LoggingModels;
using RemittanceApp.Models.Models.LogModel;
using RemittanceApp.Services.Interfaces;
using RemittanceApp.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace RemittanceApp.Controllers
{
    public class CustomerController : BaseController
    {
        #region Properties
        private CustomerRegisterationLog logModel;
        private ResponseMessage response;
        private readonly ILogger<CustomerController> logger;
        private readonly ICustomerService customerService;
        #endregion

        #region Constr
        public CustomerController(ILogger<CustomerController> _logger, ICustomerService _customerService)
        {
            logger = _logger;
            customerService = _customerService;
        }
        #endregion

        #region API Methods

        [HttpPost("RegisterCustomer")]
        public async Task<IActionResult> RegisterCustomer([FromBody] CustomerRegisteration model)
        {
            logModel = ConvertToLoggingModel(model);
            if (ModelState.IsValid)
            {
                if (APIValidator.ValidateChannelId(model.ChannelId))
                {
                    response = new ResponseMessage(HttpStatusCode.OK.ToString(), (int)HttpStatusCode.OK, new { ProfileNumber = "" });

                    logModel.Response = JsonConvert.SerializeObject(response);
                    var res = await customerService.InsertCustomerRegisterationLog(logModel);

                    return Ok(response);
                }
                else
                {
                    response = new ResponseMessage(HttpStatusCode.BadRequest.ToString(), (int)HttpStatusCode.BadRequest, "Invalid channel Id");

                    logModel.Response = JsonConvert.SerializeObject(response);
                    var res = await customerService.InsertCustomerRegisterationLog(logModel);
                    return BadRequest(response);
                }
            }
            else
            {

                var message = string.Join(" | ", ModelState.Values
                                    .SelectMany(v => v.Errors)
                                    .Select(e => e.ErrorMessage));

                response = new ResponseMessage(HttpStatusCode.BadRequest.ToString(), (int)HttpStatusCode.BadRequest, message);

                logModel.Response = JsonConvert.SerializeObject(response);
                var res = await customerService.InsertCustomerRegisterationLog(logModel);
                return BadRequest(response);
            }
        }

        [HttpGet("GetCustomer")]
        public IActionResult GetCustomer(string userId, string password, string channelId, string mobileNumber,
            string IdNumber, string IdType, string lariCardNumber, string email, string profileNumber, string name)
        {
            Customer model = new Customer(userId, password, channelId, mobileNumber, lariCardNumber, email, profileNumber, name, IdNumber, IdType); ;
            logger.LogInformation("Request - CustomerController -> GetCustomer: " + JsonConvert.SerializeObject(model));
            var isValid = this.TryValidateModel(model);

            if (isValid)
            {
                if (APIValidator.ValidateChannelId(channelId) && APIValidator.ValidateGetCustomer(model))
                {
                    response = new ResponseMessage(HttpStatusCode.OK.ToString(), (int)HttpStatusCode.OK, new CustomerResponse());
                    logger.LogInformation("Response - CustomerController -> GetCustomer: " + JsonConvert.SerializeObject(response));
                    return Ok(response);
                }
                else
                {
                    var response = new ResponseMessage(HttpStatusCode.BadRequest.ToString(), (int)HttpStatusCode.BadRequest, "Invalid channel Id OR Invalid IdType");
                    logger.LogInformation("Response - CustomerController -> GetCustomer: " + JsonConvert.SerializeObject(response));
                    return BadRequest(response);
                }
            }
            else
            {
                var response = new ResponseMessage(HttpStatusCode.BadRequest.ToString(), (int)HttpStatusCode.BadRequest, JsonConvert.SerializeObject(model));
                logger.LogInformation("Response - CustomerController -> GetCustomer: " + JsonConvert.SerializeObject(response));
                return BadRequest(response);
            }
        }

        [HttpGet("GetCustomerDetail")]
        public IActionResult GetCustomerDetail(string userId, string password, string channelId, string profileNumber)
        {
            CustomerDetail model = new CustomerDetail(userId, password, channelId, profileNumber);
            logger.LogInformation("Request - CustomerController -> GetCustomerDetail: " + JsonConvert.SerializeObject(model));
            var isValid = this.TryValidateModel(model);
            if(isValid && APIValidator.ValidateChannelId(channelId))
            {
                response = new ResponseMessage(HttpStatusCode.OK.ToString(), (int)HttpStatusCode.OK, new CustomerDetailResponse());
                logger.LogInformation("Response - CustomerController -> GetCustomerDetail: " + JsonConvert.SerializeObject(response));
                return Ok(response);
            }
            else
            {
                var response = new ResponseMessage(HttpStatusCode.BadRequest.ToString(), (int)HttpStatusCode.BadRequest, JsonConvert.SerializeObject(model));
                logger.LogInformation("Response - CustomerController -> GetCustomerDetail: " + JsonConvert.SerializeObject(response));
                return BadRequest(response);
            }
        }

        [HttpGet("DashboardAlert")]
        public IActionResult DashboardAlert(string userId, string password, string channelID)
        {
            logger.LogInformation("Request - CustomerController -> DashboardAlert: " + JsonConvert.SerializeObject(new { UserId = userId, Password = password, ChannelID = channelID }));
            if (APIValidator.ValidateChannelId(channelID))
            {
                response = new ResponseMessage(HttpStatusCode.OK.ToString(), (int)HttpStatusCode.OK, new DashboardAlert());
                logger.LogInformation("Response - CustomerController -> DashboardAlert: " + JsonConvert.SerializeObject(response));
                return Ok(response);
            }
            else
            {
                var response = new ResponseMessage(HttpStatusCode.BadRequest.ToString(), (int)HttpStatusCode.BadRequest, JsonConvert.SerializeObject(new { UserId = userId, Password = password, ChannelID = channelID }));
                logger.LogInformation("Response - CustomerController -> DashboardAlert: " + JsonConvert.SerializeObject(response));
                return BadRequest(response);
            }
        }

        [HttpGet("TransactionDetails")]
        public IActionResult TransactionDetails(string userId, string password, string channelID, string TxnType, DateTime FromDate, DateTime ToDate)
        {
            logger.LogInformation("Request - CustomerController -> TransactionDetails: " + JsonConvert.SerializeObject(new { UserId = userId, Password = password, ChannelID = channelID, TxnType = TxnType, FromDate = FromDate, ToDate = ToDate }));
            if (APIValidator.ValidateChannelId(channelID))
            {
                response = new ResponseMessage(HttpStatusCode.OK.ToString(), (int)HttpStatusCode.OK, new TransactionDetails());
                logger.LogInformation("Response - CustomerController -> TransactionDetails: " + JsonConvert.SerializeObject(response));
                return Ok(response);
            }
            else
            {
                var response = new ResponseMessage(HttpStatusCode.BadRequest.ToString(), (int)HttpStatusCode.BadRequest, "Invalid channel Id.");
                logger.LogInformation("Response - CustomerController -> TransactionDetails: " + JsonConvert.SerializeObject(response));
                return BadRequest(response);
            }
        }
        #endregion

        #region Private Methods
        private CustomerRegisterationLog ConvertToLoggingModel(CustomerRegisteration model)
        {
            CustomerRegisterationLog _model = new CustomerRegisterationLog();
            _model.UserId = model.UserId;
            _model.ChannelId = model.ChannelId;
            _model.MobileNumber = model.MobileNumber;
            _model.Name = model.Name;
            _model.FiscalYear = DateTime.Now.GetFinancialYear();
            _model.LogDate = DateTime.Now;
            _model.LogTime = DateTime.Now;
            _model.Request = JsonConvert.SerializeObject(model);

            return _model;
        }
        #endregion
    }
}
