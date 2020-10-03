package main 

import (
	//"fmt"
	//"log"
	"net/http"
	"github.com/rs/cors"
	//"./getString"
)

func employeeCreate(w http.ResponseWriter, r *http.Request) {
	w.Header().Set("Content-Type", "application/json")
	w.Write([]byte("{\"hello\": \"world\"}"))

	// enableCors(&w)
	// if (*r).Method == "OPTIONS" {
	// 	//fmt.Fprintf(w, "")
	// } else {
	// 	fmt.Fprintf(w, "{ \"ValidationResult\": 0 }")
	// }	
}

func enableCors(w *http.ResponseWriter) {
	//(*w).Header().Set("Access-Control-Allow-Origin", "*")
	//(*w).Header().Set("Access-Control-Allow-Methods", "POST")
	(*w).Header().Set("Access-Control-Allow-Origin", "[\"*\"]")
    (*w).Header().Set("Access-Control-Allow-Methods", "POST, GET, OPTIONS, PUT, DELETE")
    (*w).Header().Set("Access-Control-Allow-Headers", "Accept, Content-Type, Content-Length, Accept-Encoding, X-CSRF-Token, Authorization")
}

//todo: 
//	* listen on same port as existing back end
//	* map URL of back end onto handler func
//  * write hardcoded response (success message)

func main(){
	//fmt.Println("Hello, World!")
	//http.HandleFunc("/employee/create/", employeeCreate)
	//log.Fatal(http.ListenAndServe(":7000", nil))
	//fmt.Println(getString.GetHelloMessage())

	mux := http.NewServeMux()
    mux.HandleFunc("/employee/create", employeeCreate)

    // cors.Default() setup the middleware with default options being
    // all origins accepted with simple methods (GET, POST). See
    // documentation below for more options.
    handler := cors.Default().Handler(mux)
    http.ListenAndServe(":7000", handler)
}