package main 

import (
	"net/http" 

	"./MessageQueue"

	"github.com/rs/cors"
)

func employeeCreate(w http.ResponseWriter, r *http.Request) {
	messageQueue.CreateRpcInfo()

	w.Header().Set("Content-Type", "application/json")
	w.Write([]byte("{ \"ValidationResult\": 0 }"))
}

func main(){
	mux := http.NewServeMux()
    mux.HandleFunc("/employee/create", employeeCreate)

    handler := cors.Default().Handler(mux)
    http.ListenAndServe(":7000", handler)
}