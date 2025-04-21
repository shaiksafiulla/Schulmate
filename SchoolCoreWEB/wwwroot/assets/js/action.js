/*var basepath = window.location.origin + window.location.pathname.substr(0, window.location.pathname.lastIndexOf('/'));*/
var rootpath = window.location.pathname.split('/')[1];  // Get the first part of the path
if (!rootpath) {
    rootpath = '';  // Default to an empty string if no subdirectory is present (localhost)
} else {
    rootpath = '/' + rootpath; // Make sure it's prefixed with a single slash
}
//revert this in production
//var basepath = window.location.origin + rootpath;
var basepath = window.location.origin;
function convertNumberToWords(amount) {
    const below20 = [
        'Zero', 'One', 'Two', 'Three', 'Four', 'Five', 'Six', 'Seven', 'Eight', 'Nine',
        'Ten', 'Eleven', 'Twelve', 'Thirteen', 'Fourteen', 'Fifteen', 'Sixteen', 'Seventeen',
        'Eighteen', 'Nineteen'
    ];
    const tens = [
        '', '', 'Twenty', 'Thirty', 'Forty', 'Fifty', 'Sixty', 'Seventy', 'Eighty', 'Ninety'
    ];
    const thousands = [
        '', 'Thousand', 'Million', 'Billion', 'Trillion'
    ];

    function convertToWords(num) {
        if (num === 0) return '';
        if (num < 20) return below20[num];
        if (num < 100) return tens[Math.floor(num / 10)] + (num % 10 === 0 ? '' : ' ' + below20[num % 10]);
        if (num < 1000) return below20[Math.floor(num / 100)] + ' Hundred' + (num % 100 === 0 ? '' : ' ' + convertToWords(num % 100));

        let idx = 0;
        let word = '';
        while (num > 0) {
            if (num % 1000 !== 0) {
                word = convertToWords(num % 1000) + ' ' + thousands[idx] + (word ? ' ' + word : '');
            }
            num = Math.floor(num / 1000);
            idx++;
        }
        return word.trim();
    }

    function convertAmount(amount) {
        // Split the amount into the whole number and decimal parts
        let [whole, decimal] = amount.toString().split('.');

        let wholePart = convertToWords(Number(whole)) + ' Rupees';
        let decimalPart = decimal ? convertToWords(Number(decimal)) + ' Paise' : '';

        // Combine whole and decimal parts
        let result = wholePart + (decimalPart ? ' and ' + decimalPart : '');
        return result + ' Only';  // Append 'Only' at the end
    }

    return convertAmount(amount);
}

var onReceiptClick = function (paymentid) {
   // var url = '@Url.Action("Index", "FeeReceipt")?paymentId=' + paymentid;
    var url = basepath + "/FeeReceipt/Index?paymentId=" + paymentid;
    printStudentReport(url);
}
var onHallTicketClick = function (scheduleid,studentid) {
    // var url = '@Url.Action("Index", "FeeReceipt")?paymentId=' + paymentid;
    var url = basepath + "/Schedule/GetHallTicket?scheduleId=" + scheduleid + "&studentId=" + studentid;
    printStudentReport(url);
}

function getOperation(api, token, userparam) {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: api,
            type: 'GET',
            contentType: 'application/json',
            dataType: 'json',         
            headers: {
                'Authorization': 'Bearer ' + token,
                'UserParam': userparam
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

function sortTable(n, tablename) {
    var table, rows, switching, i, x, y, shouldSwitch, dir, switchcount = 0;
    table = document.getElementById(tablename);

    switching = true;
    //Set the sorting direction to ascending:
    dir = "asc";
    /*Make a loop that will continue until
    no switching has been done:*/
    while (switching) {
        //start by saying: no switching is done:
        switching = false;
        rows = table.rows;
        /*Loop through all table rows (except the
        first, which contains table headers):*/
        for (i = 1; i < (rows.length - 1); i++) {
            //start by saying there should be no switching:
            shouldSwitch = false;
            /*Get the two elements you want to compare,
            one from current row and one from the next:*/
            x = rows[i].getElementsByTagName("TD")[n];
            y = rows[i + 1].getElementsByTagName("TD")[n];
            /*check if the two rows should switch place,
            based on the direction, asc or desc:*/
            if (dir == "asc") {
                if (x.innerHTML.toLowerCase() > y.innerHTML.toLowerCase()) {
                    //if so, mark as a switch and break the loop:
                    shouldSwitch = true;
                    break;
                }
            } else if (dir == "desc") {
                if (x.innerHTML.toLowerCase() < y.innerHTML.toLowerCase()) {
                    //if so, mark as a switch and break the loop:
                    shouldSwitch = true;
                    break;
                }
            }
        }
        if (shouldSwitch) {
            /*If a switch has been marked, make the switch
            and mark that a switch has been done:*/
            rows[i].parentNode.insertBefore(rows[i + 1], rows[i]);
            switching = true;
            //Each time a switch is done, increase this count by 1:
            switchcount++;
        } else {
            /*If no switching has been done AND the direction is "asc",
            set the direction to "desc" and run the while loop again.*/
            if (switchcount == 0 && dir == "asc") {
                dir = "desc";
                switching = true;
            }
        }
    }
}
function preventEnter(search) {
    var input = document.getElementById(search);
    input.addEventListener("keypress", function (event) {
        if (event.key === "Enter") {
            event.preventDefault();
        }
    })
}
function performSearch(search, tablename) {

    const searchBox = document.getElementById(search);
    const table = document.getElementById(tablename);
    const trs = table.tBodies[0].getElementsByTagName("tr");
    // Declare search string 
    var filter = searchBox.value.toUpperCase();

    //  Loop through first tbody's rows
    for (var rowI = 0; rowI < trs.length; rowI++) {

        // define the row's cells
        var tds = trs[rowI].getElementsByTagName("td");

        // hide the row
        trs[rowI].style.display = "none";

        // loop through row cells
        for (var cellI = 0; cellI < tds.length; cellI++) {

            // if there's a match
            if (tds[cellI].innerHTML.toUpperCase().indexOf(filter) > -1) {
                // show the row
                trs[rowI].style.display = "";

                // skip to the next row
                continue;
            }

        }
    }


}
//function performSearch(search, tablename) {
//    var table = document.getElementById(tablename);
//    var input = document.getElementById(search);
//    let filter = input.value.toUpperCase();
//    rows = table.getElementsByTagName("TR");
//    let flag = false;

//    for (let row of rows) {
//        let cells = row.getElementsByTagName("TD");
//        for (let cell of cells) {
//            if (cell.textContent.toUpperCase().indexOf(filter) > -1) {
//                if (filter) {
//                    cell.style.backgroundColor = 'yellow';
//                } else {
//                    cell.style.backgroundColor = '';
//                }

//                flag = true;
//            } else {
//                cell.style.backgroundColor = '';
//            }
//        }

//        if (flag) {
//            row.style.display = "";
//        } else {
//            row.style.display = "none";
//        }

//        flag = false;
//    }
//}
function Export(tablename, xlname) {
    table = document.getElementById(tablename);
    $(table).table2excel({
        filename: xlname
    });
}
function formatDate(date) {
    var d = new Date(date),
        month = '' + (d.getMonth() + 1),
        day = '' + d.getDate(),
        year = d.getFullYear();

    if (month.length < 2)
        month = '0' + month;
    if (day.length < 2)
        day = '0' + day;

    return [year, month, day].join('-');
}
function PagerClick(index, hiddenpageindex) {
    pageindex = document.getElementById(hiddenpageindex).value = index;
    document.forms[0].submit();
}
function onSelect(el, ctrl) {
    var elmId = ctrl.id;
    var tbldata = $("#" + elmId).DataTable();
    jQuery(tbldata.rows().nodes()).each(function (item) {
        var tr = this;
        tr.bgColor = "";
    });
    var row = el.parentElement.parentElement;
    row.bgColor = "lightgray";
    //row.style="background-color:lightgray";    

}
function toastMessage(message) {
    toastr.success(
        message,
        '',
        {
            timeOut: 50,
            fadeOut: 50,
            onHidden: function () {
                location.reload();
            }
        }
    );
}
var pageObj = { organization: 1, branch: 2, expense: 3, denomination: 4, employee: 5, category: 6, subcategory: 7, subsubcategory: 8, roles: 9, users: 10, ptcasehistory: 11, labtesthistory: 12, teststatus: 13, viewtestreport: 14, lab: 15, approval: 16, processafterapproval: 17, acknowledgement: 18, referenceclinic: 19, referencedoctor: 20, ehstest: 21, ysrtest: 22, accounthistory: 23, division: 24, group: 25, subcategoryrange: 26, parameter: 27, parameterrange: 28, patienttest: 29, receipt: 30 }
var uploadType = { student: 1, teacher: 2, questionPaper: 3, organization: 4, attachment : 5, assignment : 6 }
var uploadFileType = { img: 1, pdfDoc: 2 }
var notificationType = { leaverequest: 1 }
function cancel() {
    $("#primary-header-modal").modal("hide");
    $("#primary-header-modal2").modal("hide");
}

function closeFunction() {
    $("#primary-header-modal").modal("hide");
    $("#primary-header-modal2").modal("hide");
}
function onModalClose() {
    window.location.reload();
}
function closeTestFunction() {
    // console.log("closed called from editpatienttest.html");
    location.reload();
}
function download(path) {
    //console.log(path);
    //window.location.href = 'Uploads\OtherLabReport\7cb14e3a-5a3f-46bc-924a-931aaaab290e_report.pdf';
    //console.log(path);

    //const doc = await fetch('\Uploads\OtherLabReport\441d3dee-ce85-450f-85c9-512c3ebc21e8_ANATOMY-1-2015.pdf');
    //const docblob = await doc.blob();
    //const docURL = URL.createObjectURL(docblob);

    //const anchor = document.createElement("a");
    //anchor.href = docURL;
    //anchor.download = 'ANATOMY-1-2015.pdf';

    //document.body.appendChild(anchor);
    //anchor.click();
    //document.body.removeChild(anchor);

    //URL.revokeObjectURL(docURL);

    //var uri = '/Uploads/OtherLabReport/441d3dee-ce85-450f-85c9-512c3ebc21e8_ANATOMY-1-2015.pdf';
    //var uri = path;
    //var link = document.createElement("a");
    //link.download = 'ANATOMY-1-2015.pdf';
    //link.href = uri;
    //link.click();
}
function printPdf(url) {
    var iframe = this._printIframe;
    if (!this._printIframe) {
        iframe = this._printIframe = document.createElement('iframe');
        document.body.appendChild(iframe);

        iframe.style.display = 'none';
        iframe.onload = function () {
            setTimeout(function () {
                iframe.focus();
                iframe.contentWindow.print();
            }, 2000);
        };
    }

    iframe.src = url;
}
function beforePrint() {
    for (const id in Chart.instances) {
       // Chart.instances[id].resize()
    }
}

if (window.matchMedia) {
    let mediaQueryList = window.matchMedia('print')
    mediaQueryList.addListener((mql) => {
        if (mql.matches) {
            beforePrint()
        }
    })
}
//window.onbeforeprint = beforePrint
function closePrint() {
    document.body.removeChild(this.__container__);
}
function setPrint() {
    this.contentWindow.__container__ = this;
    this.contentWindow.onbeforeunload = closePrint;
    this.contentWindow.onafterprint = closePrint;
    this.contentWindow.focus(); // Required for IE   
    this.contentWindow.print();
}
function printStudentReport(sURL) {

    const hideFrame = document.createElement("iframe");
    hideFrame.onload = setPrint1;
    hideFrame.style.position = "fixed";
    hideFrame.style.right = "0";
    hideFrame.style.bottom = "0";
    hideFrame.style.width = "0";
    hideFrame.style.height = "0";
    hideFrame.style.border = "0";
    hideFrame.src = sURL;   
    document.body.appendChild(hideFrame);
};
function printDiv(divName) {

    const hideFrame = document.createElement("iframe");
    hideFrame.onload = setPrint1;
    hideFrame.style.position = "fixed";
    hideFrame.style.right = "0";
    hideFrame.style.bottom = "0";
    hideFrame.style.width = "0";
    hideFrame.style.height = "0";
    hideFrame.style.border = "0";
    // hideFrame.src = sURL;

    //const printContent = document.getElementById(divName);
    //const content = printContent.cloneNode(true);
    //const canvases = content.querySelectorAll('canvas');
    //canvases.forEach(canvas => {
    //    const img = document.createElement('img');
    //    img.src = canvas.toDataURL("image/png");
    //    console.log(canvas.toDataURL("image/png")); // Check if this logs a valid data URL

    //    img.style.width = canvas.width + 'px'; // Maintain original width
    //    img.style.height = canvas.height + 'px'; // Maintain original height
    //    canvas.parentNode.replaceChild(img, canvas);

    //});

    var printContents = document.getElementById(divName).innerHTML; 

    var htmlToPrint = '<head><link rel="stylesheet" href="assets/css/app-modern.min.css" type="text/css" id="app-style"/>' +
        '<link rel="stylesheet" href="assets/css/seat.css" type="text/css" id="app-style"/>' +
        '<link rel="stylesheet" href="assets/css/reportprintstyle.css" type="text/css"/> </head>' +
        ' <script src="assets/vendor/jquery/jquery.min.js"></script>' +
        '<script src="assets/vendor/chart.js/chart.min.js"></script> ';

    htmlToPrint += printContents;
    hideFrame.srcdoc = htmlToPrint;
    document.body.appendChild(hideFrame);
};
function printDivContent1(divId) {
    const printContent = document.getElementById(divId);
    const printWindow = window.open('', '_blank');   

    setTimeout(() => {
        // Create a copy of the div's content
        const content = printContent.cloneNode(true);

        // Convert canvas elements to images
        const canvases = content.querySelectorAll('canvas');
        canvases.forEach(canvas => {
            const img = document.createElement('img');
            img.src = canvas.toDataURL("image/png");
            console.log(canvas.toDataURL("image/png")); // Check if this logs a valid data URL

            img.style.width = canvas.width + 'px'; // Maintain original width
            img.style.height = canvas.height + 'px'; // Maintain original height
            canvas.parentNode.replaceChild(img, canvas);

        });

        // Write content to the print window
        printWindow.document.write('<html><head><link rel="stylesheet" href="assets/css/app-modern.min.css" type="text/css" id="app-style"/>' +
            '<link rel="stylesheet" href="assets/css/seat.css" type="text/css" id="app-style"/>' +
            '<link rel="stylesheet" href="assets/css/reportprintstyle.css" type="text/css"/><title>Print</title>');
        printWindow.document.write('<style>@media print { body { margin: 0; } img { max-width: 100%; height: auto; } }</style>');
        printWindow.document.write('</head><body>');
        printWindow.document.write(content.innerHTML);
        printWindow.document.write('</body></html>');
        console.log(printWindow.document.body);
        printWindow.document.close();
        printWindow.print();
    }, 2000);

    
}
function printDivContent2(divId) {
    const printContent = document.getElementById(divId);
    const printWindow = window.open('', '_blank');
    const content = printContent.cloneNode(true);

    const canvases = content.querySelectorAll('canvas');
    const canvasPromises = Array.from(canvases).map(canvas => {
        return new Promise(resolve => {
            const img = new Image();
            img.src = canvas.toDataURL("image/png");
            img.onload = () => {
                canvas.parentNode.replaceChild(img, canvas);
                resolve();
            };
        });
    });

    Promise.all(canvasPromises).then(() => {
        printWindow.document.write('<html><head><link rel="stylesheet" href="assets/css/app-modern.min.css" type="text/css" id="app-style"/>' +
            '<link rel="stylesheet" href="assets/css/seat.css" type="text/css" id="app-style"/>' +
            '<link rel="stylesheet" href="assets/css/reportprintstyle.css" type="text/css"/><title>Print</title>');
        printWindow.document.write('<style>@media print { body { margin: 0; } img { max-width: 100%; height: auto; }</style>');
        printWindow.document.write('</head><body>');

        // Append each image to the print window
        content.querySelectorAll('img').forEach(img => {
            printWindow.document.write(`<img src="${img.src}" style="width: ${img.width}px; height: ${img.height}px;">`);
        });

        printWindow.document.write('<script src="assets/vendor/jquery/jquery.min.js"></script>' +
            '<script src="assets/vendor/chart.js/chart.min.js"></script></body></html>');
        console.log(printWindow.document.body);
        printWindow.document.close();
       
        printWindow.print();
    });
}

function printDivContent3(divId) {
    const printContent = document.getElementById(divId);
    const printWindow = window.open('', '_blank');
    const content = printContent.cloneNode(true);

    const canvases = content.querySelectorAll('canvas');
    const canvasPromises = Array.from(canvases).map(canvas => {
        return new Promise(resolve => {
            // Check if canvas has content
            if (canvas.width === 0 || canvas.height === 0) {
                console.error("Canvas is empty or not drawn.");
                resolve();
                return;
            }

            const imgSrc = canvas.toDataURL("image/png");
            const img = new Image();
            img.src = imgSrc;

            img.onload = () => {
                // Replace canvas with image
                canvas.parentNode.replaceChild(img, canvas);
                resolve();
            };
            img.onerror = () => {
                console.error("Error loading canvas image.");
                resolve(); // Continue even if there's an error
            };
        });
    });

    // Wait for all images to be ready
    Promise.all(canvasPromises).then(() => {
        printWindow.document.write('<html><head><title>Print</title>');
        printWindow.document.write('<style>@media print { body { margin: 0; } img { max-width: 100%; height: auto; }</style>');
        printWindow.document.write('</head><body>');

        // Append images to print window
        content.querySelectorAll('img').forEach(img => {
            printWindow.document.write(`<img src="${img.src}" style="width: ${img.width}px; height: ${img.height}px;">`);
        });

        printWindow.document.write('</body></html>');
        console.log(printWindow.document.body);
        printWindow.document.close();        
        printWindow.print();
    });
}

function printDivContent(divId) {
    const printContent = document.getElementById(divId);
    const printWindow = window.open('', '_blank');

    // Check if print window opened successfully
    if (!printWindow) {
        console.error("Failed to open print window. Please check your popup settings.");
        return;
    }

    const content = printContent.cloneNode(true);
    const canvases = content.querySelectorAll('canvas');
    const canvasPromises = Array.from(canvases).map(canvas => {
        return new Promise(resolve => {
            if (canvas.width === 0 || canvas.height === 0) {
                console.error("Canvas is empty or not drawn.");
                resolve();
                return;
            }

            const imgSrc = canvas.toDataURL("image/png");
            const img = new Image();
            img.src = imgSrc;

            img.onload = () => {
                console.log(img);
                canvas.parentNode.replaceChild(img, canvas);
                resolve();
            };
            img.onerror = () => {
                console.error("Error loading canvas image.");
                resolve(); // Continue even if there's an error
            };
            
        });
    });

    // Wait for all images to be ready
    Promise.all(canvasPromises).then(() => {
        // Ensure that we write the content after all images have been processed
        printWindow.document.write('<html><head><link rel="stylesheet" href="assets/css/app-modern.min.css" type="text/css" id="app-style"/>' +
            '<link rel="stylesheet" href="assets/css/seat.css" type="text/css" id="app-style"/>' +
            '<link rel="stylesheet" href="assets/css/reportprintstyle.css" type="text/css"/><title>Print</title>');
        printWindow.document.write('<script src="https://cdn.jsdelivr.net/npm/chart.js"></script>');
        //printWindow.document.write('<style>@media print { body { margin: 0; } img { max-width: 100%; height: auto; }</style>');
        printWindow.document.write('</head><body>');

        // Add the content
        printWindow.document.write(content.innerHTML);
        
        printWindow.document.write('</body></html>');
       
        printWindow.document.close();
        console.log(printWindow.document);
        // Optional: Delay to ensure everything is loaded before printing
        setTimeout(() => {
            printWindow.print();
        }, 10000); // Adjust delay as necessary
    });
}


function setPrint1() {
    this.contentWindow.__container__ = this;
    this.contentWindow.onbeforeunload = closePrint;
    this.contentWindow.onafterprint = closePrint;
    this.contentWindow.focus(); // Required for IE   

    setTimeout(() => {
        this.contentWindow.print();
    }, 1000);
}
function dayOfWeekToInt(day) {
    const daysOfWeek = {
        "Sunday": 0,
        "Monday": 1,
        "Tuesday": 2,
        "Wednesday": 3,
        "Thursday": 4,
        "Friday": 5,
        "Saturday": 6
    };
    // Convert day string to its corresponding integer, or return -1 if not found
    return daysOfWeek[day] !== undefined ? daysOfWeek[day] : -1;
}
function getDayOfWeekString(dayInt) {
    const daysOfWeek = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];

    // Ensure that the passed integer is within the valid range (0-6)
    if (dayInt >= 0 && dayInt <= 6) {
        return daysOfWeek[dayInt];
    } else {
        return "Invalid day";  // Handle invalid inputs
    }
}

function saveByteArray(reportName, byte) {
    // console.log(byte);
    var blob = new Blob([byte], { type: "application/pdf" });
    var link = document.createElement('a');
    link.href = window.URL.createObjectURL(blob);
    var fileName = reportName;
    link.download = fileName;
    link.click();
};


function onLoadAttachments(type, id) {
    var url = '';
    switch (type) {
        case '1':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.attachments);
                url = basepath + "/Student/LoadAttachments?referid=" + id;
            }                       
            break;
        case '2':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.attachments);
                url = basepath + "/Teacher/LoadAttachments?referid=" + id;
            }
            break;
        case '3':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.attachments);
                url = basepath + "/Admin/LoadAttachments?referid=" + id;
            }
            break;
        case '4':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.attachments);
                url = basepath + "/Staff/LoadAttachments?referid=" + id;
            }
            break;
        case '5':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.attachments);
                url = basepath + "/Driver/LoadAttachments?referid=" + id;
            }
            break;
    }  
    $("#myModalBodyDiv1").load(url, function () {
        $("#primary-header-modal").modal("show");
    })
}

