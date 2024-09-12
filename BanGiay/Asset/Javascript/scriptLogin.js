
//Đối tượng 'validator'
function Validator(options) {
    var selectorRule = {};
    function Validate(inputElement, rule) {
        var errorElement = inputElement.parentElement.querySelector(options.errorSelector);
        var errorMessage = '';

        //Lấy ra các rules của selector
        var rules = selectorRule[rule.selector];
        // Lặp qua từng rule & kiểm tra
        // Nếu có lỗi thifd dừng việc kiểm tra
        for (var i = 0; i < rules.length; i++) {
            errorMessage = selectorRule[rule.selector][i](inputElement.value);
            console.log(errorMessage)
            if (errorMessage) break;
        }
        if (errorMessage) {
            errorElement.innerText = errorMessage;
            inputElement.parentElement.classList.add('invalid');
        } else {
            errorElement.innerText = '';
            inputElement.parentElement.classList.remove('invalid');
        }
        return !!errorMessage;
    }

    var formElement = document.querySelector(options.form);
    if (formElement) {
        formElement.onsubmit = function (e) {
            e.preventDefault();

            var isFormValid = true;

            // Lặp qua từng rule và validate
            options.rules.forEach(function (rule) {
                var inputElement = formElement.querySelector(rule.selector)
                var isValid = Validate(inputElement, rule);
                if (isValid) isFormValid = false;
            });
            if (isFormValid) {
                // trường hợp submit với javascript
                if (typeof options.onSubmit === 'function') {
                    var enableInputs = formElement.querySelectorAll('[name]:not([disable])');

                    var formValues = Array.from(enableInputs).reduce(function (values, input) {
                        return (values[input.name] = input.value) && values;
                    }, {});
                    options.onSubmit(formValues);
                }
            }
            // trường hợp submit với hành vi mặc định
            else {
                formElement.submit();
            }
        }
        //Xử lí lặp qua mỗi rule và xử lý(lắng nghe sự kiện blur, input,...)
        options.rules.forEach(function (rule) {
            //Lưu lại các rules cho mỗi input vào mảng selectorRule
            if (Array.isArray(selectorRule[rule.selector])) {
                selectorRule[rule.selector].push(rule);
            } else {
                selectorRule[rule.selector] = [rule.test]
            }
            var inputElement = formElement.querySelector(rule.selector)
            if (inputElement) {
                //xử lí trường hợp blur ra khỏi thẻ input
                inputElement.onblur = function () {
                    Validate(inputElement, rule);
                }
                // xử lí trương hợp mỗi khi người dùng nhập vào thẻ input
                inputElement.oninput = function () {
                    var errorElement = inputElement.parentElement.querySelector(options.errorSelector);
                    inputElement.parentElement.classList.remove('invalid')
                    errorElement.innerText = '';
                }
            }
        });
    }
}

//Định nghĩa các rules
//Nguyên tắc của các rules
//1. Khi có lỗi => trả ra message lỗi
//2. Khi hợp lệ => không trả ra cái gì cả

Validator.isRequired = function (selector, message) {
    return {
        selector: selector,
        test: function (value) {
            return value.trim() ? undefined : message || 'Vui lòng nhập trường này'
        }
    };
}
Validator.isEmail = function (selector, message) {
    return {
        selector: selector,
        test: function (value) {
            var regex = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/;
            return regex.test(value) ? undefined : message || 'Trường này phải là email'
        }
    };
};
Validator.minLenght = function (selector, min, message) {
    return {
        selector: selector,
        test: function (value) {
            return value.length >= min ? undefined : message;
        }
    };
}
Validator.isConfirmed = function (selector, getPasswordChecked, message) {
    return {
        selector: selector,
        test: function (value) {
            return value === getPasswordChecked() ? undefined : message || 'Nhập lại mật khẩu không khớp ! Vui lòng kiểm tra lại';
        }
    }
}
