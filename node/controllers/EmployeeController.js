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

        let setValidationStateFn = (newValidationState) => { //todo_search: rename setValidationStateFn to handleResponseFromServer. The page needs to pass this function as a param. (each page will handle the response differently, and expect a different return type)
            componentThis.setState({
                firstName: componentThis.state.firstName,
                lastName: componentThis.state.lastName,
                validationState: newValidationState
            }); 
        }

        setValidationStateFn(ValidationResults$$$get_Saving()); //todo_search: this will be a different state the search operation. ("searcing...", or "loading...")
        this._validateAndSubmitFn(employee, setValidationStateFn);
    }
}

