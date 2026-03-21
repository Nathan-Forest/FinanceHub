// API Base URL
const API_URL = '/api';

// State
let categories = [];
let transactions = [];

// Initialize app when page loads
document.addEventListener('DOMContentLoaded', async () => {
    console.log('🚀 FinanceHub Dashboard Initialized!');
    
    // Set today's date as default
    document.getElementById('date').valueAsDate = new Date();
    
    // Load initial data
    await loadCategories();
    await loadSummary();
    await loadCategoryBreakdown();
    await loadRecentTransactions();
    await loadCharts();  
    
    // Setup form submission
    document.getElementById('transaction-form').addEventListener('submit', handleAddTransaction);
});
// ============================================
// LOAD DATA FROM API
// ============================================

async function loadCategories() {
    try {
        const response = await fetch(`${API_URL}/categories`);
        categories = await response.json();
        
        // Populate category dropdown
        const categorySelect = document.getElementById('category');
        categorySelect.innerHTML = categories.map(cat => 
            `<option value="${cat.id}">${cat.icon} ${cat.name}</option>`
        ).join('');
        
        console.log('✅ Categories loaded:', categories.length);
    } catch (error) {
        console.error('❌ Error loading categories:', error);
    }
}

async function loadSummary() {
    try {
        const response = await fetch(`${API_URL}/transactions/summary`);
        const summary = await response.json();
        
        // Update summary cards
        document.getElementById('total-income').textContent = formatCurrency(summary.totalIncome);
        document.getElementById('total-expenses').textContent = formatCurrency(summary.totalExpenses);
        document.getElementById('net-balance').textContent = formatCurrency(summary.netBalance);
        document.getElementById('transaction-count').textContent = summary.transactionCount;
        
        console.log('✅ Summary loaded:', summary);
    } catch (error) {
        console.error('❌ Error loading summary:', error);
    }
}

async function loadCategoryBreakdown() {
    try {
        const response = await fetch(`${API_URL}/transactions/by-category`);
        const data = await response.json();
        
        const container = document.getElementById('category-breakdown');
        
        if (data.length === 0) {
            container.innerHTML = '<p class="empty-state">No transactions yet</p>';
            return;
        }
        
        container.innerHTML = data.map(cat => `
            <div class="category-bar">
                <div class="category-header">
                    <div class="category-info">
                        <span>${cat.icon}</span>
                        <span class="category-name">${cat.category}</span>
                        <span class="category-amount">${formatCurrency(cat.totalAmount)}</span>
                    </div>
                    <span class="category-percentage">${cat.percentage.toFixed(1)}%</span>
                </div>
                <div class="bar-container">
                    <div class="bar-fill" style="width: ${cat.percentage}%; background-color: ${cat.color};"></div>
                </div>
            </div>
        `).join('');
        
        console.log('✅ Category breakdown loaded');
    } catch (error) {
        console.error('❌ Error loading category breakdown:', error);
    }
}

async function loadRecentTransactions() {
    try {
        const response = await fetch(`${API_URL}/transactions/recent?count=10`);
        transactions = await response.json();
        
        const container = document.getElementById('transactions-list');
        
        if (transactions.length === 0) {
            container.innerHTML = '<p class="empty-state">No transactions yet. Add your first one above!</p>';
            return;
        }
        
        container.innerHTML = transactions.map(t => `
            <div class="transaction-card">
                <div class="transaction-icon">${t.category?.icon || '📦'}</div>
                <div class="transaction-details">
                    <h4>${t.description}</h4>
                    <p class="transaction-category">${t.category?.name || 'Other'}</p>
                    <p class="transaction-date">${formatDate(t.date)}</p>
                </div>
                <div class="transaction-amount ${t.type}">
                    ${t.type === 'income' ? '+' : '-'}${formatCurrency(t.amount)}
                </div>
            </div>
        `).join('');
        
        console.log('✅ Recent transactions loaded:', transactions.length);
    } catch (error) {
        console.error('❌ Error loading transactions:', error);
    }
}

// ============================================
// ADD TRANSACTION
// ============================================

async function handleAddTransaction(e) {
    e.preventDefault();
    
    const transaction = {
        type: document.getElementById('type').value,
        amount: parseFloat(document.getElementById('amount').value),
        categoryId: parseInt(document.getElementById('category').value),
        description: document.getElementById('description').value,
        date: document.getElementById('date').value
    };
    
    try {
        const response = await fetch(`${API_URL}/transactions`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(transaction)
        });
        
        if (response.ok) {
            console.log('✅ Transaction added successfully!');
            
            // Clear form
            document.getElementById('transaction-form').reset();
            document.getElementById('date').valueAsDate = new Date();
            
            // Reload all data
            await loadSummary();
            await loadCategoryBreakdown();
            await loadRecentTransactions();
            await loadCharts();
            
            // Show success message (optional)
            alert('Transaction added successfully! 🎉');
        } else {
            console.error('❌ Error adding transaction');
            alert('Failed to add transaction. Please try again.');
        }
    } catch (error) {
        console.error('❌ Error:', error);
        alert('Failed to add transaction. Please try again.');
    }
}

