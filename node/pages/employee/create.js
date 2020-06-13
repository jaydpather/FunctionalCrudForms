import React, { Component } from 'react'
import Layout from '../../components/MyLayout';
import ContactInfo from '../../components/ContactInfo';

export default class extends Component {
  render () {
    

    

    //todo: consistent tab width
    return (
      <Layout>
        <ContactInfo />
      </Layout>
    )
  }
}