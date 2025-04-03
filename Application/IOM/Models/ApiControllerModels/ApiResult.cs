using IOM.Utilities;
using System;

namespace IOM.Models.ApiControllerModels
{
    public class ApiResult
    {
        public bool isSuccessful = true;

        private string _type = string.Empty;

        public string Type
        {
            get => isSuccessful == false ? "danger" : _type;
            set => _type = value;
        }

        public int Result => isSuccessful ? 1 : 0;

        public string message = string.Empty;

        public string debugMessage = string.Empty;

        public APIResultCode code { get; set; }

        public string status = string.Empty;

        public object data = null;
        private string ErrorMessage { get; set; }
        
        public static ApiResult BadRequest(string message)
        {
            return new ApiResult
            {
                code = APIResultCode.BadRequest,
                ErrorMessage = message,
                status = "Error",
                isSuccessful = false
            };
        } 
    }
}