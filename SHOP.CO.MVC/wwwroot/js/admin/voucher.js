const VoucherManager = {
    apiUrl: '',
    token: '',
    currentRole: '',
    currentPage: 1,
    pageSize: 10,
    offcanvasInstance: null,

    init: function (apiBaseUrl, jwtToken, role) {
        this.apiUrl = apiBaseUrl;
        this.token = jwtToken;
        this.currentRole = role;
        console.log('Current Role:', this.currentRole);
        // Check if current user is Admin or Staff
        if (this.currentRole === 'Admin' || this.currentRole === 'Staff') {
            $('#addVoucherBtn').show();
        } else {
            $('#addVoucherBtn').hide();
        }
        this.offcanvasInstance = new bootstrap.Offcanvas(document.getElementById('offcanvasVoucher'));

        this.bindEvents();
        this.loadVouchers();
    },

    bindEvents: function () {
        $('#searchInput').keypress((e) => {
            if (e.which == 13) {
                this.resetPageAndLoad();
            }
        });

        $(document).on('click', '.page-link', (e) => {
            e.preventDefault();
            this.currentPage = parseInt($(e.target).data('page'));
            this.loadVouchers();
        });
    },

    resetPageAndLoad: function () {
        this.currentPage = 1;
        this.loadVouchers();
    },

    loadVouchers: function () {
        let skip = (this.currentPage - 1) * this.pageSize;
        let odataQuery = `odata/AdminVouchersOData?$count=true&$top=${this.pageSize}&$skip=${skip}&$orderby=RecordId desc`;

        let keyword = $('#searchInput').val().trim();
        let filterStatus = $('#filterStatus').val();

        let filters = [];
        if (keyword) {
            filters.push(`(contains(tolower(Code), '${keyword.toLowerCase()}') or contains(tolower(Name), '${keyword.toLowerCase()}'))`);
        }
        if (filterStatus !== 'all') {
            filters.push(`Status eq '${filterStatus}'`);
        }
        if (filters.length > 0) {
            odataQuery += `&$filter=${filters.join(' and ')}`;
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
                $('#voucherTableBody').html(`<tr><td colspan="7" class="text-center text-danger py-4">Lỗi tải dữ liệu.</td></tr>`);
            },
            () => {
                $('#voucherTableBody').html(`<tr><td colspan="7" class="text-center py-4 text-muted"><i class="fas fa-spinner fa-spin me-2"></i> Đang tải dữ liệu...</td></tr>`);
            }
        );
    },

    renderTable: function (vouchers) {
        let html = '';
        if (vouchers.length === 0) {
            html = `<tr><td colspan="7" class="text-center py-4 text-muted">Không tìm thấy mã giảm giá nào!</td></tr>`;
        } else {
            vouchers.forEach(v => {
                let statusHtml = v.Status === 'Active'
                    ? `<span class="badge bg-success">Đang hoạt động</span>`
                    : `<span class="badge bg-danger">Đã khóa</span>`;

                let actionButtons = `<span class="text-muted small">Không có quyền</span>`;
                if (this.currentRole === 'Admin' || this.currentRole === 'Staff') {
                    let lockBtn = v.Status === 'Active'
                        ? `<button class="btn btn-sm btn-outline-danger me-1" title="Khóa voucher" onclick="VoucherManager.toggleStatus(${v.RecordId}, 'Active')"><i class="fas fa-lock"></i></button>`
                        : `<button class="btn btn-sm btn-outline-success me-1" title="Mở khóa voucher" onclick="VoucherManager.toggleStatus(${v.RecordId}, 'Disabled')"><i class="fas fa-unlock"></i></button>`;

                    let editBtn = `<button class="btn btn-sm btn-outline-primary" title="Chỉnh sửa voucher" onclick="VoucherManager.openOffcanvas('edit', ${v.RecordId})"><i class="fas fa-edit"></i></button>`;

                    actionButtons = lockBtn + editBtn;
                }

                let discountStr = v.DiscountType === 'Percent'
                    ? `${v.DiscountValue}%`
                    : `${v.DiscountValue.toLocaleString('vi-VN')} VNĐ`;

                let maxDiscountStr = v.MaxDiscountAmount
                    ? `<br><small class="text-muted">Tối đa: ${v.MaxDiscountAmount.toLocaleString('vi-VN')} đ</small>`
                    : '';

                let minOrderStr = v.MinOrderAmount
                    ? `<br><small class="text-muted">Đơn tối thiểu: ${v.MinOrderAmount.toLocaleString('vi-VN')} đ</small>`
                    : '';

                let startStr = v.StartAt ? new Date(v.StartAt).toLocaleString('vi-VN') : '';
                let endStr = v.EndAt ? new Date(v.EndAt).toLocaleString('vi-VN') : '';

                html += `
                    <tr>
                        <td>
                            <span class="badge bg-dark fw-bold px-2 py-1 fs-6">${v.Code}</span>
                        </td>
                        <td>
                            <strong>${v.Name}</strong>
                            ${minOrderStr}
                        </td>
                        <td class="text-center text-primary fw-bold">
                            ${discountStr}
                            ${maxDiscountStr}
                        </td>
                        <td class="text-center">
                            ${v.UsedCount} / ${v.UsageLimit}
                        </td>
                        <td class="text-center small">
                            <div><i class="fas fa-play text-success"></i> ${startStr}</div>
                            <div><i class="fas fa-stop text-danger"></i> ${endStr}</div>
                        </td>
                        <td class="text-center">${statusHtml}</td>
                        <td class="text-center">${actionButtons}</td>
                    </tr>
                `;
            });
        }
        $('#voucherTableBody').html(html);
    },

    toggleStatus: function (id, currentStatus) {
        let newStatus = currentStatus === 'Active' ? 'Disabled' : 'Active';
        let actionName = currentStatus === 'Active' ? 'Khóa' : 'Mở khóa';

        AdminHelper.confirmAction(
            `${actionName} mã giảm giá?`,
            currentStatus === 'Active' ? `Khách hàng sẽ không thể sử dụng mã giảm giá này nữa!` : `Mã giảm giá sẽ được mở khóa để sử dụng!`,
            () => {
                AdminHelper.ajaxRequest(
                    this.apiUrl + `api/admin/vouchers/${id}/status`,
                    'PUT',
                    null,
                    this.token,
                    (res) => {
                        if (res.isSuccess) {
                            AdminHelper.showSuccess(res.message);
                            this.loadVouchers();
                        } else {
                            AdminHelper.showError(res.message);
                        }
                    }
                );
            }
        );
    },

    openOffcanvas: function (mode, id = 0) {
        // Reset form
        $('#voucherForm')[0].reset();
        $('#voucherForm').removeClass('was-validated');
        this.toggleDiscountTypeUI();

        if (mode === 'create') {
            $('#offcanvasVoucherLabel').text('Tạo Voucher Mới');
            $('#VoucherId').val(0);
            $('#Code').prop('disabled', false); // Allow editing Code
            this.offcanvasInstance.show();
        } else {
            $('#offcanvasVoucherLabel').text('Chỉnh Sửa Voucher');
            $('#VoucherId').val(id);
            $('#Code').prop('disabled', true); // Do not allow editing Code

            // Load voucher detail
            AdminHelper.ajaxRequest(
                this.apiUrl + `api/admin/vouchers/${id}`,
                'GET',
                null,
                this.token,
                (res) => {
                    if (res && res.isSuccess && res.data) {
                        let v = res.data;
                        $('#Code').val(v.Code);
                        $('#Name').val(v.Name);
                        $('#DiscountType').val(v.DiscountType);
                        this.toggleDiscountTypeUI();
                        $('#DiscountValue').val(v.DiscountValue);
                        $('#MaxDiscountAmount').val(v.MaxDiscountAmount || '');
                        $('#MinOrderAmount').val(v.MinOrderAmount || '');
                        $('#UsageLimit').val(v.UsageLimit);
                        $('#StartAt').val(v.StartAt ? v.StartAt.substring(0, 16) : '');
                        $('#EndAt').val(v.EndAt ? v.EndAt.substring(0, 16) : '');
                        $('#IsActive').prop('checked', v.Status === 'Active');

                        this.offcanvasInstance.show();
                    } else {
                        AdminHelper.showError(res?.message || 'Không thể tải dữ liệu');
                    }
                }
            );
        }
    },

    toggleDiscountTypeUI: function () {
        let type = $('#DiscountType').val();
        if (type === 'Percent') {
            $('#DiscountValueAddon').text('%');
            $('#MaxDiscountAmountWrapper').show();
        } else {
            $('#DiscountValueAddon').text('VNĐ');
            $('#MaxDiscountAmountWrapper').hide();
            $('#MaxDiscountAmount').val(''); // Clear when hidden
        }
    },

    saveVoucher: function () {
        let form = $('#voucherForm')[0];
        if (!form.checkValidity()) {
            form.reportValidity();
            return;
        }

        let id = parseInt($('#VoucherId').val());
        let data = {
            Code: $('#Code').val().trim().toUpperCase(),
            Name: $('#Name').val().trim(),
            DiscountType: $('#DiscountType').val(),
            DiscountValue: parseFloat($('#DiscountValue').val() || 0),
            MaxDiscountAmount: $('#MaxDiscountAmount').val() ? parseFloat($('#MaxDiscountAmount').val()) : null,
            MinOrderAmount: $('#MinOrderAmount').val() ? parseFloat($('#MinOrderAmount').val()) : null,
            UsageLimit: parseInt($('#UsageLimit').val() || 0),
            StartAt: $('#StartAt').val(),
            EndAt: $('#EndAt').val(),
            IsActive: $('#IsActive').is(':checked')
        };

        if (data.StartAt >= data.EndAt) {
            AdminHelper.showError('Ngày kết thúc phải lớn hơn ngày bắt đầu!');
            return;
        }

        if (data.DiscountType === 'Percent' && data.DiscountValue > 100) {
            AdminHelper.showError('Giảm theo phần trăm không được vượt quá 100%!');
            return;
        }

        let url = this.apiUrl + 'api/admin/vouchers';
        let method = 'POST';

        if (id > 0) {
            url += `/${id}`;
            method = 'PUT';
        }

        $('#btnSaveVoucher').html('<i class="fas fa-spinner fa-spin"></i> ĐANG LƯU...').prop('disabled', true);

        AdminHelper.ajaxRequest(
            url,
            method,
            data,
            this.token,
            (res) => {
                $('#btnSaveVoucher').html('<i class="fas fa-save me-1"></i> LƯU VOUCHER').prop('disabled', false);
                if (res.isSuccess) {
                    AdminHelper.showSuccess(res.message, () => {
                        this.offcanvasInstance.hide();
                        this.loadVouchers();
                    });
                } else {
                    AdminHelper.showError(res.message);
                }
            },
            (xhr) => {
                $('#btnSaveVoucher').html('<i class="fas fa-save me-1"></i> LƯU VOUCHER').prop('disabled', false);
                let err = AdminHelper.getErrorMessage(xhr);
                AdminHelper.showError(err);
            }
        );
    },

    renderPagination: function (totalRecords) {
        let totalPages = Math.ceil(totalRecords / this.pageSize);
        let paginationHtml = '';
        let startRecord = totalRecords === 0 ? 0 : ((this.currentPage - 1) * this.pageSize) + 1;
        let endRecord = (this.currentPage * this.pageSize) > totalRecords ? totalRecords : (this.currentPage * this.pageSize);
        $('#paginationInfo').text(`Hiển thị ${startRecord} - ${endRecord} trên tổng ${totalRecords} mã giảm giá`);

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
