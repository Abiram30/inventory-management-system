CREATE DATABASE IF NOT EXISTS inventory_db;
USE inventory_db;

-- 1. Users Table (Authentication & Role Management)
CREATE TABLE users (
    id INT AUTO_INCREMENT PRIMARY KEY,
    username VARCHAR(100) NOT NULL,
    email VARCHAR(200) NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    role VARCHAR(50) NOT NULL -- Admin, Warehouse Operator, Purchasing Coordinator
);

-- 2. Warehouses Table (Physical Locations)
CREATE TABLE warehouses (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(150) NOT NULL,
    location VARCHAR(250) NOT NULL
);

-- 3. Products Table (Item Catalog)
CREATE TABLE products (
    id INT AUTO_INCREMENT PRIMARY KEY,
    sku VARCHAR(100) NOT NULL UNIQUE,
    name VARCHAR(200) NOT NULL,
    category VARCHAR(100) NOT NULL
);

-- 4. Stock Table (Live Inventory Quantities - Bridge between Warehouse & Product)
CREATE TABLE stock (
    id INT AUTO_INCREMENT PRIMARY KEY,
    warehouse_id INT NOT NULL,
    product_id INT NOT NULL,
    quantity INT NOT NULL DEFAULT 0,
    row_version TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (warehouse_id) REFERENCES warehouses(id) ON DELETE CASCADE,
    FOREIGN KEY (product_id) REFERENCES products(id) ON DELETE CASCADE,
    UNIQUE KEY uq_warehouse_product (warehouse_id, product_id)
);

-- 5. Stock Adjustments Table (The Core Transactional Funnel for Inflows/Outflows/Sales)
CREATE TABLE stock_adjustments (
    id INT AUTO_INCREMENT PRIMARY KEY,
    product_id INT NOT NULL,
    warehouse_id INT NOT NULL,
    type VARCHAR(20) NOT NULL, -- Inflow / Outflow
    quantity INT NOT NULL,
    reason VARCHAR(500),
    timestamp DATETIME DEFAULT CURRENT_TIMESTAMP,
    user_id INT NOT NULL,
    FOREIGN KEY (product_id) REFERENCES products(id),
    FOREIGN KEY (warehouse_id) REFERENCES warehouses(id),
    FOREIGN KEY (user_id) REFERENCES users(id)
);

