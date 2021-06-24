using RemittanceApp.Models.Models.Customer;
using RemittanceApp.Models.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemittanceApp.Utility
{
    public static class APIValidator
    {
        private static string[] ChannelIdArray = { "AGENT", "MOBILE", "ONLINE" };
        private static string[] IdType = { "EMIRATID", "PASSPORT",};

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
