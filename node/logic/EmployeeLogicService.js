import { Employee, ValidationResults$$$get_New, ValidationResults$$$get_Success, ValidationResults$$$get_Saving, ValidationResults$$$get_UnknownError, ValidationResults$$$get_FirstNameBlank,
    ValidationResults$$$get_LastNameBlank } from '../fable-include/Model'

export default class EmployeeLogicService {
    validateAndSubmit_Create(validationFn, postToServerFn){ 
        return async(employee, setValidationStateFn) => {
            let opResult = validationFn(employee);
            
            if(ValidationResults$$$get_Success() == opResult.ValidationResult){
            //if(true){ //temp: allow posting of blank names, to test server-side validation
                try{
                    let response = await postToServerFn(employee);
                    setValidationStateFn(response.data.ValidationResult); //todo_search: rename setValidationStateFn to handleResponseFromServer. change param to response.data. (that way, each page (search, create, update, can receive a different type))
                }catch(ex){
                    setValidationStateFn(ValidationResults$$$get_UnknownError());
                }
            }
            else{
                setValidationStateFn(opResult.ValidationResult);
            }
        }
    }
}