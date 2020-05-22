module ContactInfoComponent

open Fable.Core
open Fable.Core.Util
open Fable.Core.DynamicExtensions
open Fable.Core.Extensions
open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open Browser
open Browser.Types



type ContactInfoProps = {
    Name : string
}

type ContactInfoState = {
    Name : string
}

type ContactInfoComponent(initialProps) = 
    inherit Component<ContactInfoProps, ContactInfoState>(initialProps)
    override this.render() = 
        
        div [] [ 
            str "Name: "
            input [
                Type "input"
                Id "txtName"
            ]
        ]