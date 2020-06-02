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
open Model

type ContactInfoComponent(initialProps) = 
    inherit Component<ContactInfoProps, ContactInfoState>(initialProps)

    let txtName_OnChange (e:Event) =
        printfn "txtName_OnChange: %s" (e.Value)

    // member this.btnSave_Click(_:MouseEvent) = 
    //     UI.btnSave_Click this.state
    override this.render() = 
        
        div [] [ 
            str "Name: "
            input [ 
                Type "input" 
                Id "txtName" 
                OnChange txtName_OnChange
            ]
            br []
            button [ 
                Type "button" 
                Id "btnSave"
                OnClick (this.props.OnClick (fun () -> this.state))
            ] [
                str "Save"
            ]
        ]

let inline ContactInfoComponentDec props = ofType<ContactInfoComponent,_,_> props []