function navigateToPage(type, id, recordId, statusId) {
    
    var url = '';
    switch (type) {
        case 1:
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.editleaverequest);
            }           
            url = basepath + "/Notifications/Edit?Id=" + id + '&RecordId=' + recordId +'&ReadStatusId=' + statusId;
            break;
    }
    $('#myModalBodyDiv1').load(url, function () {
        $('#primary-header-modal').modal("show");
    })
}
function addEditAction(type, id) {
    var url = '';
    switch (type) {
        case '1':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.editbranch);
            }
            else {
                $("#primary-header-modalLabel").html(resourceObj.addbranch);
            }
            url = basepath + "/Branch/AddEdit?Id=" + id;
            break;
        case '2':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.editclass);
            }
            else {
                $("#primary-header-modalLabel").html(resourceObj.addclass);
            }
            url = basepath + "/Class/AddEdit?Id=" + id;
            break;
        case '3':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.editsubject);
            }
            else {
                $("#primary-header-modalLabel").html(resourceObj.addsubject);
            }
            url = basepath + "/Subject/AddEdit?Id=" + id;
            break;
        case '4':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.edittopic);
            }
            else {
                $("#primary-header-modalLabel").html(resourceObj.addtopic);
            }
            url = basepath + "/Topic/AddEdit?Id=" + id;
            break;
        case '5':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.editsection);
            }
            else {
                $("#primary-header-modalLabel").html(resourceObj.addsection);
            }
            url = basepath + "/Section/AddEdit?Id=" + id;
            break;
        case '6':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.editstudent);
            }
            else {
                $("#primary-header-modalLabel").html(resourceObj.addstudent);
            }
            url = basepath + "/Student/AddEdit?Id=" + id;
            break;
        case '7':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.editexamtype);
            }
            else {
                $("#primary-header-modalLabel").html(resourceObj.addexamtype);
            }
            url = basepath + "/ExamType/AddEdit?Id=" + id;
            break;
        case '8':
            if (id > 0) {
                $("#staticBackdropLabel").html(resourceObj.editschedule);
            }
            else {
                $("#staticBackdropLabel").html(resourceObj.addschedule);
            }
            url = basepath + "/Schedule/AddEdit?Id=" + id;
            break;
        case '9':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.editexamination);
            }
            else {
                $("#primary-header-modalLabel").html(resourceObj.addexamination);
            }
            url = basepath + "/Examination/AddEdit?Id=" + id;
            break;
        case '10':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.editteacher);
            }
            else {
                $("#primary-header-modalLabel").html(resourceObj.addteacher);
            }
            url = basepath + "/Teacher/AddEdit?Id=" + id;
            break;
        case '11':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.editgrade);
            }
            else {
                $("#primary-header-modalLabel").html(resourceObj.addgrade);
            }
            url = basepath + "/Grade/AddEdit?Id=" + id;
            break;
        case '12':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.editholiday);
            }
            else {
                $("#primary-header-modalLabel").html(resourceObj.addholiday);
            }
            url = basepath + "/Holiday/AddEdit?Id=" + id;
            break;
        case '13':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.editexamtime);
            }
            else {
                $("#primary-header-modalLabel").html(resourceObj.addexamtime);
            }
            url = basepath + "/ExamTime/AddEdit?Id=" + id;
            break;
        case '14':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.editquestiontype);
            }
            else {
                $("#primary-header-modalLabel").html(resourceObj.addquestiontype);
            }
            url = basepath + "/QuestionType/AddEdit?Id=" + id;
            break;
        case '15':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.editdifficultylevel);
            }
            else {
                $("#primary-header-modalLabel").html(resourceObj.adddifficultylevel);
            }
            url = basepath + "/QuestionDifficulty/AddEdit?Id=" + id;
            break;
        case '16':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.editquestioncategory);
            }
            else {
                $("#primary-header-modalLabel").html(resourceObj.addquestioncategory);
            }
            url = basepath + "/QuestionCategory/AddEdit?Id=" + id;
            break;
        case '17':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.editquestionbank);
            }
            else {
                $("#primary-header-modalLabel").html(resourceObj.addquestionbank);
            }
            url = basepath + "/QuestionBank/AddEdit?Id=" + id;
            break;
        case '18':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.editrole);
            }
            else {
                $("#primary-header-modalLabel").html(resourceObj.addrole);
            }
            url = basepath + "/Roles/AddEdit?Id=" + id;
            break;
        case '19':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.edituser);                
            }
            url = basepath + "/User/EditUser?Id=" + id;           
            break;
        case '20':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.editreportcard);
            }
            else {
                $("#primary-header-modalLabel").html(resourceObj.addreportcard);
            }
            url = basepath + "/ReportCard/AddEdit?Id=" + id;
            break;
        case '21':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.editattendence);
            }
            else {
                $("#primary-header-modalLabel").html(resourceObj.addattendence);
            }
            url = basepath + "/Attendence/AddEdit?Id=" + id;
            break;
        case '22':
            if (id > 0) {               
                $("#primary-header-modalLabel").html(resourceObj.editlicense);
            }
            else {
                $("#primary-header-modalLabel").html(resourceObj.addlicense);
            }
            url = basepath + "/License/AddEdit?Id=" + id;           
            break;
        case '23':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.editmedium);
            }
            else {
                $("#primary-header-modalLabel").html(resourceObj.addmedium);
            }
            url = basepath + "/Medium/AddEdit?Id=" + id;
            break;
        case '24':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.editshift);
            }
            else {
                $("#primary-header-modalLabel").html(resourceObj.addshift);
            }
            url = basepath + "/Shift/AddEdit?Id=" + id;
            break;
        case '25':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.editshift);
            }
            else {
                $("#primary-header-modalLabel").html(resourceObj.addshift);
            }
            url = basepath + "/BranchClass/AddEdit?Id=" + id;
            break;
        case '26':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.editsessionyear);
            }
            else {
                $("#primary-header-modalLabel").html(resourceObj.addsessionyear);
            }
            url = basepath + "/SessionYear/AddEdit?Id=" + id;
            break;
        case '28':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.editbloodgroup);
            }
            else {
                $("#primary-header-modalLabel").html(resourceObj.addbloodgroup);
            }
            url = basepath + "/BloodGroup/AddEdit?Id=" + id;
            break;
        case '29':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.editfeetype);
            }
            else {
                $("#primary-header-modalLabel").html(resourceObj.addfeetype);
            }
            url = basepath + "/FeeType/AddEdit?Id=" + id;
            break;
        case '30':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.editenquirystatus);
            }
            else {
                $("#primary-header-modalLabel").html(resourceObj.addenquirystatus);
            }
            url = basepath + "/EnquiryStatus/AddEdit?Id=" + id;
            break;
        case '31':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.editlesson);
            }
            else {
                $("#primary-header-modalLabel").html(resourceObj.addlesson);
            }
            url = basepath + "/Lesson/AddEdit?Id=" + id;
            break;
        case '32':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.editexpensecategory);
            }
            else {
                $("#primary-header-modalLabel").html(resourceObj.addexpensecategory);
            }
            url = basepath + "/ExpenseCategory/AddEdit?Id=" + id;
            break;
        case '33':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.editreferenceadmission);
            }
            else {
                $("#primary-header-modalLabel").html(resourceObj.addreferenceadmission);
            }
            url = basepath + "/ReferenceAdmission/AddEdit?Id=" + id;
            break;
        case '34':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.editstudentenquiry);
            }
            else {
                $("#primary-header-modalLabel").html(resourceObj.addstudentenquiry);
            }
            url = basepath + "/StudentEnquiry/AddEdit?Id=" + id;
            break;
        case '35':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.editleaverequest);
            }
            else {
                $("#primary-header-modalLabel").html(resourceObj.addleaverequest);
            }
            url = basepath + "/LeaveRequest/AddEdit?Id=" + id;
            break;
        case '36':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.editexpense);
            }
            else {
                $("#primary-header-modalLabel").html(resourceObj.addexpense);
            }
            url = basepath + "/Expense/AddEdit?Id=" + id;
            break;
        case '37':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.editstudentassignment);
            }
            else {
                $("#primary-header-modalLabel").html(resourceObj.addstudentassignment);
            }
            url = basepath + "/StudentAssignment/AddEdit?Id=" + id;
            break;
        case '38':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.editstudentassignmentstatus);
            }
            else {
                $("#primary-header-modalLabel").html(resourceObj.addstudentassignmentstatus);
            }
            url = basepath + "/StudentAssignmentStatus/AddEdit?Id=" + id;
            break;
        case '39':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.editteacherannouncement);
            }
            else {
                $("#primary-header-modalLabel").html(resourceObj.addteacherannouncement);
            }
            url = basepath + "/TeacherAnnouncement/AddEdit?Id=" + id;
            break;
        case '40':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.editadmin);
            }
            else {
                $("#primary-header-modalLabel").html(resourceObj.addadmin);
            }
            url = basepath + "/Admin/AddEdit?Id=" + id;
            break;
        case '41':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.editstaff);
            }
            else {
                $("#primary-header-modalLabel").html(resourceObj.addstaff);
            }
            url = basepath + "/Staff/AddEdit?Id=" + id;
            break;
        case '42':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.editdriver);
            }
            else {
                $("#primary-header-modalLabel").html(resourceObj.adddriver);
            }
            url = basepath + "/Driver/AddEdit?Id=" + id;
            break;
        case '44':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.editfeestructure);
            }
            else {
                $("#primary-header-modalLabel").html(resourceObj.addfeestructure);
            }
            url = basepath + "/FeeStructure/AddEdit?Id=" + id;
            break;
        case '45':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.editcalendarevent);
            }
            else {
                $("#primary-header-modalLabel").html(resourceObj.addcalendarevent);
            }
            url = basepath + "/CalendarEvent/AddEdit?Id=" + id;
            break;
        case '46':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.editadminannouncement);
            }
            else {
                $("#primary-header-modalLabel").html(resourceObj.addadminannouncement);
            }
            url = basepath + "/AdminAnnouncement/AddEdit?Id=" + id;
            break;
        case '47':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.editpaymode);
            }
            else {
                $("#primary-header-modalLabel").html(resourceObj.addpaymode);
            }
            url = basepath + "/PayMode/AddEdit?Id=" + id;
            break;
        case '48':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.editadminnotification);
            }
            else {
                $("#primary-header-modalLabel").html(resourceObj.addadminnotification);
            }
            url = basepath + "/AdminNotification/AddEdit?Id=" + id;
            break;
        case '49':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.editactivity);
            }
            else {
                $("#primary-header-modalLabel").html(resourceObj.addactivity);
            }
            url = basepath + "/Activity/AddEdit?Id=" + id;
            break;
        case '50':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.editperiodbreak);
            }
            else {
                $("#primary-header-modalLabel").html(resourceObj.addperiodbreak);
            }
            url = basepath + "/PeriodBreak/AddEdit?Id=" + id;
            break;
    }    
    if (type == "8") {
        $('#myStaticModalBodyDiv1').load(url, function () {
            $('#staticBackdrop').modal("show");
        })
    }   
    else {
        $('#myModalBodyDiv1').load(url, function () {
            $('#primary-header-modal').modal("show");
        })
    }
}

