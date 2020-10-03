package main 

import (
	"fmt"
	"log"
	"net/http"
	//"./getString"
)

func handler(w http.ResponseWriter, r *http.Request) {
	fmt.Fprintf(w, "this is the response")
}

func main(){
	//fmt.Println("Hello, World!")
	http.HandleFunc("/", handler)
	log.Fatal(http.ListenAndServe(":8080", nil))
	//fmt.Println(getString.GetHelloMessage())
}