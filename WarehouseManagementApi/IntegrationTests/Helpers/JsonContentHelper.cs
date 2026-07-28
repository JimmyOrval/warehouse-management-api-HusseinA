using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IntegrationTests.Helpers;

// this centralized Json serialization so the tests use the
// same settings that are used in the real Api
public static class JsonContentHelper
{
    public static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        //since we have Json enum converter, we need to fake it here
        // so things like product status don't cause errors
        Converters = { new JsonStringEnumConverter() }
    };

    // this turns the object to proper json Http body
    public static StringContent ToJsonContent<T>(T value)
    {
        var json = JsonSerializer.Serialize(value, Options);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    // takes json and turns it to string body so it's readable
    public static async Task<T> ReadAsAsync<T>(this HttpContent content)
    {
        var json = await content.ReadAsStringAsync();

        // if the json body is empty, we throw
        if (string.IsNullOrWhiteSpace(json))
        {
            throw new InvalidOperationException(
                "JSON response body was empty.");
        }

        try
        {
            // deserialize or throw
            return JsonSerializer.Deserialize<T>(json, Options)
                   ?? throw new InvalidOperationException(
                       $"Deserialization failed. Raw body: {json}");
        }
        catch (JsonException e)
        {
            // if everything fails
            throw new InvalidOperationException(
                $"Deserialization failed. Raw body: {json}", e);
        }
    }

    // form an easy to read response message
    public static Task<HttpResponseMessage> PostJsonAsync<T>(
        this HttpClient client, string url, T body)
    {
        return client.PostAsync(url, ToJsonContent(body));
    }

    public static Task<HttpResponseMessage> PutJsonAsync<T>(
        this HttpClient client, string url, T body)
    {
        return client.PutAsync(url, ToJsonContent(body));
    }


    // set the role claim for the authenticated users
    public static HttpClient AsRole(this HttpClient client, string role)
    {
        // repalce the normal "token" with one that has role claim
        client.DefaultRequestHeaders.Remove("Test-Role");
        client.DefaultRequestHeaders.Add("Test-Role", role);
        return client;
    }

    public static HttpClient AsAdmin(this HttpClient client)
    {
        return client.AsRole("Admin");
    }

    public static HttpClient AsUser(this HttpClient client)
    {
        return client.AsRole("User");
    }
}
