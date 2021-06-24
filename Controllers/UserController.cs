using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RemittanceApp.Models;
using RemittanceApp.Models.Models.User;
using RemittanceApp.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace RemittanceApp.Controllers
{
    public class UserController : BaseController
    {
        #region Properties
        private ResponseMessage response;
        private readonly ILogger<UserController> logger;
        #endregion

        #region Constr
        public UserController(ILogger<UserController> _logger)
        {
            logger = _logger;
        }
        #endregion

        #region API Methods
        // POST api/<UserController>
        [HttpPost("SignIn")]
        public IActionResult SignIn([FromBody] loginModel model)
        {
            logger.LogInformation("Request - UserController -> SignIn: " + JsonConvert.SerializeObject(model));
            if (ModelState.IsValid)
            {
                if (APIValidator.ValidateChannelId(model.ChannelId))
                {
                    response = new ResponseMessage(HttpStatusCode.OK.ToString(), (int)HttpStatusCode.OK, null);
                    logger.LogInformation("Response - UserController -> SignIn: " + JsonConvert.SerializeObject(response));
                    return Ok(response);
                }
                else
                {
                    var response = new ResponseMessage(HttpStatusCode.BadRequest.ToString(), (int)HttpStatusCode.BadRequest, "Invalid channel Id.");
                    logger.LogInformation("Response - UserController -> SignIn: " + JsonConvert.SerializeObject(response));
                    return BadRequest(response);
                }
            }
            else
            {
                var message = string.Join(" | ", ModelState.Values
                                    .SelectMany(v => v.Errors)
                                    .Select(e => e.ErrorMessage));

                var response = new ResponseMessage(HttpStatusCode.BadRequest.ToString(), (int)HttpStatusCode.BadRequest, message);
                logger.LogInformation("Response - UserController -> SignIn: " + JsonConvert.SerializeObject(response));
                return BadRequest(response);
            }
        }

        [HttpPost("SignUp")]
        public IActionResult SignUp([FromBody] signUpModel model)
        {
            logger.LogInformation("Request - UserController -> SignUp: " + JsonConvert.SerializeObject(model));
            if (APIValidator.ValidateChannelId(model.ChannelID))
            {
                response = new ResponseMessage(HttpStatusCode.OK.ToString(), (int)HttpStatusCode.OK, null);
                logger.LogInformation("Response - UserController -> SignUp: " + JsonConvert.SerializeObject(response));
                return Ok(response);
            }
            else
            {
                var response = new ResponseMessage(HttpStatusCode.BadRequest.ToString(), (int)HttpStatusCode.BadRequest, "Invalid channel Id.");
                logger.LogInformation("Response - UserController -> SignUp: " + JsonConvert.SerializeObject(response));
                return BadRequest(response);
            }
        }

        [HttpPost("RetriveUserId")]
        public IActionResult RetriveUserId([FromBody]  RetriveUser model)
        {
            logger.LogInformation("Request - UserController -> RetriveUserId: " + JsonConvert.SerializeObject(model));
            if (APIValidator.ValidateChannelId(model.ChannelID))
            {
                response = new ResponseMessage(HttpStatusCode.OK.ToString(), (int)HttpStatusCode.OK, new {
                    customerUserID = ""
                });
                logger.LogInformation("Response - UserController -> RetriveUserId: " + JsonConvert.SerializeObject(response));
                return Ok(response);
            }
            else
            {
                var response = new ResponseMessage(HttpStatusCode.BadRequest.ToString(), (int)HttpStatusCode.BadRequest, "Invalid channel Id.");
                logger.LogInformation("Response - UserController -> RetriveUserId: " + JsonConvert.SerializeObject(response));
                return BadRequest(response);
            }
        }

        [HttpPost("ResetPassword")]
        public IActionResult ResetPassword([FromBody] RestPassword model)
        {
            logger.LogInformation("Request - UserController -> ResetPassword: " + JsonConvert.SerializeObject(model));
            if (APIValidator.ValidateChannelId(model.ChannelID))
            {
                response = new ResponseMessage(HttpStatusCode.OK.ToString(), (int)HttpStatusCode.OK, null);
                logger.LogInformation("Response - UserController -> ResetPassword: " + JsonConvert.SerializeObject(response));
                return Ok(response);
            }
            else
            {
                var response = new ResponseMessage(HttpStatusCode.BadRequest.ToString(), (int)HttpStatusCode.BadRequest, "Invalid channel Id.");
                logger.LogInformation("Response - UserController -> ResetPassword: " + JsonConvert.SerializeObject(response));
                return BadRequest(response);
            }
        }

        [HttpPost("InviteFriends")]
        public IActionResult InviteFriends([FromBody] InviteFriend model)
        {
            logger.LogInformation("Request - UserController -> InviteFriends: " + JsonConvert.SerializeObject(model));
            if (APIValidator.ValidateChannelId(model.ChannelID) && model.friendsDatas?.Count <= 5)
            {
                response = new ResponseMessage(HttpStatusCode.OK.ToString(), (int)HttpStatusCode.OK, new FriendRequestResponse());
                logger.LogInformation("Response - UserController -> InviteFriends: " + JsonConvert.SerializeObject(response));
                return Ok(response);
            }
            else
            {
                var response = new ResponseMessage(HttpStatusCode.BadRequest.ToString(), (int)HttpStatusCode.BadRequest, "Invalid channel Id OR friend invite list contains more than 5 invites (only 5 invites are accepted).");
                logger.LogInformation("Response - UserController -> InviteFriends: " + JsonConvert.SerializeObject(response));
                return BadRequest(response);
            }
        }

        [HttpGet("InviteFriendsStatus")]
        public IActionResult InviteFriendsStatus(string userId, string password, string channelId)
        {
            logger.LogInformation("Request - UserController -> InviteFriendsStatus: " + JsonConvert.SerializeObject(new { userId = userId, password = password, channelId = channelId}));
            if (APIValidator.ValidateChannelId(channelId))
            {
                response = new ResponseMessage(HttpStatusCode.OK.ToString(), (int)HttpStatusCode.OK, new ReferalFriendRequestResponse());
                logger.LogInformation("Response - UserController -> InviteFriendsStatus: " + JsonConvert.SerializeObject(response));
                return Ok(response);
            }
            else
            {
                var response = new ResponseMessage(HttpStatusCode.BadRequest.ToString(), (int)HttpStatusCode.BadRequest, "Invalid channel Id.");
                logger.LogInformation("Response - UserController -> InviteFriendsStatus: " + JsonConvert.SerializeObject(response));
                return BadRequest(response);
            }
        }
        #endregion
    }
}
