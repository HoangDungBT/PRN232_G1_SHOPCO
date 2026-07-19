const InteractLogManager = {
    apiUrl: '',
    token: '',
    currentPage: 1,
    pageSize: 15,
    logsData: [],
    logDetailModal: null,

    init: function (apiBaseUrl, jwtToken) {
        this.apiUrl = apiBaseUrl;
        this.token = jwtToken;
        this.logDetailModal = new bootstrap.Modal(document.getElementById('logDetailModal'));

        this.bindEvents();
        this.loadLogs();
    },

    bindEvents: function () {
        $('#btnSearch').click(() => { this.resetPageAndLoad(); });
        $('#searchInput').keypress((e) => { if (e.which == 13) this.resetPageAndLoad(); });
        $('#filterLogType').change(() => { this.resetPageAndLoad(); });
    },

    resetPageAndLoad: function () {
        this.currentPage = 1;
        this.loadLogs();
    },

    loadLogs: function () {
        let skip = (this.currentPage - 1) * this.pageSize;
        let odataQuery = `odata/AdminLogsOData?$count=true&$top=${this.pageSize}&$skip=${skip}&$orderby=CreatedAt desc`;
        let filters = [];

        let keyword = $('#searchInput').val().trim();
        if (keyword) {
            filters.push(`(contains(tolower(Message), '${keyword.toLowerCase()}') or contains(tolower(UserName), '${keyword.toLowerCase()}'))`);
        }

        let type = $('#filterLogType').val();
        if (type !== 'all') {
            filters.push(`LogType eq '${type}'`);
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
                    this.logsData = res.value;
                    this.renderTable(res.value);
                    this.renderPagination(res['@odata.count']);
                }
            },
            () => {
                $('#logTableBody').html(`<tr><td colspan="6" class="text-center text-danger py-4">Lỗi tải dữ liệu.</td></tr>`);
            },
            () => {
                $('#logTableBody').html(`<tr><td colspan="6" class="text-center py-4 text-muted"><i class="fas fa-spinner fa-spin me-2"></i> Đang tải dữ liệu...</td></tr>`);
            }
        );
    },

    renderTable: function (logs) {
        let html = '';
        if (logs.length === 0) {
            html = `<tr><td colspan="6" class="text-center py-4 text-muted">Không có dữ liệu log!</td></tr>`;
        } else {
            logs.forEach((log, index) => {
                let dateStr = log.CreatedAt ? new Date(log.CreatedAt).toLocaleString('vi-VN') : '';

                let typeBadge = '';
                switch (log.LogType) {
                    case 'StockAlert': typeBadge = `<span class="badge bg-danger">Cảnh Báo Kho</span>`; break;
                    case 'StockMovement': typeBadge = `<span class="badge bg-primary">Chuyển Kho</span>`; break;
                    case 'Audit': typeBadge = `<span class="badge bg-dark">Hệ Thống</span>`; break;
                    case 'Chatbot': typeBadge = `<span class="badge bg-info text-dark">Chatbot</span>`; break;
                    default: typeBadge = `<span class="badge bg-secondary">${log.LogType}</span>`; break;
                }

                let senderIcon = log.SenderType === 'System' ? '<i class="fas fa-server text-muted"></i>'
                    : (log.SenderType === 'Bot' ? '<i class="fas fa-robot text-info"></i>' : '<i class="fas fa-user text-primary"></i>');
                
                let displayName = log.UserName;
                if (!displayName) {
                    if (log.SenderType === 'System') displayName = 'Hệ Thống';
                    else if (log.SenderType === 'Bot') displayName = 'Chatbot';
                    else displayName = 'Khách Vãng Lai';
                }

                let messageHtml = log.Title ? `<strong>${log.Title}</strong><br><span class="text-muted small">${log.Message}</span>` : `<span class="text-dark small">${log.Message}</span>`;

                let hasJson = log.OldValueJson || log.NewValueJson || log.PayloadJson;
                let btnHtml = hasJson ? `<button class="btn btn-sm btn-light border" onclick="InteractLogManager.viewDetail(${index})"><i class="fas fa-code"></i></button>` : '';

                html += `
                    <tr>
                        <td class="text-muted">#${log.LogId}</td>
                        <td class="small">${dateStr}</td>
                        <td>${typeBadge}</td>
                        <td class="small">${senderIcon} ${displayName}</td>
                        <td>${messageHtml}</td>
                        <td class="text-center">${btnHtml}</td>
                    </tr>
                `;
            });
        }
        $('#logTableBody').html(html);
    },

    viewDetail: function (index) {
        let log = this.logsData[index];

        let oldJson = log.OldValueJson || log.PayloadJson || 'Không có dữ liệu';
        let newJson = log.NewValueJson || 'Không có dữ liệu';

        try { if (oldJson !== 'Không có dữ liệu') oldJson = JSON.stringify(JSON.parse(oldJson), null, 4); } catch (e) { }
        try { if (newJson !== 'Không có dữ liệu') newJson = JSON.stringify(JSON.parse(newJson), null, 4); } catch (e) { }

        $('#jsonOldValue').text(oldJson);
        $('#jsonNewValue').text(newJson);

        this.logDetailModal.show();
    },

    renderPagination: function (totalRecords) {
        let totalPages = Math.ceil(totalRecords / this.pageSize);
        let html = '';
        for (let i = 1; i <= totalPages; i++) {
            html += `<li class="page-item ${this.currentPage === i ? 'active' : ''}"><a class="page-link" href="#" onclick="InteractLogManager.currentPage=${i}; InteractLogManager.loadLogs(); return false;">${i}</a></li>`;
        }
        $('#paginationUl').html(html);
        $('#paginationInfo').text(`Tổng: ${totalRecords} bản ghi`);
    }
};
