using System;
using System.Collections.Generic;
using System.Text;

namespace RonVideo
{
    public static class Constants
    {
        public const string BaseURL = "http://localhost:7071/api/";
        public const string RouteDownload = "files/download";
        public const string RouteLoanId = "home-lending/applications/{id}";
        public const string RouteUrl = "close/closings/{closingId}/recordings/{fileId}/download-url";
        //public const string RouteBlendUrl = "files/download? token = ";
        //public const string MyOrchestration = "MyOrchestration";
        //public const string MyActivityOne = "MyActivityOne";
        //public const string MyActivityTwo = "MyActivityTwo";

        //public const string MyExternalInputEvent = "MyExternalInputEvent";

        //public const string BeginFlowWithHttpPost = "BeginFlowWithHttpPost";
        //public const string ExternalHttpPostInput = "ExternalHttpPostInput";

        //// Diagnostics API
        //public const string Diagnostics = "Diagnostics";
        //public const string GetAllFlows = "GetAllFlows";
        //public const string GetCompletedFlows = "GetCompletedFlows";
        //public const string GetNotCompletedFlows = "GetNotCompletedFlows";
    }
}
