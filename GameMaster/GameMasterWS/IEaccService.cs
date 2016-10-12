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
    public interface IEaccService
    {

        [OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetAllDatas")]
        GetAllDataResult GetAllData();

        [OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetTeamData")]
        GetAllDataResult GetTeamData();

        [OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetAllDatas/{sTeamId}")]
        GetAllDataResult GetAllDataByTeamId(string sTeamId);


        [OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json, UriTemplate = "RegisterCar/{instanceName}")]
        GetRegisterCarResult RegisterCar(string instanceName);
        
        [OperationContract]
        [WebInvoke(Method = "OPTIONS", UriTemplate = "*")]
        void Options();
    }

    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class GetRegisterCarResult
    {
        [DataMember]
        public bool IsValid
        {
            get;
            set;
        }
    }


    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class GetAllDataResult
    {
        
        [DataMember]
        public List<Team> Teams
        {
            get;
            set;
        }

        [DataMember]
        public List<Vehicle> Vehicles
        {
            get;
            set;
        }

        [DataMember]
        public List<Duplicate> Duplicates
        {
            get;
            set;
        }


        [DataMember(Name = "Achievment")]
        public List<Achievement> Achievements
        {
            get;
            set;
        }
        
    }
}
