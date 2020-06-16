module ContactInfoPageComponent

open Fable.Core
open Fable.Core.Util
open Fable.Core.DynamicExtensions
open Fable.Core.Extensions
open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open Browser
open Browser.Types
open Model
open ContactInfoComponent


type ContactInfoPageComponent(initialProps) = 
    inherit Component<ContactInfoProps, ContactInfoState>(initialProps)

    
    override this.render() = 
        div [] [
            ContactInfoComponentDec { Name = "Maya Rudolf"; OnClick = UI.btnSave_Click }
        ]
        