export interface ProductDto {
  id: number;
  name: string;
  sku: string;
  price: number;
  category?: string;
  quantity?: number;
}

export type InventoryItem = ProductDto;

export interface NavigationModule {
  key: string;
  name: string;
  route: string;
}

export const MODULES: readonly NavigationModule[] = [
  { key: 'products', name: 'Products', route: '/api/Products' },
  { key: 'warehouses', name: 'Warehouses', route: '/api/Warehouses' },
  { key: 'purchaseOrders', name: 'Purchase Orders', route: '/api/PurchaseOrders' },
  { key: 'stock', name: 'Stock Levels', route: '/api/Stock' },
  { key: 'stockAdjustments', name: 'Stock Adjustments', route: '/api/StockAdjustments' },
];

export type ModuleKey = typeof MODULES[number]['key'];

export interface Warehouse {
  id: number;
  name: string;
  location: string;
}

export interface PurchaseOrderLine {
  id: number;
  purchaseOrderId: number;
  productId: number;
  quantity: number;
  unitPrice: number;
}

export interface PurchaseOrder {
  id: number;
  supplierName?: string;
  orderDate: string;
  status: string;
  purchaseOrderLines?: PurchaseOrderLine[];
}

export interface Stock {
  id: number;
  warehouseId: number;
  productId: number;
  quantity: number;
  warehouse?: Warehouse;
  product?: ProductDto;
}

export interface StockAdjustment {
  id: number;
  warehouseId: number | string;
  productId: number | string;
  quantity: number | string;
  type: string;
  timestamp: string;
  userId?: number;
}