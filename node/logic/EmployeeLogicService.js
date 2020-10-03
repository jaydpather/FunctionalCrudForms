import { Employee, ValidationResults$$$get_New, ValidationResults$$$get_Success, ValidationResults$$$get_Saving, ValidationResults$$$get_UnknownError, ValidationResults$$$get_FirstNameBlank,
    ValidationResults$$$get_LastNameBlank } from '../fable-include/Model'

export default class EmployeeLogicService {
    validateAndSubmit(validationFn, postToServerFn){ 
        return async(employee, setValidationStateFn) => {
            let opResult = validationFn(employee);
            
            if(ValidationResults$$$get_Success() == opResult.ValidationResult){
            //if(true){ //temp: allow posting of blank names, to test server-side validation
                try{
                    //debugger;
                    let response = await postToServerFn(employee);
                    setValidationStateFn(response.data.ValidationResult);
                }catch(ex){
                    //alert(JSON.stringify(ex));
                    setValidationStateFn(ValidationResults$$$get_UnknownError());
                }
            }
            else{
                setValidationStateFn(opResult.ValidationResult);
            }
        }
    }
}