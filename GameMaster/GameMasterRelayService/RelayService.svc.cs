using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.EC2;
using Amazon.EC2.Model;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Amazon.SQS;
using Amazon.SQS.Model;
using com.eurosport.logging;
using eacc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Text;

namespace eacc.gamemaster.ws.relay
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class RelayService : IRelayService
    {
        public ResList GetEngines(string s3name)
        {
            List<string> res = new List<string>();
            using (var client = new AmazonS3Client(RegionEndpoint.EUCentral1))
            {
                if (!AmazonS3Util.DoesS3BucketExist(client, s3name)) return new ResList() { res = res };

                ListObjectsRequest request = new ListObjectsRequest { BucketName = s3name };

                var response = client.ListObjects(request);

                if (response.S3Objects.Count > 0)
                {
                    foreach (var s3Object in response.S3Objects)
                    {
                        res.Add(s3Object.Key);
                    }
                }
            }
            return new ResList() { res = res };
        }

        public ResInt GetPilots(string sqsname)
        {
            int res = 0;
            using (var client = new AmazonSQSClient(RegionEndpoint.EUCentral1))
            {
                var resp = client.GetQueueUrl(sqsname);
                string url = resp.QueueUrl;
                ReceiveMessageRequest receiveMessageRequest = new ReceiveMessageRequest()
                {
                    QueueUrl = url,
                };
                ReceiveMessageResponse receiveMessageResponse = client.ReceiveMessage(receiveMessageRequest);
                if (receiveMessageResponse.Messages.Count == 1)
                {
                    var message = receiveMessageResponse.Messages[0];
                    res = int.Parse(message.Body);
                    //Delete the message : ACK
                    client.DeleteMessage(url, message.ReceiptHandle);
                    //Send again the message for further usage
                    client.SendMessage(url, message.Body);
                }
            }
            return new ResInt() { res = res };
        }

        public ResList GetWeapons(string tablename)
        {
            List<string> res = new List<string>();
            using (var client = new AmazonDynamoDBClient(RegionEndpoint.EUCentral1))
            {
                Table table = null;
                if (Table.TryLoadTable(client, tablename, out table))
                {
                    ScanFilter scanFilter = new ScanFilter();
                    ScanOperationConfig config = new ScanOperationConfig()
                    {
                        Filter = scanFilter
                    };
                    Search search = table.Scan(config);

                    List<Document> documentSet = new List<Document>();
                    do
                    {
                        documentSet = search.GetNextSet();
                        foreach (var document in documentSet)
                        {
                            string name = document["name"].AsString();
                            res.Add(name);
                        }
                    } while (!search.IsDone);
                }
            }
            return new ResList() { res = res };
        }

        public ResString GetInstanceDomain(string instance)
        {
            string res = "";
            using (var client = new AmazonEC2Client(RegionEndpoint.EUCentral1))
            {
                try
                {
                    var ec2Request = new DescribeInstancesRequest { InstanceIds = new List<string>() { instance } };
                    DescribeInstancesResponse describeInstancesResponse = client.DescribeInstances(ec2Request);
                    res = describeInstancesResponse.Reservations[0].Instances[0].PublicDnsName;
                }
                catch (Exception)
                {
                    res = "";
                }
            }
            return new ResString() { res = res };
        }
    }
}
