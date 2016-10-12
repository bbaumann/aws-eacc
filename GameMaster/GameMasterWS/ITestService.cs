using eacc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Web.UI;

namespace eacc.gamemaster.ws
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface ITestService
    {

        [OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json, UriTemplate = "TestFight?lambdaurl={lambdaUrl}&nbweapon={nbweapon}&othernbweapon={othernbweapon}")]
        string TestFight(string lambdaUrl, string nbweapon, string othernbweapon);

        [OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetLogs/{teamId}")]
        List<string> GetLogs(string teamId);
    }
    
}
