module UI

// open System
open Fable.Core
// open Fable.Core.JS
// open Fable.Core.JsInterop
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

[<Emit("alert('$0')")>]
let alert msg = jsNative

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

// let submitForm = 
//     let txtName = getInputElementById("txtName")
//     let postData = { Name = txtName.value }
//     0

alert("hello");
printfn "page loaded"