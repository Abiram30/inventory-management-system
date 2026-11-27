import type { ProductDto, Warehouse, Stock, StockAdjustment, PurchaseOrder } from '../types/ims';

const BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5200';

async function request<T>(endpoint: string, options?: RequestInit): Promise<T> {
  const res = await fetch(`${BASE_URL}${endpoint}`, {
    headers: { 'Content-Type': 'application/json', ...options?.headers },
    ...options,
  });

  if (!res.ok) {
    const errorText = await res.text();
    throw new Error(`HTTP ${res.status}: ${res.statusText}${errorText ? ` - ${errorText}` : ''}`);
  }

  if (res.status === 204 || res.headers.get('content-length') === '0') {
    return null as unknown as T;
  }

  return res.json();
}

export const api = {
  products: {
    list: () => request<ProductDto[]>('/api/Products'),
    getById: (id: string | number) => request<ProductDto>(`/api/Products/${id}`),
    create: (data: Omit<ProductDto, 'id'>) =>
      request<ProductDto>('/api/Products', { method: 'POST', body: JSON.stringify(data) }),
    delete: (id: string | number) => request<void>(`/api/Products/${id}`, { method: 'DELETE' }),
  },
  warehouses: {
    list: () => request<Warehouse[]>('/api/Warehouses'),
    create: (data: Partial<Warehouse>) =>
      request<Warehouse>('/api/Warehouses', { method: 'POST', body: JSON.stringify(data) }),
    delete: (id: string | number) => request<void>(`/api/Warehouses/${id}`, { method: 'DELETE' }),
  },
  purchaseOrders: {
    list: () => request<PurchaseOrder[]>('/api/PurchaseOrders'),
    getById: (id: string | number) => request<PurchaseOrder>(`/api/PurchaseOrders/${id}`),
    create: (data: Partial<PurchaseOrder>) =>
      request<PurchaseOrder>('/api/PurchaseOrders', { method: 'POST', body: JSON.stringify(data) }),
    delete: (id: string | number) => request<void>(`/api/PurchaseOrders/${id}`, { method: 'DELETE' }),
  },
  stock: {
    levels: () => request<Stock[]>('/api/Stock'),
    getByWarehouse: (warehouseId: string | number) => request<Stock[]>(`/api/Stock/warehouse/${warehouseId}`),
    upsert: (payload: Stock) =>
      request<Stock>('/api/Stock', { method: 'POST', body: JSON.stringify(payload) }),
    adjust: (payload: StockAdjustment) =>
      request<StockAdjustment>('/api/StockAdjustments', { method: 'POST', body: JSON.stringify(payload) }),
    delete: (id: string | number) => request<void>(`/api/Stock/${id}`, { method: 'DELETE' }),
  },
  stockAdjustments: {
    list: () => request<StockAdjustment[]>('/api/StockAdjustments'),
    create: (payload: StockAdjustment) =>
      request<StockAdjustment>('/api/StockAdjustments', { method: 'POST', body: JSON.stringify(payload) }),
    delete: (id: string | number) => request<void>(`/api/StockAdjustments/${id}`, { method: 'DELETE' }),
  },
};