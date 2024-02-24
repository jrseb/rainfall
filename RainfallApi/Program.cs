using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using RainfallLibrary.Interfaces;
using RainfallLibrary.Services;
using System.Reflection;

namespace RainfallApi
{
	public class Program
	{
		private const string API_SPECIFICATION_NAME = "RainfallApiSpecification";
		private const string API_TITLE = "Rainfall Api";
		private const string API_MAIN_DESCRIPTION = "An API which provides rainfall reading data";
		private const string API_VERSION = "1.0";
		private const string API_CONTACT_NAME = "Sorted";
		private const string API_CONTACT_URL = "http://www.sorted.com";
		private const string API_SHORT_NAME = "Rainfall";
		private const string API_SHORT_DESCRIPTION = "Operation relating to rainfall";
		private const string RAINFALL_LIBRARY_XML = "RainfallLibrary.xml";
		private const string API_LOCALHOST_URL = "http://localhost:3000";

		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// get the actual local host url; always show the first one
			var localHostUrl = builder.WebHost.GetSetting("ASPNETCORE_URLS") ?? string.Empty;
			if (localHostUrl.Contains(';')) localHostUrl = localHostUrl.Split(";").FirstOrDefault();
			if (localHostUrl == string.Empty) localHostUrl = API_LOCALHOST_URL;

			// forcing to use the doc path for all swagger docs
			var docPath = builder.Environment.ContentRootPath + @"\..\Doc\";

			// Add services to the container.
			builder.Services.AddMvc(action =>
			{
				action.ReturnHttpNotAcceptable = true;
				//-- removing other return type
				var jsonFormatter = action.OutputFormatters.OfType<SystemTextJsonOutputFormatter>().FirstOrDefault();
				if (jsonFormatter != null)
				{
					jsonFormatter.SupportedMediaTypes.Remove("text/json");
				}
			});
			builder.Services.AddScoped<IWeatherReport, WeatherReportService>();

			builder.Services.AddControllers();

			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();

			builder.Services.AddSwaggerGen(action =>
			{
				//-- setup header documentation
				action.SwaggerDoc(API_SPECIFICATION_NAME,
					new OpenApiInfo()
					{
						Title = API_TITLE,
						Description = API_MAIN_DESCRIPTION,
						Version = API_VERSION,
						Contact = new OpenApiContact()
						{
							Name = API_CONTACT_NAME,
							Url = new Uri(API_CONTACT_URL)
						},
						Extensions = new Dictionary<string, IOpenApiExtension>()
						{
							{ "tag", new OpenApiObject()
								{
									["name"]  = new OpenApiString(API_SHORT_NAME),
									["description"] = new OpenApiString(API_SHORT_DESCRIPTION)
								}
							}
						}
					});

				action.AddServer(new OpenApiServer() { Url = localHostUrl, Description = API_TITLE });

				//-- utilize generated xml document;
				var xmlDocFile = Path.Combine(docPath, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
				action.IncludeXmlComments(xmlDocFile);

				xmlDocFile = Path.Combine(docPath, RAINFALL_LIBRARY_XML);
				action.IncludeXmlComments(xmlDocFile);
			});

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();

				app.UseSwaggerUI(action =>
				{
					action.SwaggerEndpoint($"/swagger/{API_SPECIFICATION_NAME}/swagger.json", API_SPECIFICATION_NAME);

					//-- default ui
					action.RoutePrefix = "";
				});
			}

			app.UseHttpsRedirection();
			app.UseAuthorization();
			app.MapControllers();
			app.Run();
		}
	}
}