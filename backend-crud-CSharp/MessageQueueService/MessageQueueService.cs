using System;
using System.Text;
using System.Collections.Concurrent;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RebelSoftware.MessageQueue
{
    class RPCInfo
    {
        public IConnection Connection { get; set; }
        public IModel Channel { get; set; }
        public EventingBasicConsumer Consumer { get; set; }
        public BlockingCollection<string> RespQueue { get; set; }
        public IBasicProperties Props { get; set; }
    }

    internal class MessageQueueService : IMessageQueueService
    {
        private void onMessageReceived (BlockingCollection<string> respQueue, string correlationId, BasicDeliverEventArgs ea)
        { 
            var response = Encoding.UTF8.GetString(ea.Body);
            if(ea.BasicProperties.CorrelationId == correlationId)
            {
                respQueue.Add(response);
            }
        }

        private RPCInfo CreateRpcInfoObject()
        { 
            var factory = new ConnectionFactory();
            factory.HostName = "localhost"; //todo: caller gets host name from config file and passes it to createMessageQueuer
            var respQueue = new BlockingCollection<string>();
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            var consumer = new EventingBasicConsumer(channel);
            var props = channel.CreateBasicProperties();
            props.CorrelationId = Guid.NewGuid().ToString();
            props.ReplyTo = channel.QueueDeclare().QueueName;
            //consumer.Received.Add(onMessageReceived, respQueue, props.CorrelationId);
            consumer.Received += (sender, eventArgs) => {
                onMessageReceived(respQueue, props.CorrelationId, eventArgs);
            };
            
            var retVal = new RPCInfo
            {
                Connection = connection,
                Channel = channel,
                Consumer = consumer,
                RespQueue = respQueue,
                Props = props
            };

            return retVal;
        }

        private void PublishToMsgQueue (string jsonString, RPCInfo rpcInfo)
        { 
            var msgBytes = Encoding.UTF8.GetBytes(jsonString);
            rpcInfo.Channel.BasicPublish(exchange: String.Empty, routingKey: "employee", basicProperties: rpcInfo.Props, body: msgBytes); //todo: routing key from config file
            rpcInfo.Channel.BasicConsume(consumer: rpcInfo.Consumer, queue: rpcInfo.Props.ReplyTo, autoAck: true);
        }

        public string WriteMessageAndGetResponse(string message)
        {
            var rpcInfo = CreateRpcInfoObject();
            PublishToMsgQueue(message, rpcInfo);

            var mqResponse = String.Empty;
            if(rpcInfo.RespQueue.TryTake(out mqResponse, 30000))
            {
                return mqResponse;
            } //todo: timeout value from config file
            else
            {
                var errMsg = $"timeout while waiting for microservice response to RabbitMQ message '{message}'";
                throw new Exception(errMsg);
            }
        }
    }
}
