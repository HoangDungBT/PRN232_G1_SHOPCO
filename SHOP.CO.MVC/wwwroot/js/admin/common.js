const AdminHelper = {
    /**
     * Parse thông báo lỗi từ Response (Hỗ trợ 400 Validation và 500 ResultModel)
     */
    getErrorMessage: function(xhr) {
        if (xhr && xhr.responseJSON) {
            if (xhr.responseJSON.errors) {
                let errorList = [];
                for (let key in xhr.responseJSON.errors) {
                    errorList.push(xhr.responseJSON.errors[key].join('<br>'));
                }
                return errorList.join('<br><br>');
            }
            if (xhr.responseJSON.message) {
                return xhr.responseJSON.message;
            }
        }
        return 'Có lỗi xảy ra, vui lòng thử lại sau!';
    },

    /**
     * Gọi AJAX chung kèm Authorization Header
     */
    ajaxRequest: function(url, type, data, token, onSuccess, onError, onBeforeSend) {
        let ajaxConfig = {
            url: url,
            type: type,
            cache: false,
            headers: { 'Authorization': 'Bearer ' + token },
            success: onSuccess,
            error: onError || function(xhr) {
                let err = AdminHelper.getErrorMessage(xhr);
                AdminHelper.showError(err);
            }
        };

        if (data && type !== 'GET') {
            ajaxConfig.contentType = 'application/json';
            ajaxConfig.data = JSON.stringify(data);
        }

        if (onBeforeSend) {
            ajaxConfig.beforeSend = onBeforeSend;
        }

        $.ajax(ajaxConfig);
    },

    /**
     * Thông báo thành công tự đóng
     */
    showSuccess: function(message, callback) {
        Swal.fire({
            title: 'Thành công',
            text: message,
            icon: 'success',
            timer: 1500,
            showConfirmButton: false
        }).then(() => {
            if (callback) callback();
        });
    },

    /**
     * Thông báo lỗi
     */
    showError: function(message) {
        Swal.fire('Lỗi', message, 'error');
    },
    
    /**
     * Hiển thị hộp thoại xác nhận trước khi thực hiện hành động
     */
    confirmAction: function(title, text, confirmCallback) {
        Swal.fire({
            title: title,
            text: text,
            icon: 'question',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Đồng ý',
            cancelButtonText: 'Hủy'
        }).then((result) => {
            if (result.isConfirmed) {
                confirmCallback();
            }
        });
    }
};
