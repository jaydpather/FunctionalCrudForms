import React, { Component } from 'react'
import Layout from '../../components/MyLayout';
import ContactInfo from '../../components/ContactInfo';
import EmployeeController from '../../controllers/EmployeeController';
import { getEmployeeValidator } from '../../fable-include/Validation'

const axios = require('axios');

export default class extends Component {

  postToServer = async (employee) => {
    let response = await axios.post("http://localhost:7000/employee/create", JSON.stringify(employee));
    return response;
  }

  render () {
    //todo: consistent tab width
    let validator = getEmployeeValidator();
    let employeeController = new EmployeeController(validator, this.postToServer);

    return (
      <Layout>
        <ContactInfo employeeController = {employeeController} />
      </Layout>
    )
  }
}
//<ContactInfoComponent OnClick={this.btnSave_ClickWrapper} />