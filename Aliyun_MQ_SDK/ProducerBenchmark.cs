using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Aliyun.MQ;
using Aliyun.MQ.Model;
using Aliyun.MQ.Util;
using NLog;

namespace Aliyun_MQ_SDK
{
    public class ProducerBenchmark
    {
        private static readonly Logger Logger = MqLogManager.Instance.GetCurrentClassLogger();
        private static long _successCounter = 0;
        private static long _failureCounter = 0;
        private static long _currentSeconds = 0;

        private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(0);

        private static readonly BlockingCollection<Task<TopicMessage>> Tasks =
            new BlockingCollection<Task<TopicMessage>>();

        public static void QuickStart()
        {
            var endpoint = Environment.GetEnvironmentVariable("endpoint") ?? "endpoint";
            var ak = Environment.GetEnvironmentVariable("ak") ?? "ak";
            var sk = Environment.GetEnvironmentVariable("sk") ?? "sk";
            var topic = Environment.GetEnvironmentVariable("topic") ?? "topic";
            var instanceId = Environment.GetEnvironmentVariable("instanceId") ?? "instanceId";
            var tpsString = Environment.GetEnvironmentVariable("tps") ?? "1";
            var periodString = Environment.GetEnvironmentVariable("period") ?? "30";
            var tpsMaxString = Environment.GetEnvironmentVariable("tpsMax") ?? "500";


            MQClient _client = new MQClient(ak, sk, endpoint);
            _client.Config.MaxConnectionsPerServer = 1;
            MQProducer producer = _client.GetProducer(instanceId, topic);

            int tps = Int32.Parse(tpsString);
            int period = Int32.Parse(periodString);
            int tpsMax = Int32.Parse(tpsMaxString);
            Task.Run(async () =>
            {
                while (true)
                {
                    var seconds = Interlocked.Increment(ref _currentSeconds);
                    if (period > 0 && tpsMax > 0 && seconds % period == 0)
                    {
                        Semaphore.Release(tpsMax);
                    }
                    else
                    {
                        Semaphore.Release(tps);
                    }

                    var success = Interlocked.Exchange(ref _successCounter, 0);
                    var failure = Interlocked.Exchange(ref _successCounter, 0);
                    ThreadPool.GetMinThreads(out var minWorker, completionPortThreads: out var minIoc);
                    ThreadPool.GetMaxThreads(out var maxWorker, out var maxIoc);
                    Logger.Info(
                        $"success={success}, failure={failure}, seconds={seconds}, completedWorkItemCount={ThreadPool.CompletedWorkItemCount}, currentThread={ThreadPool.ThreadCount}, minWorker={minWorker}, minIOC={minIoc}, maxWorkder={maxWorker}, maxIOC={maxIoc}");
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            });

            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        Task<TopicMessage> task = Tasks.Take();
                        await task;
                        Interlocked.Increment(ref _successCounter);
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e, "Failed to send message");
                        Interlocked.Increment(ref _failureCounter);
                    }
                }
            });

            while (true)
            {
                Semaphore.Wait();
                var sendMsg = new TopicMessage("dfadfadfadf");
                var task = producer.PublishMessageAsync(sendMsg);
                Tasks.Add(task);
            }

            Task.WhenAll(Tasks).Wait();
        }
    }
}