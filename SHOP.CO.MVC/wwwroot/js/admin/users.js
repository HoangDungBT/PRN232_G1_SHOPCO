const UserManager = {
    apiUrl: '',
    token: '',
    currentRole: '',
    currentPage: 1,
    pageSize: 10,

    init: function (apiBaseUrl, jwtToken, role) {
        this.apiUrl = apiBaseUrl;
        this.token = jwtToken;
        this.currentRole = role;

        this.bindEvents();
        this.loadUsers();
    },

    bindEvents: function () {
        $('#btnSearch').click(() => {
            this.currentPage = 1;
            this.loadUsers();
        });

        $('#searchInput').keypress((e) => {
            if (e.which == 13) {
                $('#btnSearch').click();
            }
        });

        $(document).on('click', '.page-link', (e) => {
            e.preventDefault();
            this.currentPage = parseInt($(e.target).data('page'));
            this.loadUsers();
        });
    },

    loadUsers: function () {
        let skip = (this.currentPage - 1) * this.pageSize;
        let odataQuery = `odata/AdminUsersOData?$count=true&$top=${this.pageSize}&$skip=${skip}&$orderby=UserId desc`;

        let keyword = $('#searchInput').val().trim();
        if (keyword) {
            odataQuery += `&$filter=contains(tolower(FullName), '${keyword.toLowerCase()}') or contains(tolower(Email), '${keyword.toLowerCase()}')`;
        }

        AdminHelper.ajaxRequest(
            this.apiUrl + odataQuery,
            'GET',
            null,
            this.token,
            (res) => {
                if (res && res.value) {
                    this.renderTable(res.value);
                    this.renderPagination(res['@odata.count']);
                }
            },
            () => {
                $('#userTableBody').html(`<tr><td colspan="6" class="text-center text-danger py-4">Lỗi tải dữ liệu.</td></tr>`);
            },
            () => {
                $('#userTableBody').html(`<tr><td colspan="6" class="text-center py-4 text-muted"><i class="fas fa-spinner fa-spin me-2"></i> Đang tải dữ liệu...</td></tr>`);
            }
        );
    },

    renderTable: function (users) {
        let html = '';
        if (users.length === 0) {
            html = `<tr><td colspan="6" class="text-center py-4 text-muted">Không tìm thấy người dùng nào!</td></tr>`;
        } else {
            users.forEach(u => {
                let statusHtml = u.Status === 'Active'
                    ? `<span class="badge bg-success">Hoạt động</span>`
                    : `<span class="badge bg-danger">Đã khóa</span>`;

                let roleHtml = u.Role === 'Admin'
                    ? `<span class="badge bg-dark">Quản trị viên</span>`
                    : (u.Role === 'Staff' ? `<span class="badge bg-info text-dark">Nhân viên</span>` : `<span class="badge bg-secondary">Khách hàng</span>`);

                let actionButtons = `<span class="text-muted small">Không có quyền</span>`;
                if (this.currentRole === 'Admin' && u.Role !== 'Admin') {
                    let lockBtn = u.Status === 'Active'
                        ? `<button class="btn btn-sm btn-outline-danger me-1" title="Khóa tài khoản" onclick="UserManager.toggleStatus(${u.UserId}, 'Active')"><i class="fas fa-lock"></i></button>`
                        : `<button class="btn btn-sm btn-outline-success me-1" title="Mở khóa tài khoản" onclick="UserManager.toggleStatus(${u.UserId}, 'Locked')"><i class="fas fa-unlock"></i></button>`;

                    let roleBtn = u.Role === 'Customer'
                        ? `<button class="btn btn-sm btn-outline-info" title="Thăng cấp thành Staff" onclick="UserManager.toggleRole(${u.UserId}, 'Customer')"><i class="fas fa-arrow-up"></i></button>`
                        : `<button class="btn btn-sm btn-outline-warning" title="Giáng cấp thành Khách hàng" onclick="UserManager.toggleRole(${u.UserId}, 'Staff')"><i class="fas fa-arrow-down"></i></button>`;

                    actionButtons = lockBtn + roleBtn;
                }

                let lastLogin = u.LastLoginAt ? new Date(u.LastLoginAt).toLocaleString('vi-VN') : 'Chưa đăng nhập';

                html += `
                    <tr>
                        <td>#${u.UserId}</td>
                        <td>
                            <strong>${u.FullName}</strong><br>
                            <small class="text-muted">Đăng nhập: ${lastLogin}</small>
                        </td>
                        <td>
                            <i class="fas fa-envelope text-muted"></i> ${u.Email}<br>
                            <i class="fas fa-phone text-muted"></i> ${u.Phone || 'N/A'}
                        </td>
                        <td class="text-center">${roleHtml}</td>
                        <td class="text-center">${statusHtml}</td>
                        <td class="text-center">${actionButtons}</td>
                    </tr>
                `;
            });
        }
        $('#userTableBody').html(html);
    },

    toggleStatus: function (id, currentStatus) {
        let newStatus = currentStatus === 'Active' ? 'Locked' : 'Active';
        let actionName = currentStatus === 'Active' ? 'Khóa' : 'Mở khóa';

        AdminHelper.confirmAction(
            `${actionName} tài khoản?`,
            `Tài khoản bị khóa sẽ không thể đăng nhập vào hệ thống!`,
            () => {
                AdminHelper.ajaxRequest(
                    this.apiUrl + `api/admin/users/${id}/status`,
                    'PUT',
                    { value: newStatus },
                    this.token,
                    (res) => {
                        if (res.isSuccess) {
                            AdminHelper.showSuccess(res.message);
                            this.loadUsers();
                        } else {
                            AdminHelper.showError(res.message);
                        }
                    }
                );
            }
        );
    },

    toggleRole: function (id, currentRole) {
        let newRole = currentRole === 'Customer' ? 'Staff' : 'Customer';
        let actionName = currentRole === 'Customer' ? 'Thăng cấp thành Nhân Viên' : 'Giáng cấp thành Khách hàng';

        AdminHelper.confirmAction(
            `${actionName}?`,
            `Người dùng sẽ được thêm (hoặc mất) quyền truy cập vào Khu Vực Quản Trị.`,
            () => {
                AdminHelper.ajaxRequest(
                    this.apiUrl + `api/admin/users/${id}/role`,
                    'PUT',
                    { value: newRole },
                    this.token,
                    (res) => {
                        if (res.isSuccess) {
                            AdminHelper.showSuccess(res.message);
                            this.loadUsers();
                        } else {
                            AdminHelper.showError(res.message);
                        }
                    }
                );
            }
        );
    },

    renderPagination: function (totalRecords) {
        let totalPages = Math.ceil(totalRecords / this.pageSize);
        let paginationHtml = '';
        let startRecord = totalRecords === 0 ? 0 : ((this.currentPage - 1) * this.pageSize) + 1;
        let endRecord = (this.currentPage * this.pageSize) > totalRecords ? totalRecords : (this.currentPage * this.pageSize);
        $('#paginationInfo').text(`Hiển thị ${startRecord} - ${endRecord} trên tổng ${totalRecords} khách hàng`);

        for (let i = 1; i <= totalPages; i++) {
            paginationHtml += `
                <li class="page-item ${this.currentPage === i ? 'active' : ''}">
                    <a class="page-link" href="#" data-page="${i}">${i}</a>
                </li>
            `;
        }
        $('#paginationUl').html(paginationHtml);
    }
};