-- 6. Purchase Orders Table (Supplier Orders - Buying Flow)
CREATE TABLE purchase_orders (
    id INT AUTO_INCREMENT PRIMARY KEY,
    supplier_name VARCHAR(150) NOT NULL,
    status VARCHAR(50) NOT NULL, -- Pending, Approved, Received
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- 7. Purchase Order Lines Table (Order Line Items)
CREATE TABLE purchase_order_lines (
    id INT AUTO_INCREMENT PRIMARY KEY,
    purchase_order_id INT NOT NULL,
    product_id INT NOT NULL,
    quantity_ordered INT NOT NULL,
    quantity_received INT NOT NULL DEFAULT 0,
    unit_price DECIMAL(18,2) NOT NULL,
    FOREIGN KEY (purchase_order_id) REFERENCES purchase_orders(id) ON DELETE CASCADE,
    FOREIGN KEY (product_id) REFERENCES products(id)
);

-- 8. System Settings Table (Global Configurations)
CREATE TABLE system_settings (
    id INT AUTO_INCREMENT PRIMARY KEY,
    setting_key VARCHAR(100) NOT NULL UNIQUE,
    setting_value VARCHAR(500) NOT NULL,
    updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- 9. Audit Logs Table (Administrative Tracking)
CREATE TABLE audit_logs (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
    action VARCHAR(100) NOT NULL,
    details VARCHAR(1000),
    timestamp DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES users(id)
);

USE inventory_db;

-- =========================================================
-- 1. USERS
-- =========================================================
INSERT INTO users (username, email, password_hash, role) VALUES
('arjun.mehta', 'arjun.mehta@invsys.com', '$2b$12$KIXqf3f9zQd8wZ1nB2mYeO7yV5rT1pL9sHc3XgQwR4tE6uY8iA2Km', 'Admin'),
('priya.nair', 'priya.nair@invsys.com', '$2b$12$8dRt2nQ7wXz1pL3mB9cYeO5vT6rK4sHc1XgQwR2tE9uY7iA3Km5Jq', 'Warehouse Operator'),
('rahul.singh', 'rahul.singh@invsys.com', '$2b$12$3fWq8nZ2wYz5pL7mB1cXeO9vT4rK6sHc2XgQwR8tE3uY1iA5Km7Lp', 'Warehouse Operator'),
('sneha.kapoor', 'sneha.kapoor@invsys.com', '$2b$12$6hUq1nX4wYz8pL2mB5cZeO3vT7rK9sHc4XgQwR6tE1uY9iA8Km2Nr', 'Purchasing Coordinator'),
('vikram.rao', 'vikram.rao@invsys.com', '$2b$12$9jTq5nV3wYz2pL9mB8cWeO1vT2rK7sHc6XgQwR4tE7uY3iA1Km9Ps', 'Purchasing Coordinator');

-- =========================================================
-- 2. WAREHOUSES
-- =========================================================
INSERT INTO warehouses (name, location) VALUES
('Delhi Darkstore 12', 'Sector 18, Noida, Uttar Pradesh'),
('Mumbai Fulfilment Center', 'Bhiwandi, Thane, Maharashtra'),
('Bangalore Micro-Warehouse', 'Whitefield, Bengaluru, Karnataka'),
('Chennai Regional Hub', 'Ambattur Industrial Estate, Chennai, Tamil Nadu');

-- =========================================================
-- 3. PRODUCTS
-- =========================================================
INSERT INTO products (sku, name, category) VALUES
('EL-PB-001', 'Mi 10000mAh 3i Power Bank (Metallic Black)', 'Power Banks'),
('EL-CB-002', 'boAt Type-C to Type-C 65W Fast Charging Cable 1.5m', 'Cables'),
('EL-AU-003', 'boAt BassHeads 100 Wired Earphones with Mic', 'Audio'),
('EL-CH-004', 'Apple 20W USB-C Power Adapter', 'Chargers'),
('EL-BT-005', 'Duracell Ultra Alkaline AA Batteries (Pack of 4)', 'Batteries'),
('EL-LD-006', 'Wipro 9W B22 LED Cool Day Light Bulb', 'Lighting'),
('EL-MS-007', 'Zebronics Zeb-Compliance USB Optical Mouse', 'Computer Accessories'),
('EL-AU-008', 'Boult Audio AirBass True Wireless Earbuds', 'Audio');

-- =========================================================
-- 4. STOCK  (warehouse_id x product_id, must be unique pairs)
-- =========================================================
INSERT INTO stock (warehouse_id, product_id, quantity) VALUES
(1, 1, 42), (1, 2, 115), (1, 3, 80), (1, 4, 28),
(2, 1, 30), (2, 5, 220), (2, 6, 140), (2, 7, 55),
(3, 2, 90), (3, 3, 60), (3, 8, 35), (3, 4, 12),
(4, 5, 185), (4, 6, 75), (4, 7, 40), (4, 8, 18);

-- =========================================================
-- 5. STOCK ADJUSTMENTS  (Inflow/Outflow ledger)
-- =========================================================
INSERT INTO stock_adjustments (product_id, warehouse_id, type, quantity, reason, user_id) VALUES
(1, 1, 'Inflow', 50, 'Initial stock load from supplier GRN #2201', 4),
(1, 1, 'Outflow', 8, 'Customer order fulfilment - batch #DS12-889', 2),
(2, 1, 'Inflow', 150, 'Purchase order PO-1001 received', 4),
(2, 1, 'Outflow', 35, 'Customer order fulfilment - batch #DS12-891', 2),
(3, 1, 'Outflow', 5, 'Damaged in transit, written off', 3),
(4, 1, 'Inflow', 30, 'Purchase order PO-1002 received', 5),
(5, 2, 'Inflow', 250, 'Bulk restock ahead of festive season', 4),
(5, 2, 'Outflow', 30, 'Customer order fulfilment - batch #MUM-102', 3),
(6, 2, 'Outflow', 20, 'Store transfer to Chennai hub', 2),
(8, 3, 'Inflow', 40, 'New SKU onboarding stock', 5);

-- =========================================================
-- 6. PURCHASE ORDERS
-- =========================================================
INSERT INTO purchase_orders (supplier_name, status, created_at) VALUES
('Xiaomi India Distributors Pvt Ltd', 'Received', '2026-08-01 10:15:00'),
('Imagine Marketing Ltd (boAt)', 'Received', '2026-08-05 14:30:00'),
('Apple India Authorized Distributor', 'Approved', '2026-08-20 09:00:00'),
('Duracell India Pvt Ltd', 'Pending', '2026-09-10 11:45:00'),
('Wipro Consumer Care & Lighting', 'Approved', '2026-09-12 16:20:00');

-- =========================================================
-- 7. PURCHASE ORDER LINES
-- =========================================================
INSERT INTO purchase_order_lines (purchase_order_id, product_id, quantity_ordered, quantity_received, unit_price) VALUES
(1, 1, 50, 50, 999.00),
(2, 2, 150, 150, 179.00),
(2, 3, 100, 100, 249.00),
(3, 4, 40, 0, 1450.00),
(4, 5, 300, 0, 95.00),
(5, 6, 200, 180, 89.00),
(5, 7, 60, 60, 199.00);

-- =========================================================
-- 8. SYSTEM SETTINGS
-- =========================================================
INSERT INTO system_settings (setting_key, setting_value) VALUES
('low_stock_threshold', '20'),
('default_currency', 'INR'),
('auto_reorder_enabled', 'true'),
('order_fulfilment_sla_hours', '2'),
('warehouse_default_timezone', 'Asia/Kolkata');

-- =========================================================


USE inventory_db;

-- 1. Create Live Inventory & Low-Stock View
CREATE OR REPLACE VIEW v_live_inventory AS
SELECT 
    s.warehouse_id,
    w.name AS warehouse_name,
    s.product_id,
    p.sku,
    p.name AS product_name,
    p.category,
    s.quantity,
    CAST(ss.setting_value AS UNSIGNED) AS low_stock_threshold,
    CASE 
        WHEN s.quantity <= CAST(ss.setting_value AS UNSIGNED) THEN 'CRITICAL / LOW' 
        ELSE 'HEALTHY' 
    END AS stock_status
FROM stock s
JOIN warehouses w ON s.warehouse_id = w.id
JOIN products p ON s.product_id = p.id
CROSS JOIN system_settings ss 
WHERE ss.setting_key = 'low_stock_threshold';

-- 2. Create Outflow Fulfillment View
CREATE OR REPLACE VIEW v_po_fulfillment AS
SELECT 
    po.id AS po_id,
    po.supplier_name,
    po.status AS po_status,
    p.id AS product_id,
    p.sku,
    pol.quantity_ordered,
    pol.quantity_received,
    (pol.quantity_ordered - pol.quantity_received) AS pending_quantity,
    ROUND((pol.quantity_received * 100.0 / NULLIF(pol.quantity_ordered, 0)), 1) AS fulfillment_pct
FROM purchase_orders po
JOIN purchase_order_lines pol ON po.id = pol.purchase_order_id
JOIN products p ON pol.product_id = p.id;

-- 3. Create Activity & Movement Ledger View
CREATE OR REPLACE VIEW v_movement_ledger AS
SELECT 
    sa.id,
    sa.warehouse_id,
    w.name AS warehouse_name,
    sa.product_id,
    p.sku,
    sa.type AS movement_type,
    sa.quantity,
    sa.reason,
    sa.user_id,
    u.username,
    sa.timestamp
FROM stock_adjustments sa
JOIN warehouses w ON sa.warehouse_id = w.id
JOIN products p ON sa.product_id = p.id
JOIN users u ON sa.user_id = u.id;

-- ==========================================================
-- QUICK FILTER TEST EXAMPLES (Run individual SELECTs)
-- ==========================================================

-- Filter Report 1: Low stock items in Warehouse 1
-- SELECT * FROM v_live_inventory WHERE warehouse_id = 1 AND stock_status = 'CRITICAL / LOW';

-- Filter Report 2: PO fulfillment for specific SKU
-- SELECT * FROM v_po_fulfillment WHERE sku = 'EL-PB-001';

-- Filter Report 3: Inflows only for Warehouse 1
-- SELECT * FROM v_movement_ledger WHERE warehouse_id = 1 AND movement_type = 'Inflow';