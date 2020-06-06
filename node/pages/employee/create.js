import React, { Component } from 'react'
import Layout from '../../components/MyLayout';
import { btnSave_Click } from '../../fable-include/fable-bundle';
import { ContactInfoPageComponent, ContactInfoComponent } from '../../components/fable-components';
//const axios = require('axios');

export default class extends Component {
  submitFormJS() {
    eval("submitForm();");

    //todo: get whole form as one JSON object
    // var name = document.getElementById("txtName").value
    // var data = { Name:name }
    // this.postToServer("http://localhost:5000/employee/create", JSON.stringify(data));
    
  }

  btnSave_ClickWrapper(){
    btnSave_Click({});
  }

  postToServer(url, strData){
    // axios.post(url, strData)
    //   .then(function (response) {
    //     alert(response.data.Status);
    //   })
    //   .catch(function(error){
    //     alert(error);
    //   })
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
      "display": "none",
    };

    const failureMsgStyle = {
      "color": "#300",
      "background-color": "#FDD",
      "padding": "3px",
      "width": "40%",
      "display": "none",
    };

    //todo: consistent tab width
    return (
      <Layout>
        <div style={formStyle}>
            <h1>
                Create Employee
            </h1>
            Name: <input type="text" id="txtName" />
            <br />
            <br />
            <button id="btnSaveJS" type="button" onClick={this.submitFormJS}>Save</button>
            <br />
            <br />
            <div id="divSuccessMsg" style={successMsgStyle}>
              Saved successfully.
            </div>
            <div id="divFailureMsg" style={failureMsgStyle}>
              Unable to save: unknown error occurred.
            </div>
            
            <br />
            <hr />
            <br />

            <ContactInfoPageComponent />
            <br/>
            <ContactInfoPageComponent />
            

        </div>
      </Layout>
    )
  }
}
//<ContactInfoComponent OnClick={this.btnSave_ClickWrapper} />