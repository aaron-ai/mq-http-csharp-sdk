using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Aliyun.MQ.Model;
using Aliyun.MQ.Util;
using NLog;

namespace Aliyun.MQ.Sample
{
    public class ProducerSample
    {
        private static readonly Logger Logger = MqLogManager.Instance.GetCurrentClassLogger();

        private static string _endpoint = Environment.GetEnvironmentVariable("endpoint") ?? "http://1266540465474762.mqrest.cn-hangzhou.aliyuncs.com";
        private static string _accessKeyId = Environment.GetEnvironmentVariable("ak") ?? "ak";
        private static string _secretAccessKey = Environment.GetEnvironmentVariable("sk") ?? "sk";
        private static string _topicName = Environment.GetEnvironmentVariable("topic") ?? "topic";
        private static string _instanceId = Environment.GetEnvironmentVariable("instanceId") ?? "instanceId";

        public static long currentSeconds = 0;
        
        public static long totalCostTime = 0;
        public static long totalCounts = 0;
        public static long maxCostTime = 0;

        private static long _counter = 0;
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(0);

        private static MQClient _client = new MQClient(_accessKeyId, _secretAccessKey, _endpoint, 0);
        static MQProducer producer = _client.GetProducer(_instanceId, _topicName);
        
        public static bool InterlockedExchangeIfGreaterThan(ref long location, long comparison, long newValue)
        {
            long initialValue;
            do
            {
                initialValue = location;
                if (initialValue >= comparison) return false;
            }
            while (Interlocked.CompareExchange(ref location, newValue, initialValue) != initialValue);
            return true;
        }

        static void Send(Object o)
        {
            var startTime = DateTime.Now;
            var endTime = DateTime.Now;
            try
            {
                // _semaphore.Wait();
                TopicMessage sendMsg = new TopicMessage("dfadfadfadf");
                // 设置KEY
                sendMsg.MessageKey = "MessageKey";

                TopicMessage result = producer.PublishMessage(sendMsg);
                // Console.WriteLine(result);
                Interlocked.Increment(ref _counter);
                // Console.WriteLine("publish message success:" + result);
            }
            catch (Exception ex)
            {
                endTime = DateTime.Now;
                Console.WriteLine("============error===========");
                Console.WriteLine(startTime);
                Console.WriteLine(endTime);
                Console.Write(ex);
            }
        }

        static void Main(string[] args)
        {
            Logger.Info("====================start==================");

            var tps_ = Environment.GetEnvironmentVariable("tps") ?? "100";
            var tps = int.Parse(tps_);

            var maxTps_ = Environment.GetEnvironmentVariable("maxTps") ?? "100";
            var maxTps = int.Parse(maxTps_);
            
            var workerThreads_ = Environment.GetEnvironmentVariable("workersThreads") ?? "0";
            var workerThreads = int.Parse(workerThreads_);
            var completionPortThreads_ = Environment.GetEnvironmentVariable("completionPortThreads") ?? "0";
            var completionPortThreads = int.Parse(completionPortThreads_);
            
            if (workerThreads > 0 && completionPortThreads > 0)
            {
                ThreadPool.SetMinThreads(workerThreads, completionPortThreads);
            }
            
            var period_ = Environment.GetEnvironmentVariable("period") ?? "0";
            var period = int.Parse(period_);

            Task.Run(async () =>
            {
                while (true)
                {
                    Interlocked.Increment(ref currentSeconds);
                    if (period > 0 && currentSeconds % period == 0)
                    {
                        for (int i = 0; i < maxTps; i++)
                        {
                            ThreadPool.QueueUserWorkItem(Send);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < tps; i++)
                        {
                            ThreadPool.QueueUserWorkItem(Send);
                        }
                    }
                    
                    int minWorker, minIOC, maxWorker, maxIOC;
                    ThreadPool.GetMinThreads(out minWorker, out minIOC);
                    ThreadPool.GetMaxThreads(out maxWorker, out maxIOC);
                    Logger.Info($"Send {_counter} messages successfully, average RT: {totalCostTime*1.0/totalCounts}, max RT: {maxCostTime}, currentSeconds: {currentSeconds}, currentThread:{ThreadPool.ThreadCount}, minWorker: {minWorker}, minIOC: {minIOC}, maxWorker: {maxWorker}, maxIOC: {maxIOC}");
                    Console.WriteLine($"Send {_counter} messages successfully, average RT: {totalCostTime*1.0/totalCounts}, max RT: {maxCostTime}, currentSeconds: {currentSeconds}, currentThread:{ThreadPool.ThreadCount}, minWorker: {minWorker}, minIOC: {minIOC}, maxWorker: {maxWorker}, maxIOC: {maxIOC}");

                    Interlocked.Exchange(ref _counter, 0);
                    Interlocked.Exchange(ref totalCostTime, 0);
                    Interlocked.Exchange(ref totalCounts, 0);
                    Interlocked.Exchange(ref maxCostTime, 0);
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            });
            // ThreadPool.GetMaxThreads(out a, out b);
            // int minA;
            // int minB;
            // int maxA;
            // int maxB;
            // ThreadPool.GetMinThreads(out minA, out minB);
            // ThreadPool.GetMaxThreads(out maxA, out maxB);
            // Console.WriteLine(minA);
            // Console.WriteLine(minB);
            // Console.WriteLine(maxA);
            // Console.WriteLine(maxB);

            // for (int i = 0; i < task; i++)
            // {
            //     ThreadPool.QueueUserWorkItem(Send);
            // }


            // 可以自行调整超时时间，默认为 3s
            // _client.Config.Timeout = TimeSpan.FromSeconds(3);
            // 可以自行调整读写超时时间，默认为 3s
            // _client.Config.ReadWriteTimeout = TimeSpan.FromSeconds(3);

            // Console.WriteLine("=================start=================");
            Thread.Sleep(999999999);
        }
    }
}