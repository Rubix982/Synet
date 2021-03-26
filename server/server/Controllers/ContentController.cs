using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CsvHelper;
using CsvHelper.Configuration;

namespace server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ContentController : ControllerBase
    {
        private readonly ILogger<ContentController> _logger;
        private String path = @"data.json";
        private String data = "";
        private String capturedResponseContent = "";

        public ContentController(ILogger<ContentController> logger)
        {
            _logger = logger;
        }

        [HttpGet("")]
        public async Task<string> Domain(string domain,
            Int32 clientId,
            Int32 requestId)
        {
            try
            {
                checkPathExistence(path);
                // Read a text file line by line.  
                string[] lines = System.IO.File.ReadAllLines(path);

                foreach (string line in lines)
                    data = data + line;
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nException Caught In Domain!");
                Console.WriteLine("Message :{0} ", ex.Message);
            }

            FilterJSON deserializedProduct = JsonConvert.DeserializeObject<FilterJSON>(data);

            foreach (string name in deserializedProduct.Domains)
            {
                if ($"http://www.{name}" == domain ||
                $"www.{name}" == domain ||
                $"{name}" == domain)
                {
                    Redirect("http://172.30.0.5:3000/forbidden");
                }
            }

            domain = ConvertToUTF8Standard(domain);

            // Make HTTP request, yay! Finally
            return await HttpInvokeGetAsync(domain, clientId, requestId);
        }

        private async Task<string>
            HttpInvokeGetAsync(string uri,
            Int32 clientId,
            Int32 requestId)
        {
            // HttpClient is intended to be instantiated once per application, rather than per-use. See Remarks.
            HttpClient client = new HttpClient();

            try
            {
                // https://stackoverflow.com/questions/6045343/how-to-make-an-asynchronous-method-return-a-value
                capturedResponseContent = await AsyncResourceAlloc(client, uri);
            }
            catch (Exception e)
            {
                Console.WriteLine("\nException Caught In HttpInvokeGetAsync while retrieving from AsyncResourceAlloc!");
                Console.WriteLine("Message :{0} ", e.Message);

                Redirect("http://172.30.0.5:3000/badrequest");
            }

            try
            {
                Task<string> t2 = Task<string>.Run(() =>
                {
                    return capturedResponseContent;
                });

                return await t2;
            }
            catch (Exception e)
            {
                Console.WriteLine("\nException Caught In HttpInvokeGetAsync while initiating Response object!");
                Console.WriteLine("Message :{0} ", e.Message);

                // Redirecting to bad request
                Redirect("http://172.30.0.5:3000/badrequest");                
            }

            return "No content found";
        }

        static async Task<string> AsyncResourceAlloc(HttpClient client, string uri)
        {
            // Call asynchronous network methods in a try/catch block to handle exceptions.
            try
            {
                // string responseBody = await client.GetStringAsync(uri);
                // Use the above if only the actual content body is required

                Task<string> t1 = Task<string>.Run(async () =>
                {
                    HttpResponseMessage response = await client.GetAsync(uri);
                    response.EnsureSuccessStatusCode();
                    Console.WriteLine($"{uri} {response.Headers}");
                    string responseBody = await response.Content.ReadAsStringAsync();
                    return responseBody;
                });

                return await t1;
            }
            catch (HttpRequestException e)
            {
                Task<string> t2 = Task<string>.Run(() =>
                {
                    Console.WriteLine("\nException Caught In AsyncResourceAlloc!");
                    Console.WriteLine("Message :{0} ", e.Message);
                    return e.Message;
                });

                return await t2;
            }
        }

        static void checkPathExistence(string path)
        {
            if (!System.IO.File.Exists(path))
            {
                var errorResponse = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Data.JSON file was not found: ", System.Text.Encoding.UTF8, "text/plain"),
                    StatusCode = HttpStatusCode.NotFound
                };
                throw new HttpResponseException(errorResponse);
            }
        }

        static private String ConvertToUTF8Standard(string uri)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
                HasHeaderRecord = false,
                HeaderValidated = null,
                BadDataFound = null,
            };

            using (var reader = new StreamReader("encodings.csv"))
            using (var csv = new CsvReader(reader, config))
            {
                while (csv.Read())
                {
                    server.Encoding record = csv.GetRecord<Encoding>();
                    if (uri.Contains(record.UTF8String))
                    {
                        uri.Replace(record.UTF8String, record.UTF8Encoding);
                    }
                }
                return uri;
            }
        }
    }
}