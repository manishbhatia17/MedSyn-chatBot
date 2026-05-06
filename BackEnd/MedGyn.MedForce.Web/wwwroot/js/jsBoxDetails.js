var boxes = [];
var sortedBoxes = [];

function createTableOfFilledBoxes(thisBox) {   
    
    if (thisBox.shipment.numberOfSameBoxes > 1) {
        for (i = 0; i < thisBox.shipment.numberOfSameBoxes; i++) {
            boxes.push(thisBox.shipmentBox);
        }
    } else {
        boxes.push(thisBox.shipmentBox);
    }
}

function showTableOfFilledBoxes(currentBoxId) {
    document.getElementById("labelBoxDetails").style.display = 'block';
    document.getElementById("divFilledBox").innerHTML = "";
    
    var table = document.createElement('table');
    //table.setAttribute('border', '1');
    table.setAttribute('width', '100%');
    table.className = "boxDetailsTable dataTable stripe";
    var tableBody = document.createElement('tbody');
    var header = "<thead><tr class=\"boxDetailsTableHeader\"><th>Box ID</th><th>Box weight</th><th>Weight unit</th>";    
    header = header + "<th>Length</th><th>Width</th><th>Depth</th><th>Dim unit</th></tr></thead>";
    sortedBoxes = boxes.sort((a, b) => (a.customerOrderShipmentBoxID > b.customerOrderShipmentBoxID) ? 1 : ((b.customerOrderShipmentBoxID > a.customerOrderShipmentBoxID) ? -1 : 0)); 
    var i = 1;
    sortedBoxes.forEach(function (rowData) {
        
        var row = document.createElement('tr');    
        if (i % 2) {
            row.className = "odd";
        }

        var cell = document.createElement('td');
        if (currentBoxId == rowData.customerOrderShipmentBoxID) {
            cell.className = "boxDetailsTableTdActive";
        } else {
            cell.className = "boxDetailsTableTd";
        }        
        cell.appendChild(document.createTextNode(i));
        row.appendChild(cell);
        i += 1;

        cell = document.createElement('td');
        if (currentBoxId == rowData.customerOrderShipmentBoxID) {
            cell.className = "boxDetailsTableTdActive";
        } else {
            cell.className = "boxDetailsTableTd";
        }        
        cell.appendChild(document.createTextNode(rowData.isFormPrefilled == true ? "" : (rowData.weight === null ? "" : rowData.weight)));
        row.appendChild(cell);

        cell = document.createElement('td');
        if (currentBoxId == rowData.customerOrderShipmentBoxID) {
            cell.className = "boxDetailsTableTdActive";
        } else {
            cell.className = "boxDetailsTableTd";
        }
        cell.appendChild(document.createTextNode(rowData.isFormPrefilled == true ? "" : GetDimentionTextFromCode(rowData.weightUnitCodeID)));
        row.appendChild(cell);    

        cell = document.createElement('td');
        if (currentBoxId == rowData.customerOrderShipmentBoxID) {
            cell.className = "boxDetailsTableTdActive";
        } else {
            cell.className = "boxDetailsTableTd";
        }
        cell.appendChild(document.createTextNode(rowData.isFormPrefilled == true ? "" : (rowData.length === null ? "" : rowData.length)));
        row.appendChild(cell);

        cell = document.createElement('td');
        if (currentBoxId == rowData.customerOrderShipmentBoxID) {
            cell.className = "boxDetailsTableTdActive";
        } else {
            cell.className = "boxDetailsTableTd";
        }
        cell.appendChild(document.createTextNode(rowData.isFormPrefilled == true ? "" : (rowData.width === null ? "" : rowData.width)));
        row.appendChild(cell);

        cell = document.createElement('td');
        if (currentBoxId == rowData.customerOrderShipmentBoxID) {
            cell.className = "boxDetailsTableTdActive";
        } else {
            cell.className = "boxDetailsTableTd";
        }
        cell.appendChild(document.createTextNode(rowData.isFormPrefilled == true ? "" : (rowData.depth === null ? "" : rowData.depth) ));
        row.appendChild(cell);

        cell = document.createElement('td');
        if (currentBoxId == rowData.customerOrderShipmentBoxID) {
            cell.className = "boxDetailsTableTdActive";
        } else {
            cell.className = "boxDetailsTableTd";
        }
        cell.appendChild(document.createTextNode(rowData.isFormPrefilled == true ? "" :GetDimentionTextFromCode(rowData.dimensionUnitCodeID)));
        row.appendChild(cell);            

        tableBody.appendChild(row);
    });
    table.innerHTML = header;
    table.appendChild(tableBody);
    document.getElementById("divFilledBox").appendChild(table);
}

function GetDimentionTextFromCode(code) {
    switch (code) {
        case 11: return "cm";
        case 460: return "CM";
        case 12: return "in";
        case 469: return "IN";
        case 9: return "kg";
        case 10: return "lb";
        default: return "";
    }
}

function UpdateBoxDetails(boxDetails) {
    sortedBoxes.forEach(function (box) {
        
        if (box.customerOrderShipmentBoxID == boxDetails.customerOrderShipmentBoxID) {
            var index = sortedBoxes.indexOf(box);
            if (index !== -1) {
                sortedBoxes[index] = boxDetails;
            }
            showTableOfFilledBoxes(boxDetails.customerOrderShipmentBoxID);
        }
    });
}