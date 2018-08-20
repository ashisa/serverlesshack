using Microsoft.Azure.Documents.Client;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info("C# HTTP trigger function processed a request.");

    // parse query parameter
    string ratingId = req.GetQueryNameValuePairs()
        .FirstOrDefault(q => string.Compare(q.Key, "ratingId", true) == 0)
        .Value;

    if (ratingId == null)
    {
        // Get request body
        dynamic data = await req.Content.ReadAsAsync<object>();
        ratingId = data?.ratingId;
    }

    if (ratingId != null)
    {
        var result = await FetchRating(ratingId);
        return req.CreateResponse(HttpStatusCode.OK, result);
    }
    else
    {
        return req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a ratingId on the query string or in the request body");
    }
}

public static async Task<string> FetchRating(string ratingId)
{
    string result = "";
    string endpointUrl = "https://serverlesshack1808.documents.azure.com:443/";
    string authorizationKey = "2AdVNa2gCjdx99C789jadhMNDu4R82fFns4aOWDmvrkpvGaFk1qXQIyqEXyHz6yMzxcskgz5cKAbvFmFpDG3bQ==";
    string databaseId = "serverlesshackdb";
    string collectionId = "ratings";

    using (DocumentClient client = new DocumentClient(new Uri(endpointUrl), authorizationKey))
    {
        //var collectionLink = UriFactory.CreateDocumentCollectionUri(databaseId, collectionId);
        //Document created = await client.CreateDocumentAsync(collectionLink, jsonData);
        var response = await client.ReadDocumentAsync(UriFactory.CreateDocumentUri(databaseId, collectionId, ratingId));
        result = response.Resource.ToString();
    }
    return result;
}
