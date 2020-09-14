import { Employee, ValidationResults$$$get_New, ValidationResults$$$get_Success, ValidationResults$$$get_Saving, ValidationResults$$$get_UnknownError, ValidationResults$$$get_FirstNameBlank,
    ValidationResults$$$get_LastNameBlank } from '../fable-include/Model'


export default class EmployeeController {
    constructor(validateAndSubmitFn){
        this._validateAndSubmitFn = validateAndSubmitFn;
    }

    getInitialState = () => {
        return { 
            firstName: "", 
            lastName: "",
            validationState: ValidationResults$$$get_New()
        };
    }

    submitForm = (componentThis) => async(event) => {
        let employee = { FirstName: componentThis.state.firstName, LastName: componentThis.state.lastName };

        let setValidationStateFn = (newValidationState) => {
            componentThis.setState({
                firstName: componentThis.state.firstName,
                lastName: componentThis.state.lastName,
                validationState: newValidationState
            }); 
        }

        setValidationStateFn(ValidationResults$$$get_Saving());
        this._validateAndSubmitFn(employee, setValidationStateFn);
    }
}

