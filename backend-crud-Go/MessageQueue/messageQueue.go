package messageQueue

import (
	"log"

	"github.com/streadway/amqp"
)

type RpcInfo struct {
	Connection 		*amqp.Connection
	AmqpChannel		*amqp.Channel
	Queue			amqp.Queue
	SendChannel		<-chan amqp.Delivery
}

func failOnError(err error, msg string) {
	if err != nil {
			log.Fatalf("%s: %s", msg, err)
	}
}

func CreateRpcInfo() {
	rpcInfo := new(RpcInfo)

	conn, err := amqp.Dial("amqp://guest:guest@localhost:5672/")
	failOnError(err, "Failed to connect to RabbitMQ")
	rpcInfo.Connection = conn
	defer conn.Close()

	ch, err := rpcInfo.Connection.Channel()
	failOnError(err, "Failed to open a channel")
	rpcInfo.AmqpChannel = ch
	defer ch.Close()

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

	sendChannel, err := ch.Consume(
		q.Name, // queue
		"",     // consumer
		true,   // auto-ack
		false,  // exclusive
		false,  // no-local
		false,  // no-wait
		nil,    // args
	)
	failOnError(err, "Failed to register a consumer")
	rpcInfo.SendChannel = sendChannel
}