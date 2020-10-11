package main 

import (
	"log"
	"net/http" 

	"./MessageQueue"

	"github.com/rs/cors"
)

func failOnError(err error, msg string) {
	if err != nil {
			log.Fatalf("%s: %s", msg, err)
	}
}

func employeeCreate(w http.ResponseWriter, r *http.Request) {
	rpcInfo := messageQueue.CreateRpcInfo()
	defer rpcInfo.Connection.Close()
	defer rpcInfo.AmqpChannel.Close()

	//GOOGLE: GO unit tests and injectability
	//	* EXAMPLE PROJECT: install GoMock, MockGen, run MockGen
	//create employee struct
	//GOOGLE: Go convert to JSON
	//	* EXAMPLE PROJECT
	//port validation function to Go
	//don't write to MQ unless valid
	//if invalid: return error response
	err := messageQueue.PublishMessage(rpcInfo, "{ \"FirstName\": \"Will\", \"LastName\":\"Smith\" }")
	failOnError(err, "failed to publish message")	
	
	responseStr := "{ \"ValidationResult\": -1 }"
	for response:= range rpcInfo.ResponseChannel{
		if(response.CorrelationId == rpcInfo.CorrelationId){
			responseStr = string(response.Body)
			break
		}
	}
	
	w.Header().Set("Content-Type", "application/json")
	w.Write([]byte(responseStr))
}

func main(){
	mux := http.NewServeMux()
    mux.HandleFunc("/employee/create", employeeCreate)

    handler := cors.Default().Handler(mux)
    http.ListenAndServe(":7000", handler)
}