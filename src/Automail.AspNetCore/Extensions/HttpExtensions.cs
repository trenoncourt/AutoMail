using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Automail.AspNetCore.Extensions
{
    public static class HttpJsonExtensions
    {
        private static readonly JsonSerializer JsonSerializer = JsonSerializer.Create(new JsonSerializerSettings {
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        });

        public static async Task WriteResponseBodyAsync(this HttpContext httpContext, object value)
        {
            httpContext.Response.ContentType = "application/json";
            using (var writer = new HttpResponseStreamWriter(httpContext.Response.Body, Encoding.UTF8))
            {
                using (var jsonWriter = new JsonTextWriter(writer) { CloseOutput = false, AutoCompleteOnClose = false })
                {
                    JsonSerializer.Serialize(jsonWriter, value);
                }
                await writer.FlushAsync();
            }
        }
 
        public static async Task<T> ReadFromJson<T>(this HttpContext httpContext)
        {
            using (var streamReader = new StreamReader(httpContext.Request.Body))
            {
                using (var jsonTextReader = new JsonTextReader(streamReader) { CloseInput = false })
                {
                    var model = JsonSerializer.Deserialize<T>(jsonTextReader);

                    var results = new List<ValidationResult>();
                    if (Validator.TryValidateObject(model, new ValidationContext(model), results))
                    {
                        return model;
                    }

                    await httpContext.WriteResponseBodyAsync(results);

                    return default(T);
                }
            }
        }
    }
}