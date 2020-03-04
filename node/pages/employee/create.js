import React, { Component } from 'react'
import Layout from '../../components/MyLayout';
const axios = require('axios');

export default class extends Component {
  submitForm() {
    axios.get("http://localhost:5000/employee/create").then(function (response) {
      alert(JSON.stringify(response.data.status));
    })
  }

  render () {
    console.log("rendering") //todo: why do we see this log message on both server and console when doing SSR?
    const formStyle = {
        "marginTop":"10px",
        "border": "solid 1px #BBB",
        "padding": "10px",
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
            <button type="button" onClick={this.submitForm}>Save</button>
        </div>
      </Layout>
    )
  }
}