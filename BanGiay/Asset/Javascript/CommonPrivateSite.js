
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
    $('#pagination').html(str);
};
function getDate(dateString) {
    var momentTime = moment(dateString);

    var formattedDate = momentTime.format("DD/MM/YYYY");

    return formattedDate;
}
function getDateEpoch(epochTime) {
    var momentTime = moment(epochTime);

    var formattedDate = momentTime.format("DD/MM/YYYY HH:mm:ss");

    return formattedDate;
}
function getDateEpoch1(epochTimeString) {
    var epochString = epochTimeString.substring(6, epochTimeString.length - 2);

    var epochTime = parseInt(epochString);

    var momentTime = moment.unix(epochTime);

    var formattedDate = momentTime.format("YYYY-MM-DD HH:mm:ss");

    return formattedDate;
}
function formatJsonDate(jsonDate) {
    var ticks = parseInt(jsonDate.replace("/Date(", "").replace(")/", ""));

    var date = new Date(ticks);

    var year = date.getFullYear();
    var month = (date.getMonth() + 1).toString().padStart(2, '0'); // Thêm số 0 phía trước nếu cần
    var day = date.getDate().toString().padStart(2, '0'); // Thêm số 0 phía trước nếu cần

    var formattedDate = year + '-' + month + '-' + day;

    return formattedDate;
}
function formatJsonDate1(jsonDate) {
    var ticks = parseInt(jsonDate.replace("/Date(", "").replace(")/", ""));

    var date = new Date(ticks);

    var year = date.getFullYear();
    var month = (date.getMonth() + 1).toString().padStart(2, '0'); // Thêm số 0 phía trước nếu cần
    var day = date.getDate().toString().padStart(2, '0'); // Thêm số 0 phía trước nếu cần

    var hours = date.getHours().toString().padStart(2, '0'); // Thêm số 0 phía trước nếu cần
    var minutes = date.getMinutes().toString().padStart(2, '0'); // Thêm số 0 phía trước nếu cần

    var formattedDate = year + '-' + month + '-' + day + ' ' + hours + ':' + minutes;

    return formattedDate;
}
