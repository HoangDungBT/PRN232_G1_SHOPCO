const InventoryManager = {
    apiUrl: '',
    token: '',
    currentPage: 1,
    pageSize: 10,
    adjustModal: null,

    init: function (apiBaseUrl, jwtToken) {
        this.apiUrl = apiBaseUrl;
        this.token = jwtToken;
        this.adjustModal = new bootstrap.Modal(document.getElementById('adjustModal'));

        this.bindEvents();
        this.loadInventory();
    },

    bindEvents: function () {
        $('#btnSearch').click(() => { this.resetAndLoad(); });
        $('#searchInput').keypress((e) => { if (e.which == 13) this.resetAndLoad(); });
    },

    resetAndLoad: function () {
        this.currentPage = 1;
        this.loadInventory();
    },

    loadInventory: function () {
        let skip = (this.currentPage - 1) * this.pageSize;
        let keyword = $('#searchInput').val() || '';
        let filterQuery = `Product/IsActive eq true`;
        if (keyword) {
            filterQuery += ` and contains(tolower(Product/ProductName), '${keyword.toLowerCase()}')`;
        }

        let url = `${this.apiUrl}odata/AdminInventoryOData?$expand=Product&$filter=${filterQuery}&$orderby=StockQuantity asc&$top=${this.pageSize}&$skip=${skip}&$count=true`;

        AdminHelper.ajaxRequest(
            url,
            'GET',
            null,
            this.token,
            (res) => {
                let html = '';
                let totalCount = res['@odata.count'] || 0;

                if (res.value && res.value.length > 0) {
                    res.value.forEach(v => {
                        let pName = v.Product ? v.Product.ProductName : "N/A";
                        html += `<tr>
                            <td>${pName}</td>
                            <td><code>${v.Sku}</code></td>
                            <td>${v.Color || ''} - ${v.Size || ''}</td>
                            <td id="qCurrent" class="fw-bold ${v.StockQuantity <= (v.LowStockThreshold || 5) ? 'text-danger' : 'text-dark'}">
                                ${v.StockQuantity}
                            </td>
                            <td>
                                <button class="btn btn-sm btn-outline-primary" onclick="InventoryManager.showAdjustModal(${v.VariantId})">Điều chỉnh</button>
                            </td>
                        </tr>`;
                    });
                } else {
                    html = '<tr><td colspan="5" class="text-center">Không tìm thấy sản phẩm!</td></tr>';
                }
                $('#inventoryBody').html(html);
                this.renderPagination(totalCount);
            }
        );
    },

    showAdjustModal: function (id) {
        $('#vId').val(id);
        this.adjustModal.show();
    },

    submitAdjust: function () {
        let data = {
            variantId: $('#vId').val(),
            quantityCurrent: parseInt($('#qCurrent').val()),
            quantityChange: parseInt($('#qChange').val()),
            reason: $('#reason').val()
        };

        AdminHelper.ajaxRequest(
            this.apiUrl + 'api/admin/inventory/adjust',
            'POST',
            data,
            this.token,
            (res) => {
                this.adjustModal.hide();
                AdminHelper.showSuccess('Cập nhật tồn kho thành công');
                this.loadInventory();
            },
            (xhr) => {
                let errorMsg = AdminHelper.getErrorMessage(xhr);
                AdminHelper.showError(errorMsg);
            }
        );
    },

    renderPagination: function (total) {
        let totalPages = Math.ceil(total / this.pageSize);
        let html = '';
        for (let i = 1; i <= totalPages; i++) {
            html += `<li class="page-item ${i === this.currentPage ? 'active' : ''}">
                <a class="page-link" href="#" onclick="InventoryManager.currentPage=${i}; InventoryManager.loadInventory(); return false;">${i}</a>
            </li>`;
        }
        $('#paginationUl').html(html);
    }
};
