const OrderManager = {
    apiUrl: '',
    token: '',
    currentPage: 1,
    pageSize: 10,
    currentEditingOrderId: 0,
    sortColumn: 'CreatedAt',
    sortDirection: 'desc',
    orderOffcanvas: null,

    init: function (apiBaseUrl, jwtToken) {
        this.apiUrl = apiBaseUrl;
        this.token = jwtToken;
        this.orderOffcanvas = new bootstrap.Offcanvas(document.getElementById('offcanvasOrder'));

        this.bindEvents();
        this.loadOrders();
    },

    bindEvents: function () {
        $('#btnSearch').click(() => { this.resetPageAndLoad(); });
        $('#searchInput').keypress((e) => { if (e.which == 13) this.resetPageAndLoad(); });

        $('#OrderStatusSelect').change((e) => {
            if ($(e.target).val() === 'Canceled') {
                $('#cancelReasonDiv').removeClass('d-none');
            } else {
                $('#cancelReasonDiv').addClass('d-none');
                $('#CancelReasonInput').val('');
            }
        });

        $(document).on('click', '.page-link', (e) => {
            e.preventDefault();
            this.currentPage = parseInt($(e.target).data('page'));
            this.loadOrders();
        });
    },

    resetPageAndLoad: function () {
        this.currentPage = 1;
        this.loadOrders();
    },

    changeSort: function (column) {
        if (this.sortColumn === column) {
            this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
        } else {
            this.sortColumn = column;
            this.sortDirection = 'desc'; // Đơn hàng mặc định nên sort giảm dần
        }

        $('.sort-icon').removeClass('fa-sort-up fa-sort-down text-primary').addClass('fa-sort text-muted');
        $(`#sort-${column}`).removeClass('fa-sort text-muted')
            .addClass(this.sortDirection === 'asc' ? 'fa-sort-up text-primary' : 'fa-sort-down text-primary');

        this.loadOrders();
    },

    loadOrders: function () {
        let skip = (this.currentPage - 1) * this.pageSize;
        let odataQuery = `odata/AdminOrdersOData?$count=true&$top=${this.pageSize}&$skip=${skip}&$orderby=${this.sortColumn} ${this.sortDirection}`;
        let filters = [];

        let keyword = $('#searchInput').val().trim();
        if (keyword) {
            filters.push(`(contains(tolower(OrderCode), '${keyword.toLowerCase()}') or contains(tolower(ReceiverName), '${keyword.toLowerCase()}'))`);
        }

        let status = $('#filterStatus').val();
        if (status === 'processing') {
            filters.push(`OrderStatus ne 'Completed' and OrderStatus ne 'Canceled'`);
        } else if (status === 'closed') {
            filters.push(`(OrderStatus eq 'Completed' or OrderStatus eq 'Canceled')`);
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
                $('#orderTableBody').html(`<tr><td colspan="7" class="text-center text-danger py-4">Lỗi tải dữ liệu.</td></tr>`);
            },
            () => {
                $('#orderTableBody').html(`<tr><td colspan="7" class="text-center py-4 text-muted"><i class="fas fa-spinner fa-spin me-2"></i> Đang tải dữ liệu...</td></tr>`);
            }
        );
    },

    renderTable: function (orders) {
        let html = '';
        if (orders.length === 0) {
            html = `<tr><td colspan="7" class="text-center py-4 text-muted">Không tìm thấy đơn hàng nào!</td></tr>`;
        } else {
            orders.forEach(o => {
                let totalStr = o.TotalAmount.toLocaleString('vi-VN') + ' đ';
                let dateStr = new Date(o.CreatedAt).toLocaleString('vi-VN');

                let paymentBadge = o.PaymentStatus === 'Paid' ? `<span class="badge bg-success">Đã Thanh Toán</span>` : `<span class="badge bg-secondary">Chưa Thanh Toán</span>`;
                let orderBadge = this.getOrderStatusBadge(o.OrderStatus);

                let opacityClass = (o.OrderStatus === 'Completed' || o.OrderStatus === 'Canceled') ? 'opacity-75' : '';

                html += `
                    <tr class="${opacityClass}">
                        <td>#${o.OrderId}</td>
                        <td>
                            <strong>${o.OrderCode}</strong><br>
                            <small class="text-muted">${dateStr}</small>
                        </td>
                        <td>${o.ReceiverName}</td>
                        <td class="text-end fw-bold text-danger">${totalStr}</td>
                        <td class="text-center">${paymentBadge}</td>
                        <td class="text-center">${orderBadge}</td>
                        <td class="text-center">
                            <button class="btn btn-sm btn-primary" onclick="OrderManager.openOrderDetails(${o.OrderId})">
                                <i class="fas fa-eye me-1"></i> Xem Xử Lý
                            </button>
                        </td>
                    </tr>
                `;
            });
        }
        $('#orderTableBody').html(html);
    },

    openOrderDetails: function (orderId) {
        this.currentEditingOrderId = orderId;

        AdminHelper.ajaxRequest(
            this.apiUrl + `api/admin/orders/${orderId}`,
            'GET',
            null,
            this.token,
            (res) => {
                if (res.isSuccess) {
                    let o = res.data;

                    $('#detailOrderId').text(o.orderId);
                    $('#detailOrderCode').text(o.orderCode);
                    $('#detailReceiverName').text(o.receiverName);
                    $('#detailReceiverPhone').text(o.receiverPhone);
                    $('#detailAddress').text(o.shippingAddressText);
                    $('#detailCustomerNote').text(o.customerNote || "Không có");

                    $('#detailSubtotal').text(o.subtotalAmount.toLocaleString('vi-VN'));
                    $('#detailDiscount').text(o.discountAmount.toLocaleString('vi-VN'));
                    $('#detailShippingFee').text(o.shippingFee.toLocaleString('vi-VN'));
                    $('#detailTotal').text(o.totalAmount.toLocaleString('vi-VN'));

                    $('#OrderStatusSelect').val(o.orderStatus).change();

                    if (o.paymentStatus === 'Paid') {
                        $('#detailPaymentBadge').html('<span class="badge bg-success">Đã Thanh Toán</span>');
                        $('#btnMarkPaid').addClass('d-none');
                    } else {
                        $('#detailPaymentBadge').html('<span class="badge bg-secondary">Chưa Thanh Toán</span>');
                        $('#btnMarkPaid').removeClass('d-none');
                    }

                    let itemsHtml = '';
                    o.items.forEach(item => {
                        let imgUrl = item.imageUrl ? this.apiUrl + item.imageUrl : '/images/no-image.png';
                        let variantTxt = `${item.sku}`;
                        if (item.size || item.color) variantTxt += ` (${item.color || ''} - ${item.size || ''})`;

                        itemsHtml += `
                            <tr>
                                <td><img src="${imgUrl}" style="width: 40px; height: 40px; object-fit: cover;" class="rounded border"></td>
                                <td>
                                    <div class="fw-bold">${item.productName}</div>
                                    <div class="small text-muted">${variantTxt}</div>
                                </td>
                                <td class="text-center">x${item.quantity}</td>
                                <td class="text-end">${item.salePrice.toLocaleString('vi-VN')}</td>
                                <td class="text-end fw-bold pe-3">${item.lineTotal.toLocaleString('vi-VN')}</td>
                            </tr>
                        `;
                    });
                    $('#detailItemsBody').html(itemsHtml);

                    this.orderOffcanvas.show();
                } else {
                    AdminHelper.showError(res.message);
                }
            }
        );
    },

    updateOrderStatus: function () {
        let newStatus = $('#OrderStatusSelect').val();
        let cancelReason = $('#CancelReasonInput').val().trim();

        if (newStatus === 'Canceled' && !cancelReason) {
            Swal.fire('Lỗi', 'Vui lòng nhập lý do hủy đơn hàng!', 'warning');
            return;
        }

        AdminHelper.confirmAction(
            'Cập nhật trạng thái?',
            'Xác nhận cập nhật trạng thái đơn hàng?',
            () => {
                AdminHelper.ajaxRequest(
                    this.apiUrl + `api/admin/orders/${this.currentEditingOrderId}/status`,
                    'PUT',
                    { status: newStatus, cancelReason: cancelReason },
                    this.token,
                    (res) => {
                        if (res.isSuccess) {
                            AdminHelper.showSuccess(res.message);
                            this.orderOffcanvas.hide();
                            this.loadOrders();
                        } else {
                            AdminHelper.showError(res.message);
                        }
                    }
                );
            }
        );
    },

    markAsPaid: function () {
        AdminHelper.confirmAction(
            'Xác nhận Đã Thu Tiền?',
            'Bạn xác nhận đã nhận được tiền từ đơn hàng này?',
            () => {
                AdminHelper.ajaxRequest(
                    this.apiUrl + `api/admin/orders/${this.currentEditingOrderId}/pay`,
                    'PUT',
                    null,
                    this.token,
                    (res) => {
                        if (res.isSuccess) {
                            $('#detailPaymentBadge').html('<span class="badge bg-success">Đã Thanh Toán</span>');
                            $('#btnMarkPaid').addClass('d-none');
                            this.loadOrders();
                            AdminHelper.showSuccess('Cập nhật thanh toán thành công');
                        }
                    }
                );
            }
        );
    },

    getOrderStatusBadge: function (status) {
        switch (status) {
            case 'Pending': return '<span class="badge bg-warning text-dark">Chờ Xác Nhận</span>';
            case 'Confirmed': return '<span class="badge bg-info text-dark">Đã Xác Nhận</span>';
            case 'Shipping': return '<span class="badge bg-primary">Đang Giao Hàng</span>';
            case 'Completed': return '<span class="badge bg-success">Hoàn Thành</span>';
            case 'Canceled': return '<span class="badge bg-danger">Đã Hủy</span>';
            default: return `<span class="badge bg-secondary">${status}</span>`;
        }
    },

    renderPagination: function (totalRecords) {
        let totalPages = Math.ceil(totalRecords / this.pageSize);
        let paginationHtml = '';
        let startRecord = totalRecords === 0 ? 0 : ((this.currentPage - 1) * this.pageSize) + 1;
        let endRecord = (this.currentPage * this.pageSize) > totalRecords ? totalRecords : (this.currentPage * this.pageSize);
        $('#paginationInfo').text(`Hiển thị ${startRecord} - ${endRecord} trên tổng ${totalRecords} đơn hàng`);

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
