using StackExchange.Redis;
public class RedisEndpoint
{

    protected static ConfigurationOptions redisConfiguration;
    protected static ConnectionMultiplexer multiplexer;
    protected ISubscriber connection;

    public RedisEndpoint(string url, ushort port)
    {
        if (multiplexer == null)
        {
            string host = url + ":" + port.ToString()+",";
            string redisConfiguration2 = host + "Password=password,Ssl=false,ConnectTimeout=6000,SyncTimeout=6000,AllowAdmin=true";
            multiplexer = ConnectionMultiplexer.Connect(redisConfiguration2);
        }
    }

    static int Main()
    {
        return 0;
    }
}

public class Publisher : RedisEndpoint
{
    public Publisher(string url, ushort port) : base(url, port)
    {
        connection = multiplexer.GetSubscriber();
    }
    public void Publish(string channelName, string msg)
    {
        connection.PublishAsync(channelName, msg, flags: CommandFlags.FireAndForget);
    }
}

public class Subscriber : RedisEndpoint
{
    public ChannelMessageQueue msgQueue;
    public Subscriber(string url, ushort port) : base(url, port)
    {
        connection = multiplexer.GetSubscriber();
    }
    public void SubscribeTo(string channelName)
    {
        msgQueue = connection.Subscribe(channelName);
    }
}