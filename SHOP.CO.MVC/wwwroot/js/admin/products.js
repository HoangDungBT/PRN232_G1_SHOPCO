const ProductManager = {
    apiUrl: '',
    token: '',
    currentPage: 1,
    pageSize: 10,
    currentMode: 'create',
    currentProductData: {},
    sortColumn: 'ProductId',
    sortDirection: 'desc',
    selectedIds: [],
    formAttributes: { sizes: [], colors: [] },
    productOffcanvas: null,

    init: function (apiBaseUrl, jwtToken) {
        this.apiUrl = apiBaseUrl;
        this.token = jwtToken;
        this.productOffcanvas = new bootstrap.Offcanvas(document.getElementById('offcanvasProduct'));

        this.bindEvents();
        this.loadFormAttributes();
        this.loadProducts();
    },

    bindEvents: function () {
        $('#btnSearch').click(() => { this.resetPageAndLoad(); });
        $('#searchInput').keypress((e) => { if (e.which == 13) this.resetPageAndLoad(); });

        $('#chkSelectAll').change((e) => {
            $('.chk-item').prop('checked', $(e.target).prop('checked'));
            this.updateSelectedIds();
        });

        $(document).on('click', '.page-link', (e) => {
            e.preventDefault();
            this.currentPage = parseInt($(e.target).data('page'));
            this.loadProducts();
        });

        $(document).on('input', '#ProductName', () => {
            $('.variant-row').not('.is-existing').each((index, element) => {
                let rowId = $(element).attr('id').replace('row_', '');
                this.autoGenerateSku(rowId);
            });
        });
    },

    loadFormAttributes: function () {
        AdminHelper.ajaxRequest(
            this.apiUrl + 'api/admin/products/attributes',
            'GET',
            null,
            this.token,
            (res) => {
                if (res.isSuccess) {
                    let data = res.data;

                    let catHtml = '<option value="">-- Vui lòng chọn danh mục --</option>';
                    data.categories.forEach(c => { catHtml += `<option value="${c.id}">${c.name}</option>`; });
                    $('#CategoryId').html(catHtml);

                    let brandHtml = '';
                    data.brands.forEach(b => { brandHtml += `<option value="${b}">`; });
                    $('#brandList').html(brandHtml);

                    let materialHtml = '';
                    data.materials.forEach(m => { materialHtml += `<option value="${m}">`; });
                    $('#materialList').html(materialHtml);

                    let sizeHtml = '';
                    data.sizes.forEach(s => { sizeHtml += `<option value="${s}">`; });
                    $('#sizeList').html(sizeHtml);

                    let colorHtml = '';
                    data.colors.forEach(c => { colorHtml += `<option value="${c}">`; });
                    $('#colorList').html(colorHtml);

                    this.formAttributes.sizes = data.sizes;
                    this.formAttributes.colors = data.colors;
                }
            },
            () => console.error("Không tải được dữ liệu dropdown")
        );
    },

    resetPageAndLoad: function () {
        this.currentPage = 1;
        this.loadProducts();
    },

    changeSort: function (column) {
        if (this.sortColumn === column) {
            this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
        } else {
            this.sortColumn = column;
            this.sortDirection = 'asc';
        }

        $('.sort-icon').removeClass('fa-sort-up fa-sort-down text-primary').addClass('fa-sort text-muted');
        $(`#sort-${column}`).removeClass('fa-sort text-muted')
            .addClass(this.sortDirection === 'asc' ? 'fa-sort-up text-primary' : 'fa-sort-down text-primary');

        this.loadProducts();
    },

    loadProducts: function () {
        let skip = (this.currentPage - 1) * this.pageSize;
        let odataQuery = `odata/AdminProductsOData?$count=true&$top=${this.pageSize}&$skip=${skip}&$orderby=${this.sortColumn} ${this.sortDirection}`;
        let filters = [];

        let keyword = $('#searchInput').val().trim();
        if (keyword) filters.push(`contains(tolower(ProductName), '${keyword.toLowerCase()}')`);

        let status = $('#filterStatus').val();
        if (status === 'active') filters.push(`IsActive eq true`);
        else if (status === 'inactive') filters.push(`IsActive eq false`);

        let stock = $('#filterStock').val();
        if (stock === 'lowstock') filters.push(`HasLowStock eq true`);

        if (filters.length > 0) odataQuery += `&$filter=${filters.join(' and ')}`;

        $('#chkSelectAll').prop('checked', false);
        this.selectedIds = [];
        this.toggleBulkActions();

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
                $('#productTableBody').html(`<tr><td colspan="7" class="text-danger text-center py-4">Lỗi tải dữ liệu bảng.</td></tr>`);
            },
            () => {
                $('#productTableBody').html(`<tr><td colspan="7" class="text-center py-4 text-muted"><i class="fas fa-spinner fa-spin me-2"></i> Đang tải dữ liệu...</td></tr>`);
            }
        );
    },

    renderTable: function (products) {
        let html = '';
        if (products.length === 0) {
            html = `<tr><td colspan="7" class="text-center py-4 text-muted">Không có dữ liệu!</td></tr>`;
        } else {
            products.forEach(p => {
                let statusHtml = p.IsActive ? `<span class="badge bg-success">Đang mở bán</span>` : `<span class="badge bg-secondary">Dừng bán</span>`;
                let price = p.BasePrice.toLocaleString('vi-VN') + ' đ';
                let warningIcon = p.HasLowStock ? `<i class="fas fa-exclamation-triangle text-warning ms-2" title="Sắp hết hàng"></i>` : '';

                let imgHtml = p.ThumbnailUrl
                    ? `<img src="${this.apiUrl}${p.ThumbnailUrl}" style="width: 50px; height: 50px; object-fit: cover;" class="rounded border" />`
                    : `<div class="bg-light rounded border d-flex align-items-center justify-content-center" style="width: 50px; height: 50px;"><i class="fas fa-image text-muted"></i></div>`;

                html += `
                    <tr>
                        <td class="text-center align-middle">
                            <input class="form-check-input chk-item" type="checkbox" value="${p.ProductId}" onchange="ProductManager.updateSelectedIds()">
                        </td>
                        <td class="text-center">${imgHtml}</td>
                        <td>
                            <strong>${p.ProductName}</strong> ${warningIcon}<br>
                            <small class="text-muted">Mã ID: #${p.ProductId}</small>
                        </td>
                        <td>${p.CategoryName || 'N/A'}</td>
                        <td class="text-danger fw-bold">${price}</td>
                        <td>${statusHtml}</td>
                        <td class="text-center">
                            <button class="btn btn-sm btn-outline-info me-1" title="Sửa" onclick="ProductManager.openOffcanvas('edit', ${p.ProductId})"><i class="fas fa-edit"></i></button>
                            <button class="btn btn-sm btn-outline-danger" title="Xóa" onclick="ProductManager.deleteProduct(${p.ProductId})"><i class="fas fa-trash"></i></button>
                        </td>
                    </tr>
                `;
            });
        }
        $('#productTableBody').html(html);
    },

    updateSelectedIds: function () {
        this.selectedIds = [];
        $('.chk-item:checked').each((i, el) => {
            this.selectedIds.push(parseInt($(el).val()));
        });
        this.toggleBulkActions();
    },

    toggleBulkActions: function () {
        $('#selectedCount').text(this.selectedIds.length);
        if (this.selectedIds.length > 0) {
            $('#bulkActions').removeClass('d-none').addClass('d-flex');
        } else {
            $('#bulkActions').addClass('d-none').removeClass('d-flex');
        }
    },

    bulkUpdateStatus: function (isActive) {
        let actionText = isActive ? 'Mở bán' : 'Dừng bán';
        AdminHelper.confirmAction(
            `Xác nhận ${actionText}?`,
            `Áp dụng cho ${this.selectedIds.length} sản phẩm đã chọn.`,
            () => {
                AdminHelper.ajaxRequest(
                    this.apiUrl + 'api/admin/products/bulk/status',
                    'PUT',
                    { productIds: this.selectedIds, isActive: isActive },
                    this.token,
                    (res) => {
                        AdminHelper.showSuccess(res.message);
                        this.loadProducts();
                    }
                );
            }
        );
    },

    bulkUpdateFeatured: function (isFeatured) {
        AdminHelper.ajaxRequest(
            this.apiUrl + 'api/admin/products/bulk/featured',
            'PUT',
            { productIds: this.selectedIds, isFeatured: isFeatured },
            this.token,
            (res) => {
                AdminHelper.showSuccess(res.message);
                this.loadProducts();
            }
        );
    },

    renderPagination: function (totalRecords) {
        let totalPages = Math.ceil(totalRecords / this.pageSize);
        let html = '';
        let startRecord = totalRecords === 0 ? 0 : ((this.currentPage - 1) * this.pageSize) + 1;
        let endRecord = (this.currentPage * this.pageSize) > totalRecords ? totalRecords : (this.currentPage * this.pageSize);
        $('#paginationInfo').text(`Hiển thị ${startRecord} - ${endRecord} trên tổng ${totalRecords} sản phẩm`);

        for (let i = 1; i <= totalPages; i++) {
            html += `<li class="page-item ${this.currentPage === i ? 'active' : ''}"><a class="page-link" href="#" data-page="${i}">${i}</a></li>`;
        }
        $('#paginationUl').html(html);
    },

    openOffcanvas: function (mode, id = 0) {
        this.currentMode = mode;
        $('#productForm')[0].reset();
        $('#variantContainer').empty();
        $('#imagePreviewContainer').empty();
        $('#variantsSection').show();

        $('#CategoryId')[0].setCustomValidity('');
        $('#ProductName')[0].setCustomValidity('');
        $('#BasePrice')[0].setCustomValidity('');

        if (mode === 'create') {
            $('#offcanvasProductLabel').text('THÊM SẢN PHẨM MỚI');
            this.addVariantRow();
            this.productOffcanvas.show();
        } else if (mode === 'edit') {
            $('#offcanvasProductLabel').text('SỬA SẢN PHẨM #' + id);

            AdminHelper.ajaxRequest(
                this.apiUrl + `api/admin/products/${id}`,
                'GET',
                null,
                this.token,
                (res) => {
                    if (res.isSuccess) {
                        let p = res.data;
                        this.currentProductData = p;

                        $('#ProductId').val(p.productId);
                        $('#CategoryId').val(p.categoryId);
                        $('#ProductName').val(p.productName);

                        $('#Brand').val(p.brand);
                        $('#Material').val(p.material);

                        $('#BasePrice').val(p.basePrice);
                        $('#SalePrice').val(p.salePrice);
                        $('#Description').val(p.description);
                        $('#IsActive').prop('checked', p.isActive);
                        $('#IsFeatured').prop('checked', p.isFeatured);

                        if (p.images && p.images.length > 0) {
                            let imgPreviewHtml = '';
                            p.images.forEach(img => {
                                let badge = img.isThumbnail ? '<span class="position-absolute top-0 start-0 badge rounded-0 bg-danger" style="font-size: 0.6rem;">Bìa</span>' : '';
                                imgPreviewHtml += `
                                    <div class="position-relative d-inline-block" id="img-container-${img.imageId}">
                                        <img src="${this.apiUrl}${img.imageUrl}" class="rounded border" style="width: 80px; height: 80px; object-fit: cover;" />
                                        ${badge}
                                        <button type="button" class="btn btn-sm btn-danger position-absolute top-0 end-0 translate-end rounded-circle p-0"
                                                style="width: 22px; height: 22px; line-height: 1;"
                                                onclick="ProductManager.deleteImage(${img.imageId})" title="Xóa ảnh này">
                                           X
                                        </button>
                                    </div>
                                `;
                            });
                            $('#imagePreviewContainer').html(imgPreviewHtml);
                        }

                        if (p.variants && p.variants.length > 0) {
                            p.variants.forEach(v => {
                                this.addVariantRow(v.sku, v.size, v.color, v.extraPrice, v.stockQuantity);
                            });
                        } else {
                            $('#variantContainer').html('<p class="text-muted fst-italic">Sản phẩm này không có biến thể.</p>');
                        }

                        this.productOffcanvas.show();
                    }
                }
            );
        }
    },

    deleteImage: function (imageId) {
        AdminHelper.confirmAction(
            'Xóa ảnh này?',
            'Hành động này sẽ xóa file vật lý trên máy chủ và không thể hoàn tác!',
            () => {
                AdminHelper.ajaxRequest(
                    this.apiUrl + `api/admin/products/images/${imageId}`,
                    'DELETE',
                    null,
                    this.token,
                    (res) => {
                        if (res.isSuccess) {
                            $(`#img-container-${imageId}`).fadeOut(300, function () { $(this).remove(); });
                            AdminHelper.showSuccess('Đã xóa ảnh!');
                            this.loadProducts();
                        } else {
                            AdminHelper.showError(res.message);
                        }
                    }
                );
            }
        );
    },

    addVariantRow: function (sku = '', size = '', color = '', extraPrice = 0, stockQuantity = 0) {
        let rowId = Date.now() + Math.floor(Math.random() * 100);
        let isExisting = (sku !== '') ? 'is-existing' : '';

        let rowHtml = `
            <div class="row g-2 mb-2 p-2 border rounded bg-white variant-row shadow-sm ${isExisting}" id="row_${rowId}">
                <div class="col-12 col-md-3">
                    <label class="small text-muted fw-bold">Mã SKU (*)</label>
                    <input type="text" class="form-control form-control-sm v-sku border-primary" value="${sku}" placeholder="Tự tạo..." required
                           oninvalid="this.setCustomValidity('Vui lòng nhập mã SKU cho biến thể')"
                           oninput="this.setCustomValidity('')">
                </div>
                <div class="col-6 col-md-2">
                    <label class="small text-muted">Kích cỡ</label>
                    <input type="text" class="form-control form-control-sm v-size" list="sizeList" value="${size}" oninput="ProductManager.autoGenerateSku('${rowId}')" placeholder="Chọn/gõ..." autocomplete="off">
                </div>
                <div class="col-6 col-md-2">
                    <label class="small text-muted">Màu sắc</label>
                    <input type="text" class="form-control form-control-sm v-color" list="colorList" value="${color}" oninput="ProductManager.autoGenerateSku('${rowId}')" placeholder="Chọn/gõ..." autocomplete="off">
                </div>
                <div class="col-6 col-md-2">
                    <label class="small text-muted">Giá (+)</label>
                    <input type="number" step="1000" min="0" class="form-control form-control-sm v-extra" value="${extraPrice}" placeholder="VNĐ"
                           oninvalid="this.setCustomValidity('Giá thêm phải phải từ 0 VNĐ và chẵn hàng nghìn')"
                           oninput="this.setCustomValidity('')">
                </div>
                <div class="col-6 col-md-2">
                    <label class="small text-muted fw-bold">Tồn kho (*)</label>
                    <input type="number" class="form-control form-control-sm v-stock border-warning" value="${stockQuantity}" placeholder="SL" required min="0"
                           oninvalid="this.setCustomValidity('Tồn kho phải từ 0 trở lên')"
                           oninput="this.setCustomValidity('')">
                </div>
                <div class="col-12 col-md-1 d-flex align-items-end">
                    <button type="button" class="btn btn-sm btn-outline-danger w-100" onclick="$('#row_${rowId}').remove()" title="Xóa dòng này">
                        <i class="fas fa-times"></i>
                    </button>
                </div>
            </div>
        `;
        $('#variantContainer').append(rowHtml);
    },

    autoGenerateSku: function (rowId) {
        if ($(`#row_${rowId}`).hasClass('is-existing')) return;

        let productName = $('#ProductName').val();
        let size = $(`#row_${rowId} .v-size`).val();
        let color = $(`#row_${rowId} .v-color`).val();

        if (!productName) return;

        let prefix = productName.split(' ')
            .map(word => word.charAt(0))
            .join('')
            .toUpperCase()
            .normalize('NFD').replace(/[\u0300-\u036f]/g, '');

        let colorCode = '';
        if (color) {
            colorCode = '-' + color.split(' ')
                .map(w => w.charAt(0))
                .join('')
                .toUpperCase()
                .normalize('NFD').replace(/[\u0300-\u036f]/g, '');
        }

        let sizeCode = size ? '-' + size.toUpperCase() : '';
        let randomNum = Math.floor(100 + Math.random() * 900);
        let finalSku = `${prefix}${randomNum}${colorCode}${sizeCode}`;

        $(`#row_${rowId} .v-sku`).val(finalSku);
        $(`#row_${rowId} .v-sku`)[0].setCustomValidity('');
    },

    saveProduct: async function () {
        if (!$('#productForm')[0].checkValidity()) {
            $('#productForm')[0].reportValidity();
            return;
        }

        let catId = parseInt($('#CategoryId').val());
        let basePrice = parseFloat($('#BasePrice').val());
        let salePriceInput = $('#SalePrice').val();
        let salePrice = salePriceInput ? parseFloat(salePriceInput) : null;

        $('#btnSaveProduct').html('<i class="fas fa-spinner fa-spin"></i> Đang xử lý...').prop('disabled', true);

        try {
            let uploadedImageUrls = [];
            let files = $('#ProductImages')[0].files;

            if (files.length > 0) {
                let formData = new FormData();
                for (let i = 0; i < files.length; i++) {
                    formData.append("images", files[i]);
                }

                let uploadRes = await $.ajax({
                    url: this.apiUrl + 'api/admin/products/upload-images',
                    type: 'POST',
                    data: formData,
                    processData: false,
                    contentType: false,
                    headers: { 'Authorization': 'Bearer ' + this.token }
                });

                if (uploadRes && uploadRes.isSuccess) {
                    uploadedImageUrls = uploadRes.data;
                } else {
                    AdminHelper.showError(uploadRes.message || 'Upload ảnh thất bại');
                    $('#btnSaveProduct').html('<i class="fas fa-save me-1"></i> LƯU SẢN PHẨM').prop('disabled', false);
                    return;
                }
            }

            let variantsArray = [];
            $('.variant-row').each(function () {
                variantsArray.push({
                    sku: $(this).find('.v-sku').val(),
                    size: $(this).find('.v-size').val() || null,
                    color: $(this).find('.v-color').val() || null,
                    extraPrice: parseFloat($(this).find('.v-extra').val() || 0),
                    stockQuantity: parseInt($(this).find('.v-stock').val() || 0)
                });
            });

            if (variantsArray.length === 0) {
                Swal.fire('Cảnh báo', 'Vui lòng thêm ít nhất 1 dòng phân loại SKU!', 'warning');
                $('#btnSaveProduct').html('<i class="fas fa-save me-1"></i> LƯU SẢN PHẨM').prop('disabled', false);
                return;
            }

            let dataPayload = {
                categoryId: catId,
                productName: $('#ProductName').val(),
                brand: $('#Brand').val(),
                material: $('#Material').val(),
                basePrice: basePrice,
                salePrice: salePrice,
                description: $('#Description').val(),
                isActive: $('#IsActive').is(':checked'),
                isFeatured: $('#IsFeatured').is(':checked'),
                genderTarget: this.currentProductData.genderTarget || "Unisex",
                isBestSeller: this.currentProductData.isBestSeller || false,
                isNewArrival: this.currentProductData.isNewArrival || true,
                imageUrls: uploadedImageUrls,
                variants: variantsArray
            };

            let ajaxUrl = this.apiUrl + 'api/admin/products';
            let ajaxMethod = 'POST';

            if (this.currentMode === 'edit') {
                ajaxMethod = 'PUT';
                ajaxUrl += '/' + $('#ProductId').val();
            }

            AdminHelper.ajaxRequest(
                ajaxUrl,
                ajaxMethod,
                dataPayload,
                this.token,
                (res) => {
                    $('#btnSaveProduct').html('<i class="fas fa-save me-1"></i> LƯU SẢN PHẨM').prop('disabled', false);
                    if (res.isSuccess) {
                        this.productOffcanvas.hide();
                        AdminHelper.showSuccess(res.message);
                        $('#ProductImages').val('');
                        this.loadFormAttributes();
                        this.loadProducts();
                    } else {
                        AdminHelper.showError(res.message);
                    }
                },
                (xhr) => {
                    $('#btnSaveProduct').html('<i class="fas fa-save me-1"></i> LƯU SẢN PHẨM').prop('disabled', false);
                    let errMessage = AdminHelper.getErrorMessage(xhr);
                    Swal.fire('Lỗi Dữ Liệu', errMessage, 'error');
                }
            );

        } catch (error) {
            $('#btnSaveProduct').html('<i class="fas fa-save me-1"></i> LƯU SẢN PHẨM').prop('disabled', false);
            AdminHelper.showError('Quá trình tải ảnh lên gặp sự cố, vui lòng thử lại');
        }
    },

    deleteProduct: function (id) {
        AdminHelper.confirmAction(
            'Khóa sản phẩm?',
            'Sản phẩm sẽ bị chuyển trạng thái Ngừng bán thay vì xóa vĩnh viễn để bảo vệ dữ liệu hóa đơn cũ.',
            () => {
                AdminHelper.ajaxRequest(
                    this.apiUrl + `api/admin/products/${id}`,
                    'DELETE',
                    null,
                    this.token,
                    (res) => {
                        if (res.isSuccess) {
                            AdminHelper.showSuccess(res.message);
                            this.loadProducts();
                        } else {
                            AdminHelper.showError(res.message);
                        }
                    }
                );
            }
        );
    }
};
