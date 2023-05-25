using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace LambdaViaCep;

public class Function
{
    private readonly HttpClient _httpClient;

    public Function()
    {
        _httpClient = new HttpClient();
    }
    
    public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
    {

        try
        {
            //var cep = request.QueryStringParameters?["cep"];
            var cep = request.PathParameters["cep"];
            var url = $"https://viacep.com.br/ws/{cep}/json/";
            var endereco = await _httpClient.GetFromJsonAsync<Endereco>(url);
            var responseBody = JsonSerializer.Serialize(endereco);

            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = responseBody,
                Headers = new Dictionary<string, string>
                        {
                            { "Content-Type", "application/json" }
                        }
            };
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Ocorreu uma exceção: {ex}");

            return new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.InternalServerError
            };
        }
    }
}
