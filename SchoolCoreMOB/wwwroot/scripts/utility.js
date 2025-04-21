// Utility file for generic CRUD operations
//const apiUrl = window.apiUrl;
//const strtoken = window.strtoken;
//const userparam = window.userParam;

function genericRequest(method, type, data) {
    let url = `${apiUrl}/${type}`;  // Default URL based on the type (could change based on your API structure)
    let payload = data || null;

    return new Promise((resolve, reject) => {
        $.ajax({
            url: url,
            type: method,
            contentType: 'application/json',
            dataType: 'json',
            data: payload ? JSON.stringify(payload) : null,
            headers: {
                'Authorization': 'Bearer ' + window.strtoken,
                'UserParam': window.userParam
            },
            success: function (response) {
                resolve(response);
            },
            error: function (xhr, status, error) {
                reject({ status: xhr.status, message: error || xhr.responseText });
            }
        });
    });
}

//function getOperation(type, data) {
//    switch (type) {
//        case '1':
//            // Handle GET operation for model1
//            return genericRequest('GET', '1', data);

//        case 'model2':
//            // Handle GET operation for model2
//            return genericRequest('GET', 'model2', data);

//        // Add more cases as needed
//        default:
//            return Promise.reject('Unknown model type for GET operation');
//    }
//}

function postOperation(type, data) {
    switch (type) {
        case 'model1':
            // Handle POST operation for model1
            return genericRequest('POST', 'model1', data);

        case 'model2':
            // Handle POST operation for model2
            return genericRequest('POST', 'model2', data);

        // Add more cases as needed
        default:
            return Promise.reject('Unknown model type for POST operation');
    }
}

function putOperation(type, data) {
    switch (type) {
        case 'model1':
            // Handle PUT operation for model1
            return genericRequest('PUT', 'model1', data);

        case 'model2':
            // Handle PUT operation for model2
            return genericRequest('PUT', 'model2', data);

        // Add more cases as needed
        default:
            return Promise.reject('Unknown model type for PUT operation');
    }
}

function deleteOperation(type, data) {
    switch (type) {
        case 'model1':
            // Handle DELETE operation for model1
            return genericRequest('DELETE', 'model1', data);

        case 'model2':
            // Handle DELETE operation for model2
            return genericRequest('DELETE', 'model2', data);

        // Add more cases as needed
        default:
            return Promise.reject('Unknown model type for DELETE operation');
    }
}


function generalRequest(api, token, userparam) { 
    return new Promise((resolve, reject) => {
        $.ajax({
            url: api,
            type: 'GET',
            contentType: 'application/json',
            dataType: 'json',
           // data: payload ? JSON.stringify(payload) : null,
            headers: {
                'Authorization': 'Bearer ' + token,
                'UserParam': userparam
            },
            success: function (response) {             
               resolve(response);
            },
            error: function (xhr, status, error) {
              //  console.log(error);
                reject({ status: xhr.status, message: error || xhr.responseText });
            }
        });
    });
}
function getOperation(api, token, userparam) {   
    return generalRequest(api, token, userparam);
}

