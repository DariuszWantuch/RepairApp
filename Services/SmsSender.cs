using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Twilio;
using Twilio.AspNet.Core;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace RepairApp.Services
{
    public class SmsSender : TwilioController
    {
        public ActionResult SendSms(int to, string message)
        {
            var accountSid = "ACa21823a55c15259108ba93f96248504d";
            var authToken = "e038e24181b69cab8339d7085b3da0cc";
            TwilioClient.Init(accountSid, authToken);
            var toPhone = "+48" + to.ToString();
            var _to = new PhoneNumber(toPhone);
            var _from = new PhoneNumber("+16128000582");

            var _message = MessageResource.Create(
                to: _to,
                from: _from,
                body: message);
                
            return Content(_message.Sid);
        }    
    }
}
