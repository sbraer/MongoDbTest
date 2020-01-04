using Microsoft.AspNetCore.Mvc;
using MongoDbTest.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Linq;

namespace MongoDbTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InfoController : ControllerBase
    {
        private readonly IBookstoreDatabaseSettings _settings;

        public InfoController(IBookstoreDatabaseSettings settings)
        {
            _settings = settings;
        }

        [HttpGet]
        public IActionResult Get()
        {
            JObject envList = new JObject();
            Environment.GetEnvironmentVariables().Cast<DictionaryEntry>().ToList()
				.OrderBy(t=>t.Key.ToString().ToLower()).ToList()
				.ForEach(t => envList.Add(new JProperty(t.Key.ToString(), t.Value)));

            JObject output =
                new JObject(
                    new JProperty("Settings",
                        new JObject(
                            new JProperty("BooksCollectionName", _settings.BooksCollectionName),
                            new JProperty("DatabaseName", _settings.DatabaseName),
                            new JProperty("ReplicaSet", _settings.ReplicaSet)
                            )
                    ),
                    new JProperty("Environment", envList)
                );

            return Ok(output.ToString());
        }
    }
}