function viewAction(type, id) {
    var url = '';
    switch (type) {
        case '1':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.viewbranch);
            }
            url = basepath + "/Branch/View?Id=" + id;
            break;
        case '2':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.viewclass);
            }
            url = basepath + "/Class/View?Id=" + id;
            break;
        case '3':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.viewsubject);
            }
            url = basepath + "/Subject/View?Id=" + id;
            break;
        case '4':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.viewtopic);
            }
            url = basepath + "/Topic/View?Id=" + id;
            break;
        case '5':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.viewsection);
            }
            url = basepath + "/Section/View?Id=" + id;
            break;
        case '6':
            if (id > 0) {
                $("#primary-header-modalLabel2").html(resourceObj.viewstudent);
            }
            url = basepath + "/Student/View?Id=" + id;
            break;
        case '7':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.viewexamtype);
            }
            url = basepath + "/ExamType/View?Id=" + id;
            break;
        case '8':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.viewschedule);
            }
            url = basepath + "/Schedule/View?Id=" + id;
            break;
        case '9':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.viewexamination);
            }
            url = basepath + "/Examination/View?Id=" + id;
            break;
        case '10':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.viewteacher);
            }
            url = basepath + "/Teacher/View?Id=" + id;
            break;
        case '11':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.viewgrade);
            }
            url = basepath + "/Grade/View?Id=" + id;
            break;
        case '12':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.viewholiday);
            }
            url = basepath + "/Holiday/View?Id=" + id;
            break;
        case '13':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.viewexamtime);
            }
            url = basepath + "/ExamTime/View?Id=" + id;
            break;
        case '14':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.viewquestiontype);
            }
            url = basepath + "/QuestionType/View?Id=" + id;
            break;
        case '15':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.viewdifficultylevel);
            }
            url = basepath + "/QuestionDifficulty/View?Id=" + id;
            break;
        case '16':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.viewquestioncategory);
            }
            url = basepath + "/QuestionCategory/View?Id=" + id;
            break;
        case '17':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.viewquestionbank);
            }
            url = basepath + "/QuestionBank/View?Id=" + id;
            break;
        case '18':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.viewrole);
            }
            url = basepath + "/Roles/View?Id=" + id;
           
            break;
        case '19':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.viewuser);
            }
            url = basepath + "/User/View?Id=" + id;            
            break;
        case '20':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.viewreportcard);
            }
            url = basepath + "/ReportCard/View?Id=" + id;
            break;
        case '21':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.viewattendence);
            }
            url = basepath + "/Attendence/View?Id=" + id;
            break;
        case '23':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.viewmedium);
            }
            url = basepath + "/Medium/View?Id=" + id;
            break;
        case '24':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.viewshift);
            }
            url = basepath + "/Shift/View?Id=" + id;
            break;
        case '28':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.viewbloodgroup);
            }
            url = basepath + "/BloodGroup/View?Id=" + id;
            break;
        case '29':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.viewfeetype);
            }
            url = basepath + "/FeeType/View?Id=" + id;
            break;
        case '30':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.viewenquirystatus);
            }
            url = basepath + "/EnquiryStatus/View?Id=" + id;
            break;
        case '31':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.viewlesson);
            }
            url = basepath + "/Lesson/View?Id=" + id;
            break;
        case '32':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.viewexpensecategory);
            }
            url = basepath + "/ExpenseCategory/View?Id=" + id;
            break;
        case '33':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.viewreferenceadmission);
            }
            url = basepath + "/ReferenceAdmission/View?Id=" + id;
            break;
        case '34':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.viewstudentenquiry);
            }
            url = basepath + "/StudentEnquiry/View?Id=" + id;
            break;
        case '35':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.viewleaverequest);
            }
            url = basepath + "/LeaveRequest/View?Id=" + id;
            break;
        case '36':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.viewexpense);
            }
            url = basepath + "/Expense/View?Id=" + id;
            break;
        case '37':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.viewstudentassignment);
            }
            url = basepath + "/StudentAssignment/View?Id=" + id;
            break;
        case '39':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.viewteacherannouncement);
            }
            url = basepath + "/TeacherAnnouncement/View?Id=" + id;
            break;
        case '40':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.viewadmin);
            }
            url = basepath + "/Admin/GetAdminDetail?Id=" + id;
            break;
        case '41':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.viewstaff);
            }
            url = basepath + "/Staff/View?Id=" + id;
            break;
        case '42':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.viewdriver);
            }
            url = basepath + "/Driver/View?Id=" + id;
            break;
        case '44':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.viewfeestructure);
            }
            url = basepath + "/FeeStructure/View?Id=" + id;
            break;
        case '45':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.viewcalendarevent);
            }
            url = basepath + "/CalendarEvent/View?Id=" + id;
            break;
        case '46':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.viewadminannouncement);
            }
            url = basepath + "/AdminAnnouncement/View?Id=" + id;
            break;
        case '47':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.viewpaymode);
            }
            url = basepath + "/PayMode/View?Id=" + id;
            break;
        case '49':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.viewactivity);
            }
            url = basepath + "/Activity/View?Id=" + id;
            break;
        case '50':
            if (id > 0) {
                $("#primary-header-modalLabel").html(resourceObj.viewperiodbreak);
            }
            url = basepath + "/PeriodBreak/View?Id=" + id;
            break;
    }
    if (type == "6") {
        $('#myModalBodyDiv2').load(url, function () {
            $('#primary-header-modal2').modal("show");
        })
    }
    else {
        $('#myModalBodyDiv1').load(url, function () {
            $('#primary-header-modal').modal("show");
        })
    }

}
function deleteAction(type, elid, id) {
    
    var delurl = '';
    if (id > 0) {
        bootbox.confirm({
            message: resourceObj.proceed,
            buttons: {
                confirm: {
                    label: resourceObj.yes,
                    className: 'btn-success'
                },
                cancel: {
                    label: resourceObj.no,
                    className: 'btn-danger'
                }
            },
            callback: function (result) {
                if (result) {
                    switch (type) {
                        case '1':
                            delurl = basepath + "/Branch/Delete";
                            break;
                        case '2':
                            delurl = basepath + "/Class/Delete";
                            break;
                        case '3':
                            delurl = basepath + "/Subject/Delete";
                            break;
                        case '4':
                            delurl = basepath + "/Topic/Delete";
                            break;
                        case '5':
                            delurl = basepath + "/Section/Delete";
                            break;
                        case '6':
                            delurl = basepath + "/Student/Delete";
                            break;
                        case '7':
                            delurl = basepath + "/ExamType/Delete";
                            break;
                        case '8':
                            delurl = basepath + "/Schedule/Delete";
                            break;
                        case '9':
                            delurl = basepath + "/Examination/Delete";
                            break;
                        case '10':
                            delurl = basepath + "/Teacher/Delete";
                            break;
                        case '11':
                            delurl = basepath + "/Grade/Delete";
                            break;
                        case '12':
                            delurl = basepath + "/Holiday/Delete";
                            break;
                        case '13':
                            delurl = basepath + "/ExamTime/Delete";
                            break;
                        case '14':
                            delurl = basepath + "/QuestionType/Delete";
                            break;
                        case '15':
                            delurl = basepath + "/QuestionDifficulty/Delete";
                            break;
                        case '16':
                            delurl = basepath + "/QuestionCategory/Delete";
                            break;
                        case '17':
                            delurl = basepath + "/QuestionBank/Delete";
                            break;
                        case '18':
                            delurl = basepath + "/Roles/DeleteRole";
                            break;
                        case '20':
                            delurl = basepath + "/ReportCard/Delete";
                            break;
                        case '23':
                            delurl = basepath + "/Medium/Delete";
                            break;
                        case '24':
                            delurl = basepath + "/Shift/Delete";
                            break;
                        case '26':
                            delurl = basepath + "/SessionYear/Delete";
                            break;
                        case '28':
                            delurl = basepath + "/BloodGroup/Delete";
                            break;
                        case '29':
                            delurl = basepath + "/FeeType/Delete";
                            break;
                        case '30':
                            delurl = basepath + "/EnquiryStatus/Delete";
                            break;
                        case '31':
                            delurl = basepath + "/Lesson/Delete";
                            break;
                        case '32':
                            delurl = basepath + "/ExpenseCategory/Delete";
                            break;
                        case '33':
                            delurl = basepath + "/ReferenceAdmission/Delete";
                            break;
                        case '34':
                            delurl = basepath + "/StudentEnquiry/Delete";
                            break;
                        case '35':
                            delurl = basepath + "/LeaveRequest/Delete";
                            break;
                        case '36':
                            delurl = basepath + "/Expense/Delete";
                            break;
                        case '37':
                            delurl = basepath + "/StudentAssignment/Delete";
                            break;
                        case '39':
                            delurl = basepath + "/TeacherAnnouncement/Delete";
                            break;
                        case '40':
                            delurl = basepath + "/Admin/Delete";
                            break;
                        case '41':
                            delurl = basepath + "/Staff/Delete";
                            break;
                        case '42':
                            delurl = basepath + "/Driver/Delete";
                            break;
                        case '44':
                            delurl = basepath + "/FeeStructure/Delete";
                            break;
                        case '45':
                            delurl = basepath + "/CalendarEvent/Delete";
                            break;
                        case '46':
                            delurl = basepath + "/AdminAnnouncement/Delete";
                            break;
                        case '47':
                            delurl = basepath + "/PayMode/Delete";
                            break;
                        case '48':
                            delurl = basepath + "/AdminNotification/Delete";
                            break;
                        case '49':
                            delurl = basepath + "/Activity/Delete";
                            break;
                        case '50':
                            delurl = basepath + "/PeriodBreak/Delete";
                            break;
                    }                   
                    $.ajax({
                        type: 'POST',
                        url: delurl,
                        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
                        data: { id: id },
                        success: function (response) {
                            if (response != null) {
                                var serviceresult = JSON.parse(response);                          
                                actionAfterResponse(elid, serviceresult.recordId, serviceresult.statusId, type);
                            }
                            else {                              
                                toastr.error(resourceObj.error);
                            }
                        },
                        error: function (response) {
                            toastr.error(resourceObj.error);
                        }
                    })
                }

            }
        });
    }
}
function onuploadFile1(id, file) {

    if (file.files.length == 0) {
        toastr.error("No file selected");
    }
    var ext = file.value.split('.').pop();
    if (ext !== "pdf") {
        toastr.error("only pdf is allowed");
    } else {
        const size = (file.files[0].size / 1024 / 1024).toFixed(2);
        if (size > 4) {
            toastr.error("File size must be less than 4 MB");
        }
        else {
            $("#loaderDiv").show();
            var postData = new FormData($('#myForm')[0]);
            postData.append("File", file.files[0]); // file
            postData.append("Id", id);
            $.ajax({
                type: 'POST',
                url: basepath + "/LabTests/UploadTestFile",
                data: postData,
                processData: false,
                contentType: false,
                success: function (response) {
                    if (response != null) {
                        $("#loaderDiv").hide();
                        toastMessage('File uploaded Succesfully');
                    } else {
                        toastr.error(`@_localization.Getkey("Error`);

                        $("#loaderDiv").hide();

                    }
                },
                error: function (response) {
                    toastr.error(`@_localization.Getkey("Error`);
                    $("#loaderDiv").hide();


                }
            });
        }
    }

}
function validateuploadFile(type, filetype, file) {
    var ext;
    var size;
    var errMsg = "";
    if (file.files.length == 0) {
        errMsg = "No file selected";
    }
    let typexists = Object.values(uploadType).includes(type);
    if (typexists) {
        ext = file.value.split('.').pop();
        size = (file.files[0].size / 1024 / 1024).toFixed(2);
        if (size > 4) {
            errMsg = "File size must be less than 4 MB";
        }
        else {
            switch (filetype) {
                case uploadFileType.img:
                    if (ext !== "jpg" & ext !== "jpeg" & ext !== "png") {
                        errMsg = "only jpg,jpeg,png are allowed";
                    }
                    break;
                case uploadFileType.pdfDoc:
                    if (ext !== "pdf") {
                        errMsg = "only pdf is allowed";
                    }
                    break;
            }
        }
    }
    //console.log(typexists);
    if (errMsg !== "") {
        toastr.error(errMsg);
    }
    return errMsg;
    //var ext = file.value.split('.').pop();
    //if (ext !== "pdf") {
    //    toastr.error("only pdf is allowed");
    //} else {
    //    const size = (file.files[0].size / 1024 / 1024).toFixed(2);
    //    if (size > 4) {
    //        toastr.error("File size must be less than 4 MB");
    //    }
    //    else {
    //        $("#loaderDiv").show();
    //        var postData = new FormData($('#myForm')[0]);
    //        postData.append("File", file.files[0]); // file
    //        postData.append("Id", id);
    //        $.ajax({
    //            type: 'POST',
    //            url: basepath + "/LabTests/UploadTestFile",
    //            data: postData,
    //            processData: false,
    //            contentType: false,
    //            success: function (response) {
    //                if (response != null) {
    //                    $("#loaderDiv").hide();
    //                    toastMessage('File uploaded Succesfully');
    //                } else {
    //                    toastr.error(`@_localization.Getkey("Error`);

    //                    $("#loaderDiv").hide();

    //                }
    //            },
    //            error: function (response) {
    //                toastr.error(`@_localization.Getkey("Error`);
    //                $("#loaderDiv").hide();


    //            }
    //        });
    //    }
    //}

}
function onuploadQPaper(id, file) {
   
    if (file.files.length == 0) {
        toastr.error("No file selected");
    }
    var ext = file.value.split('.').pop();
    if (ext !== "pdf") {
        toastr.error("only pdf is allowed");
    } else {
        const size = (file.files[0].size / 1024 / 1024).toFixed(2);
        if (size > 4) {
            toastr.error("File size must be less than 4 MB");
        }
        else {
            $("#loaderDiv").show();
            var postData = new FormData();
            postData.append("formFile", file.files[0]); // file
            postData.append("ExamId", id);
            $.ajax({
                type: 'POST',
                url: basepath + "/Examination/UploadQPaper",
                data: postData,
                // data: { ExamId: id, formFile: file.files[0] },
                processData: false,
                contentType: false,
                success: function (response) {
                    if (response != null) {
                        $("#loaderDiv").hide();
                        toastr.success('File uploaded Succesfully');
                    } else {
                        toastr.error(`@_localization.Getkey("Error`);

                        $("#loaderDiv").hide();

                    }
                },
                error: function (response) {
                    toastr.error(`@_localization.Getkey("Error`);
                    $("#loaderDiv").hide();


                }
            });
        }
    }

}
function ondownloadQPaper(id) {
    var url = basepath + "/Examination/DownloadQpaper/?id=" + id;
    window.location = url;
}
function ondownloadAttach(id) {
    var url = basepath + "/Attachment/Download/?id=" + id;
    window.location = url;
}
function ondownloadExcel(id) {
    var url = basepath + "/ScheduleResult/GetExportData/?id=" + id;
    window.location = url;
}
function DownloadFile(id, filename) {
    var url = basepath + "/LabTests/DownloadTestFile/" + id;
    $.ajax({
        url: url,
        cache: false,
        xhr: function () {
            var xhr = new XMLHttpRequest();
            xhr.onreadystatechange = function () {
                if (xhr.readyState == 2) {
                    if (xhr.status == 200) {
                        xhr.responseType = "blob";
                    } else {
                        xhr.responseType = "text";
                    }
                }
            };
            return xhr;
        },
        success: function (data) {
            //Convert the Byte Data to BLOB object.
            var blob = new Blob([data], { type: "application/octetstream" });

            //Check the Browser type and download the File.
            var isIE = false || !!document.documentMode;
            if (isIE) {
                window.navigator.msSaveBlob(blob, fileName);
            } else {
                var url = window.URL || window.webkitURL;
                // var filename= url.split('/').pop();
                link = url.createObjectURL(blob);
                var a = $("<a />");
                a.attr("download", filename);
                a.attr("href", link);
                $("body").append(a);
                a[0].click();
                $("body").remove(a);
            }
        }
    });
};
function SelectedStatIndexChanged(id, select) {
    if (select.value === "") {
        toastr.error("Please select a valid status");
    }
    else {
        $("#loaderDiv").show();
        $.ajax({
            type: "post",
            url: basepath + "/LabTests/UpdateTestStatus",
            data: { id: id, statusId: select.value },
            dataType: 'json',
            traditional: true,
            success: function (response) {
                if (response != null) {
                    if (select.value == "1") {
                        select.style.backgroundColor = "#0099ff";
                    }
                    else if (select.value == "2") {
                        select.style.backgroundColor = "#008080";
                    }
                    else if (select.value == "3") {
                        select.style.backgroundColor = "#d4af37";
                    }
                    // var cell = select.parentElement;
                    // if (select.value == "1") {
                    //     cell.style.backgroundColor ="blue";
                    // }
                    //else if (select.value == "2") {
                    //     cell.style.backgroundColor ="green";
                    // }
                    // else if (select.value == "3") {
                    //     cell.style.backgroundColor ="orange";
                    // }
                    toastr.success("Status Changed Succesfully");
                } else {
                    toastr.error(`@_localization.Getkey("Error`);
                }
                $("#loaderDiv").hide();
            },
            error: function (response) {
                toastr.error(`@_localization.Getkey("Error`);
            }
        });
    }
}
function GetStudentDue(studentId) {
    if (studentId > 0) {
        document.getElementById('primary-header-modalLabel').innerHTML = "Student Payment";
    }
    var url = basepath + "/Student/GetStudentPay?StudentId=" + studentId;
    $("#myModalBodyDiv1").load(url, function () {
        $("#primary-header-modal").modal("show");
    })
}


