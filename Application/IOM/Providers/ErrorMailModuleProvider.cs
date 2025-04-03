using Elmah;
using IOM.Services;
using IOM.Utilities;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Mail;
using System.Web.Http.Controllers;

namespace IOM.Providers
{
    public class ErrorMailModuleProvider : ErrorMailModule
    {
        protected override string MailSubjectFormat
        {
            get
            {
                return (base.MailSubjectFormat) + " (Code: " + ServiceUtility.GetRandomCode() + ")";
            }
        }

        protected override async void SendMail(MailMessage mail)
        {
            try
            {
                string email = "";

                foreach (MailAddress address in mail.To)
                {
                    email = address.Address;
                }

                await SendGridMailServices.Instance.SendAsync(new IdentityMessage
                {
                    Subject = MailSubjectFormat,
                    Body = mail.Body,
                    Destination = email,
                }).ConfigureAwait(false);
            }
            catch { }
        }

        public static string PreEmailContent(Exception e, HttpRequestMessage httpReqMsg)
        {
            using (var streamReader = new StreamReader(httpReqMsg.Content.ReadAsStreamAsync().Result))
            {
                streamReader.BaseStream.Position = 0;
                using (var jsonReader = new JsonTextReader(streamReader))
                {
                    object parameters = "";

                    try
                    {
                        parameters = JsonConvert.DeserializeObject(jsonReader.ReadAsString());
                    }
                    catch { }

                    return string.Join("\n", (new string[] {
                            $"Message: {e.Message}\n{e.StackTrace}",
                            $"Input Stream: { parameters }",
                            $"Query String: { (string.IsNullOrEmpty(httpReqMsg.RequestUri.Query) ? "(No Query string found.)" : httpReqMsg.RequestUri.Query) }"
                        }));
                }
            }
        }

        public static void RaiseCaughtErrors(Exception e, HttpActionContext actionContext)
        {
            ErrorSignal.FromCurrentContext().Raise(new Exception(PreEmailContent(e, actionContext.Request)));
        }
    }
}