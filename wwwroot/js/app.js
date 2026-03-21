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