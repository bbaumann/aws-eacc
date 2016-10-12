using eacc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Web.UI;

namespace eacc.gamemaster.ws.relay
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IRelayService
    {

        [OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json, UriTemplate = "Pilots/{sqsname}")]
        ResInt GetPilots(string sqsname);

        [OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json, UriTemplate = "Weapons/{tablename}")]
        ResList GetWeapons(string tablename);

        [OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json, UriTemplate = "Engines/{s3name}")]
        ResList GetEngines(string s3name);

        [OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json, UriTemplate = "Instance/{instance}")]
        ResString GetInstanceDomain(string instance);
    }

    [DataContract]
    public class ResInt
    {
        [DataMember]
        public int res { get; set; }
    }
    [DataContract]
    public class ResList
    {
        [DataMember]
        public List<string> res { get; set; }
    }
    [DataContract]
    public class ResString
    {
        [DataMember]
        public string res { get; set; }
    }

}
