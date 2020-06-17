import React, { Component } from 'react'
import { validateFirstName } from '../fable-include/UI'
const axios = require('axios');

export default class extends Component {
    validationEnum = {
        Success: 0,
        FirstNameBlank: 1,
        UnknownError: 64,
        New: 128
    };

    state = { 
        name:"", 
        validationState: this.validationEnum.New
    };

    self = this;

    //todo: remove this function from component
    submitForm = async(event) => {
        //alert(JSON.stringify(ui));
        var data = { Name:this.state.name }
        var self = this;

        if(this.validationEnum.Success == validateFirstName(this.state.name)){
            axios.post("http://localhost:7000/employee/create", JSON.stringify(data))
            .then(function (response) {
                //alert(response.data);
                //alert(JSON.stringify(response.data));
                self.setState({
                    name: self.state.name,
                    validationState: self.validationEnum.Success
                });
            })
            .catch(function(error){
                //alert("error:" + error);
                self.setState({
                    name: self.state.name,
                    validationState: self.validationEnum.UnknownError
                });
            })
        }
        else{
            self.setState({
                name: self.state.name,
                validationState: self.validationEnum.FirstNameBlank
            });
        }
    }

    render () {
        console.log("rendering") //todo: why do we see this log message on both server and console when doing SSR?
        const formStyle = {
            "marginTop": "10px",
            "border": "solid 1px #BBB",
            "padding": "10px",
        };
        
        const successMsgStyle = {
            "color": "#030",
            "background-color": "#DFD",
            "padding": "3px",
            "width": "20%",
          };
      
        const failureMsgStyle = {
            "color": "#300",
            "background-color": "#FDD",
            "padding": "3px",
            "width": "40%",
          };
        
        return(
            <div style={formStyle}>
                <h1>
                    Create Employee
                </h1>
                Name: 
                <input type="text" id="txtName" value={this.state.name} 
                    //todo: move onChange handler to new method (too unreadable here)
                    onChange =  { e => this.setState({ 
                        name:e.target.value, 
                        validationState: this.state.validationState
                    }) } 
                />
                <br />
                <br />
                <button type="button" onClick={this.submitForm}>Save</button>
                <br />
                <br />
                {
                    (this.state.validationState == this.validationEnum.Success) ?
                        <div id="divSuccessMsg" style={successMsgStyle}>
                            Saved successfully.
                        </div>
                    :
                        null
                }
                {
                    (0 != (this.state.validationState & this.validationEnum.FirstNameBlank)) ?
                        <div id="divFailureMsg" style={failureMsgStyle}>
                            First name cannot be blank.
                        </div>
                    :
                        null
                }
                {
                    (0 != (this.state.validationState & this.validationEnum.UnknownError)) ?
                        <div id="divFailureMsg" style={failureMsgStyle}>
                            Unable to save: unknown error occurred.
                        </div>
                    :
                        null
                }
            </div>
        );
    }
}