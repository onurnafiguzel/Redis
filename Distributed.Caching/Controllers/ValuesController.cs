using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;

namespace Distributed.Caching.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ValuesController : ControllerBase
	{
		readonly IDistributedCache distributedCache;

		public ValuesController(IDistributedCache distributedCache)
		{
			this.distributedCache = distributedCache;
		}

		[HttpGet("set")]
		public async Task<IActionResult> Set(string name, string surName)
		{
			await distributedCache.SetStringAsync("name", name, options: new()
			{
				AbsoluteExpiration = DateTime.Now.AddSeconds(30),
				SlidingExpiration = TimeSpan.FromSeconds(3)
			});

			await distributedCache.SetAsync("surName", Encoding.UTF8.GetBytes(surName));

			return Ok();
		}

		[HttpGet("get")]
		public async Task<IActionResult> Get()
		{
			var name = await distributedCache.GetStringAsync("name");
			var surNameBinary = await distributedCache.GetAsync("surName");

			var surName = Encoding.UTF8.GetString(surNameBinary);

			return Ok(new
			{
				name,
				surName
			});
		}
	}
}
