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

type IAlert =
    abstract triggerAlert : message:string -> unit
    abstract someString: string

[<ImportAll("./js/alert.js")>]
let mylib: IAlert = jsNative

[<Emit("alert('$0')")>]
let alert msg = jsNative

// [<Emit("postToServer(\"$0\", \"$1\")")>]
// let postToServer (url:string) (strData:string) : unit = jsNative

let private getElementByIdAbstract (window:Window) id = 
    unbox window.document.getElementById id

let getButtonElementById (id:string):Browser.Types.HTMLButtonElement = 
    getElementByIdAbstract window id

let getInputElementById (id:string):Browser.Types.HTMLInputElement = 
    getElementByIdAbstract window id


// let someFunc e = 
//     alert("returned value")

//let btnMain = getButtonElementById "btnMain" 
//btnMain.onclick <- someFunc

//let submitForm = 
    // let txtName = getInputElementById("txtName")
    // let postData = { Name = txtName.value }
    //postToServer "abc" "def" |> ignore

//alert("hello");
mylib.triggerAlert("hello" + mylib.someString)
printfn "page loaded"