package messageQueue

import (
	"log"

	"github.com/streadway/amqp"
	"github.com/google/uuid"
)

type RpcInfo struct {
	Connection 		*amqp.Connection
	AmqpChannel		*amqp.Channel
	Queue			amqp.Queue
	ResponseChannel	<-chan amqp.Delivery
	CorrelationId   string
}

func failOnError(err error, msg string) {
	if err != nil {
			log.Fatalf("%s: %s", msg, err)
	}
}

func CreateRpcInfo() *RpcInfo {
	rpcInfo := new(RpcInfo)

	conn, err := amqp.Dial("amqp://guest:guest@localhost:5672/")
	failOnError(err, "Failed to connect to RabbitMQ")
	rpcInfo.Connection = conn

	ch, err := rpcInfo.Connection.Channel()
	failOnError(err, "Failed to open a channel")
	rpcInfo.AmqpChannel = ch

	q, err := ch.QueueDeclare(
		"",    // name
		false, // durable
		false, // delete when unused
		true,  // exclusive
		false, // noWait
		nil,   // arguments
	)
	failOnError(err, "Failed to declare a queue")
	rpcInfo.Queue = q

	responseChannel, err := ch.Consume(
		q.Name, // queue
		"",     // consumer
		true,   // auto-ack
		false,  // exclusive
		false,  // no-local
		false,  // no-wait
		nil,    // args
	)
	failOnError(err, "Failed to register a consumer")
	rpcInfo.ResponseChannel = responseChannel

	guid := uuid.New()
	rpcInfo.CorrelationId = guid.String()

	return rpcInfo
}

func PublishMessage(rpcInfo *RpcInfo, msg string) error {
	return rpcInfo.AmqpChannel.Publish(
		"",          // exchange
		"employee", // routing key
		false,       // mandatory
		false,       // immediate
		amqp.Publishing {
			ContentType:   "text/plain",
			CorrelationId: rpcInfo.CorrelationId,
			ReplyTo:       rpcInfo.Queue.Name,
			Body:          []byte(msg),
		})
}