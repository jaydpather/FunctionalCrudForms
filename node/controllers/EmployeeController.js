import { Employee, ValidationResults$$$get_New, ValidationResults$$$get_Success, ValidationResults$$$get_Saving, ValidationResults$$$get_UnknownError, ValidationResults$$$get_FirstNameBlank,
    ValidationResults$$$get_LastNameBlank } from '../fable-include/Model'


export default class EmployeeController {
    constructor(validator, postToServerFn){
        
        this._validationFn = (employee) => { 
            return validator.ValidateEmployee(employee);
        }

        this._postToServerFn = postToServerFn;
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
        this.validateAndSubmit(employee, this._validationFn, this._postToServerFn, setValidationStateFn);
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
}

