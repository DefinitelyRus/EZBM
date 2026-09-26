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

document.addEventListener('DOMContentLoaded', () => {
	loadProducts();
	
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