using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.S3;
using Amazon.S3.Util;
using com.eurosport.logging;
using eacc.Properties;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;
using Amazon.S3.Model;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace eacc
{
    [DataContract]
    public class Vehicle
    {
        public const int NB_MODULES_MAX = 10;


        #region Properties

        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// Must be the EC2 Instance Name
        /// </summary>
        [DataMember]
        public string Name { get; set; }


        [DataMember]
        public int Square { get; set; }


        [DataMember]
        public int TeamId { get; set; }

        public string Domain { get; set; }

        [DataMember]
        public bool IsAlive { get; set; }

        [DataMember]
        public int TurnsToMiss { get; set; }

        [DataMember]
        public int FightWon { get; set; }

        private string Secret { get; set; }

        #region Modules

        public List<Module> Modules { get; private set; }

        [DataMember]
        public int NbEngines
        {
            get { return Modules.Count(m => m.ModuleType == ModuleType.Engine); }
            set { }
        }

        [DataMember]
        public int NbPilots
        {
            get { return Modules.Count(m => m.ModuleType == ModuleType.Pilot); }
            set { }
        }

        [DataMember]
        public int NbWeapons
        {
            get { return Modules.Count(m => m.ModuleType == ModuleType.Weapon); }
            set { }
        }

        public int ComputeScore()
        {
            if (Square < 30)
                return 0;
            if (Square < 50)
                return 1;
            if (Square < 100)
                return 2;
            if (Square < 200)
                return 8;
            if (Square < 300)
                return 18;
            if (Square < 400)
                return 30;
            return 10 * (int)(Square / 100);
        }

        #endregion

        #endregion

        #region Ctor

        public Vehicle()
        {
            this.Modules = new List<Module>();
            this.Id = -1;
            this.IsAlive = true;
        }

        public Vehicle(DataSet set) : this(set.Tables[0].Rows[0], set.Tables[1].Rows.OfType<DataRow>())
        {
        }

        public Vehicle(List<DataRow> accu) : this(accu[0], accu)
        {

        }

        public Vehicle(DataRow vehicleRow, IEnumerable<DataRow> moduleRows) : this()
        {
            this.Domain = (string)vehicleRow["VCL_DOMAIN_CH"];
            this.Name = (string)vehicleRow["VCL_NAME_CH"];
            this.Square = (int)vehicleRow["VCL_SQUARE_NU"];
            this.TeamId = (int)vehicleRow["TEA_ID"];
            this.Id = (int)vehicleRow["VCL_ID"];
            this.IsAlive = !((bool)vehicleRow["DEL_BL"]);
            this.TurnsToMiss = (int)vehicleRow["VCL_TURNS_TO_MISS_NU"];
            this.FightWon = (int)vehicleRow["VCL_FIGHT_WON_NU"];
            this.Secret = (string)vehicleRow["TEA_SECRET_CH"];

            //modules
            foreach (DataRow item in moduleRows)
            {
                var module = Module.Create(item);
                if (module != null)
                {
                    this.Modules.Add(module);
                }
            }
        }


        private static int i = 0;
        private static Random r = new Random();

        public static Vehicle CreateOne(int? id = null)
        {
            Vehicle v = new Vehicle();
            if (id.HasValue)
            {
                v.Id = id.Value;
            }
            else
            {
                v.Id = i++;
            }
            v.Name = "Vehicle-" + v.Id;
            v.Square = r.Next() % 300;
            v.TeamId = r.Next() % 3;
            v.Domain = "http://domain";
            v.Modules.Add(new PilotModule() { VehicleId = v.Id });
            for (int i = 0; i < r.Next() % 8 + 1; i++)
            {
                v.Modules.Add(new WeaponModule() { VehicleId = v.Id });
            }
            for (int i = v.Modules.Count; i < 10; i++)
            {
                v.Modules.Add(new EngineModule() { VehicleId = v.Id });
            }

            return v;
        }

        #endregion

        #region Methods

        public bool CanBeDriven()
        {
            return this.NbPilots > 0;
        }


        public bool CanFight()
        {
            return this.NbWeapons > 0;
        }

        public int GetPrice()
        {
            return this.Modules.Sum(m => m.GetCost());
        }

        public bool CanBeRegistered(int budget)
        {
            return this.CanBeDriven()
                   && this.Modules.Count <= NB_MODULES_MAX
                   && this.GetPrice() <= budget;
        }

        #endregion

        #region WS Calls
        public string RequestIAUrl()
        {
            string url = null;
            if (Properties.Settings.Default.UseMock)
            {
                return "https://g53zh5z4tj.execute-api.eu-central-1.amazonaws.com/prod/action";
            }
            try
            {
                //else
                WebClient wc = getWebClient();
                Uri call = getEndpointUri("/fight/iaurl");
                string res = wc.DownloadString(call);
                JObject jObject = JObject.Parse(res);
                JToken jToken = jObject.GetValue("res");
                url = jToken.Value<string>();
            }
            catch (Exception e)
            {
                Log.Error(e);
                Log.Error("[Team #{0}] Unable to request IA Url from vehicle {1}", TeamId, Id);
            }
            return url;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>NbEngines according to vehicle, -1 if problem</returns>
        public int RequestNbEngine()
        {
            int nbEngineAccordingToVehicle = -1;
            if (Properties.Settings.Default.UseMock)
            {
                int i = r.Next() % 100;
                if (i < 5)
                {
                    return NbEngines + 1;
                }
                //else
                return NbEngines;
            }
            try
            {
                //else
                WebClient wc = getWebClient();
                Uri call = getEndpointUri("/engine/count");
                string res = wc.DownloadString(call);
                JObject jObject = JObject.Parse(res);
                JToken jToken = jObject.GetValue("res");
                nbEngineAccordingToVehicle = jToken.Value<int>();
            }
            catch (Exception e)
            {
                Log.Error(e);
                Log.Error("[Team #{0}] Unable to request NbEngine from vehicle {1}", TeamId, Id);
            }
            return nbEngineAccordingToVehicle;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>NbPilots according to vehicle, -1 if problem</returns>
        public int RequestNbPilot()
        {
            int nbPilotAccordingToVehicle = -1;
            if (Properties.Settings.Default.UseMock)
            {
                int i = r.Next() % 100;
                if (i < 5)
                {
                    return NbPilots + 1;
                }
                //else
                return NbPilots;
            }
            //else
            try
            {
                WebClient wc = getWebClient();
                Uri call = getEndpointUri("/pilot/count");
                string res = wc.DownloadString(call);
                JObject jObject = JObject.Parse(res);
                JToken jToken = jObject.GetValue("res");
                nbPilotAccordingToVehicle = jToken.Value<int>();
            }
            catch (Exception e)
            {
                Log.Error(e);
                Log.Error("[Team #{0}] Unable to request NbPilot from vehicle {1}", TeamId, Id);
            }
            return nbPilotAccordingToVehicle;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>NbWeapons according to vehicle, -1 if problem</returns>
        public int RequestNbWeapon()
        {
            int nbWeaponAccordingToVehicle = -1;
            if (Properties.Settings.Default.UseMock)
            {
                int i = r.Next() % 100;
                if (i < 5)
                {
                    return NbWeapons + 1;
                }
                //else
                return NbWeapons;
            }
            //else
            try
            {
                WebClient wc = getWebClient();
                Uri call = getEndpointUri("/weapon/count");
                string res = wc.DownloadString(call);
                JObject jObject = JObject.Parse(res);
                JToken jToken = jObject.GetValue("res");
                nbWeaponAccordingToVehicle = jToken.Value<int>();
            }
            catch (Exception e)
            {
                Log.Error(e);
                Log.Error("[Team #{0}] Unable to request NbWeapons from vehicle {1}", TeamId, Id);
            }
            return nbWeaponAccordingToVehicle;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Square according to vehicle, -1 if problem</returns>
        public int RequestSquare()
        {
            int square = -1;
            if (Settings.Default.UseMock)
            {
                int i = r.Next() % 100;
                if (i < 2)
                {
                    return Square + 1;
                }
                //else
                return Square;
            }
            try
            {
                //else
                WebClient wc = getWebClient();
                string state = wc.DownloadString(getEndpointUri("/state"));
                JObject jObject = JObject.Parse(state);
                JToken jToken = jObject.GetValue("square");
                square = jToken.Value<int>();
            }
            catch (Exception e)
            {
                Log.Error(e);
                Log.Error("[Team #{0}] Unable to request Square from vehicle {1}", TeamId, Id);
            }
            return square;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>True if OK, false if problem</returns>
        public bool SendSquare()
        {
            bool res = true;
            try
            {
                if (Settings.Default.UseMock)
                {
                    return res;
                }
                //else
                WebClient wc = getWebClient();
                Uri call = getEndpointUri("/state");
                wc.UploadString(call, "PATCH", "{\"square\" : " + this.Square.ToString() + "}");
            }
            catch (Exception e)
            {
                Log.Error(e);
                Log.Error("[Team #{0}] Unable to send new square to vehicle {1}", TeamId, Id);
                res = false;
            }
            return res;
        }

        public bool? RequestExploreWreckAction()
        {
            bool? res = false;
            if (Settings.Default.UseMock)
            {
                int i = r.Next() % 100;
                if (i < 5)
                {
                    return false;
                }
                //else
                return true;
            }
            try
            {
                //else
                WebClient wc = getWebClient();
                string state = wc.DownloadString(getEndpointUri("/event/wreck"));
                JObject jObject = JObject.Parse(state);
                JToken jToken = jObject.GetValue("res");
                res = jToken.Value<bool>();
            }
            catch (Exception e)
            {
                Log.Error(e);
                Log.Error("[Team #{0}] Unable to request wreck exploring action from vehicle {1}", TeamId, Id);
                res = null;
            }
            return res;
        }

        public ModuleType? RequestGarageAction()
        {

            ModuleType? res = null;
            if (Settings.Default.UseMock)
            {
                int i = r.Next() % 100;
                if (i < 24)
                {
                    return ModuleType.Pilot;
                }
                if (i < 48)
                {
                    return ModuleType.Engine;
                }
                if (i < 72)
                {
                    return ModuleType.Weapon;
                }
                if (i < 96)
                {
                    return ModuleType.None;
                }
                //else
                return null;
            }
            try
            {
                //else
                WebClient wc = getWebClient();
                string state = wc.UploadString(getEndpointUri("/event/garage"),"{\"engine\":"+Module.GetCost(ModuleType.Engine)+",\"pilot\":"+Module.GetCost(ModuleType.Pilot)+",\"weapon\":"+Module.GetCost(ModuleType.Weapon)+"}");
                JObject jObject = JObject.Parse(state);
                JToken jToken = jObject.GetValue("res");
                if (jToken == null)
                    res = ModuleType.None;
                else
                {
                    var tmp = jToken.Value<string>();
                    if (tmp == null)
                        res = ModuleType.None;
                    else
                        res = (ModuleType)Enum.Parse(typeof(ModuleType), tmp);
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
                Log.Error("[Team #{0}] Unable to request Garage action from vehicle {1}", TeamId, Id);
                res = null;
            }
            return res;
        }

        public PilotModule SendPilot()
        {
            PilotModule res = new PilotModule()
            {
                Name = "NewPilotModule" + DateTime.Now.ToString("hhmmssttt"),
                VehicleId = this.Id
            };
            try
            {
                if (Settings.Default.UseMock)
                {
                    this.Modules.Add(res);
                    return res;
                }
                //else
                WebClient wc = getWebClient();
                Uri call = getEndpointUri("/pilot");
                wc.UploadString(call, "POST", "{\"name\" : \"" + res.Name + "\"}");
            }
            catch (Exception e)
            {
                Log.Error(e);
                Log.Error("[Team #{0}] Unable to send a new Pilot module to vehicle {1}", TeamId, Id);
                res = null;
            }
            if (res != null)
            {
                this.Modules.Add(res);
            }
            return res;
        }

        public EngineModule SendEngine()
        {
            EngineModule res = new EngineModule()
            {
                Name = "NewEngineModule" + DateTime.Now.ToString("hhmmssttt")
            };
            try
            {
                if (Settings.Default.UseMock)
                {
                    this.Modules.Add(res);
                    return res;
                }
                //else
                WebClient wc = getWebClient();
                Uri call = getEndpointUri("/engine");
                wc.UploadString(call, "POST", "{\"name\" : \"" + res.Name + "\"}");
            }
            catch (Exception e)
            {
                Log.Error(e);
                Log.Error("[Team #{0}] Unable to send a new Engine module to vehicle {1}", TeamId, Id);
                res = null;
            }
            if (res != null)
            {
                this.Modules.Add(res);
            }
            return res;
        }

        public WeaponModule SendWeapon()
        {
            WeaponModule res = new WeaponModule()
            {
                Name = "NewWeaponModule" + DateTime.Now.ToString("hhmmssttt")
            };
            try
            {
                if (Settings.Default.UseMock)
                {
                    this.Modules.Add(res);
                    return res;
                }
                //else
                WebClient wc = getWebClient();
                Uri call = getEndpointUri("/weapon");
                wc.UploadString(call, "POST", "{\"name\" : \"" + res.Name + "\"}");
            }
            catch (Exception e)
            {
                Log.Error(e);
                Log.Error("[Team #{0}] Unable to send a new Weapon module to vehicle {1}", TeamId, Id);
                res = null;
            }
            if (res != null)
            {
                this.Modules.Add(res);
            }
            return res;
        }


        public bool SendNbPilot()
        {
            bool res = true;
            try
            {
                if (Settings.Default.UseMock)
                {
                    return res;
                }
                //else
                WebClient wc = getWebClient();
                Uri call = getEndpointUri("/pilot");
                wc.UploadString(call, "PATCH", "{\"count\" : " + this.NbPilots.ToString() + "}");
            }
            catch (Exception e)
            {
                Log.Error(e);
                Log.Error("[Team #{0}] Unable to send new NbPilots to vehicle {1}", TeamId, Id);
                res = false;
            }
            return res;
        }

        public bool SendNbWeapon()
        {
            bool res = true;
            try
            {
                if (Settings.Default.UseMock)
                {
                    return res;
                }
                //else
                WebClient wc = getWebClient();
                Uri call = getEndpointUri("/weapon");
                wc.UploadString(call, "PATCH", "{\"count\" : " + this.NbWeapons.ToString() + "}");
            }
            catch (Exception e)
            {
                Log.Error(e);
                Log.Error("[Team #{0}] Unable to send new NbWeapons to vehicle {1}", TeamId, Id);
                res = false;
            }
            return res;
        }

        public bool SendNbEngine()
        {
            bool res = true;
            try
            {
                if (Settings.Default.UseMock)
                {
                    return res;
                }
                //else
                WebClient wc = getWebClient();
                Uri call = getEndpointUri("/engine");
                wc.UploadString(call, "PATCH", "{\"count\" : " + this.NbEngines.ToString() + "}");
            }
            catch (Exception e)
            {
                Log.Error(e);
                Log.Error("[Team #{0}] Unable to send new NbENfines to vehicle {1}", TeamId, Id);
                res = false;
            }
            return res;
        }

        public bool Stop()
        {
            bool res = true;
            Log.Fatal("[Team #{0}] Vehicle {1} is stopping.", TeamId, Id);
            this.IsAlive = false;
            if (Settings.Default.UseMock)
            {
                return res;
            }
            //else
            try
            {
                WebClient wc = getWebClient();
                Uri call = getEndpointUri("/state");
                wc.UploadString(call, "DELETE", String.Empty);
            }
            catch (Exception e)
            {
                Log.Error(e);
                Log.Error("[Team #{0}] Unable to stop {1}", TeamId, Id);
                res = false;
            }
            return res;
        }

        public bool RequestMinedZoneAction()
        {
            bool res = false;
            if (Settings.Default.UseMock)
            {
                int i = r.Next() % 100;
                if (i < 50)
                {
                    return false;
                }
                //else
                return true;
            }
            try
            {
                //else
                WebClient wc = getWebClient();
                string state = wc.DownloadString(getEndpointUri("/event/mine"));
                JObject jObject = JObject.Parse(state);
                JToken jToken = jObject.GetValue("res");
                if (jToken == null)
                    res = false;
                else
                {
                    res = jToken.Value<bool>();
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
                Log.Error("[Team #{0}] Unable to request mined zone action from vehicle {1}", TeamId, Id);
            }
            return res;
        }


        public bool DeleteModule(Module module)
        {
            bool res = true;
            if (this.Modules.Remove(module))
            {
                if (Settings.Default.UseMock)
                {
                    return true;
                }
                //else
                WebClient wc = getWebClient();
                string path = "/";
                switch (module.ModuleType)
                {
                    case ModuleType.Pilot:
                        path = "/pilot/{0}";
                        break;
                    case ModuleType.Weapon:
                        path = "/weapon/{0}";
                        break;
                    case ModuleType.Engine:
                        path = "/engine/{0}";
                        break;
                    case ModuleType.None:
                    default:
                        break;
                }
                try
                {
                    Uri call = getEndpointUri(String.Format(path, module.Name));
                    wc.UploadString(call, "DELETE", String.Empty);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    Log.Error("[Team #{0}] Unable to delete module from vehicle {1}", TeamId, Id);
                    res = false;
                }
            }
            else
            {
                Log.Error("Module {0} does not belong to vehicle {1}", module.Id, this.Id);
            }
            return res;
        }

        public int FillWeapons()
        {
            int res = -1;
            List<Module> realWeapons = GetRealWeapons();
            if (realWeapons != null)
            {
                this.Modules.AddRange(realWeapons);
                res = this.NbWeapons;
            }
            return res;
        }

        public List<Module> GetRealWeapons()
        {
            List<Module> res = new List<Module>();
            string tableName = "eacc-test";
            try
            {
                if (!Settings.Default.UseMock)
                {
                    WebClient wc = getWebClient();
                    string tmp = wc.DownloadString(getEndpointUri("/weapon"));
                    JObject jObject = JObject.Parse(tmp);
                    JToken jToken = jObject.GetValue("res");
                    tableName = jToken.Value<string>();
                }

                WebClient wcRelay = getWebClient();
                string tmpRelay = wcRelay.DownloadString(getRelayUrl(this.TeamId) + "/Weapons/" + tableName);
                Log.Debug("dbg {0}", tmpRelay);
                JObject jObjectRelay = JObject.Parse(tmpRelay);
                Log.Debug("jobject");
                foreach (dynamic name in jObjectRelay["res"])
                {
                    res.Add(new WeaponModule() { Name = (string)name });
                }
                Log.Debug("end");
            }
            catch (Exception e)
            {
                Log.Error(e);
                res = null;
            }
            return res;
        }

        /// <summary>
        /// Crée un module Pilot par fichier présent sur le bucket du véhicule et l'ajoute à la liste `Modules`
        /// </summary>
        /// <returns>NbEngines, -1 if problem</returns>
        public int FillEngines()
        {
            int res = -1;
            var realEngines = GetRealEngines();
            if (realEngines != null)
            {
                this.Modules.AddRange(realEngines);
                res = this.NbEngines;
            }
            return res;
        }

        public List<Module> GetRealEngines()
        {
            List<Module> res = new List<Module>();
            string bucketName = "test-eacc";
            try
            {
                if (!Settings.Default.UseMock)
                {
                    WebClient wc = getWebClient();
                    string tmp = wc.DownloadString(getEndpointUri("/engine"));
                    Log.Debug("dbg got {0}", tmp);
                    JObject jObject = JObject.Parse(tmp);
                    JToken jToken = jObject.GetValue("res");
                    bucketName = jToken.Value<string>();
                }

                
                WebClient wcRelay = getWebClient();
                string tmpRelay = wcRelay.DownloadString(getRelayUrl(this.TeamId) + "/Engines/" + bucketName);
                Log.Debug("dbg got {0}", tmpRelay);
                JObject jObjectRelay = JObject.Parse(tmpRelay);
                foreach (dynamic name in jObjectRelay["res"])
                {
                    res.Add(new EngineModule() { Name = (string)name });
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
                res = null;
            }
            return res;
        }



        /// <summary>
        /// Crée un module Pilot par message présent sur le SQS du véhicule et l'ajoute à la liste `Modules`
        /// </summary>
        /// <returns>NbPilots, -1 if problem</returns>
        public int FillPilots()
        {
            int res = -1;
            int realNbPilots = GetRealPilots();
            if (realNbPilots != -1)
            {
                for (int i = 0; i < realNbPilots; i++)
                {
                    this.Modules.Add(new PilotModule() { Name = "Pilot" + i });
                }
                res = this.NbPilots;
            }
            return res;
        }

        public int GetRealPilots()
        {
            int res = -1;
            string queueName = "eacc";
            try
            {
                if (!Settings.Default.UseMock)
                {
                    WebClient wc = getWebClient();
                    string tmp = wc.DownloadString(getEndpointUri("/pilot"));
                    JObject jObject = JObject.Parse(tmp);
                    JToken jToken = jObject.GetValue("res");
                    queueName = jToken.Value<string>();
                }


                WebClient wcRelay = getWebClient();
                string tmpRelay = wcRelay.DownloadString(getRelayUrl(this.TeamId) + "/Pilots/" + queueName);
                JObject jObjectRelay = JObject.Parse(tmpRelay);
                JToken jTokenRelay = jObjectRelay.GetValue("res");
                res = jTokenRelay.Value<int>();
            }
            catch (Exception e)
            {
                Log.Error(e);
                res = -1;
            }
            return res;
        }

        private Uri getEndpointUri(string path)
        {
            Log.Debug("dbg : {0},{1}", this.Domain, path);
            Uri uri = new Uri("http://"+this.Domain);
            Uri res = new Uri(uri, path);
            return res;
        }

        private WebClient getWebClient()
        {
            WebClient wc = new WebClient();
            wc.Headers["X-EACC-SECRET"] = Secret;
            wc.Headers["Content-Type"] = "application/json";
            return wc;
        }

        public static string GetDomain(string instanceid, int teamId)
        {
            string res = null;
            try
            {
                WebClient wcRelay = new WebClient();
                string tmpRelay = wcRelay.DownloadString(getRelayUrl(teamId) + "/Instance/" + instanceid);
                JObject jObjectRelay = JObject.Parse(tmpRelay);
                JToken jTokenRelay = jObjectRelay.GetValue("res");
                res = jTokenRelay.Value<string>();
            }
            catch (Exception e)
            {
                Log.Error(e);
                res = "";
            }
            return res;
        }

        private static string getRelayUrl(int teamId)
        {
            if (teamId == 1)
                return "http://ec2-54-93-156-51.eu-central-1.compute.amazonaws.com/RelayService.svc";
            if (teamId == 2)
                return "http://ec2-52-57-129-243.eu-central-1.compute.amazonaws.com/RelayService.svc";
            return "http://ec2-52-57-122-136.eu-central-1.compute.amazonaws.com/RelayService.svc";
        }


        #endregion
    }
}

