module UI

open System
open Fable.Core
open Fable.Import
open Browser.Dom
open Browser.Types
open Fable.Core.Util

[<Emit("alert('$0')")>]
let alert msg = jsNative

let private getElementByIdAbstract (window:Window) id = 
    unbox window.document.getElementById id

let getButtonElementById (id:string):Browser.Types.HTMLButtonElement = 
    getElementByIdAbstract window id


let someFunc e = 
    alert("returned value");

//let btnMain = getButtonElementById "btnMain" 
//btnMain.onclick <- someFunc

alert("hello");
printfn "page loaded"