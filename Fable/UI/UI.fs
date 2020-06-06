module UI

// open System
open Fable.Core
// open Fable.Core.JS
open Fable.Core.JsInterop
open Fable.Import
open Fable.PowerPack
open Fable.PowerPack.Fetch
// open Fable.PowerPack.Result
open Thoth.Json
open Browser.Dom
open Browser.Types
//open Fable.Core.Util
open Model
open Fable.PowerPack.PromiseImpl
open Fable.Import.Axios
open Fable.Import.Axios.Globals
//open Fable.Axios

type IAlert =
    abstract triggerAlert : message:string -> unit
    abstract postToServer : url:string -> jsonObj:obj -> unit
    abstract someString: string

[<ImportAll("./js/alert.js")>]
let mylib: IAlert = jsNative

[<Emit("alert('$0')")>]
let alert msg = jsNative

// [<Emit("alert(JSON.stringify('$0'))")>]
// let alertJSON obj = jsNative

// [<Emit("postToServer(\"$0\", \"$1\")")>]
// let postToServer (url:string) (strData:string) : unit = jsNative

let private getElementByIdAbstract (window:Window) id = 
    unbox window.document.getElementById id

let getButtonElementById (id:string):Browser.Types.HTMLButtonElement = 
    getElementByIdAbstract window id

let getInputElementById (id:string):Browser.Types.HTMLInputElement = 
    getElementByIdAbstract window id





let submitForm () = 
    mylib.triggerAlert("hello" + mylib.someString)
    // let txtName = getInputElementById("txtName")
    // let postData = { Name = txtName.value }
    //postToServer "abc" "def" |> ignore


type ResultType = Result<string option, exn>

let private parseResponse (response : AxiosXHR<string>) : ResultType =
    Ok (Some "str")

let handleError error : ResultType =
    Ok (Some "err")

let private catchAxiosError (error : AxiosError<_, _>) =
    match error with
    | ErrorResponse r ->
        match r.response.status with
        | 403
        | 404 ->
            Ok None
        | _ ->
            handleError error
    | _ ->
        handleError error

let fetchWithAxios url =
    axios.get(url)
    |> Promise.map parseResponse
    |> Promise.catchAxios catchAxiosError

let btnSave_Click (getState:unit -> ContactInfoState) (_:MouseEvent) = 
    //let target:obj = e.target
    //mylib.triggerAlert(Thoth.Json.Encode.toString 0 e)
    //let state = getState ()
    
    //printfn "state().Name: %s" state.Name
    //mylib.postToServer "http://localhost:5000/employee/create" state

    submitForm ()
    //alert(msg)
    //()

// let btnSave = getButtonElementById "btnSave" 
// btnSave.onclick <- btnSave_Click


//alert("hello");

printfn "page loaded"