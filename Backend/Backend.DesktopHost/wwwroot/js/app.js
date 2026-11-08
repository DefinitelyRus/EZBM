async function loadProducts() {
	const tableBody = getElement("products-table-body", true);
	if (!tableBody) return;

	try {
		const response = await fetch('/api/Products');

		if (!response.ok) {
			throw new Error(`Failed to load products: ${response.statusText}`);
		}

		const products = await response.json();
		tableBody.innerHTML = '';

		products.forEach(product => {
			const row = document.createElement('tr');
			row.innerHTML = `
				<td>${product.id}</td>
				<td>${product.name}</td>
				<td>${product.sku}</td>
				<td>${product.price}</td>
				<td>${product.totalQuantity}</td>
				<td>
					<button type="button" class="button-delete" data-id="${product.id}">Delete</button>
				</td>
			`;
			tableBody.appendChild(row);
		});
	}
	catch (err) { console.error(err); }
}

async function createProduct(product) {
	const resultContainer = document.getElementById('create-product-result');
	
	try {
		const response = await fetch('/api/Products', {
			method: 'POST',
			headers: { 'Content-Type': 'application/json' },
			body: JSON.stringify(product)
		});

		if (!response.ok) {
			const errorText = await response.text();
			resultContainer.textContent = `Error: ${errorText}`;
			return;
		}

		const data = await response.json();
		resultContainer.textContent = `Created product ID ${data.id}`;
		await loadProducts();
	}
	
	catch (err) {
		resultContainer.textContent = `Error: ${err.message}`;
	}
}

async function createProductStock(stockData) {
	const resultContainer = document.getElementById('create-product-stock-result');
	
	try {
		const response = await fetch('/api/ProductStocks/create-stock', {
			method: 'POST',
			headers: { 'Content-Type': 'application/json' },
			body: JSON.stringify(stockData)
		});

		if (!response.ok) {
			const errorText = await response.text();
			resultContainer.textContent = `Error: ${errorText}`;
			return;
		}

		const transaction = await response.json();
		resultContainer.textContent = `Created product ID ${transaction.id}`;
		await loadProducts();
	}
	
	catch (err) {
		resultContainer.textContent = `Error: ${err.message}`;
	}
}

async function loadSettings() {
	const dbPathInput = document.getElementById('settings-db-path');
	const defaultPathInput = document.getElementById('settings-default-db-path');
	if (!dbPathInput || !defaultPathInput) return;

	try {
		const response = await fetch('/api/Settings');
		if (!response.ok) return;

		const data = await response.json();
		dbPathInput.value = data.activeDatabasePath || '';
		defaultPathInput.value = data.defaultDatabasePath || '';
	}
	catch (err) {
		console.error('Failed to load settings:', err);
	}
}

async function updateDatabasePath(path) {
	const resultContainer = document.getElementById('settings-result');
	resultContainer.textContent = 'Saving...';

	try {
		const response = await fetch('/api/Settings/database-path', {
			method: 'PUT',
			headers: { 'Content-Type': 'application/json' },
			body: JSON.stringify({ databasePath: path })
		});

		if (!response.ok) {
			const errorText = await response.text();
			resultContainer.textContent = `Error: ${errorText}`;
			return;
		}

		const data = await response.json();
		resultContainer.textContent = data.message || 'Saved successfully.';
		await loadSettings();
	}
	catch (err) {
		resultContainer.textContent = `Error: ${err.message}`;
	}
}

async function resetDatabasePath() {
	const resultContainer = document.getElementById('settings-result');
	resultContainer.textContent = 'Resetting...';

	try {
		const response = await fetch('/api/Settings/reset-database-path', {
			method: 'POST'
		});

		if (!response.ok) {
			const errorText = await response.text();
			resultContainer.textContent = `Error: ${errorText}`;
			return;
		}

		const data = await response.json();
		resultContainer.textContent = data.message || 'Reset successfully.';
		await loadSettings();
	}
	catch (err) {
		resultContainer.textContent = `Error: ${err.message}`;
	}
}

document.addEventListener('DOMContentLoaded', () => {
	loadProducts();
	loadSettings();
	
	const refreshButton = getElement('refresh-products-button', true);
	if (refreshButton) {
		refreshButton.addEventListener('click', () => loadProducts());
	}

	const createProductForm = document.getElementById('create-product-form');
	if (createProductForm) {
		createProductForm.addEventListener('submit', async (e) => {
			e.preventDefault();
			
			target = "create-product";

			const product = {
				name: getValue("name"),
				sku: getValue("sku"),
				shortDescription: getValue("price"),
				price: parseFloat(getValue("price")),
				cost: parseFloat(getValue("cost")) || 0,
				totalQuantity: parseFloat(getValue("quantity")) || 0,
				isForSale: true
			};

			await createProduct(product);
		});
	}

	const createProductStockForm = document.getElementById('create-product-stock-form');
	if (createProductStockForm) {
		createProductStockForm.addEventListener('submit', async (e) => {
			e.preventDefault();

			target = "create-product-stock";

			const costVal = getValue("cost");
			const expiryVal = getValue("expiry");
			const batchNotesVal = getValue("batch-notes");
			const transNotesVal = getValue("transaction-notes");

			const stockData = {
				productId: parseInt(getValue("id"), 10),
				quantity: parseFloat(getValue("quantity")),
				cost: costVal ? parseFloat(costVal) : null,
				expiryDate: expiryVal ? new Date(expiryVal).toISOString() : null,
				batchNotes: batchNotesVal ? [batchNotesVal] : null,
				notes: transNotesVal ? [transNotesVal] : null
			};

			await createProductStock(stockData);
		});
	}

	const businessSettingsForm = document.getElementById('business-settings-form');
	if (businessSettingsForm) {
		businessSettingsForm.addEventListener('submit', async (e) => {
			e.preventDefault();
			const dbPath = document.getElementById('settings-db-path').value;
			await updateDatabasePath(dbPath);
		});
	}

	const settingsResetButton = document.getElementById('settings-reset-button');
	if (settingsResetButton) {
		settingsResetButton.addEventListener('click', async () => {
			await resetDatabasePath();
		});
	}
});

var target;

function getElement(elementId, useFullId = false) {
	if (!useFullId) {
		if (!target) throw "`target` is null.";
		
		elementId = `${target}-${elementId}`;
	}

	const element = document.getElementById(elementId);

	if (!element) throw `No existing element with ID '${elementId}'.`;

	return element;
}

function getValue(elementId, useFullId = false) {
	return getElement(elementId, useFullId).value;
}