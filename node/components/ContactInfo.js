import React, { Component } from 'react'
import { validateFirstName } from '../fable-include/UI'
const axios = require('axios');

export default class extends Component {
    state = { 
        name:"", 
        isSuccess: false,
        isError: false
    };
    self = this;
    
    handleSubmit = async(event) => {
        this.setState(
            {
                name: this.state.name,
                isSuccess: false,
                isError: true
            }
        );
    };

    //todo: remove this function from component
    submitForm = async(event) => {
        //alert(JSON.stringify(ui));

        var data = { Name:this.state.name }
        var self = this;

        let isClientSide = (typeof window !== 'undefined');

        if(isClientSide && validateFirstName(this.state.name)){
            axios.post("http://localhost:7000/employee/create", JSON.stringify(data))
            .then(function (response) {
                //alert(response.data);
                alert(JSON.stringify(response.data));
                self.setState({
                    name: self.state.name,
                    isSuccess: true,
                    isError: false
                });
            })
            .catch(function(error){
                alert(error);
                self.setState({
                    name: self.state.name,
                    isSuccess: false,
                    isError: true   
                });
            })
        }
        else{
            alert("validation failed");
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
                    onChange =  { e => this.setState({ 
                        name:e.target.value, 
                        isSuccess: this.state.isSuccess,
                        isError: this.state.isError 
                    }) } 
                />
                <br />
                <br />
                <button type="button" onClick={this.submitForm}>Save</button>
                <br />
                <br />
                {
                    this.state.isSuccess ?
                        <div id="divSuccessMsg" style={successMsgStyle}>
                            Saved successfully.
                        </div>
                    :
                        null
                }

                {
                    this.state.isError?
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