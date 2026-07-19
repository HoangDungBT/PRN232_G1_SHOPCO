const DashboardManager = {
    apiUrl: '',
    token: '',
    revenueChartInstance: null,

    init: function (apiBaseUrl, jwtToken) {
        this.apiUrl = apiBaseUrl;
        this.token = jwtToken;

        this.loadChartData(7);
    },

    loadChartData: function (days) {
        let url = this.apiUrl.endsWith('/') ? `${this.apiUrl}api/admin/dashboard/revenue-chart?days=${days}` : `${this.apiUrl}/api/admin/dashboard/revenue-chart?days=${days}`;

        AdminHelper.ajaxRequest(
            url,
            'GET',
            null,
            this.token,
            (res) => {
                if (res.isSuccess) {
                    let data = res.data;
                    let labels = data.map(item => item.date);
                    let revenueValues = data.map(item => item.revenue);
                    this.renderChart(labels, revenueValues);
                }
            },
            (xhr) => {
                console.error("Lỗi lấy dữ liệu biểu đồ:", AdminHelper.getErrorMessage(xhr));
            }
        );
    },

    renderChart: function (labels, data) {
        const ctx = document.getElementById('revenueChart').getContext('2d');

        if (this.revenueChartInstance != null) {
            this.revenueChartInstance.destroy();
        }

        let gradient = ctx.createLinearGradient(0, 0, 0, 400);
        gradient.addColorStop(0, 'rgba(13, 110, 253, 0.5)');
        gradient.addColorStop(1, 'rgba(13, 110, 253, 0.0)');

        this.revenueChartInstance = new Chart(ctx, {
            type: 'line',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Doanh thu (VNĐ)',
                    data: data,
                    borderColor: '#0d6efd',
                    backgroundColor: gradient,
                    borderWidth: 2,
                    pointBackgroundColor: '#fff',
                    pointBorderColor: '#0d6efd',
                    pointRadius: 4,
                    fill: true,
                    tension: 0.3
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: { display: false },
                    tooltip: {
                        callbacks: {
                            label: function(context) {
                                let label = context.dataset.label || '';
                                if (label) {
                                    label += ': ';
                                }
                                if (context.parsed.y !== null) {
                                    label += new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(context.parsed.y);
                                }
                                return label;
                            }
                        }
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: {
                            callback: function(value) {
                                if (value >= 1000000) return (value / 1000000) + ' Tr';
                                if (value >= 1000) return (value / 1000) + ' K';
                                return value;
                            }
                        }
                    },
                    x: {
                        grid: { display: false }
                    }
                }
            }
        });
    }
};
