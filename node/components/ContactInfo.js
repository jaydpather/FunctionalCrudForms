import React, { Component } from 'react'
import { Employee, ValidationResults$$$get_New, ValidationResults$$$get_Success, ValidationResults$$$get_Saving, ValidationResults$$$get_UnknownError, ValidationResults$$$get_FirstNameBlank,
        ValidationResults$$$get_LastNameBlank } from '../fable-include/Model'

import { getEmployeeValidator } from '../fable-include/Validation'
const axios = require('axios');

export default class extends Component {
    state = this.props.employeeController.getInitialState();

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
                First Name: 
                <input type="text" id="txtName" value={this.state.firstName} 
                    //todo: move onChange handler to new method (too unreadable here)
                    onChange =  { e => this.setState({ 
                        firstName:e.target.value, 
                        lastName: this.state.lastName,
                        validationState: this.state.validationState
                    }) } 
                />
                <br />
                Last Name: 
                <input type="text" id="txtLastName" value={this.state.lastName} 
                    //todo: move onChange handler to new method (too unreadable here)
                    onChange =  { e => this.setState({ 
                        firstName: this.state.firstName,
                        lastName: e.target.value, 
                        validationState: this.state.validationState
                    }) } 
                />
                <br />
                <br />
                <button type="button" onClick={this.props.employeeController.submitForm(this)}>Save</button>
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
                        <div id="divFailureMsgFirstName" style={failureMsgStyle}>
                            First name cannot be blank.
                        </div>
                    :
                        null
                }
                {
                    (0 != (this.state.validationState & ValidationResults$$$get_LastNameBlank())) ?
                        <div id="divFailureMsgLastName" style={failureMsgStyle}>
                            Last name cannot be blank.
                        </div>
                    :
                        null
                }
                {
                    (0 != (this.state.validationState & ValidationResults$$$get_UnknownError())) ?
                        <div id="divFailureMsgUnknown" style={failureMsgStyle}>
                            Unable to save: unknown error occurred.
                        </div>
                    :
                        null
                }
            </div>
        );
    }
}