module Model
open Browser.Types

type ContactInfoProps = {
    Name : string;
    OnClick: MouseEvent -> unit
}

type ContactInfoState = {
    Name : string
}