// ============================================
// UTILITY FUNCTIONS
// ============================================

function formatCurrency(amount) {
    return new Intl.NumberFormat('en-US', {
        style: 'currency',
        currency: 'USD'
    }).format(amount);
}

function formatDate(dateString) {
    const date = new Date(dateString);
    return new Intl.DateTimeFormat('en-US', {
        month: 'short',
        day: 'numeric',
        year: 'numeric'
    }).format(date);
}

// ============================================
// CHARTS
// ============================================

let pieChart = null;
let lineChart = null;

async function loadCharts() {
    await loadPieChart();
    await loadLineChart();
}

async function loadPieChart() {
    try {
        const response = await fetch(`${API_URL}/transactions/by-category`);
        const data = await response.json();
        
        // Filter out categories with no spending
        const chartData = data.filter(cat => cat.totalAmount > 0);
        
        if (chartData.length === 0) {
            console.log('⚠️ No data for pie chart');
            return;
        }
        
        const ctx = document.getElementById('pieChart').getContext('2d');
        
        // Destroy existing chart if it exists
        if (pieChart) {
            pieChart.destroy();
        }
        
        pieChart = new Chart(ctx, {
            type: 'doughnut',
            data: {
                labels: chartData.map(cat => cat.category),
                datasets: [{
                    label: 'Spending by Category',
                    data: chartData.map(cat => cat.totalAmount),
                    backgroundColor: chartData.map(cat => cat.color),
                    borderWidth: 0
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: true,
                plugins: {
                    legend: {
                        position: 'bottom',
                        labels: {
                            padding: 15,
                            font: {
                                size: 12
                            }
                        }
                    },
                    tooltip: {
                        callbacks: {
                            label: function(context) {
                                const label = context.label || '';
                                const value = formatCurrency(context.parsed);
                                const percentage = context.dataset.data
                                    .reduce((a, b) => a + b, 0);
                                const percent = ((context.parsed / percentage) * 100).toFixed(1);
                                return `${label}: ${value} (${percent}%)`;
                            }
                        }
                    }
                }
            }
        });
        
        console.log('✅ Pie chart loaded');
    } catch (error) {
        console.error('❌ Error loading pie chart:', error);
    }
}

async function loadLineChart() {
    try {
        const response = await fetch(`${API_URL}/transactions/monthly`);
        const data = await response.json();
        
        if (data.length === 0) {
            console.log('⚠️ No data for line chart');
            return;
        }
        
        // Sort by date (oldest first for timeline)
        data.sort((a, b) => {
            if (a.year !== b.year) return a.year - b.year;
            return a.month - b.month;
        });
        
        const ctx = document.getElementById('lineChart').getContext('2d');
        
        // Destroy existing chart if it exists
        if (lineChart) {
            lineChart.destroy();
        }
        
        lineChart = new Chart(ctx, {
            type: 'line',
            data: {
                labels: data.map(d => `${getMonthName(d.month)} ${d.year}`),
                datasets: [
                    {
                        label: 'Income',
                        data: data.map(d => d.income),
                        borderColor: '#10b981',
                        backgroundColor: 'rgba(16, 185, 129, 0.1)',
                        tension: 0.4,
                        fill: true
                    },
                    {
                        label: 'Expenses',
                        data: data.map(d => d.expenses),
                        borderColor: '#ef4444',
                        backgroundColor: 'rgba(239, 68, 68, 0.1)',
                        tension: 0.4,
                        fill: true
                    }
                ]
            },
            options: {
                responsive: true,
                maintainAspectRatio: true,
                interaction: {
                    intersect: false,
                    mode: 'index'
                },
                plugins: {
                    legend: {
                        position: 'bottom'
                    },
                    tooltip: {
                        callbacks: {
                            label: function(context) {
                                return `${context.dataset.label}: ${formatCurrency(context.parsed.y)}`;
                            }
                        }
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: {
                            callback: function(value) {
                                return '$' + value.toLocaleString();
                            }
                        }
                    }
                }
            }
        });
        
        console.log('✅ Line chart loaded');
    } catch (error) {
        console.error('❌ Error loading line chart:', error);
    }
}

function getMonthName(month) {
    const months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 
                    'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
    return months[month - 1];
}