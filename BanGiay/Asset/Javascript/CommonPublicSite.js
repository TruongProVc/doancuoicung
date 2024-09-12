function ShowLoading(isShow) {
    const loading = document.querySelector("#loading");
    if (isShow) {
        loading.classList.add("loader");

    } else {
        loading.classList.remove("loader");
    }
}
function NextPage(keyword1, page, pageSize) {
    loadData(keyword1, page, pageSize);
};
function Pagination(keyword, currentPage, NumberPage, pageSize) {
    var kw = `'${keyword}'`;
    var str = "";
    if (NumberPage == 0) {
        var str = `<ul class="pagination pagination-sm"></ul>`;
    }
    if (NumberPage > 0) {
        str = `<ul class="pagination pagination-sm">`;
        if (currentPage != 1) {
            str += `<li class="page-item"><a class="page-link" onclick="NextPage(${kw},${currentPage - 1},${pageSize})" href="javascript:void(0);">&laquo;</a></li>`;
        }
        var startPage = 1;
        var endPage = NumberPage;
        if (NumberPage > 5) {
            startPage = Math.max(currentPage - 2, 1);
            endPage = Math.min(startPage + 4, NumberPage);
            if (startPage > 1) {
                str += `<li class="page-item"><a class="page-link" onclick="NextPage(${kw},1,${pageSize})" href="javascript:void(0);">1</a></li>`;
                if (startPage > 2) {
                    str += `<li class="page-item"><span class="page-link">...</span></li>`;
                }
            }
        }
        for (let i = startPage; i <= endPage; i++) {
            if (currentPage === i) {
                str += `<li class="page-item active"><a class="page-link" href="javascript:void(0);">${i}</a></li>`;
            } else {
                str += `<li class="page-item"><a class="page-link" onclick="NextPage(${kw},${i},${pageSize})" href="javascript:void(0);">${i}</a></li>`;
            }
        }

        if (NumberPage > 5 && endPage < NumberPage) {
            if (endPage < NumberPage - 1) {
                str += `<li class="page-item"><span class="page-link">...</span></li>`;
            }
            str += `<li class="page-item"><a class="page-link" onclick="NextPage(${kw},${NumberPage},${pageSize})" href="javascript:void(0);">${NumberPage}</a></li>`;
        }

        if (currentPage != NumberPage) {
            str += ` <li class="page-item"><a class="page-link" onclick="NextPage(${kw},${currentPage + 1},${pageSize})" href="javascript:void(0);">&raquo;</a></li>`;
        }
        str += "</ul></nav>";

    }
    $('#pagination2').html(str);
};
function formatJsonDate(jsonDate) {
    // Tách lấy số miligiây từ chuỗi
    var ticks = parseInt(jsonDate.replace("/Date(", "").replace(")/", ""));

    // Tạo một đối tượng Date từ số miligiây
    var date = new Date(ticks);

    // Lấy các thành phần của ngày
    var year = date.getFullYear();
    var month = (date.getMonth() + 1).toString().padStart(2, '0'); // Thêm số 0 phía trước nếu cần
    var day = date.getDate().toString().padStart(2, '0'); // Thêm số 0 phía trước nếu cần

    // Lấy các thành phần của thời gian
    var hours = date.getHours().toString().padStart(2, '0'); // Thêm số 0 phía trước nếu cần
    var minutes = date.getMinutes().toString().padStart(2, '0'); // Thêm số 0 phía trước nếu cần

    // Định dạng lại chuỗi ngày thành "yyyy-MM-dd"
    var formattedDate = year + '-' + month + '-' + day + ' ' + hours + ':' + minutes;

    return formattedDate;
}