var resourceObj = {};
function getResourceObj(jsonobj) {
    resourceObj = JSON.parse(jsonobj);
}
function actionAfterResponse(elid, id, statusid, type) {
    var obj = [];
    if (statusid != 3) {
        switch (type) {
            case '1':
                obj = getBranchById(id);
                break;
            case '2':
                obj = getClassById(id);
                break;
            case '3':
                obj = getSubjectById(id);
                break;
            case '4':
                obj = getTopicById(id);
                break;
            case '5':
                obj = getSectionById(id);
                break;
            case '6':
                obj = getStudentById(id);
                break;
            case '7':
                obj = getExamTypeById(id);
                break;
            case '10':
                obj = getTeacherById(id);
                break;
            case '11':
                obj = getGradeById(id);
                break;
            case '12':
                obj = getHolidayById(id);
                break;
            case '13':
                obj = getExamTimeById(id);
                break;
            case '14':
                obj = getQuestionTypeById(id);
                break;
            case '15':
                obj = getQuestionDifficultyById(id);
                break;
            case '16':
                obj = getQuestionCategoryById(id);
                break;
            case '17':
                obj = getQuestionBankById(id);
                break;                
            case '18':
                obj = getRoleById(id);
                break;
            case '19':
                obj = getUserById(id);
                break;
            case '20':
                obj = getReportCardById(id);
                break;                
            case '22':
                obj = getLicenseById(id);
                break;
            case '23':
                obj = getMediumById(id);
                break;
            case '24':
                obj = getShiftById(id);
                break;
            case '25':
                obj = getBranchClassById(id);
                break; 
            case '26':
                obj = getSessionYearById(id);
                break; 
            case '27':
                obj = getAttachmentById(id);
                break;  
            case '28':
                obj = getBloodGroupById(id);
                break;  
            case '29':
                obj = getFeeTypeById(id);
                break;  
            case '30':
                obj = getEnquiryStatusById(id);
                break;  
            case '31':
                obj = getLessonById(id);
                break;  
            case '32':
                obj = getExpenseCategoryById(id);
                break; 
            case '33':
                obj = getReferenceAdmissionById(id);
                break; 
            case '34':
                obj = getStudentEnquiryById(id);
                break;
            case '35':
                obj = getLeaveRequestById(id);
                break;
            case '36':
                obj = getExpenseById(id);
                break;
            case '37':
                obj = getStudentAssignmentById(id);
                break; 
            case '38':
                obj = getStudentAssignmentStatusById(id);
                break;  
            case '39':
                obj = getTeacherAnnouncementById(id);
                break;  
            case '40':
                obj = getAdminById(id);
                break;    
            case '41':
                obj = getStaffById(id);
                break; 
            case '42':
                obj = getDriverById(id);
                break;  
            case '43':
                obj = getNotificationById(id);
                break; 
            case '44':
                obj = getFeeStructureById(id);
                break;
            case '46':
                obj = getAdminAnnouncementById(id);
                break;
            case '47':
                obj = getPayModeById(id);
                break;
            case '48':
                obj = getAdminNotificationById(id);
                break;
            case '49':
                obj = getActivityById(id);
                
                break;
            case '50':
                obj = getPeriodBreakById(id);
                break;
                
                
        }
    }    
    var el = document.getElementById(elid);
    var elmId = el.id;
    var table = $("#" + elmId).DataTable();
   
    if (statusid == 1) {       
        var newRow = table.row.add(obj[0]).draw(false).node();

        // Insert the new row at the beginning of the table
        table.rows().every(function (index) {
            var rowNode = this.node();
            if (rowNode === newRow) {
                // Get the current page information
                var pageInfo = table.page.info();
                if (pageInfo.page === 0) {
                    $(rowNode).prependTo(table.table().body());
                }
                else {
                    table.page(0).draw(false);
                    $(rowNode).prependTo(table.table().body());
                }
                return false;
            }
        });
        toastr.success(resourceObj.added);
    }
    else if (statusid == 2) {
        jQuery(table.rows().nodes()).each(function (item) {
            var tr = this;
            var d = table.row(tr).data();
            if (d[0] == id) {
                table.row(tr).data(obj[0]).draw(false);
                toastr.success(resourceObj.updated);
            }
        });
    }
    else if (statusid == 3) {
        jQuery(table.rows().nodes()).each(function (item) {
            var tr = this;
            var d = table.row(tr).data();
            if (d[0] == id) {
                table.row(tr).remove().draw(false);                
                toastr.success(resourceObj.deleted);
            }
        });

    }
    if (type == '27') {
        $("#primary-header-modal3").modal("hide");
        $("#loaderDiv3").hide();
    }
    else {
        $("#primary-header-modal").modal("hide");
        $("#primary-header-modal2").modal("hide");
        $("#loaderDiv1").hide();
    }
   
   // testNotification(obj);
}
function testNotification(obj) {
    var strObj = JSON.stringify(obj);
    $.ajax({
        type: 'POST',
        url: 'http://192.168.58.106/SchoolAPI/api/notification/send/message=' + 'test',
        //contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        //data: strObj,
        success: function (response) {
            if (response != null) {
                toastr.success(response);
            } else {
                toastr.error(response);
            }
        },
        error: function (response) {
            toastr.error(response);

        }
    })
}
function getBranchById(id) {
    var branchData = [];
    $.ajax({
        type: 'GET',
        async: false,
        url: basepath + "/Branch/GetBranch?Id=" + id,        
        Accept: 'application/json, text/javascript',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (response) {
            if (response != null) {               
                var obj = response;
                var btndelete = "";
                if (obj.isDelete == "1") {
                    btndelete = '<a class="dropdown-item" href="#" onclick="deleteAction(`1`,`BranchGrid`, ' + obj.id + ')"><i class="ri-delete-bin-line"></i>&nbsp; ' + resourceObj.delete + '</a>'
                }
                var strclass = "";
                if (obj.classCount > 0) {
                    strclass = obj.classCount;
                }
                var strsection = "";
                if (obj.sectionCount > 0) {
                    strsection = obj.sectionCount;
                }
                var strstudent = "";
                if (obj.studentCount > 0) {
                    strstudent = obj.studentCount;
                }
                var strteacher = "";
                if (obj.teacherCount > 0) {
                    strteacher = obj.teacherCount;
                }
                var stradmin = "";
                if (obj.adminName != null) {
                    stradmin = `<span class="badge bg-success">` + obj.adminName + `</span>`
                }
                //var strschedule = "";
                //if (obj.scheduleCount > 0) {
                //    strschedule = obj.scheduleCount;
                //}
                var btnaction = '<div class="btn-group btn-group-sm dropdown" onclick="onSelect(this,BranchGrid);"> ' +
                    '<a href="#" class="table-action-btn dropdown-toggle arrow-none btn btn-light btn-xs" data-bs-toggle="dropdown" aria-expanded="false"><i class="mdi mdi-dots-horizontal"></i></a>' +
                    '<div class="dropdown-menu dropdown-menu-end">' +
                    '<a class="dropdown-item" href="#" onclick="viewAction(`1`, ' + obj.id + ')"><i class="ri-eye-line"></i>&nbsp; ' + resourceObj.view +'</a>' +
                    '<a class="dropdown-item" href="#" onclick="addEditAction(`1`, ' + obj.id + ')"><i class="ri-pencil-line"></i>&nbsp; ' + resourceObj.edit +'</a>' +
                    ' <a class="dropdown-item" href="#" onclick="AddEditBranchClass(' + obj.id + ')"><i class="ri-list-ordered"></i>&nbsp; ' + resourceObj.classes + '</a>' +
                    ' <a class="dropdown-item" href = "#" onclick = "ViewBranchSectionTimeTable(' + obj.id + ')" > <i class="ri-list-ordered" > </i>&nbsp; ' + resourceObj.branchtimetable + '</a > ' + btndelete +
                    '</div>' +
                    '</div>'

                branchData.push([obj.id, obj.name, obj.theme, obj.phoneNo, obj.emailAddress, obj.address, stradmin, strclass, strsection, strstudent, strteacher, btnaction])
               
            } else {
                toastr.error(resourceObj.error);
            }
        },
        error: function (response) {
            toastr.error(resourceObj.error);

        }
    })
    return branchData;
}
function getClassById(id) {
    var classData = [];
    $.ajax({
        type: 'GET',
        async: false,
        url: basepath + "/Class/GetClass?Id=" + id,
        Accept: 'application/json, text/javascript',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (response) {
            if (response != null) {
                var obj = response;

                var btndelete = "";
                if (obj.isDelete == "1") {
                    btndelete = '<a class="dropdown-item" href="#" onclick="deleteAction(`2`,`ClassGrid`, ' + obj.id + ')"><i class="ri-delete-bin-line"></i>&nbsp; ' + resourceObj.delete + '</a>'
                }
                var btnaction = '<div class="btn-group btn-group-sm dropdown" onclick="onSelect(this,ClassGrid);">' +
                    '<a href="#" class="table-action-btn dropdown-toggle arrow-none btn btn-light btn-xs" data-bs-toggle="dropdown" aria-expanded="false"><i class="mdi mdi-dots-horizontal"></i></a>' +
                    '<div class="dropdown-menu dropdown-menu-end">' +
                    '<a class="dropdown-item" href="#" onclick="viewAction(`2`, ' + obj.id + ')"><i class="ri-eye-line"></i>&nbsp; ' + resourceObj.view + '</a>' +
                    '<a class="dropdown-item" href="#" onclick="addEditAction(`2`, ' + obj.id + ')"><i class="ri-pencil-line"></i>&nbsp; ' + resourceObj.edit + '</a>' +
                    ' <a class="dropdown-item" href="#" onclick="AddEditClassSubject(' + obj.id + ')"><i class="ri-list-ordered"></i>&nbsp; ' + resourceObj.classsubjects + '</a>' + btndelete +
                    '</div>' +
                    '</div>'
                classData.push([obj.id, obj.name, obj.shortName, obj.description, obj.mediumName, obj.subjectCount, btnaction])

            } else {
                toastr.error(resourceObj.error);
            }
        },
        error: function (response) {
            toastr.error(resourceObj.error);

        }
    })
    return classData;
}
function getSectionById(id) {
    var sectionData = [];
    $.ajax({
        type: 'GET',
        async: false,
        url: basepath + "/Section/GetSection?Id=" + id,
        Accept: 'application/json, text/javascript',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (response) {
            if (response != null) {
                var obj = response;                
                var spanslot = "";
                var spanshift = "";

                var spanclsteacher = "";
                var spansubteacher = "";
                var spantimetable = "";
                var btndelete = "";
                if (obj.slotDuration != "" && obj.slotDuration != null) {
                    spanslot = obj.slotDuration + " min";
                }
                if (obj.fromTime != "" && obj.fromTime != null && obj.toTime != "" && obj.toTime != null) {
                    spanshift = obj.fromTime + " - " + obj.toTime;
                }

                if (obj.classTeachers == "" || obj.classTeachers == null) {
                    spanclsteacher = `<span class="badge bg-danger"> <i class="ri-close-line"> </span>`
                }
                else {
                    spanclsteacher = `<span class="badge bg-success">` + obj.classTeachers + `</span>`
                }
                if (obj.subjectCount == obj.classSubjectCount) {
                    spansubteacher = `<span class="badge bg-success"><i class="ri-check-line"></i></span>`
                }
                else {
                    spansubteacher = `<span class="badge bg-danger"> <i class="ri-close-line"> </span>`
                }
                if (obj.timeTableCount == obj.classSubjectCount) {
                    spantimetable = `<span class="badge bg-success"><i class="ri-check-line"></i></span>`
                }
                else {
                    spantimetable = `<span class="badge bg-danger"> <i class="ri-close-line"> </span>`
                }
                //if (obj.subjectCount < obj.classSubjectCount) {
                //    spansubteacher = `<span class="badge bg-warning">` + resourceObj.incomplete + `</span>`
                //}
                //else {
                //    spansubteacher = `<span class="badge bg-success">` + resourceObj.complete + `</span>`
                //}
                //if (obj.timeTableCount < obj.classSubjectCount) {
                //    spantimetable = `<span class="badge bg-warning">` + resourceObj.incomplete + `</span>`
                //}
                //else {
                //    spantimetable = `<span class="badge bg-success">` + resourceObj.complete + `</span>`
                //}
                if (obj.isDelete == "1") {
                    btndelete = '<a class="dropdown-item" href="#" onclick="deleteAction(`5`,`SectionGrid`, ' + obj.id + ')"><i class="ri-delete-bin-line"></i>&nbsp; ' + resourceObj.delete + '</a>'
                }
                var btnaction = '<div class="btn-group btn-group-sm dropdown" onclick="onSelect(this,SectionGrid);"> ' +
                    '<a href="#" class="table-action-btn dropdown-toggle arrow-none btn btn-light btn-xs" data-bs-toggle="dropdown" aria-expanded="false"><i class="mdi mdi-dots-horizontal"></i></a>' +
                    '<div class="dropdown-menu dropdown-menu-end">' +
                    '<a class="dropdown-item" href="#" onclick="viewAction(`5`, ' + obj.id + ')"><i class="ri-eye-line"></i>&nbsp; ' + resourceObj.view + '</a>' +
                    '<a class="dropdown-item" href="#" onclick="addEditAction(`5`, ' + obj.id + ')"><i class="ri-pencil-line"></i>&nbsp; ' + resourceObj.edit + '</a>' +                  
                    ' <a class="dropdown-item" href="#" onclick="AddEditSectionTeacher(' + obj.id + ')"><i class="ri-list-ordered"></i>&nbsp; ' + resourceObj.classteacher + '</a>' +
                    ' <a class="dropdown-item" href="#" onclick="AddEditSectionActivityPersonnel(' + obj.id + ')"><i class="ri-list-ordered"></i>&nbsp; ' + resourceObj.activityteacher + '</a>' +
                    ' <a class="dropdown-item" href = "#" onclick = "AddEditSectionTimeTable(' + obj.id + ')" > <i class="ri-list-ordered" > </i>&nbsp; ' + resourceObj.timetable + '</a > ' + btndelete +

                    '</div>' +
                    '</div>'

                sectionData.push([obj.id, obj.name, obj.shortName, obj.branchName, obj.className, spanshift, spanslot, obj.studentCount, spanclsteacher, spansubteacher, spantimetable, btnaction])

            } else {
                toastr.error(resourceObj.error);
            }
        },
        error: function (response) {
            toastr.error(resourceObj.error);

        }
    })
    return sectionData;
}
function getExamTimeById(id) {
    var examtimeData = [];
    $.ajax({
        type: 'GET',
        async: false,
        url: basepath + "/ExamTime/GetExamTime?Id=" + id,
        Accept: 'application/json, text/javascript',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (response) {
            if (response != null) {
                var obj = response;               

                var btndelete = "";
                if (obj.isDelete == "1") {
                    btndelete = '<a class="dropdown-item" href="#" onclick="deleteAction(`13`,`ExamTimeGrid`, ' + obj.id + ')"><i class="ri-delete-bin-line"></i>&nbsp; ' + resourceObj.delete + '</a>'
                }
                var btnaction = '<div class="btn-group btn-group-sm dropdown" onclick="onSelect(this,ExamTimeGrid);">  ' +
                    '<a href="#" class="table-action-btn dropdown-toggle arrow-none btn btn-light btn-xs" data-bs-toggle="dropdown" aria-expanded="false"><i class="mdi mdi-dots-horizontal"></i></a>' +
                    '<div class="dropdown-menu dropdown-menu-end">' +
                    '<a class="dropdown-item" href="#" onclick="viewAction(`13`, ' + obj.id + ')"><i class="ri-eye-line"></i>&nbsp; ' + resourceObj.view + '</a>' +
                    '<a class="dropdown-item" href="#" onclick="addEditAction(`13`, ' + obj.id + ')"><i class="ri-pencil-line"></i>&nbsp; ' + resourceObj.edit + '</a>' + btndelete +

                    '</div>' +
                    '</div>'

                examtimeData.push([obj.id, obj.fromTime, obj.toTime, obj.description, btnaction])
            } else {
                toastr.error(resourceObj.error);
            }
        },
        error: function (response) {
            toastr.error(resourceObj.error);
        }
    })
    return examtimeData;
}
function getExamTypeById(id) {
    var examtypeData = [];
    $.ajax({
        type: 'GET',
        async: false,
        url: basepath + "/ExamType/GetExamType?Id=" + id,
        Accept: 'application/json, text/javascript',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (response) {
            if (response != null) {
                var obj = response;    

                var btndelete = "";
                if (obj.isDelete == "1") {
                    btndelete = '<a class="dropdown-item" href="#" onclick="deleteAction(`7`,`ExamTypeGrid`, ' + obj.id + ')"><i class="ri-delete-bin-line"></i>&nbsp; ' + resourceObj.delete + '</a>'
                }
                var btnaction = '<div class="btn-group btn-group-sm dropdown" onclick="onSelect(this,ExamTypeGrid);"> ' +
                    '<a href="#" class="table-action-btn dropdown-toggle arrow-none btn btn-light btn-xs" data-bs-toggle="dropdown" aria-expanded="false"><i class="mdi mdi-dots-horizontal"></i></a>' +
                    '<div class="dropdown-menu dropdown-menu-end">' +
                    '<a class="dropdown-item" href="#" onclick="viewAction(`7`, ' + obj.id + ')"><i class="ri-eye-line"></i>&nbsp; ' + resourceObj.view + '</a>' +
                    '<a class="dropdown-item" href="#" onclick="addEditAction(`7`, ' + obj.id + ')"><i class="ri-pencil-line"></i>&nbsp; ' + resourceObj.edit + '</a>' + btndelete +
                    '</div>' +
                    '</div>'

                examtypeData.push([obj.id, obj.name, obj.description, btnaction])

            } else {
                toastr.error(resourceObj.error);
            }
        },
        error: function (response) {
            toastr.error(resourceObj.error);
        }
    })
    return examtypeData;
}
function getGradeById(id) {
    var gradeData = [];
    $.ajax({
        type: 'GET',
        async: false,
        url: basepath + "/Grade/GetGrade?Id=" + id,
        Accept: 'application/json, text/javascript',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (response) {
            if (response != null) {
                var obj = response;
                var inputcolor = '<input class="form-control" type="color" name="color" disabled value=' + obj.color + '>'
                var btndelete = "";
                //btndelete = '<a class="dropdown-item" href="#" onclick="deleteAction(`1`, '+obj.Id+')"><i class="ri-delete-bin-line"></i>&nbsp; Delete</a>'

                var btnaction = '<div class="btn-group btn-group-sm dropdown" onclick="onSelect(this,GradeGrid);"> ' +
                    '<a href="#" class="table-action-btn dropdown-toggle arrow-none btn btn-light btn-xs" data-bs-toggle="dropdown" aria-expanded="false"><i class="mdi mdi-dots-horizontal"></i></a>' +
                    '<div class="dropdown-menu dropdown-menu-end">' +
                    '<a class="dropdown-item" href="#" onclick="viewAction(`11`, ' + obj.id + ')"><i class="ri-eye-line"></i>&nbsp; ' + resourceObj.view + '</a>' +
                    '<a class="dropdown-item" href="#" onclick="addEditAction(`11`, ' + obj.id + ')"><i class="ri-pencil-line"></i>&nbsp; ' + resourceObj.edit + '</a>' + btndelete +

                    '</div>' +
                    '</div>'

                gradeData.push([obj.id, obj.name, obj.minPercent, obj.maxPercent, inputcolor, obj.description, btnaction])
            } else {
                toastr.error(resourceObj.error);
            }
        },
        error: function (response) {
            toastr.error(resourceObj.error);
        }
    })
    return gradeData;
}
function getHolidayById(id) {
    var holidayData = [];
    $.ajax({
        type: 'GET',
        async: false,
        url: basepath + "/Holiday/GetHoliday?Id=" + id,
        Accept: 'application/json, text/javascript',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (response) {
            if (response != null) {
                var obj = response;          

                
                var btndelete = "";
                btndelete = '<a class="dropdown-item" href="#" onclick="deleteAction(`12`,`HolidayGrid`, ' + obj.id + ')"><i class="ri-delete-bin-line"></i>&nbsp; ' + resourceObj.delete + '</a>'

                var btnaction = '<div class="btn-group btn-group-sm dropdown" onclick="onSelect(this,HolidayGrid);"> ' +
                    '<a href="#" class="table-action-btn dropdown-toggle arrow-none btn btn-light btn-xs" data-bs-toggle="dropdown" aria-expanded="false"><i class="mdi mdi-dots-horizontal"></i></a>' +
                    '<div class="dropdown-menu dropdown-menu-end">' +
                    '<a class="dropdown-item" href="#" onclick="viewAction(`12`, ' + obj.id + ')"><i class="ri-eye-line"></i>&nbsp; ' + resourceObj.view + '</a>' +
                    '<a class="dropdown-item" href="#" onclick="addEditAction(`12`, ' + obj.id + ')"><i class="ri-pencil-line"></i>&nbsp; ' + resourceObj.edit + '</a>' + btndelete +
                    '</div>' +
                    '</div>'

                holidayData.push([obj.id, obj.title, obj.strStartDate, obj.strEndDate, obj.description, btnaction])

            } else {
                toastr.error(resourceObj.error);
            }
        },
        error: function (response) {
            toastr.error(resourceObj.error);
        }
    })
    return holidayData;
}
function getSubjectById(id) {
    var subjectData = [];
    $.ajax({
        type: 'GET',
        async: false,
        url: basepath + "/Subject/GetSubject?Id=" + id,
        Accept: 'application/json, text/javascript',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (response) {
            if (response != null) {
                var obj = response;
                //var inputcolor = '<input class="form-control" type="color" name="color" disabled value=' + obj.subjectColor + '>'
                var subtype = "";
                if (obj.subjectType == "1") {
                    subtype = resourceObj.theory;
                }
                else {
                    subtype = resourceObj.practical;
                }
                var btndelete = "";
                if (obj.isDelete == "1") {
                    btndelete = '<a class="dropdown-item" href="#" onclick="deleteAction(`3`,`SubjectGrid`, ' + obj.id + ')"><i class="ri-delete-bin-line"></i>&nbsp; ' + resourceObj.delete + '</a>'
                }
                var btnaction = '<div class="btn-group btn-group-sm dropdown" onclick="onSelect(this,SubjectGrid);"> ' +
                    '<a href="#" class="table-action-btn dropdown-toggle arrow-none btn btn-light btn-xs" data-bs-toggle="dropdown" aria-expanded="false"><i class="mdi mdi-dots-horizontal"></i></a>' +
                    '<div class="dropdown-menu dropdown-menu-end">' +
                    '<a class="dropdown-item" href="#" onclick="viewAction(`3`, ' + obj.id + ')"><i class="ri-eye-line"></i>&nbsp; ' + resourceObj.view + '</a>' +
                    '<a class="dropdown-item" href="#" onclick="addEditAction(`3`, ' + obj.id + ')"><i class="ri-pencil-line"></i>&nbsp; ' + resourceObj.edit + '</a>' + btndelete +
                    '</div>' +
                    '</div>'
                subjectData.push([obj.id, obj.name, obj.shortName, obj.mediumName, obj.subjectColor, obj.subjectCode, subtype, obj.description, btnaction])

            } else {
                toastr.error(resourceObj.error);
            }
        },
        error: function (response) {
            toastr.error(resourceObj.error);
        }
    })
    return subjectData;
}
function getTopicById(id) {
    var topicData = [];
    $.ajax({
        type: 'GET',
        async: false,
        url: basepath + "/Topic/GetTopic?Id=" + id,
        Accept: 'application/json, text/javascript',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (response) {
            if (response != null) {
                var obj = response;

                var btndelete = "";
                if (obj.isDelete == "1") {
                    btndelete = '<a class="dropdown-item" href="#" onclick="deleteAction(`4`, `TopicGrid`,' + obj.id + ')"><i class="ri-delete-bin-line"></i>&nbsp; ' + resourceObj.delete + '</a>'
                }

                var btnaction = '<div class="btn-group btn-group-sm dropdown" onclick="onSelect(this,TopicGrid);"> ' +
                    '<a href="#" class="table-action-btn dropdown-toggle arrow-none btn btn-light btn-xs" data-bs-toggle="dropdown" aria-expanded="false"><i class="mdi mdi-dots-horizontal"></i></a>' +
                    '<div class="dropdown-menu dropdown-menu-end">' +
                    '<a class="dropdown-item" href="#" onclick="viewAction(`4`, ' + obj.id + ')"><i class="ri-eye-line"></i>&nbsp; ' + resourceObj.view + '</a>' +
                    '<a class="dropdown-item" href="#" onclick="addEditAction(`4`, ' + obj.id + ')"><i class="ri-pencil-line"></i>&nbsp; ' + resourceObj.edit + '</a>' + btndelete +
                    '</div>' +
                    '</div>'

                topicData.push([obj.id, obj.name, obj.lessonName, obj.subjectName, obj.className, obj.description, btnaction])

            } else {
                toastr.error(resourceObj.error);
            }
        },
        error: function (response) {
            toastr.error(resourceObj.error);
        }
    })
    return topicData;
}
function getQuestionTypeById(id) {
    var questTypeData = [];
    $.ajax({
        type: 'GET',
        async: false,
        url: basepath + "/QuestionType/GetQuestionType?Id=" + id,
        Accept: 'application/json, text/javascript',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (response) {
            if (response != null) {
                var obj = response;

                var btndelete = "";
                if (obj.isDelete == "1") {
                    btndelete = '<a class="dropdown-item" href="#" onclick="deleteAction(`14`,`QuestionTypeGrid`, ' + obj.id + ')"><i class="ri-delete-bin-line"></i>&nbsp; ' + resourceObj.delete + '</a>'
                }
                var btnaction = '<div class="btn-group btn-group-sm dropdown" onclick="onSelect(this,QuestionTypeGrid);"> ' +
                    '<a href="#" class="table-action-btn dropdown-toggle arrow-none btn btn-light btn-xs" data-bs-toggle="dropdown" aria-expanded="false"><i class="mdi mdi-dots-horizontal"></i></a>' +
                    '<div class="dropdown-menu dropdown-menu-end">' +
                    '<a class="dropdown-item" href="#" onclick="viewAction(`14`, ' + obj.id + ')"><i class="ri-eye-line"></i>&nbsp; ' + resourceObj.view + '</a>' +
                    '<a class="dropdown-item" href="#" onclick="addEditAction(`14`, ' + obj.id + ')"><i class="ri-pencil-line"></i>&nbsp; ' + resourceObj.edit + '</a>' + btndelete +
                    '</div>' +
                    '</div>'

                questTypeData.push([obj.id, obj.name, obj.description, btnaction])

            } else {
                toastr.error(resourceObj.error);
            }
        },
        error: function (response) {
            toastr.error(resourceObj.error);
        }
    })
    return questTypeData;
}
function getQuestionDifficultyById(id) {
    var diffData = [];
    $.ajax({
        type: 'GET',
        async: false,
        url: basepath + "/QuestionDifficulty/GetQuestionDifficulty?Id=" + id,
        Accept: 'application/json, text/javascript',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (response) {
            if (response != null) {
                var obj = response;               

                var btndelete = "";
                if (obj.isDelete == "1") {
                    btndelete = '<a class="dropdown-item" href="#" onclick="deleteAction(`15`,`DifficultyGrid`, ' + obj.id + ')"><i class="ri-delete-bin-line"></i>&nbsp; ' + resourceObj.delete + '</a>'
                }
                var btnaction = '<div class="btn-group btn-group-sm dropdown" onclick="onSelect(this,DifficultyGrid);"> ' +
                    '<a href="#" class="table-action-btn dropdown-toggle arrow-none btn btn-light btn-xs" data-bs-toggle="dropdown" aria-expanded="false"><i class="mdi mdi-dots-horizontal"></i></a>' +
                    '<div class="dropdown-menu dropdown-menu-end">' +
                    '<a class="dropdown-item" href="#" onclick="viewAction(`15`, ' + obj.id + ')"><i class="ri-eye-line"></i>&nbsp; ' + resourceObj.view + '</a>' +
                    '<a class="dropdown-item" href="#" onclick="addEditAction(`15`, ' + obj.id + ')"><i class="ri-pencil-line"></i>&nbsp; ' + resourceObj.edit + '</a>' + btndelete +
                    '</div>' +
                    '</div>'

                diffData.push([obj.id, obj.name, obj.description, btnaction])

            } else {
                toastr.error(resourceObj.error);
            }
        },
        error: function (response) {
            toastr.error(resourceObj.error);
        }
    })
    return diffData;
}
function getQuestionCategoryById(id) {
    var questCategoryData = [];
    $.ajax({
        type: 'GET',
        async: false,
        url: basepath + "/QuestionCategory/GetQuestionCategory?Id=" + id,
        Accept: 'application/json, text/javascript',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (response) {
            if (response != null) {
                var obj = response;               

                var btndelete = "";
                if (obj.isDelete == "1") {
                    btndelete = '<a class="dropdown-item" href="#" onclick="deleteAction(`16`,`QuestionCategoryGrid`, ' + obj.id + ')"><i class="ri-delete-bin-line"></i>&nbsp; ' + resourceObj.delete + '</a>'
                }
                var btnaction = '<div class="btn-group btn-group-sm dropdown" onclick="onSelect(this,QuestionCategoryGrid);"> ' +
                    '<a href="#" class="table-action-btn dropdown-toggle arrow-none btn btn-light btn-xs" data-bs-toggle="dropdown" aria-expanded="false"><i class="mdi mdi-dots-horizontal"></i></a>' +
                    '<div class="dropdown-menu dropdown-menu-end">' +
                    '<a class="dropdown-item" href="#" onclick="viewAction(`16`, ' + obj.id + ')"><i class="ri-eye-line"></i>&nbsp; ' + resourceObj.view + '</a>' +
                    '<a class="dropdown-item" href="#" onclick="addEditAction(`16`, ' + obj.id + ')"><i class="ri-pencil-line"></i>&nbsp; ' + resourceObj.edit + '</a>' + btndelete +
                    '</div>' +
                    '</div>'
                questCategoryData.push([obj.id, obj.name, obj.description, btnaction])

            } else {
                toastr.error(resourceObj.error);
            }
        },
        error: function (response) {
            toastr.error(resourceObj.error);
        }
    })
    return questCategoryData;
}
function getStudentById(id) {
    var studentData = [];
    $.ajax({
        type: 'GET',
        async: false,
        url: basepath + "/Student/GetStudent?Id=" + id,
        Accept: 'application/json, text/javascript',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (response) {
            if (response != null) {
                var obj = response;   
               

                var paybtn = "";
                if (obj.dueAmount > 0) {
                    paybtn = '<button type="button" class="btn btn-outline-danger btn-sm" href="#" onclick="GetStudentDue(' + obj.id + ')"> <span>' + obj.dueAmount + '</span></button>'
                }
                var spanactive = "";
                if (obj.activeStatus == '1') {
                    spanactive = `<span class="badge bg-success">`  + resourceObj.active + `</span>`
                }
                else {
                    spanactive = `<span class="badge bg-danger">` + resourceObj.inactive + `</span>`
                }
                var attpercentage = "";
                if (obj.attPercent > 0) {
                    attpercentage = obj.attPercent + " %";
                }         
                var spark = function (data, type, row) {
                    return '<span class="spark">' + obj.lstSpark + '</span>'
                }
                var pic = "";
                pic = '<div class="table-user"><img src="' + obj.filePath + '"  class="me-2 rounded-circle" /><span>' + obj.fullName + '</span></div>'

                var btndelete = "";
                if (obj.isDelete == "1") {
                    btndelete = '<a class="dropdown-item" href="#" onclick="deleteAction(`6`, `StudentGrid`, ' + obj.id + ')"><i class="ri-delete-bin-line"></i>&nbsp; ' + resourceObj.delete + '</a>'
                }

                var btnaction = '<div class="btn-group btn-group-sm dropdown" onclick="onSelect(this,StudentGrid);"> ' +
                    '<a href="#" class="table-action-btn dropdown-toggle arrow-none btn btn-light btn-xs" data-bs-toggle="dropdown" aria-expanded="false"><i class="mdi mdi-dots-horizontal"></i></a>' +
                    '<div class="dropdown-menu dropdown-menu-end">' +
                    '<a class="dropdown-item" href="#" onclick="viewProfile(' + obj.id + ')"><i class="ri-eye-line"></i>&nbsp; ' + resourceObj.profile + '</a>' +
                    ' <a class="dropdown-item" href="#" onclick="onLoadAttachments(`1`,' + obj.id + ')"><i class="ri-list-ordered"></i>&nbsp; ' + resourceObj.attachments + '</a>' +
                    '<a class="dropdown-item" href="#" onclick="addEditAction(`6`, ' + obj.id + ')"><i class="ri-pencil-line"></i>&nbsp; ' + resourceObj.edit + '</a>' + btndelete +
                    '</div>' +
                    '</div>'

                studentData.push([obj.id, pic, obj.admissionNo, obj.rollNo, obj.age, obj.genderName, obj.defaultMobileNo, obj.className, obj.sectionName, paybtn, attpercentage, spark, spanactive, btnaction])

            } else {
                toastr.error(resourceObj.error);
            }
        },
        error: function (response) {
            toastr.error(resourceObj.error);
        }
    })
    return studentData;
}
function getTeacherById(id) {
    var teacherData = [];
    $.ajax({
        type: 'GET',
        async: false,
        url: basepath + "/Teacher/GetTeacher?Id=" + id,
        Accept: 'application/json, text/javascript',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (response) {
            if (response != null) {
                var obj = response;

                var className = "";
                var spans = "";
                if (obj.classTeacherCount > 0 && obj.teacherClassName != null) {
                    if (obj.classTeacherCount == 1) {
                        spans = `<span class="badge bg-info">` + obj.teacherClassName + `</span>`
                    }
                    if (obj.classTeacherCount > 1) {                        
                        var clsArray = obj.teacherClassName.split(",");                        
                        for (var i = 0; i < clsArray.length; i++) {                           
                            spans += `<span class="badge bg-info">` + clsArray[i] + `</span><br/>`;                            
                        }
                    }
                    className = '<div>' + spans + '</div>'
                }

                //var subjName = "";
                //var subspans = "";
                //if (obj.classSubjectTeacherCount > 0) {
                //    if (obj.classSubjectTeacherCount == 1) {
                //        subspans = `<span class="badge bg-info">` + obj.teacherSubjectName + `</span>`
                //    }
                //    if (obj.classSubjectTeacherCount > 1) {
                //        var clssubArray = obj.teacherSubjectName.split(",");
                //        for (var i = 0; i < clssubArray.length; i++) {
                //            subspans += `<span class="badge bg-info">` + clssubArray[i] + `</span><br/>`;
                //        }
                //    }
                //    subjName = '<div>' + subspans + '</div>'
                //}

                var spanactive = "";
                if (obj.activeStatus == '1') {
                    spanactive = `<span class="badge bg-success"> ` + resourceObj.active + ` </span>`
                }
                else {
                    spanactive = `<span class="badge bg-danger"> ` + resourceObj.inactive + ` </span>`
                }
                var attpercentage = "";
                if (obj.attPercent > 0) {
                    attpercentage = obj.attPercent + " %";
                }

                var pic = "";
                pic = '<div class="table-user"><img src="' + obj.filePath + '"  class="me-2 rounded-circle" /><span>' + obj.fullName + '</span></div>'

                var btndelete = "";
                if (obj.isDelete == "1") {
                    btndelete = '<a class="dropdown-item" href="#" onclick="deleteAction(`10`, `TeacherGrid`, ' + obj.id + ')"><i class="ri-delete-bin-line"></i>&nbsp; ' + resourceObj.delete + '</a>'
                }
                var btnaction = '<div class="btn-group btn-group-sm dropdown" onclick="onSelect(this,TeacherGrid);"> ' +
                    '<a href="#" class="table-action-btn dropdown-toggle arrow-none btn btn-light btn-xs" data-bs-toggle="dropdown" aria-expanded="false"><i class="mdi mdi-dots-horizontal"></i></a>' +
                    '<div class="dropdown-menu dropdown-menu-end">' +
                '<a class="dropdown-item" href="#" onclick="viewProfile(' + obj.id + ')"><i class="ri-eye-line"></i>&nbsp; ' + resourceObj.profile + '</a>' +
                    ' <a class="dropdown-item" href="#" onclick="onLoadAttachments(`2`,' + obj.id + ')"><i class="ri-list-ordered"></i>&nbsp; ' + resourceObj.attachments + '</a>' +
                    '<a class="dropdown-item" href="#" onclick="addEditAction(`10`, ' + obj.id + ')"><i class="ri-pencil-line"></i>&nbsp; ' + resourceObj.edit + '</a>' + btndelete +
                   /* ' <a class="dropdown-item" href = "#" onclick = "AddEditTeacherTimeTable(' + obj.id + ')" > <i class="ri-list-ordered" > </i>&nbsp; ' + resourceObj.timetable + '</a > ' + */
               /*     ' <a class="dropdown-item" href="#" onclick="AddEditClassSubjects(' + obj.id + ')"><i class="ri-list-ordered"></i>&nbsp; ' + resourceObj.classsubjects + '</a>' + */
                    '</div>' +
                    '</div>'

                teacherData.push([obj.id, pic, obj.employeeNo, obj.age, obj.genderName, obj.defaultMobileNo, obj.qualification, obj.branchName, className,  attpercentage, spanactive, btnaction])

            } else {
                toastr.error(resourceObj.error);
            }
        },
        error: function (response) {
            toastr.error(resourceObj.error);
        }
    })
    return teacherData;
}
function getLicenseById(id) {
    var licenseData = [];
    $.ajax({
        type: 'GET',
        async: false,
        url: basepath + "/License/GetLicense?Id=" + id,
        Accept: 'application/json, text/javascript',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (response) {
            if (response != null) {
                var obj = response;
                licenseData.push([obj.id, obj.noOfMonths, obj.createdDate, obj.expirationDate])     

            } else {
                toastr.error(resourceObj.error);
            }
        },
        error: function (response) {
            toastr.error(resourceObj.error);
        }
    })
    return licenseData;
}
function getRoleById(id) {
    var roleData = [];
    $.ajax({
        type: 'GET',
        async: false,
        url: basepath + "/Roles/GetRole?Id=" + id,
        Accept: 'application/json, text/javascript',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (response) {
            if (response != null) {
                var obj = response;
                var btndelete = "";
                if (obj.isDelete == "1") {
                    btndelete = '<a class="dropdown-item" href="#" onclick="deleteAction(`18`,`RoleGrid`, ' + obj.id + ')"><i class="ri-delete-bin-line"></i>&nbsp; ' + resourceObj.delete + '</a>'
                }
                var btnaction = '<div class="btn-group btn-group-sm dropdown" onclick="onSelect(this,RoleGrid);"> ' +
                    '<a href="#" class="table-action-btn dropdown-toggle arrow-none btn btn-light btn-xs" data-bs-toggle="dropdown" aria-expanded="false"><i class="mdi mdi-dots-horizontal"></i></a>' +
                    '<div class="dropdown-menu dropdown-menu-end">' +
                    '<a class="dropdown-item" href="#" onclick="viewAction(`18`, ' + obj.id + ')"><i class="ri-eye-line"></i>&nbsp; ' + resourceObj.view + '</a>' +
                    '<a class="dropdown-item" href="#" onclick="addEditAction(`18`, ' + obj.id + ')"><i class="ri-pencil-line"></i>&nbsp; ' + resourceObj.edit + '</a>' +
                    '<a class="dropdown-item" href="#" onclick="AddEditRolePage(' + obj.id + ')"><i class="ri-list-ordered"></i>&nbsp; ' + resourceObj.pages + '</a>' +
                    '<a class="dropdown-item" href="#" onclick="AddEditRoleUser(' + obj.id + ')"><i class="ri-group-line"></i>&nbsp; ' + resourceObj.users + '</a>' + btndelete +

                    '</div>' +
                    '</div>'
                roleData.push([obj.id, obj.name, obj.description, obj.userCount, btnaction])

            } else {
                toastr.error(resourceObj.error);
            }
        },
        error: function (response) {
            toastr.error(resourceObj.error);
        }
    })
    return roleData;
}
function getUserById(id) {
    var userData = [];
    $.ajax({
        type: 'GET',
        async: false,
        url: basepath + "/User/GetUser?Id=" + id,
        Accept: 'application/json, text/javascript',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (response) {
            if (response != null) {
                var obj = response;              

                var haslogin = "";
                if (obj.hasLogin == "1") {
                    haslogin = '<i class="ri-check-line"></i>';
                }
                else {
                    haslogin = '<i class="ri-close-line"></i>';
                }

                var btnaction = '<div class="btn-group btn-group-sm dropdown" onclick="onSelect(this,UserGrid);"> ' +
                    '<a href="#" class="table-action-btn dropdown-toggle arrow-none btn btn-light btn-xs" data-bs-toggle="dropdown" aria-expanded="false"><i class="mdi mdi-dots-horizontal"></i></a>' +
                    '<div class="dropdown-menu dropdown-menu-end">' +
                    '<a class="dropdown-item" href="#" onclick="viewAction(`19`, ' + obj.id + ')"><i class="ri-eye-line"></i>&nbsp; ' + resourceObj.view + '</a>' +
                    '<a class="dropdown-item" href="#" onclick="addEditAction(`19`, ' + obj.id + ')"><i class="ri-pencil-line"></i>&nbsp; ' + resourceObj.edit + '</a>' +
                    '<a class="dropdown-item" href="#" onclick="AddEditUserRole(' + obj.id + ')"><i class="ri-list-ordered"></i>&nbsp; ' + resourceObj.userroles + '</a>' +

                    '</div>' +
                    '</div>'

                userData.push([obj.id, obj.fullName, obj.userTypeName, obj.employeeNo, obj.userName, obj.lastLoggedIn, haslogin, btnaction])
            } else {
                toastr.error(resourceObj.error);
            }
        },
        error: function (response) {
            toastr.error(resourceObj.error);
        }
    })
    return userData;
}
function getQuestionBankById(id) {
    var questBankData = [];
    $.ajax({
        type: 'GET',
        async: false,
        url: basepath + "/QuestionBank/GetQuestionBank?Id=" + id,
        Accept: 'application/json, text/javascript',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (response) {
            if (response != null) {
                var obj = response;               

                var btndelete = "";
                if (obj.isDelete == "1") {
                    btndelete = '<a class="dropdown-item" href="#" onclick="deleteAction(`17`, `QuestionBankGrid`, ' + obj.id + ')"><i class="ri-delete-bin-line"></i>&nbsp; ' + resourceObj.delete + '</a>'
                }
                var btnaction = '<div class="btn-group btn-group-sm dropdown" onclick="onSelect(this,QuestionBankGrid);"> ' +
                    '<a href="#" class="table-action-btn dropdown-toggle arrow-none btn btn-light btn-xs" data-bs-toggle="dropdown" aria-expanded="false"><i class="mdi mdi-dots-horizontal"></i></a>' +
                    '<div class="dropdown-menu dropdown-menu-end">' +
                    '<a class="dropdown-item" href="#" onclick="addEditAction(`17`, ' + obj.id + ')"><i class="ri-pencil-line"></i>&nbsp; ' + resourceObj.edit + '</a>' + btndelete +

                    '</div>' +
                    '</div>'
                questBankData.push([obj.id, obj.className, obj.subjectName, obj.lessonName,
                    obj.questionTypeName, obj.questionDifficultyName, obj.questionCategoryName,
                    obj.question, obj.marks,
                    btnaction])


            } else {
                toastr.error(resourceObj.error);
            }
        },
        error: function (response) {
            toastr.error(resourceObj.error);
        }
    })
    return questBankData;
}
function getReportCardById(id) {
    var cardData = [];
    $.ajax({
        type: 'GET',
        async: false,
        url: basepath + "/ReportCard/GetReportCard?Id=" + id,
        Accept: 'application/json, text/javascript',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (response) {
            if (response != null) {
                var obj = response;
                var btndelete = "";
                if (obj.isDelete == "1") {
                    btndelete = '<a class="dropdown-item" href="#" onclick="deleteAction(`20`, `ReportCardGrid`, ' + obj.id + ')"><i class="ri-delete-bin-line"></i>&nbsp; ' + resourceObj.delete + '</a>'
                }
                var btnaction = '<div class="btn-group btn-group-sm dropdown" onclick="onSelect(this,ReportCardGrid);"> ' +
                    '<a href="#" class="table-action-btn dropdown-toggle arrow-none btn btn-light btn-xs" data-bs-toggle="dropdown" aria-expanded="false"><i class="mdi mdi-dots-horizontal"></i></a>' +
                    '<div class="dropdown-menu dropdown-menu-end">' +
                    '<a class="dropdown-item" href="#" onclick="viewDetails(' + obj.id + ')"><i class="ri-eye-line"></i>&nbsp; ' + resourceObj.view + '</a>' +
                    '<a class="dropdown-item" href="#" onclick="addEditAction(`20`, ' + obj.id + ')"><i class="ri-pencil-line"></i>&nbsp; ' + resourceObj.edit + '</a>' + btndelete +

                    '</div>' +
                    '</div>'

                cardData.push([obj.id, obj.name, obj.branchName, obj.description, obj.scheduleCount, btnaction])

            } else {
                toastr.error(resourceObj.error);
            }
        },
        error: function (response) {
            toastr.error(resourceObj.error);
        }
    })
    return cardData;
}
function getMediumById(id) {
    var mediumData = [];
    $.ajax({
        type: 'GET',
        async: false,
        url: basepath + "/Medium/GetMedium?Id=" + id,
        Accept: 'application/json, text/javascript',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (response) {
            if (response != null) {
                var obj = response;
                var btndelete = "";
                if (obj.isDelete == "1") {
                    btndelete = '<a class="dropdown-item" href="#" onclick="deleteAction(`23`,`MediumGrid`, ' + obj.id + ')"><i class="ri-delete-bin-line"></i>&nbsp; ' + resourceObj.delete + '</a>'
                }
                var btnaction = '<div class="btn-group btn-group-sm dropdown" onclick="onSelect(this,ExamTypeGrid);"> ' +
                    '<a href="#" class="table-action-btn dropdown-toggle arrow-none btn btn-light btn-xs" data-bs-toggle="dropdown" aria-expanded="false"><i class="mdi mdi-dots-horizontal"></i></a>' +
                    '<div class="dropdown-menu dropdown-menu-end">' +
                    '<a class="dropdown-item" href="#" onclick="viewAction(`23`, ' + obj.id + ')"><i class="ri-eye-line"></i>&nbsp; ' + resourceObj.view + '</a>' +
                    '<a class="dropdown-item" href="#" onclick="addEditAction(`23`, ' + obj.id + ')"><i class="ri-pencil-line"></i>&nbsp; ' + resourceObj.edit + '</a>' + btndelete +
                    '</div>' +
                    '</div>'

                mediumData.push([obj.id, obj.name, obj.description, btnaction])

            } else {
                toastr.error(resourceObj.error);
            }
        },
        error: function (response) {
            toastr.error(resourceObj.error);
        }
    })
    return mediumData;
}
function getShiftById(id) {
    var shiftData = [];
    $.ajax({
        type: 'GET',
        async: false,
        url: basepath + "/Shift/GetShift?Id=" + id,
        Accept: 'application/json, text/javascript',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (response) {
            if (response != null) {
                var obj = response;
                var btndelete = "";
                if (obj.isDelete == "1") {
                    btndelete = '<a class="dropdown-item" href="#" onclick="deleteAction(`24`,`ShiftGrid`, ' + obj.id + ')"><i class="ri-delete-bin-line"></i>&nbsp; ' + resourceObj.delete + '</a>'
                }

                var btnaction = '<div class="btn-group btn-group-sm dropdown" onclick="onSelect(this,ShiftGrid);">  ' +
                    '<a href="#" class="table-action-btn dropdown-toggle arrow-none btn btn-light btn-xs" data-bs-toggle="dropdown" aria-expanded="false"><i class="mdi mdi-dots-horizontal"></i></a>' +
                    '<div class="dropdown-menu dropdown-menu-end">' +
                    '<a class="dropdown-item" href="#" onclick="viewAction(`24`, ' + obj.id + ')"><i class="ri-eye-line"></i>&nbsp; ' + resourceObj.view + '</a>' +
                    '<a class="dropdown-item" href="#" onclick="addEditAction(`24`, ' + obj.id + ')"><i class="ri-pencil-line"></i>&nbsp; ' + resourceObj.edit + '</a>' + btndelete +

                    '</div>' +
                    '</div>'

                shiftData.push([obj.id, obj.fromTime, obj.toTime, obj.description, btnaction])

            } else {
                toastr.error(resourceObj.error);
            }
        },
        error: function (response) {
            toastr.error(resourceObj.error);
        }
    })
    return shiftData;
}
function getBranchClassById(id) {
    var branchData = [];
    $.ajax({
        type: 'GET',
        async: false,
        url: basepath + "/BranchClass/GetBranchClass?Id=" + id,
        Accept: 'application/json, text/javascript',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (response) {
            if (response != null) {
                var obj = response;    
                
                var strsection = "";
                if (obj.sectionCount > 0) {
                    strsection = obj.sectionCount;
                }
                var strstudent = "";
                if (obj.studentCount > 0) {
                    strstudent = obj.studentCount;
                }
                var strprbreak = "";
                if (obj.periodBreakCount > 0) {
                    strprbreak = obj.periodBreakCount;
                }

                var btnaction = '<div class="btn-group btn-group-sm dropdown" onclick="onSelect(this,BranchClassGrid);"> ' +
                    '<a href="#" class="table-action-btn dropdown-toggle arrow-none btn btn-light btn-xs" data-bs-toggle="dropdown" aria-expanded="false"><i class="mdi mdi-dots-horizontal"></i></a>' +
                    '<div class="dropdown-menu dropdown-menu-end">' +                   
                    '<a class="dropdown-item" href="#" onclick="addEditAction(`25`, ' + obj.id + ')"><i class="ri-pencil-line"></i>&nbsp; ' + resourceObj.shifts + '</a>' +
                    '<a class="dropdown-item" href="#" onclick="addEditPeriodBreak(' + obj.id + ')"><i class="ri-pencil-line"></i>&nbsp; ' + resourceObj.periodbreaks + '</a>' +
                    '</div>' +
                    '</div>'

                branchData.push([obj.id, obj.branchName, obj.className, obj.shiftName, obj.slotDuration, strsection, strstudent, strprbreak, btnaction])
            } else {
                toastr.error(resourceObj.error);
            }
        },
        error: function (response) {
            toastr.error(resourceObj.error);
        }
    })
    return branchData;
}
function getSessionYearById(id) {
    var sessionData = [];
    $.ajax({
        type: 'GET',
        async: false,
        url: basepath + "/SessionYear/GetSessionYear?Id=" + id,
        Accept: 'application/json, text/javascript',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (response) {
            if (response != null) {
                var obj = response;   
                var stat = "";
                if (obj.isDefault == true) {
                    stat = '<span class="badge bg-success">' + resourceObj.yes + '</span>'
                }        
                var btndelete = "";
                if (obj.isDelete == "1") {
                    btndelete = '<a class="dropdown-item" href="#" onclick="deleteAction(`26`,`SessionYearGrid`, ' + obj.id + ')"><i class="ri-delete-bin-line"></i>&nbsp; ' + resourceObj.delete + '</a>'
                }
                var btnaction = '<div class="btn-group btn-group-sm dropdown" onclick="onSelect(this,SessionYearGrid);"> ' +
                    '<a href="#" class="table-action-btn dropdown-toggle arrow-none btn btn-light btn-xs" data-bs-toggle="dropdown" aria-expanded="false"><i class="mdi mdi-dots-horizontal"></i></a>' +
                    '<div class="dropdown-menu dropdown-menu-end">' +
                    '<a class="dropdown-item" href="#" onclick="addEditAction(`26`, ' + obj.id + ')"><i class="ri-pencil-line"></i>&nbsp; ' + resourceObj.edit + '</a>' + btndelete +
                    '</div>' +
                    '</div>'

                sessionData.push([obj.id, obj.name, obj.strFromDate, obj.strToDate, stat, btnaction])

            } else {
                toastr.error(resourceObj.error);
            }
        },
        error: function (response) {
            toastr.error(resourceObj.error);
        }
    })
    return sessionData;
}
function getAttachmentById(id) {
    var attachData = [];
    $.ajax({
        type: 'GET',
        async: false,
        url: basepath + "/Attachment/GetAttachment?Id=" + id,
        Accept: 'application/json, text/javascript',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (response) {
            if (response != null) {               
                var obj = response;
                var btndownload = "";
                if (obj.fileName != "" && obj.fileName != null) {
                    btndownload = '<a href="#" onclick="ondownloadAttach(' + obj.id + ')"><i class="ri-download-2-line"></i>&nbsp;</a>'
                }

                var btnaction = '<div class="btn-group btn-group-sm dropdown" onclick="onSelect(this,AttachmentGrid);"> ' +
                    '<a href="#" class="table-action-btn dropdown-toggle arrow-none btn btn-light btn-xs" data-bs-toggle="dropdown" aria-expanded="false"><i class="mdi mdi-dots-horizontal"></i></a>' +
                    '<div class="dropdown-menu dropdown-menu-end">' +                    
                    '<a class="dropdown-item" href="#" onclick="AddEditAttach(' + obj.id + ')"><i class="ri-pencil-line"></i>&nbsp; ' + resourceObj.edit + '</a>' +
                    '</div>' +
                    '</div>'

                attachData.push([obj.id, obj.name, obj.fileTypeName, obj.fileName, btndownload, btnaction])                
            } else {
                toastr.error(resourceObj.error);
            }
        },
        error: function (response) {
            toastr.error(resourceObj.error);
        }
    })
    return attachData;
}
function getBloodGroupById(id) {
    var bloodgroupData = [];
    $.ajax({
        type: 'GET',
        async: false,
        url: basepath + "/BloodGroup/GetBloodGroup?Id=" + id,
        Accept: 'application/json, text/javascript',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (response) {
            if (response != null) {
                var obj = response;
                var btndelete = "";
                if (obj.isDelete == "1") {
                    btndelete = '<a class="dropdown-item" href="#" onclick="deleteAction(`28`,`BloodGroupGrid`, ' + obj.id + ')"><i class="ri-delete-bin-line"></i>&nbsp; ' + resourceObj.delete + '</a>'
                }
                var btnaction = '<div class="btn-group btn-group-sm dropdown" onclick="onSelect(this,BloodGroupGrid);"> ' +
                    '<a href="#" class="table-action-btn dropdown-toggle arrow-none btn btn-light btn-xs" data-bs-toggle="dropdown" aria-expanded="false"><i class="mdi mdi-dots-horizontal"></i></a>' +
                    '<div class="dropdown-menu dropdown-menu-end">' +
                    '<a class="dropdown-item" href="#" onclick="viewAction(`28`, ' + obj.id + ')"><i class="ri-eye-line"></i>&nbsp; ' + resourceObj.view + '</a>' +
                    '<a class="dropdown-item" href="#" onclick="addEditAction(`28`, ' + obj.id + ')"><i class="ri-pencil-line"></i>&nbsp; ' + resourceObj.edit + '</a>' + btndelete +
                    '</div>' +
                    '</div>'

                bloodgroupData.push([obj.id, obj.name, obj.description, btnaction])
            } else {
                toastr.error(resourceObj.error);
            }
        },
        error: function (response) {
            toastr.error(resourceObj.error);
        }
    })
    return bloodgroupData;
}
function getFeeTypeById(id) {
    var feeTypeData = [];
    $.ajax({
        type: 'GET',
        async: false,
        url: basepath + "/FeeType/GetFeeType?Id=" + id,
        Accept: 'application/json, text/javascript',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (response) {
            if (response != null) {
                var obj = response;         

                var btndelete = "";
                if (obj.isDelete == "1") {
                    btndelete = '<a class="dropdown-item" href="#" onclick="deleteAction(`29`,`FeeTypeGrid`, ' + obj.id + ')"><i class="ri-delete-bin-line"></i>&nbsp; ' + resourceObj.delete + '</a>'
                }
                var btnaction = '<div class="btn-group btn-group-sm dropdown" onclick="onSelect(this,FeeTypeGrid);"> ' +
                    '<a href="#" class="table-action-btn dropdown-toggle arrow-none btn btn-light btn-xs" data-bs-toggle="dropdown" aria-expanded="false"><i class="mdi mdi-dots-horizontal"></i></a>' +
                    '<div class="dropdown-menu dropdown-menu-end">' +
                    '<a class="dropdown-item" href="#" onclick="viewAction(`29`, ' + obj.id + ')"><i class="ri-eye-line"></i>&nbsp; ' + resourceObj.view + '</a>' +
                    '<a class="dropdown-item" href="#" onclick="addEditAction(`29`, ' + obj.id + ')"><i class="ri-pencil-line"></i>&nbsp; ' + resourceObj.edit + '</a>' + btndelete +
                    '</div>' +
                    '</div>'
                feeTypeData.push([obj.id, obj.name, obj.description, btnaction])

            } else {
                toastr.error(resourceObj.error);
            }
        },
        error: function (response) {
            toastr.error(resourceObj.error);
        }
    })
    return feeTypeData;
}
function getEnquiryStatusById(id) {
    var enquiryStatusData = [];
    $.ajax({
        type: 'GET',
        async: false,
        url: basepath + "/EnquiryStatus/GetEnquiryStatus?Id=" + id,
        Accept: 'application/json, text/javascript',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (response) {
            if (response != null) {
                var obj = response;

                var btndelete = "";
                if (obj.isDelete == "1") {
                    btndelete = '<a class="dropdown-item" href="#" onclick="deleteAction(`30`,`EnquiryStatusGrid`, ' + obj.id + ')"><i class="ri-delete-bin-line"></i>&nbsp; ' + resourceObj.delete + '</a>'
                }
                var btnaction = '<div class="btn-group btn-group-sm dropdown" onclick="onSelect(this,EnquiryStatusGrid);"> ' +
                    '<a href="#" class="table-action-btn dropdown-toggle arrow-none btn btn-light btn-xs" data-bs-toggle="dropdown" aria-expanded="false"><i class="mdi mdi-dots-horizontal"></i></a>' +
                    '<div class="dropdown-menu dropdown-menu-end">' +
                    '<a class="dropdown-item" href="#" onclick="viewAction(`30`, ' + obj.id + ')"><i class="ri-eye-line"></i>&nbsp; ' + resourceObj.view + '</a>' +
                    '<a class="dropdown-item" href="#" onclick="addEditAction(`30`, ' + obj.id + ')"><i class="ri-pencil-line"></i>&nbsp; ' + resourceObj.edit + '</a>' + btndelete +
                    '</div>' +
                    '</div>'
                enquiryStatusData.push([obj.id, obj.name, obj.description, btnaction])

            } else {
                toastr.error(resourceObj.error);
            }
        },
        error: function (response) {
            toastr.error(resourceObj.error);
        }
    })
    return enquiryStatusData;
}
function getLessonById(id) {
    var lessonData = [];
    $.ajax({
        type: 'GET',
        async: false,
        url: basepath + "/Lesson/GetLesson?Id=" + id,
        Accept: 'application/json, text/javascript',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (response) {
            if (response != null) {
                var obj = response;

                var btndelete = "";
                if (obj.isDelete == "1") {
                    btndelete = '<a class="dropdown-item" href="#" onclick="deleteAction(`31`,`LessonGrid`, ' + obj.id + ')"><i class="ri-delete-bin-line"></i>&nbsp; ' + resourceObj.delete + '</a>'
                }
                var btnaction = '<div class="btn-group btn-group-sm dropdown" onclick="onSelect(this,LessonGrid);"> ' +
                    '<a href="#" class="table-action-btn dropdown-toggle arrow-none btn btn-light btn-xs" data-bs-toggle="dropdown" aria-expanded="false"><i class="mdi mdi-dots-horizontal"></i></a>' +
                    '<div class="dropdown-menu dropdown-menu-end">' +
                    '<a class="dropdown-item" href="#" onclick="viewAction(`31`, ' + obj.id + ')"><i class="ri-eye-line"></i>&nbsp; ' + resourceObj.view + '</a>' +
                    '<a class="dropdown-item" href="#" onclick="addEditAction(`31`, ' + obj.id + ')"><i class="ri-pencil-line"></i>&nbsp; ' + resourceObj.edit + '</a>' + btndelete +
                    '</div>' +
                    '</div>'
                lessonData.push([obj.id, obj.name, obj.subjectName, obj.className, obj.description, btnaction])

            } else {
                toastr.error(resourceObj.error);
            }
        },
        error: function (response) {
            toastr.error(resourceObj.error);
        }
    })
    return lessonData;
}
function getExpenseCategoryById(id) {
    var expenseCategoryData = [];
    $.ajax({
        type: 'GET',
        async: false,
        url: basepath + "/ExpenseCategory/GetExpenseCategory?Id=" + id,
        Accept: 'application/json, text/javascript',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (response) {
            if (response != null) {
                var obj = response;

                var btndelete = "";
                if (obj.isDelete == "1") {
                    btndelete = '<a class="dropdown-item" href="#" onclick="deleteAction(`32`,`ExpenseCategoryGrid`, ' + obj.id + ')"><i class="ri-delete-bin-line"></i>&nbsp; ' + resourceObj.delete + '</a>'
                }
                var btnaction = '<div class="btn-group btn-group-sm dropdown" onclick="onSelect(this,ExpenseCategoryGrid);"> ' +
                    '<a href="#" class="table-action-btn dropdown-toggle arrow-none btn btn-light btn-xs" data-bs-toggle="dropdown" aria-expanded="false"><i class="mdi mdi-dots-horizontal"></i></a>' +
                    '<div class="dropdown-menu dropdown-menu-end">' +
                    '<a class="dropdown-item" href="#" onclick="viewAction(`32`, ' + obj.id + ')"><i class="ri-eye-line"></i>&nbsp; ' + resourceObj.view + '</a>' +
                    '<a class="dropdown-item" href="#" onclick="addEditAction(`32`, ' + obj.id + ')"><i class="ri-pencil-line"></i>&nbsp; ' + resourceObj.edit + '</a>' + btndelete +
                    '</div>' +
                    '</div>'
                    expenseCategoryData.push([obj.id, obj.name, obj.description, btnaction])
            } else {
                toastr.error(resourceObj.error);
            }
        },
        error: function (response) {
            toastr.error(resourceObj.error);
        }
    })
    return expenseCategoryData;
}
function getReferenceAdmissionById(id) {
    var referenceAdmissionData = [];
    $.ajax({
        type: 'GET',
        async: false,
        url: basepath + "/ReferenceAdmission/GetReferenceAdmission?Id=" + id,
        Accept: 'application/json, text/javascript',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (response) {
            if (response != null) {
                var obj = response;

                var btndelete = "";
                if (obj.isDelete == "1") {
                    btndelete = '<a class="dropdown-item" href="#" onclick="deleteAction(`33`,`ReferenceAdmissionGrid`, ' + obj.id + ')"><i class="ri-delete-bin-line"></i>&nbsp; ' + resourceObj.delete + '</a>'
                }
                var btnaction = '<div class="btn-group btn-group-sm dropdown" onclick="onSelect(this,ReferenceAdmissionGrid);"> ' +
                    '<a href="#" class="table-action-btn dropdown-toggle arrow-none btn btn-light btn-xs" data-bs-toggle="dropdown" aria-expanded="false"><i class="mdi mdi-dots-horizontal"></i></a>' +
                    '<div class="dropdown-menu dropdown-menu-end">' +
                    '<a class="dropdown-item" href="#" onclick="viewAction(`33`, ' + obj.id + ')"><i class="ri-eye-line"></i>&nbsp; ' + resourceObj.view + '</a>' +
                    '<a class="dropdown-item" href="#" onclick="addEditAction(`33`, ' + obj.id + ')"><i class="ri-pencil-line"></i>&nbsp; ' + resourceObj.edit + '</a>' + btndelete +
                    '</div>' +
                    '</div>'
                referenceAdmissionData.push([obj.id, obj.name, obj.description, btnaction])
            } else {
                toastr.error(resourceObj.error);
            }
        },
        error: function (response) {
            toastr.error(resourceObj.error);
        }
    })
    return referenceAdmissionData;
}
function getStudentEnquiryById(id) {
    var studentEnquiryData = [];
    $.ajax({
        type: 'GET',
        async: false,
        url: basepath + "/StudentEnquiry/GetStudentEnquiry?Id=" + id,
        Accept: 'application/json, text/javascript',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (response) {
            if (response != null) {
                var obj = response;
                var status = "";
                if (obj.enquiryStatusName != "") {
                    status = `<span class="badge" style="background-color:` + obj.enquiryStatusColor + `">` + obj.enquiryStatusName + `</span>`
                }

                //var btndelete = "";
                //if (obj.isDelete == "1") {
                //    btndelete = '<a class="dropdown-item" href="#" onclick="deleteAction(`34`,`StudentEnquiryGrid`, ' + obj.id + ')"><i class="ri-delete-bin-line"></i>&nbsp; ' + resourceObj.delete + '</a>'
                //}
                var btnaction = '<div class="btn-group btn-group-sm dropdown" onclick="onSelect(this,StudentEnquiryGrid);"> ' +
                    '<a href="#" class="table-action-btn dropdown-toggle arrow-none btn btn-light btn-xs" data-bs-toggle="dropdown" aria-expanded="false"><i class="mdi mdi-dots-horizontal"></i></a>' +
                    '<div class="dropdown-menu dropdown-menu-end">' +
                    '<a class="dropdown-item" href="#" onclick="viewAction(`34`, ' + obj.id + ')"><i class="ri-eye-line"></i>&nbsp; ' + resourceObj.view + '</a>' +
                    '<a class="dropdown-item" href="#" onclick="addEditAction(`34`, ' + obj.id + ')"><i class="ri-pencil-line"></i>&nbsp; ' + resourceObj.edit + '</a>' +
                    '</div>' +
                    '</div>'
                studentEnquiryData.push([obj.id, obj.name, obj.age, obj.genderName, obj.mobileNo, obj.className, obj.enquiryReferenceName, obj.enquiryDate, status, btnaction])
            } else {
                toastr.error(resourceObj.error);
            }
        },
        error: function (response) {
            toastr.error(resourceObj.error);
        }
    })
    return studentEnquiryData;
}
function getLeaveRequestById(id) {
    var leaveRequestData = [];
    $.ajax({
        type: 'GET',
        async: false,
        url: basepath + "/LeaveRequest/GetLeaveRequest?Id=" + id,
        Accept: 'application/json, text/javascript',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (response) {
            if (response != null) {
                var obj = response;
                var statusName = "";               
                if (obj.statusId == 1) {
                    statusName = '<span class="badge bg-warning">' + resourceObj.underprocess + '</span>'
                }
                if (obj.statusId == 2) {
                    statusName = '<span class="badge bg-danger">' + resourceObj.rejected + '</span>'
                }
                if (obj.statusId == 3) {
                    statusName = '<span class="badge bg-success">' + resourceObj.approved + '</span>'
                }
                var btndelete = "";
                btndelete = '<a class="dropdown-item" href="#" onclick="deleteAction(`35`,`LeaveRequestGrid`, ' + obj.id + ')"><i class="ri-delete-bin-line"></i>&nbsp; ' + resourceObj.delete + '</a>'
                var btnaction = '<div class="btn-group btn-group-sm dropdown" onclick="onSelect(this,LeaveRequestGrid);"> ' +
                    '<a href="#" class="table-action-btn dropdown-toggle arrow-none btn btn-light btn-xs" data-bs-toggle="dropdown" aria-expanded="false"><i class="mdi mdi-dots-horizontal"></i></a>' +
                    '<div class="dropdown-menu dropdown-menu-end">' +
                    '<a class="dropdown-item" href="#" onclick="viewAction(`35`, ' + obj.id + ')"><i class="ri-eye-line"></i>&nbsp; ' + resourceObj.view + '</a>' + btndelete +
                    '</div>' +
                    '</div>'
                leaveRequestData.push([obj.id, obj.strFromDate, obj.strToDate, obj.leaveRequestTypeName, obj.reason, obj.dayCount, obj.strCreatedDate, statusName, btnaction])

            } else {
                toastr.error(resourceObj.error);
            }
        },
        error: function (response) {
            toastr.error(resourceObj.error);
        }
    })
    return leaveRequestData;
}
function getExpenseById(id) {
    var expenseData = [];
    $.ajax({
        type: 'GET',
        async: false,
        url: basepath + "/Expense/GetExpense?Id=" + id,
        Accept: 'application/json, text/javascript',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (response) {
            if (response != null) {
                var obj = response;               

                var btndelete = "";
                btndelete = '<a class="dropdown-item" href="#" onclick="deleteAction(`36`,`ExpenseGrid`, ' + obj.id + ')"><i class="ri-delete-bin-line"></i>&nbsp; ' + resourceObj.delete + '</a>'
                // if (value.isDelete == "1") {

                // }
                var btnaction = '<div class="btn-group btn-group-sm dropdown" onclick="onSelect(this,ExpenseGrid);"> ' +
                    '<a href="#" class="table-action-btn dropdown-toggle arrow-none btn btn-light btn-xs" data-bs-toggle="dropdown" aria-expanded="false"><i class="mdi mdi-dots-horizontal"></i></a>' +
                    '<div class="dropdown-menu dropdown-menu-end">' +
                    '<a class="dropdown-item" href="#" onclick="viewAction(`36`, ' + obj.id + ')"><i class="ri-eye-line"></i>&nbsp; ' + resourceObj.view + '</a>' +
                    '<a class="dropdown-item" href="#" onclick="addEditAction(`36`, ' + obj.id + ')"><i class="ri-pencil-line"></i>&nbsp; ' + resourceObj.edit + '</a>' + btndelete +
                    '</div>' +
                    '</div>'

                expenseData.push([obj.id, obj.branchName, obj.sessionYearName, obj.categoryName, obj.referenceNo, obj.description, obj.strExpenseDate, obj.amount, btnaction])

            } else {
                toastr.error(resourceObj.error);
            }
        },
        error: function (response) {
            toastr.error(resourceObj.error);
        }
    })
    return expenseData;
}
function getStudentAssignmentById(id) {
   
    var studentAssignmentData = [];
    $.ajax({
        type: 'GET',
        async: false,
        url: basepath + "/StudentAssignment/GetStudentAssignment?Id=" + id,
        Accept: 'application/json, text/javascript',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (response) {
            if (response != null) {
                var obj = response;
                var btnpath = "";
                btnpath = '<a href="' + obj.filePath + '">' + obj.fileName + '</a>'
                var btndelete = "";
                btndelete = '<a class="dropdown-item" href="#" onclick="deleteAction(`37`,`StudentAssignmentGrid`, ' + obj.id + ')"><i class="ri-delete-bin-line"></i>&nbsp; ' + resourceObj.delete + '</a>'
                // if (value.isDelete == "1") {

                // }
                var btnaction = '<div class="btn-group btn-group-sm dropdown" onclick="onSelect(this,StudentAssignmentGrid);"> ' +
                    '<a href="#" class="table-action-btn dropdown-toggle arrow-none btn btn-light btn-xs" data-bs-toggle="dropdown" aria-expanded="false"><i class="mdi mdi-dots-horizontal"></i></a>' +
                    '<div class="dropdown-menu dropdown-menu-end">' +
                    '<a class="dropdown-item" href="#" onclick="viewAction(`37`, ' + obj.id + ')"><i class="ri-eye-line"></i>&nbsp; ' + resourceObj.view + '</a>' + btndelete +
                    // '<a class="dropdown-item" href="#" onclick="addEditAction(`37`, ' + value.id + ')"><i class="ri-pencil-line"></i>&nbsp; ' + resourceObj.edit + '</a>' +
                    '</div>' +
                    '</div>'

                studentAssignmentData.push([obj.id, obj.branchName, obj.sessionYearName, obj.title, obj.className, obj.subjectName, obj.description, obj.submissionDate, btnpath, btnaction])

            } else {
                toastr.error(resourceObj.error);
            }
        },
        error: function (response) {
            toastr.error(resourceObj.error);
        }
    })
    return studentAssignmentData;
}
function getStudentAssignmentStatusById(id) {

    var studentAssignStatusData = [];
    $.ajax({
        type: 'GET',
        async: false,
        url: basepath + "/StudentAssignment/GetStudentAssignStatus?Id=" + id,
        Accept: 'application/json, text/javascript',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (response) {
            if (response != null) {
                var obj = response;
                var btnpath = "";
                btnpath = '<a href="' + obj.filePath + '">' + obj.fileName + '</a>'    
                var statusName = "";
                if (obj.statusId == "0") {
                    statusName = '<span class="badge bg-warning">' + resourceObj.new + '</span>'                  
                }
                if (obj.statusId == "2") {
                    statusName = '<span class="badge bg-danger">' + resourceObj.rejected + '</span>'
                }
                if (obj.statusId == "1") {
                    statusName = '<span class="badge bg-success">' + resourceObj.accepted + '</span>'
                }              
                var btnaction = '<div class="btn-group btn-group-sm dropdown" onclick="onSelect(this,StudentAssignStatusGrid);"> ' +
                    '<a href="#" class="table-action-btn dropdown-toggle arrow-none btn btn-light btn-xs" data-bs-toggle="dropdown" aria-expanded="false"><i class="mdi mdi-dots-horizontal"></i></a>' +
                    '<div class="dropdown-menu dropdown-menu-end">' +                    
                    '<a class="dropdown-item" href="#" onclick="addEditAction(`38`, ' + obj.id + ')"><i class="ri-pencil-line"></i>&nbsp; ' + resourceObj.edit + '</a>' +
                    '</div>' +
                    '</div>'
                studentAssignStatusData.push([obj.id, obj.branchName, obj.sessionYearName, obj.title, obj.className, obj.subjectName, obj.sectionName, obj.fullName, statusName, btnpath, btnaction])

            } else {
                toastr.error(resourceObj.error);
            }
        },
        error: function (response) {
            toastr.error(resourceObj.error);
        }
    })
    return studentAssignStatusData;
}
function getTeacherAnnouncementById(id) {

    var teacherAnnouncementData = [];
    $.ajax({
        type: 'GET',
        async: false,
        url: basepath + "/TeacherAnnouncement/GetTeacherAnnouncement?Id=" + id,
        Accept: 'application/json, text/javascript',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (response) {
            if (response != null) {
                var obj = response;
                var btnpath = "";
                btnpath = '<a href="' + obj.filePath + '">' + obj.fileName + '</a>'
                var btndelete = "";
                btndelete = '<a class="dropdown-item" href="#" onclick="deleteAction(`39`,`TeacherAnnouncementGrid`, ' + obj.id + ')"><i class="ri-delete-bin-line"></i>&nbsp; ' + resourceObj.delete + '</a>'
                // if (value.isDelete == "1") {

                // }
                var btnaction = '<div class="btn-group btn-group-sm dropdown" onclick="onSelect(this,TeacherAnnouncementGrid);"> ' +
                    '<a href="#" class="table-action-btn dropdown-toggle arrow-none btn btn-light btn-xs" data-bs-toggle="dropdown" aria-expanded="false"><i class="mdi mdi-dots-horizontal"></i></a>' +
                    '<div class="dropdown-menu dropdown-menu-end">' +
                    '<a class="dropdown-item" href="#" onclick="viewAction(`39`, ' + obj.id + ')"><i class="ri-eye-line"></i>&nbsp; ' + resourceObj.view + '</a>' + btndelete +
                    // '<a class="dropdown-item" href="#" onclick="addEditAction(`39`, ' + value.id + ')"><i class="ri-pencil-line"></i>&nbsp; ' + resourceObj.edit + '</a>' +
                    '</div>' +
                    '</div>'

                teacherAnnouncementData.push([obj.id, obj.branchName, obj.sessionYearName, obj.title, obj.className, obj.subjectName, obj.description, obj.sectionName, btnpath, btnaction])

            } else {
                toastr.error(resourceObj.error);
            }
        },
        error: function (response) {
            toastr.error(resourceObj.error);
        }
    })
    return teacherAnnouncementData;
}


