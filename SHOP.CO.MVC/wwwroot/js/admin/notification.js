const NotificationManager = (function () {
    let _apiUrl = '';
    let _token = '';
    let _role = '';

    let currentPage = 1;
    const pageSize = 10;
    let totalItems = 0;
    let odataUrlBase = '';

    function init(apiUrl, token, role) {
        _apiUrl = apiUrl.endsWith('/') ? apiUrl.slice(0, -1) : apiUrl;
        _token = token;
        _role = role;
        
        // Base OData URL
        odataUrlBase = `${_apiUrl}/odata/AdminNotificationsOData`;

        loadData();
    }

    function resetPageAndLoad() {
        currentPage = 1;
        loadData();
    }

    function loadData() {
        const searchInput = $('#searchInput').val().trim();
        const skip = (currentPage - 1) * pageSize;

        // Xây dựng query OData
        let filterParts = [];
        if (searchInput) {
            filterParts.push(`(contains(tolower(Title),'${searchInput.toLowerCase()}') or contains(tolower(Message),'${searchInput.toLowerCase()}'))`);
        }

        let filterQuery = filterParts.length > 0 ? `$filter=${filterParts.join(' and ')}&` : '';
        const query = `?${filterQuery}$count=true&$top=${pageSize}&$skip=${skip}&$orderby=CreatedAt desc`;

        AdminCommon.ajaxGet(`${odataUrlBase}${query}`, _token)
            .then(res => {
                totalItems = res['@odata.count'] || 0;
                renderTable(res.value);
                renderPagination();
            })
            .catch(err => {
                console.error("Lỗi khi tải thông báo:", err);
                AdminCommon.showToast('Lỗi khi tải danh sách thông báo!', 'error');
            });
    }

    function renderTable(items) {
        const tbody = $('#notificationTableBody');
        tbody.empty();

        if (!items || items.length === 0) {
            tbody.html(`<tr><td colspan="6" class="text-center py-4 text-muted">Không tìm thấy thông báo nào.</td></tr>`);
            return;
        }

        items.forEach(item => {
            const dateStr = AdminCommon.formatDateTime(item.CreatedAt);
            
            // Trạng thái (hiển thị tĩnh là Đã gửi)
            const statusBadge = `<span class="badge bg-success">Đã Gửi</span>`;

            const tr = `
                <tr>
                    <td class="fw-bold text-center">#${item.LogId}</td>
                    <td class="fw-bold text-primary">${item.Title}</td>
                    <td>
                        <div class="text-truncate" style="max-width: 250px;" title="${item.Message}">
                            ${item.Message}
                        </div>
                    </td>
                    <td class="text-center">${dateStr}</td>
                    <td class="text-center">${statusBadge}</td>
                    <td class="text-center">
                        <button class="btn btn-outline-danger btn-sm" onclick="NotificationManager.deleteNotification(${item.LogId})" title="Xóa">
                            <i class="fas fa-trash-alt"></i>
                        </button>
                    </td>
                </tr>
            `;
            tbody.append(tr);
        });
    }

    function renderPagination() {
        AdminCommon.renderPagination(totalItems, pageSize, currentPage, 'paginationUl', 'paginationInfo', function (page) {
            currentPage = page;
            loadData();
        });
    }

    function openOffcanvas(mode) {
        $('#notificationForm')[0].reset();
        
        const myOffcanvas = new bootstrap.Offcanvas(document.getElementById('offcanvasNotification'));
        myOffcanvas.show();
    }

    function saveNotification() {
        const title = $('#Title').val().trim();
        const message = $('#Message').val().trim();

        if (!title || !message) {
            AdminCommon.showToast('Vui lòng nhập đầy đủ Tiêu đề và Nội dung!', 'warning');
            return;
        }

        const payload = {
            Title: title,
            Message: message,
            SendImmediately: true
        };

        const saveBtn = $('#btnSaveNotification');
        AdminCommon.setButtonLoading(saveBtn, true, 'ĐANG GỬI...');

        const url = `${_apiUrl}/api/admin/notifications`;

        AdminCommon.ajaxPost(url, payload, _token)
            .then(res => {
                AdminCommon.showToast('Đã gửi thông báo thành công!', 'success');
                const offcanvas = bootstrap.Offcanvas.getInstance(document.getElementById('offcanvasNotification'));
                if (offcanvas) offcanvas.hide();
                resetPageAndLoad();
            })
            .catch(err => {
                const msg = err.responseJSON?.message || 'Có lỗi xảy ra khi gửi thông báo.';
                AdminCommon.showToast(msg, 'error');
            })
            .finally(() => {
                AdminCommon.setButtonLoading(saveBtn, false, '<i class="fas fa-paper-plane me-1"></i> GỬI THÔNG BÁO');
            });
    }

    function deleteNotification(id) {
        Swal.fire({
            title: 'Xóa Thông Báo?',
            text: "Thao tác này sẽ xóa thông báo khỏi hệ thống. Bạn có chắc chắn không?",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#6c757d',
            confirmButtonText: 'Đồng ý, Xóa!',
            cancelButtonText: 'Hủy'
        }).then((result) => {
            if (result.isConfirmed) {
                AdminCommon.ajaxDelete(`${_apiUrl}/api/admin/notifications/${id}`, _token)
                    .then(res => {
                        AdminCommon.showToast('Đã xóa thông báo!', 'success');
                        loadData();
                    })
                    .catch(err => {
                        AdminCommon.showToast('Lỗi khi xóa thông báo!', 'error');
                    });
            }
        });
    }

    return {
        init: init,
        resetPageAndLoad: resetPageAndLoad,
        openOffcanvas: openOffcanvas,
        saveNotification: saveNotification,
        deleteNotification: deleteNotification
    };
})();
