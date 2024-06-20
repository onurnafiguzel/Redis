using StackExchange.Redis;

namespace Redis.Sentinel.Services;

public class RedisService
{
	static ConfigurationOptions sentinelOptions => new()
	{
		EndPoints =
		{
			{"localhost", 6383},
			{"localhost", 6384},
			{"localhost", 6385}
		},
		CommandMap = CommandMap.Sentinel,
		AbortOnConnectFail = false
	};

	static ConfigurationOptions masterOptions => new()
	{
		AbortOnConnectFail = false
	};

	static public async Task<IDatabase> RedisMasterDatabase()
	{
		ConnectionMultiplexer sentinelConnection = await ConnectionMultiplexer.SentinelConnectAsync(sentinelOptions);

		System.Net.EndPoint masterEndpoint = null; // Docker internal IP

		foreach (System.Net.EndPoint endPoint in sentinelConnection.GetEndPoints())
		{
			IServer server = sentinelConnection.GetServer(endPoint);
			if (!server.IsConnected) continue;

			masterEndpoint = await server.SentinelGetMasterAddressByNameAsync("mymaster");
			break;
		}

		var localMasterIp = masterEndpoint.ToString() switch //Docker local ips
		{
			"172.18.0.8:6379" => "localhost:6379",
			"172.18.0.7:6379" => "localhost:6380",
			"172.18.0.6:6379" => "localhost:6381",
			"172.18.0.3:6379" => "localhost:6382",
			"172.18.0.5:6379" => "localhost:6383",
			"172.18.0.4:6379" => "localhost:6384",
			"172.18.0.2:6379" => "localhost:6385",
			"172.17.0.2:6379" => "localhost:6386",
		};

		ConnectionMultiplexer masterConnection = await ConnectionMultiplexer.ConnectAsync(localMasterIp);
		IDatabase database = masterConnection.GetDatabase();
		return database;
	}
}
