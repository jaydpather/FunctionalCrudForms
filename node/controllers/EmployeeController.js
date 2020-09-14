const axios = require('axios');

import { getEmployeeValidator } from '../fable-include/Validation'

import { Employee, ValidationResults$$$get_New, ValidationResults$$$get_Success, ValidationResults$$$get_Saving, ValidationResults$$$get_UnknownError, ValidationResults$$$get_FirstNameBlank,
    ValidationResults$$$get_LastNameBlank } from '../fable-include/Model'


export default class EmployeeController {
    //_componentThis = null;
    constructor(componentThis){
        this._componentThis = componentThis;
    }

    submitForm = (componentThis) => async(event) => {
        let employee = { FirstName: componentThis.state.firstName, LastName: componentThis.state.lastName };

        let validator = getEmployeeValidator();
        let validationFunction = (employee) => { 
            return validator.ValidateEmployee(employee);
        }

        let setValidationStateFunction = (newValidationState) => {
            componentThis.setState({
                firstName: componentThis.state.firstName,
                lastName: componentThis.state.lastName,
                validationState: newValidationState
            }); 
        }

        setValidationStateFunction(ValidationResults$$$get_Saving());
        componentThis.validateAndSubmit(employee, validationFunction, this.postToServer, setValidationStateFunction);
    }

    async validateAndSubmit(employee, validationFn, postToServerFn, setValidationStateFn) {
        let opResult = validationFn(employee);
        
        if(ValidationResults$$$get_Success() == opResult.ValidationResult){
        //if(true){ //temp: allow posting of blank names, to test server-side validation
            try{
                let response = await postToServerFn(employee);
                setValidationStateFn(response.data.ValidationResult);
            }catch(ex){
                setValidationStateFn(ValidationResults$$$get_UnknownError());
            }
        }
        else{
            setValidationStateFn(opResult.ValidationResult);
        }
    }

    postToServer = async (employee) => {
        let response = await axios.post("http://localhost:7000/employee/create", JSON.stringify(employee));
        return response;
    }
}

