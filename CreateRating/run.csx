using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info("Testing CD/CI with Functions.");
    log.Info("C# HTTP trigger function processed a request.");
    string userId = "", productId = "", locationName = "", userNotes = "", rating = "";
            var id = Guid.NewGuid();
            log.Info(id.ToString());

            string reqContent = await req.Content.ReadAsStringAsync();
            ReqPayload reqPayload = JsonConvert.DeserializeObject<ReqPayload>(reqContent);

            userId = reqPayload.userId;
            productId = reqPayload.productId;
            locationName = reqPayload.locationName;
            rating = reqPayload.rating;
            userNotes = reqPayload.userNotes;

            log.Info(userId);
            log.Info(productId);

            var timestamp = DateTime.UtcNow;
            log.Info(timestamp.ToString());
            log.Info(locationName);
            log.Info(rating.ToString());
            log.Info(userNotes);

            if (userId == "" || productId == "" || locationName == "" || userNotes == "" || rating == "" )
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "Please pass required parameters in the request body");
            }
            else
            {
                
                dynamic jsonResponse = new JObject();
                jsonResponse.id = id;
                jsonResponse.userId = userId;
                jsonResponse.productId = productId;
                jsonResponse.timestamp = timestamp.ToString();
                jsonResponse.locationName = locationName;
                jsonResponse.rating = rating.ToString();
                jsonResponse.userNotes = userNotes;

                log.Info(jsonResponse.ToString());
                var a = JsonConvert.SerializeObject(jsonResponse, Formatting.Indented);

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(a, Encoding.UTF8, "application/json")
                };
            }
}

        public static async void WriteToDB(JObject jsonData)
        {
            string endpointUrl = "https://serverlesshack1808.documents.azure.com:443/";
            string authorizationKey = "2AdVNa2gCjdx99C789jadhMNDu4R82fFns4aOWDmvrkpvGaFk1qXQIyqEXyHz6yMzxcskgz5cKAbvFmFpDG3bQ==";
            string databaseId = "serverlesshackdb";
            string collectionId = "ratings";

            using (DocumentClient client = new DocumentClient(new Uri(endpointUrl), authorizationKey))
            {
                var collectionLink = UriFactory.CreateDocumentCollectionUri(databaseId, collectionId);
                Document created = await client.CreateDocumentAsync(collectionLink, jsonData);
            }
        }


        public class ReqPayload
        {
            public string id { get; set; }
            public string userId { get; set; }
            public string productId { get; set; }
            public string locationName { get; set; }
            public string rating { get; set; }
            public string userNotes { get; set; }
        }