using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info("C# HTTP trigger function processed a request.");

    // parse query parameter
    string userId = req.GetQueryNameValuePairs()
        .FirstOrDefault(q => string.Compare(q.Key, "userId", true) == 0)
        .Value;

    if (userId == null)
    {
        // Get request body
        dynamic data = await req.Content.ReadAsAsync<object>();
        userId = data?.userId;
    }

    if (userId != null)
    {
        var result = await FetchRatings(userId);
        log.Info(result.ToString());

        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(result.ToString(), Encoding.UTF8, "application/json")
        };
    }
    else
    {
        return req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a userId on the query string or in the request body");
    }
}

public static async Task<JArray> FetchRatings(string userId)
{
    string result = "";
    string endpointUrl = "https://serverlesshack1808.documents.azure.com:443/";
    string authorizationKey = "2AdVNa2gCjdx99C789jadhMNDu4R82fFns4aOWDmvrkpvGaFk1qXQIyqEXyHz6yMzxcskgz5cKAbvFmFpDG3bQ==";
    string databaseId = "serverlesshackdb";
    string collectionId = "ratings";
    JArray jArray = new JArray();
    JObject jObj = new JObject();

    using (DocumentClient client = new DocumentClient(new Uri(endpointUrl), authorizationKey))
    {
        var collectionLink = UriFactory.CreateDocumentCollectionUri(databaseId, collectionId);
        var results = client.CreateDocumentQuery<Ratings>(collectionLink).Where(doc => doc.userId == userId).AsEnumerable();


        foreach (var item in results.ToList())
        {
            dynamic b = new JObject();
            b = JsonConvert.SerializeObject(item);
            try
            {
                jArray.Add(b);
                jArray.Add(b);
            }
            catch (Exception ex)
            {
                var c = ex.Message;
            }
        }
    }
    return jArray;
}

public class Ratings
{
    public string id { get; set; }
    public string userId { get; set; }
    public string productId { get; set; }
    public string timestamp { get; set; }
    public string locationName { get; set; }
    public string rating { get; set; }
    public string userNotes { get; set; }
    public string _rid { get; set; }
    public string _self { get; set; }
    public string _etag { get; set; }
    public string _attachments { get; set; }
    public int _ts { get; set; }
}