function getAdminById(id) {

    var adminData = [];
    $.ajax({
        type: 'GET',
        async: false,
        url: basepath + "/Admin/GetAdmin?Id=" + id,
        Accept: 'application/json, text/javascript',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (response) {
            if (response != null) {
                var obj = response;
                var branchadmin = "";
                if (obj.isBranchAdmin == "1") {
                    branchadmin = `<span class="badge bg-success">` + resourceObj.yes + ` </span>`
                }
                var spanactive = "";
                if (obj.activeStatus == '1') {
                    spanactive = `<span class="badge bg-success"> ` + resourceObj.active + ` </span>`
                }
                else {
                    spanactive = `<span class="badge bg-danger"> ` + resourceObj.inactive + ` </span>`
                }
                var attpercentage = "";
                if (obj.attPercent > 0) {
                    attpercentage = obj.attPercent + " %";
                }

                var pic = "";
                pic = '<div class="table-user"><img src="' + obj.filePath + '"  class="me-2 rounded-circle" /><span>' + obj.fullName + '</span></div>'
                var btndelete = "";
                btndelete = '<a class="dropdown-item" href="#" onclick="deleteAction(`40`,`AdminGrid`, ' + obj.id + ')"><i class="ri-delete-bin-line"></i>&nbsp; ' + resourceObj.delete + '</a>'
                //if (value.isDelete == "1") {
                    
                //}
                var btnaction = '<div class="btn-group btn-group-sm dropdown" onclick="onSelect(this,AdminGrid);"> ' +
                    '<a href="#" class="table-action-btn dropdown-toggle arrow-none btn btn-light btn-xs" data-bs-toggle="dropdown" aria-expanded="false"><i class="mdi mdi-dots-horizontal"></i></a>' +
                    '<div class="dropdown-menu dropdown-menu-end">' +
                    '<a class="dropdown-item" href="#" onclick="viewAction(`40`, ' + obj.id + ')"><i class="ri-eye-line"></i>&nbsp; ' + resourceObj.view + '</a>' +
                    ' <a class="dropdown-item" href="#" onclick="onLoadAttachments(`3`,' + obj.id + ')"><i class="ri-list-ordered"></i>&nbsp; ' + resourceObj.attachments + '</a>' +
                    '<a class="dropdown-item" href="#" onclick="addEditAction(`40`, ' + obj.id + ')"><i class="ri-pencil-line"></i>&nbsp; ' + resourceObj.edit + '</a>' + btndelete +
                    '</div>' +
                    '</div>'
                adminData.push([obj.id, pic, obj.employeeNo, obj.age, obj.genderName, obj.defaultMobileNo, obj.designation, obj.branchName, branchadmin, attpercentage, spanactive, btnaction])
            } else {
                toastr.error(resourceObj.error);
            }
        },
        error: function (response) {
            toastr.error(resourceObj.error);
        }
    })
    return adminData;
}
function getStaffById(id) {

    var staffData = [];
    $.ajax({
        type: 'GET',
        async: false,
        url: basepath + "/Staff/GetStaff?Id=" + id,
        Accept: 'application/json, text/javascript',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (response) {
            if (response != null) {
                var obj = response;
                var pic = "";
                pic = '<div class="table-user"><img src="' + obj.filePath + '"  class="me-2 rounded-circle" /><span>' + obj.fullName + '</span></div>'
                var btndelete = "";
                btndelete = '<a class="dropdown-item" href="#" onclick="deleteAction(`41`,`StaffGrid`, ' + obj.id + ')"><i class="ri-delete-bin-line"></i>&nbsp; ' + resourceObj.delete + '</a>'
                //if (value.isDelete == "1") {

                //}
                var btnaction = '<div class="btn-group btn-group-sm dropdown" onclick="onSelect(this,StaffGrid);"> ' +
                    '<a href="#" class="table-action-btn dropdown-toggle arrow-none btn btn-light btn-xs" data-bs-toggle="dropdown" aria-expanded="false"><i class="mdi mdi-dots-horizontal"></i></a>' +
                    '<div class="dropdown-menu dropdown-menu-end">' +
                    '<a class="dropdown-item" href="#" onclick="viewAction(`41`, ' + obj.id + ')"><i class="ri-eye-line"></i>&nbsp; ' + resourceObj.view + '</a>' +
                    ' <a class="dropdown-item" href="#" onclick="onLoadAttachments(`4`,' + obj.id + ')"><i class="ri-list-ordered"></i>&nbsp; ' + resourceObj.attachments + '</a>' +
                    '<a class="dropdown-item" href="#" onclick="addEditAction(`41`, ' + obj.id + ')"><i class="ri-pencil-line"></i>&nbsp; ' + resourceObj.edit + '</a>' + btndelete +
                    '</div>' +
                    '</div>'
                staffData.push([obj.id, pic, obj.employeeNo, obj.age, obj.genderName, obj.defaultMobileNo, obj.designation, obj.branchName, btnaction])
            } else {
                toastr.error(resourceObj.error);
            }
        },
        error: function (response) {
            toastr.error(resourceObj.error);
        }
    })
    return staffData;
}
function getDriverById(id) {

    var driverData = [];
    $.ajax({
        type: 'GET',
        async: false,
        url: basepath + "/Driver/GetDriver?Id=" + id,
        Accept: 'application/json, text/javascript',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (response) {
            if (response != null) {
                var obj = response;
                var pic = "";
                pic = '<div class="table-user"><img src="' + obj.filePath + '"  class="me-2 rounded-circle" /><span>' + obj.fullName + '</span></div>'
                var btndelete = "";
                btndelete = '<a class="dropdown-item" href="#" onclick="deleteAction(`42`,`DriverGrid`, ' + obj.id + ')"><i class="ri-delete-bin-line"></i>&nbsp; ' + resourceObj.delete + '</a>'
                //if (value.isDelete == "1") {

                //}
                var btnaction = '<div class="btn-group btn-group-sm dropdown" onclick="onSelect(this,DriverGrid);"> ' +
                    '<a href="#" class="table-action-btn dropdown-toggle arrow-none btn btn-light btn-xs" data-bs-toggle="dropdown" aria-expanded="false"><i class="mdi mdi-dots-horizontal"></i></a>' +
                    '<div class="dropdown-menu dropdown-menu-end">' +
                    '<a class="dropdown-item" href="#" onclick="viewAction(`42`, ' + obj.id + ')"><i class="ri-eye-line"></i>&nbsp; ' + resourceObj.view + '</a>' +
                    ' <a class="dropdown-item" href="#" onclick="onLoadAttachments(`5`,' + obj.id + ')"><i class="ri-list-ordered"></i>&nbsp; ' + resourceObj.attachments + '</a>' +
                    '<a class="dropdown-item" href="#" onclick="addEditAction(`42`, ' + obj.id + ')"><i class="ri-pencil-line"></i>&nbsp; ' + resourceObj.edit + '</a>' + btndelete +
                    '</div>' +
                    '</div>'
                driverData.push([obj.id, pic, obj.employeeNo, obj.age, obj.genderName, obj.defaultMobileNo, obj.designation, obj.branchName, btnaction])
            } else {
                toastr.error(resourceObj.error);
            }
        },
        error: function (response) {
            toastr.error(resourceObj.error);
        }
    })
    return driverData;
}
function getNotificationById(id) {
    var notifyData = [];
    $.ajax({
        type: 'GET',
        async: false,
        url: basepath + "/Notifications/GetNotification?Id=" + id,
        Accept: 'application/json, text/javascript',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (response) {
            if (response != null) {
                var obj = response;
                var btnaction = '<div class="btn-group btn-group-sm dropdown" onclick="onSelect(this,NotificationsGrid);"> ' +
                    '<a href="#" class="table-action-btn dropdown-toggle arrow-none btn btn-light btn-xs" data-bs-toggle="dropdown" aria-expanded="false"><i class="mdi mdi-dots-horizontal"></i></a>' +
                    '<div class="dropdown-menu dropdown-menu-end">' +
                    '<a class="dropdown-item" href="#" onclick="navigateToPage(' + obj.notificationType + ',' + obj.id + ',' + obj.recordId + ',' + obj.readStatusId + ')"><i class="ri-pencil-line"></i>&nbsp; @_localization.Getkey("Edit")</a>' +
                    '</div>' +
                    '</div>'
                notifyData.push([obj.id, obj.fromPersonnelName, obj.fromUserTypeName, obj.notificationTypeName, obj.message, obj.strCreatedDate, btnaction])
            } else {
                toastr.error(resourceObj.error);
            }
        },
        error: function (response) {
            toastr.error(resourceObj.error);
        }
    })
    return notifyData;
}
function getFeeStructureById(id) {
    var feeStructureData = [];
    $.ajax({
        type: 'GET',
        async: false,
        url: basepath + "/FeeStructure/GetFeeStructure?Id=" + id,
        Accept: 'application/json, text/javascript',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (response) {
            if (response != null) {
                var obj = response;
                var installType = "";
                if (obj.hasInstallment == '1') {
                    installType = `<span class="badge bg-success">` +resourceObj.yes + `</span>`
                }
                else if (obj.hasInstallment == '2') {
                    installType = `<span class="badge bg-danger">` + resourceObj.no + `</span>`
                }
                var btndelete = "";
                btndelete = '<a class="dropdown-item" href="#" onclick="deleteAction(`44`,`FeeStructureGrid`, ' + obj.id + ')"><i class="ri-delete-bin-line"></i>&nbsp; ' + resourceObj.delete + '</a>'
                var btnaction = '<div class="btn-group btn-group-sm dropdown" onclick="onSelect(this,FeeStructureGrid);"> ' +
                    '<a href="#" class="table-action-btn dropdown-toggle arrow-none btn btn-light btn-xs" data-bs-toggle="dropdown" aria-expanded="false"><i class="mdi mdi-dots-horizontal"></i></a>' +
                    '<div class="dropdown-menu dropdown-menu-end">' +
                    '<a class="dropdown-item" href="#" onclick="viewAction(`44`, ' + obj.id + ')"><i class="ri-eye-line"></i>&nbsp; ' + resourceObj.view + '</a>' + btndelete +
                    '<a class="dropdown-item" href="#" onclick="addEditAction(`44`, ' + obj.id + ')"><i class="ri-pencil-line"></i>&nbsp; ' + resourceObj.edit + '</a>' +
                    '</div>' +
                    '</div>'

                feeStructureData.push([obj.id, obj.branchName, obj.sessionYearName, obj.title, obj.className, installType, obj.totalAmount, btnaction])

            } else {
                toastr.error(resourceObj.error);
            }
        },
        error: function (response) {
            toastr.error(resourceObj.error);
        }
    })
    return feeStructureData;
}
function getAdminAnnouncementById(id) {

    var adminAnnouncementData = [];
    $.ajax({
        type: 'GET',
        async: false,
        url: basepath + "/AdminAnnouncement/GetAdminAnnouncement?Id=" + id,
        Accept: 'application/json, text/javascript',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (response) {
            if (response != null) {
                var obj = response;
                var btnpath = "";
                btnpath = '<a href="' + obj.filePath + '">' + obj.fileName + '</a>'
                var btndelete = "";
                btndelete = '<a class="dropdown-item" href="#" onclick="deleteAction(`46`,`AdminAnnouncementGrid`, ' + obj.id + ')"><i class="ri-delete-bin-line"></i>&nbsp; ' + resourceObj.delete + '</a>'                
                var btnaction = '<div class="btn-group btn-group-sm dropdown" onclick="onSelect(this,AdminAnnouncementGrid);"> ' +
                    '<a href="#" class="table-action-btn dropdown-toggle arrow-none btn btn-light btn-xs" data-bs-toggle="dropdown" aria-expanded="false"><i class="mdi mdi-dots-horizontal"></i></a>' +
                    '<div class="dropdown-menu dropdown-menu-end">' +
                    '<a class="dropdown-item" href="#" onclick="viewAction(`46`, ' + obj.id + ')"><i class="ri-eye-line"></i>&nbsp; ' + resourceObj.view + '</a>' + btndelete +                   
                    '</div>' +
                    '</div>'

                adminAnnouncementData.push([obj.id, obj.branchName, obj.sessionYearName, obj.title, obj.description, obj.classSectionName, btnpath, btnaction])

            } else {
                toastr.error(resourceObj.error);
            }
        },
        error: function (response) {
            toastr.error(resourceObj.error);
        }
    })
    return adminAnnouncementData;
}
function getPayModeById(id) {
    var paymodeData = [];
    $.ajax({
        type: 'GET',
        async: false,
        url: basepath + "/PayMode/GetPayMode?Id=" + id,
        Accept: 'application/json, text/javascript',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (response) {
            if (response != null) {
                var obj = response;              
                var btndelete = "";
                if (obj.isDelete == "1") {
                    btndelete = '<a class="dropdown-item" href="#" onclick="deleteAction(`47`, ' + obj.Id + ')"><i class="ri-delete-bin-line"></i>&nbsp; ' + resourceObj.delete + '</a>'
                }
                var btnaction = '<div class="btn-group btn-group-sm dropdown" onclick="onSelect(this,PayModeGrid);"> ' +
                    '<a href="#" class="table-action-btn dropdown-toggle arrow-none btn btn-light btn-xs" data-bs-toggle="dropdown" aria-expanded="false"><i class="mdi mdi-dots-horizontal"></i></a>' +
                    '<div class="dropdown-menu dropdown-menu-end">' +
                    '<a class="dropdown-item" href="#" onclick="viewAction(`47`, ' + obj.id + ')"><i class="ri-eye-line"></i>&nbsp; ' + resourceObj.view + '</a>' +
                    '<a class="dropdown-item" href="#" onclick="addEditAction(`47`, ' + obj.id + ')"><i class="ri-pencil-line"></i>&nbsp; ' + resourceObj.edit + '</a>' + btndelete +
                    '</div>' +
                    '</div>'
                paymodeData.push([obj.id, obj.name, obj.description, btnaction])
            } else {
                toastr.error(resourceObj.error);
            }
        },
        error: function (response) {
            toastr.error(resourceObj.error);
        }
    })
    return paymodeData;
}
function getAdminNotificationById(id) {

    var adminNotificationData = [];
    $.ajax({
        type: 'GET',
        async: false,
        url: basepath + "/AdminNotification/GetAdminNotification?Id=" + id,
        Accept: 'application/json, text/javascript',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (response) {
            if (response != null) {
                var obj = response;
                var strSentTo = "";
                if (obj.userTypeId != null) {
                    if (obj.userTypeId == "7") {
                        strSentTo = resourceObj.all;
                    }
                    if (obj.userTypeId == "4") {
                        strSentTo = resourceObj.students;
                    }
                    if (obj.userTypeId == "3") {
                        strSentTo = resourceObj.teachers;;
                    }
                    if (obj.userTypeId == "5") {
                        strSentTo = resourceObj.staff;;
                    }
                    if (obj.userTypeId == "6") {
                        strSentTo = resourceObj.drivers;;
                    }
                }
                else if (obj.userTypeId == null) {
                    strSentTo = resourceObj.students;
                }
               

                var btnpath = "";
                if (obj.filePath != null && obj.fileName != null) {
                    btnpath = '<a href="' + obj.filePath + '">' + obj.fileName + '</a>'
                }  
              
                //var btndelete = "";
                //btndelete = '<a class="dropdown-item" href="#" onclick="deleteAction(`48`,`AdminNotificationGrid`, ' + obj.id + ')"><i class="ri-delete-bin-line"></i>&nbsp; ' + resourceObj.delete + '</a>'
                var btnaction = '<div class="btn-group btn-group-sm dropdown" onclick="onSelect(this,AdminNotificationGrid);"> ' +
                    '<a href="#" class="table-action-btn dropdown-toggle arrow-none btn btn-light btn-xs" data-bs-toggle="dropdown" aria-expanded="false"><i class="mdi mdi-dots-horizontal"></i></a>' +
                    '<div class="dropdown-menu dropdown-menu-end">' +
                    /*'<a class="dropdown-item" href="#" onclick="viewAction(`46`, ' + obj.id + ')"><i class="ri-eye-line"></i>&nbsp; ' + resourceObj.view + '</a>' +*/
                  //  btndelete +
                    '</div>' +
                    '</div>'

                adminNotificationData.push([obj.id, obj.title, obj.message, btnpath, strSentTo, obj.strCreatedDate, btnaction])

            } else {
                toastr.error(resourceObj.error);
            }
        },
        error: function (response) {
            toastr.error(resourceObj.error);
        }
    })
    return adminNotificationData;
}
function getActivityById(id) {
    var activityData = [];
    $.ajax({
        type: 'GET',
        async: false,
        url: basepath + "/Activity/GetActivity?Id=" + id,
        Accept: 'application/json, text/javascript',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (response) {
            if (response != null) {
                var obj = response;
               // var inputcolor = '<input class="form-control" type="color" name="color" disabled value=' + obj.activityColor + '>'
                
                var btndelete = "";
                if (obj.isDelete == "1") {
                    btndelete = '<a class="dropdown-item" href="#" onclick="deleteAction(`49`,`ActivityGrid`, ' + obj.id + ')"><i class="ri-delete-bin-line"></i>&nbsp; ' + resourceObj.delete + '</a>'
                }
                var btnaction = '<div class="btn-group btn-group-sm dropdown" onclick="onSelect(this,ActivityGrid);"> ' +
                    '<a href="#" class="table-action-btn dropdown-toggle arrow-none btn btn-light btn-xs" data-bs-toggle="dropdown" aria-expanded="false"><i class="mdi mdi-dots-horizontal"></i></a>' +
                    '<div class="dropdown-menu dropdown-menu-end">' +
                    '<a class="dropdown-item" href="#" onclick="viewAction(`49`, ' + obj.id + ')"><i class="ri-eye-line"></i>&nbsp; ' + resourceObj.view + '</a>' +
                    '<a class="dropdown-item" href="#" onclick="addEditAction(`49`, ' + obj.id + ')"><i class="ri-pencil-line"></i>&nbsp; ' + resourceObj.edit + '</a>' + btndelete +
                    '</div>' +
                    '</div>'
                activityData.push([obj.id, obj.name, obj.shortName, obj.activityColor, obj.description, btnaction])

            } else {
                toastr.error(resourceObj.error);
            }
        },
        error: function (response) {
            toastr.error(resourceObj.error);
        }
    })
    return activityData;
}
function getPeriodBreakById(id) {
    var periodBreakData = [];
    $.ajax({
        type: 'GET',
        async: false,
        url: basepath + "/PeriodBreak/GetPeriodBreak?Id=" + id,
        Accept: 'application/json, text/javascript',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (response) {
            if (response != null) {
                var obj = response;
               // var inputcolor = '<input class="form-control" type="color" name="color" disabled value=' + obj.breakColor + '>'

                var btndelete = "";
                if (obj.isDelete == "1") {
                    btndelete = '<a class="dropdown-item" href="#" onclick="deleteAction(`50`,`PeriodBreakGrid`, ' + obj.id + ')"><i class="ri-delete-bin-line"></i>&nbsp; ' + resourceObj.delete + '</a>'
                }
                var btnaction = '<div class="btn-group btn-group-sm dropdown" onclick="onSelect(this,PeriodBreakGrid);"> ' +
                    '<a href="#" class="table-action-btn dropdown-toggle arrow-none btn btn-light btn-xs" data-bs-toggle="dropdown" aria-expanded="false"><i class="mdi mdi-dots-horizontal"></i></a>' +
                    '<div class="dropdown-menu dropdown-menu-end">' +
                    '<a class="dropdown-item" href="#" onclick="viewAction(`50`, ' + obj.id + ')"><i class="ri-eye-line"></i>&nbsp; ' + resourceObj.view + '</a>' +
                    '<a class="dropdown-item" href="#" onclick="addEditAction(`50`, ' + obj.id + ')"><i class="ri-pencil-line"></i>&nbsp; ' + resourceObj.edit + '</a>' + btndelete +
                    '</div>' +
                    '</div>'
                periodBreakData.push([obj.id, obj.name, obj.shortName, obj.breakColor, obj.fromTime, obj.toTime, obj.description, btnaction])

            } else {
                toastr.error(resourceObj.error);
            }
        },
        error: function (response) {
            toastr.error(resourceObj.error);
        }
    })
    return periodBreakData;
}