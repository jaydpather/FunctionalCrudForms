import React, { Component } from 'react'
import { ValidationResults$$$get_New, ValidationResults$$$get_Success, ValidationResults$$$get_Saving, ValidationResults$$$get_UnknownError, ValidationResults$$$get_FirstNameBlank } from '../fable-include/Model'

import { validateFirstName } from '../fable-include/Validation'
const axios = require('axios');

export default class extends Component {
    state = { 
        name:"", 
        validationState: ValidationResults$$$get_New()
    };

    self = this;

    //todo: remove this function from component
    submitForm = async(event) => {
        //alert(JSON.stringify(ui));
        var data = { Name:this.state.name }
        var self = this;
        var opResult = validateFirstName(this.state.name)
        if(ValidationResults$$$get_Success() == opResult.ValidationResult){
        //if(true){ //temp: allow posting of blank names, to test server-side validation
            self.setState({
                name: self.state.name,
                validationState: ValidationResults$$$get_Saving() //
            });
            axios.post("http://localhost:7000/employee/create", JSON.stringify(data))
            .then(function (response) {
                //alert(JSON.stringify(response.data));
                self.setState({
                    name: self.state.name,
                    validationState: response.data.ValidationResult
                });
            })
            .catch(function(error){
                //alert("error:" + error);
                self.setState({
                    name: self.state.name,
                    validationState: ValidationResults$$$get_UnknownError()
                });
            })
        }
        else{
            self.setState({
                name: self.state.name,
                validationState: opResult.ValidationResult
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
            "backgroundColor": "#DFD",
            "padding": "3px",
            "width": "20%",
          };
      
        const failureMsgStyle = {
            "color": "#300",
            "backgroundColor": "#FDD",
            "padding": "3px",
            "width": "40%",
          };

        const savingMsgStyle = {
            "color": "#001375",
            "backgroundColor": "#DAEDFF",
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
                    (this.state.validationState == ValidationResults$$$get_Saving()) ?
                        <div id="divSuccessMsg" style={savingMsgStyle}>
                            Saving...
                        </div>
                    :
                        null
                }
                {
                    (this.state.validationState == ValidationResults$$$get_Success()) ?
                        <div id="divSuccessMsg" style={successMsgStyle}>
                            Saved successfully.
                        </div>
                    :
                        null
                }
                {
                    (0 != (this.state.validationState & ValidationResults$$$get_FirstNameBlank())) ?
                        <div id="divFailureMsg" style={failureMsgStyle}>
                            First name cannot be blank.
                        </div>
                    :
                        null
                }
                {
                    (0 != (this.state.validationState & ValidationResults$$$get_UnknownError())) ?
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