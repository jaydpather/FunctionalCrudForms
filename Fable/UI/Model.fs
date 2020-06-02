module Model
open Browser.Types

type ContactInfoState = {
    Name : string
}


type ContactInfoProps = {
    Name : string;
    OnClick: (unit -> ContactInfoState) -> MouseEvent -> unit
}
