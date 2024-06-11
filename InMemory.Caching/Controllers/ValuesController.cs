using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace InMemory.Caching.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ValuesController : ControllerBase
{
	readonly IMemoryCache memoryCache;

	public ValuesController(IMemoryCache memoryCache)
	{
		this.memoryCache = memoryCache;
	}

	[HttpGet("set/{name}")]
	public void Set(string name) => memoryCache.Set("name", name);


	[HttpGet]
	public string Get()
	{
		if (memoryCache.TryGetValue<string>("name", out string name))
		{
			return name.Substring(1);
		}
		return string.Empty;

	}

	[HttpGet("setDate")]
	public void SetDate()
	{
		memoryCache.Set<DateTime>("date", DateTime.UtcNow, options: new()
		{
			AbsoluteExpiration = DateTime.Now.AddSeconds(30),
			SlidingExpiration = TimeSpan.FromSeconds(5)
		});
	}

	[HttpGet("getDate")]
	public DateTime GetDate()
	{
		return memoryCache.Get<DateTime>("date");
	}
}
