import { Employee, ValidationResults$$$get_New, ValidationResults$$$get_Success, ValidationResults$$$get_Saving, ValidationResults$$$get_UnknownError, ValidationResults$$$get_FirstNameBlank,
    ValidationResults$$$get_LastNameBlank } from '../fable-include/Model'

export default class EmployeeLogicService {
    validateAndSubmit(validationFn, postToServerFn){ 
        return function (employee, setValidationStateFn) {
            let opResult = validationFn(employee);
            
            if(ValidationResults$$$get_Success() == opResult.ValidationResult){
            //if(true){ //temp: allow posting of blank names, to test server-side validation
                let successFn = (response) => { setValidationStateFn(response.data.ValidationResult); }
                let errorFn = (error) => { setValidationStateFn(ValidationResults$$$get_UnknownError()); }
                postToServerFn(employee, successFn, errorFn);
            }
            else{
                setValidationStateFn(opResult.ValidationResult);
            }
        }
    }
}