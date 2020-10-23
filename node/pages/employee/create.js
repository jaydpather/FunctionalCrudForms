import React, { Component } from 'react'
import Layout from '../../components/MyLayout';
import ContactInfo from '../../components/ContactInfo';
import EmployeeController from '../../controllers/EmployeeController';
import { getEmployeeValidator } from '../../fable-include/Validation'
import EmployeeLogicService from '../../logic/EmployeeLogicService';

const axios = require('axios');

export default class extends Component {

  //todo: make postToServer into a reusable method w/ a param for URL
  postToServer = (employee, successFn, errorFn) => {
    axios.post("http://localhost:7000/employee/create", JSON.stringify(employee))
      .then(function(response){
        successFn(response);
      }).catch(function(error){
        errorFn(error);
      });
  }

  render () {
    //todo: consistent tab width
    let validator = getEmployeeValidator();
    let employeeLogicService = new EmployeeLogicService();
    let validateAndSubmitFn = employeeLogicService.validateAndSubmit(validator.ValidateEmployee, this.postToServer);
    let employeeController = new EmployeeController(validateAndSubmitFn);
    
    return (
      <Layout>
        <ContactInfo employeeController = {employeeController} />
      </Layout>
    )
  }
}
//<ContactInfoComponent OnClick={this.btnSave_ClickWrapper} />