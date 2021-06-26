using RemittanceApp.Models.Models.Customer;
using RemittanceApp.Models.Models.User;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RemittanceApp.Utility
{
    public static class APIValidator
    {
        private static string[] ChannelIdArray = { "AGENT", "MOBILE", "ONLINE" };
        private static string[] IdType = { "EMIRATID", "PASSPORT",};

        public enum ErrorCodes : int
        {
            [Description("Success Response")]
            SUCCESS_RESPONSE = 00,
            [Description("Invalid Channel ID")]
            INVALID_CHANNEL_ID = 101,
            [Description("Invalid user Name or password")]
            invalid_user_password = 102
        }


        public static bool ValidateChannelId(string channelId)
        {
            return ChannelIdArray.Contains(channelId.ToUpper());
        }

        public static bool ValidateIdType(string idType)
        {
            return IdType.Contains(idType.ToUpper());
        }

        public static bool ValidateUserAgent(loginModel model)
        {
            // later on put the business logic to validate the User agent
            return true;
        }

        public static bool ValidateUser(string _username, string _password)
        {
            Helper helper = new Helper();
            DataSet dsResult = helper.fmApiUser(new loginModel() { userId = _username, password = _password },"CHK");
            if (dsResult.Tables.Count > 0)
            {
                if(dsResult.Tables[0].Rows[0].ItemArray[0].ToString()=="P")
                {
                    return true;
                }
                else
                {
                    return false;
                }
               
            }
            else
            {
                return false;
            }

            
        }

        public static bool SignInUser(string _username, string _password)
        {
            Helper helper = new Helper();
            DataSet dsResult = helper.fmApiUser(new loginModel() { userId = _username, password = _password }, "SIGNIN");
            if (dsResult.Tables.Count > 0)
            {
                if (dsResult.Tables[0].Rows[0].ItemArray[0].ToString() == "P")
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            else
            {
                return false;
            }


        }

        public static bool ValidateGetCustomer(Customer model)
        {
            if (string.IsNullOrEmpty(model.detailId.IdNumber))
                return true;
            else
            {
                return IdType.Contains(model.detailId.IdType.ToUpper());
            }
        }
    }
}
