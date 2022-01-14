"use strict";

function exporting(e) {
    var workbook = new ExcelJS.Workbook();
    var worksheet = workbook.addWorksheet('Report');

    DevExpress.excelExporter.exportDataGrid({
        component: e.component,
        worksheet: worksheet,
        topLeftCell: { row: 4, column: 1 }
    }).then(function (dataGridRange) {
        // header
        var headerRow = worksheet.getRow(2);
        headerRow.height = 30;
        worksheet.mergeCells(2, 1, 2, 9);

        headerRow.getCell(1).value = 'Report';
        headerRow.getCell(1).font = { name: 'Segoe UI Light', size: 22 };
        headerRow.getCell(1).alignment = { horizontal: 'center' };

        // footer
        var footerRowIndex = dataGridRange.to.row + 2;
        var footerRow = worksheet.getRow(footerRowIndex);
        worksheet.mergeCells(footerRowIndex, 1, footerRowIndex, 9);

        footerRow.getCell(1).value = 'https://www.fact-link.com.vn/mem_profile.php?id=00051619&page=00005181&lang=vn';
        footerRow.getCell(1).font = { color: { argb: 'BFBFBF' }, italic: true };
        footerRow.getCell(1).alignment = { horizontal: 'right' };
    }).then(function () {
        workbook.xlsx.writeBuffer().then(function (buffer) {
            saveAs(new Blob([buffer], { type: 'application/octet-stream' }), 'Report.xlsx');
        });
    });
    e.cancel = true;
}