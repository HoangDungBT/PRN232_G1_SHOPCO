const CategoryManager = {
    apiUrl: '',
    token: '',
    currentPage: 1,
    pageSize: 10,
    currentMode: 'create',
    categoryOffcanvas: null,

    init: function (apiBaseUrl, jwtToken) {
        this.apiUrl = apiBaseUrl;
        this.token = jwtToken;
        this.categoryOffcanvas = new bootstrap.Offcanvas(document.getElementById('offcanvasCategory'));

        this.bindEvents();
        this.loadCategories();
        this.loadParentCategoriesForDropdown();
    },

    bindEvents: function () {
        $('#btnSearch').click(() => {
            this.currentPage = 1;
            this.loadCategories();
        });

        $('#searchInput').keypress((e) => {
            if (e.which == 13) $('#btnSearch').click();
        });
    },

    // 1. TẢI DANH MỤC CHA (Cho Dropdown)
    loadParentCategoriesForDropdown: function () {
        let query = `odata/AdminCategoriesOData?$filter=IsActive eq true&$orderby=CategoryName asc`;
        AdminHelper.ajaxRequest(
            this.apiUrl + query,
            'GET',
            null,
            this.token,
            (res) => {
                if (res && res.value) {
                    let html = '<option value="">-- Đây là danh mục gốc --</option>';
                    res.value.forEach(c => {
                        html += `<option value="${c.CategoryId}">${c.CategoryName}</option>`;
                    });
                    $('#ParentCategoryId').html(html);
                }
            }
        );
    },

    // 2. LOAD BẢNG ODATA
    loadCategories: function () {
        let skip = (this.currentPage - 1) * this.pageSize;

        let odataQuery = `odata/AdminCategoriesOData?$count=true&$top=${this.pageSize}&$skip=${skip}&$orderby=CategoryId desc`;

        let keyword = $('#searchInput').val().trim();
        if (keyword) odataQuery += `&$filter=contains(tolower(CategoryName), '${keyword.toLowerCase()}')`;

        AdminHelper.ajaxRequest(
            this.apiUrl + odataQuery,
            'GET',
            null,
            this.token,
            (response) => {
                if (response && response.value) {
                    this.renderTable(response.value);
                    this.renderPagination(response['@odata.count']);
                }
            },
            (xhr) => {
                $('#categoryTableBody').html(`<tr><td colspan="6" class="text-danger text-center py-4">Lỗi tải dữ liệu.</td></tr>`);
            },
            () => {
                $('#categoryTableBody').html(`<tr><td colspan="6" class="text-center py-4 text-muted"><i class="fas fa-spinner fa-spin me-2"></i> Đang tải dữ liệu...</td></tr>`);
            }
        );
    },

    renderTable: function (categories) {
        let html = '';
        if (categories.length === 0) {
            html = `<tr><td colspan="6" class="text-center py-4 text-muted">Không có dữ liệu!</td></tr>`;
        } else {
            categories.forEach(c => {
                let statusHtml = c.IsActive ? `<span class="badge bg-success">Hoạt động</span>` : `<span class="badge bg-danger">Đã khóa</span>`;
                let parentName = c.ParentCategoryName
                    ? `<span class="badge bg-secondary">${c.ParentCategoryName}</span>`
                    : `<span class="text-muted fst-italic">Danh mục gốc</span>`;

                let lockBtnClass = c.IsActive ? 'btn-outline-danger' : 'btn-outline-success';
                let lockIcon = c.IsActive ? 'fa-lock' : 'fa-unlock';
                let lockTitle = c.IsActive ? 'Khóa' : 'Mở khóa';

                html += `
                    <tr>
                        <td>#${c.CategoryId}</td>
                        <td><strong>${c.CategoryName}</strong><br><small class="text-muted">/${c.Slug}</small></td>
                        <td>${parentName}</td>
                        <td class="text-center">${c.SortOrder}</td>
                        <td>${statusHtml}</td>
                        <td class="text-center">
                            <button class="btn btn-sm btn-outline-info me-1" title="Sửa" onclick="CategoryManager.openOffcanvas('edit', ${c.CategoryId})">
                                <i class="fas fa-edit"></i>
                            </button>

                             <button class="btn btn-sm ${lockBtnClass}" title="${lockTitle}" onclick="CategoryManager.toggleCategoryStatus(${c.CategoryId})">
                            <i class="fas ${lockIcon}"></i>
                        </button>
                        </td>
                    </tr>
                `;
            });
        }
        $('#categoryTableBody').html(html);
    },

    renderPagination: function (total) {
        let totalPages = Math.ceil(total / this.pageSize);
        let html = '';
        for (let i = 1; i <= totalPages; i++) {
            html += `<li class="page-item ${this.currentPage === i ? 'active' : ''}"><a class="page-link" href="#" onclick="CategoryManager.changePage(${i}); return false;">${i}</a></li>`;
        }
        $('#paginationUl').html(html);
        $('#paginationInfo').text(`Tổng: ${total} danh mục`);
    },

    changePage: function (page) {
        this.currentPage = page;
        this.loadCategories();
    },

    // 3. MỞ OFFCANVAS & ĐIỀN DỮ LIỆU
    openOffcanvas: function (mode, id = 0) {
        this.currentMode = mode;
        $('#categoryForm')[0].reset();
        $('#CategoryName')[0].setCustomValidity('');

        $('#ParentCategoryId option').prop('disabled', false);

        if (mode === 'create') {
            $('#offcanvasCategoryLabel').text('THÊM DANH MỤC MỚI');
            $('#SortOrder').val(0);
            this.categoryOffcanvas.show();
        }
        else if (mode === 'edit') {
            $('#offcanvasCategoryLabel').text('SỬA DANH MỤC #' + id);

            $(`#ParentCategoryId option[value='${id}']`).prop('disabled', true);

            AdminHelper.ajaxRequest(
                this.apiUrl + `api/admin/categories/${id}`,
                'GET',
                null,
                this.token,
                (res) => {
                    if (res.isSuccess) {
                        let c = res.data;

                        $('#CategoryId').val(c.categoryId);
                        $('#ParentCategoryId').val(c.parentCategoryId || '');
                        $('#CategoryName').val(c.categoryName);
                        $('#ImageUrl').val(c.imageUrl);
                        $('#Description').val(c.description);
                        $('#SortOrder').val(c.sortOrder);
                        $('#IsActive').prop('checked', c.isActive);

                        this.categoryOffcanvas.show();
                    }
                }
            );
        }
    },

    // 4. LƯU DỮ LIỆU (POST / PUT)
    saveCategory: function () {
        if (!$('#categoryForm')[0].checkValidity()) {
            $('#categoryForm')[0].reportValidity();
            return;
        }

        let parentId = $('#ParentCategoryId').val();

        let dataPayload = {
            parentCategoryId: parentId ? parseInt(parentId) : null,
            categoryName: $('#CategoryName').val(),
            description: $('#Description').val(),
            imageUrl: $('#ImageUrl').val(),
            sortOrder: parseInt($('#SortOrder').val()) || 0,
            isActive: $('#IsActive').is(':checked')
        };

        let ajaxUrl = this.apiUrl + 'api/admin/categories';
        let ajaxMethod = 'POST';

        if (this.currentMode === 'edit') {
            ajaxMethod = 'PUT';
            ajaxUrl += '/' + $('#CategoryId').val();
        }

        AdminHelper.ajaxRequest(
            ajaxUrl,
            ajaxMethod,
            dataPayload,
            this.token,
            (res) => {
                $('#btnSaveCategory').html('<i class="fas fa-save me-1"></i> LƯU DỮ LIỆU').prop('disabled', false);
                if (res.isSuccess) {
                    this.categoryOffcanvas.hide();
                    AdminHelper.showSuccess(res.message);
                    this.loadCategories();
                    this.loadParentCategoriesForDropdown();
                } else {
                    AdminHelper.showError(res.message);
                }
            },
            (xhr) => {
                $('#btnSaveCategory').html('<i class="fas fa-save me-1"></i> LƯU DỮ LIỆU').prop('disabled', false);
                let err = AdminHelper.getErrorMessage(xhr);
                AdminHelper.showError(err);
            },
            () => {
                $('#btnSaveCategory').html('<i class="fas fa-spinner fa-spin"></i> Đang lưu...');
                $('#btnSaveCategory').prop('disabled', true);
            }
        );
    },

    toggleCategoryStatus: function (id) {
        AdminHelper.confirmAction(
            'Xác nhận thay đổi?',
            'Bạn muốn thay đổi trạng thái hoạt động của danh mục này?',
            () => {
                AdminHelper.ajaxRequest(
                    this.apiUrl + `api/admin/categories/${id}/status`,
                    'PUT',
                    null,
                    this.token,
                    (res) => {
                        if (res.isSuccess) {
                            AdminHelper.showSuccess(res.message);
                            this.loadCategories();
                        } else {
                            AdminHelper.showError(res.message);
                        }
                    }
                );
            }
        );
    }
};
