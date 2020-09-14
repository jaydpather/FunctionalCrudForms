import React, { Component } from 'react'
import Layout from '../../components/MyLayout';
import ContactInfo from '../../components/ContactInfo';
import EmployeeController from '../../controllers/EmployeeController';

export default class extends Component {
  render () {
    //todo: consistent tab width
    let employeeController = new EmployeeController();

    return (
      <Layout>
        <ContactInfo employeeController = {employeeController} />
      </Layout>
    )
  }
}
//<ContactInfoComponent OnClick={this.btnSave_ClickWrapper} />