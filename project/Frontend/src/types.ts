// src/types.ts
export interface ProductDto {
  id: string;
  name: string;
  sku: string;
  price?: number;
}

export interface Warehouse {
  id: string;
  name: string;
  location?: string;
}

export interface Stock {
  id: string;
  productId: string;
  warehouseId: string;
  quantity: number;
}

export interface StockAdjustment {
  productId: string;
  warehouseId: string;
  quantityDelta: number;
}