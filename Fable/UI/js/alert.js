//const axios = require('axios');

function triggerAlert(message) {
  alert(message);

}

//function postToServer(url, objParam){
  // var strParam = JSON.stringify(objParam);
  // axios.post(url, strParam)
  //   .then(function (response) {
  //     alert(response.data.Status);
  //   })
  //   .catch(function(error){
  //     alert(error);
  //   })
//}

const someString = "And I Like that!";

export { triggerAlert, /*postToServer,*/ someString };
