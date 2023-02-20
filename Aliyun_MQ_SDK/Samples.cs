using System;
using System.Threading;
using System.Threading.Tasks;
using Aliyun_MQ_SDK;
using Aliyun.MQ.Model;
using Aliyun.MQ.Util;
using NLog;

namespace Aliyun.MQ.Sample
{
    public class ProducerSample
    {
        private static readonly Logger Logger = MqLogManager.Instance.GetCurrentClassLogger();
        private static long _successCounter = 0;
        private static long _failureCounter = 0;

        static void Main(string[] args)
        {
            ProducerBenchmark.QuickStart();

            // var endpoint = Environment.GetEnvironmentVariable("endpoint") ?? "endpoint";
            // var ak = Environment.GetEnvironmentVariable("ak") ?? "ak";
            // var sk = Environment.GetEnvironmentVariable("sk") ?? "sk";
            // var topic = Environment.GetEnvironmentVariable("topic") ?? "topic";
            // var instanceId = Environment.GetEnvironmentVariable("instanceId") ?? "instanceId";
            //
            //
            // MQClient _client = new MQClient(ak, sk, endpoint);
            // _client.Config.MaxConnectionsPerServer = 1;
            // MQProducer producer = _client.GetProducer(instanceId, topic);
            //
            // Task.Run(async () =>
            // {
            //     while (true)
            //     {
            //         var successCount = Interlocked.Exchange(ref _successCounter, 0);
            //         var failureCount = Interlocked.Exchange(ref _failureCounter, 0);
            //         ThreadPool.GetMinThreads(out var minWorker, completionPortThreads: out var minIoc);
            //         ThreadPool.GetMaxThreads(out var maxWorker, out var maxIoc);
            //         Logger.Info(
            //             $"successCount: {successCount}, failureCount: {failureCount}, completedWorkItemCount={ThreadPool.CompletedWorkItemCount}, currentThread={ThreadPool.ThreadCount}, minWorker={minWorker}, minIOC={minIoc}, maxWorkder={maxWorker}, maxIOC={maxIoc}");
            //         await Task.Delay(TimeSpan.FromSeconds(1));
            //     }
            // });
            //
            // try
            // {
            //     for (int i = 0; i < int.MaxValue; i++)
            //     {
            //         TopicMessage sendMsg = new TopicMessage("dfadfadfadf");
            //         producer.PublishMessage(sendMsg);
            //         Interlocked.Increment(ref _successCounter);
            //     }
            // }
            // catch (Exception ex)
            // {
            //     Interlocked.Increment(ref _failureCounter);
            //     Logger.Error(ex, "Failed to send message");
            // }
        }
    }
}