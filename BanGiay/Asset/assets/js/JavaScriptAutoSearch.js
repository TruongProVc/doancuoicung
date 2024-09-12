$('#keyword').keyup(function () {
    var searchField = $('#keyword').val();
    var sf = document.getElementById('keyword');

    // Xóa các mục tìm kiếm trước đó
    $('.product-list').remove();

    // Thay đổi border-radius tùy thuộc vào việc có văn bản trong trường tìm kiếm hay không
    if (searchField.length > 0) {
        sf.style.borderRadius = "20px 20px 0 0";
    } else {
        sf.style.borderRadius = "20px";
    }

    // Gửi yêu cầu AJAX
    $.ajax({
        type: "GET",
        url: "/Home/AutoComplete",
        data: { keyword: searchField },
        success: function (response) {
            // Tạo một div chứa danh sách sản phẩm
            var html_Body = `<div class="product-list"></div>`;
            $('#infoSearch').append(html_Body);

            // Duyệt qua các sản phẩm và tạo HTML cho mỗi sản phẩm
            $.each(response.data, function (index, item) {
                var formattedPrice = parseFloat(item.giaTien).toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });

                var html_Search = `<div class="product-item">
                    <a href="/ProductDetails?idSanPham=${item.maSanPham}">
                        <img src="${item.hinhSanPham}" alt="${item.tenSanPham}" />
                    </a>
                    <p class="price">${formattedPrice}</p>
                    <p class="name">${item.tenSanPham}</p>
                </div>`;
                $('.product-list').append(html_Search);
            });
        },
        error: function (xhr, status, error) {
            console.error("Error fetching data:", error);
        }
    });
});
