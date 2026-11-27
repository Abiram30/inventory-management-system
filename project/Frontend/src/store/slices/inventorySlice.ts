import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import type { ProductDto } from '../../types/ims';

const API_BASE_URL = 'http://localhost:5200';

// Async thunk to fetch products
export const fetchProducts = createAsyncThunk(
  'inventory/fetchProducts',
  async () => {
    const response = await fetch(`${API_BASE_URL}/api/Products`);
    if (!response.ok) throw new Error('Failed to fetch products');
    return (await response.json()) as ProductDto[];
  }
);

// Async thunk to fetch custom report endpoints dynamically by route
export const fetchReportData = createAsyncThunk(
  'inventory/fetchReportData',
  async (reportRoute: string) => {
    const response = await fetch(`${API_BASE_URL}${reportRoute}`);
    if (!response.ok) throw new Error('Failed to fetch report data');
    return (await response.json()) as any[];
  }
);

interface InventoryState {
  items: any[]; // Can hold products or dynamic report rows
  status: 'idle' | 'loading' | 'succeeded' | 'failed';
  error: string | null;
}

const initialState: InventoryState = {
  items: [],
  status: 'idle',
  error: null,
};

export const inventorySlice = createSlice({
  name: 'inventory',
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      // Handle Products Fetch
      .addCase(fetchProducts.pending, (state) => {
        state.status = 'loading';
      })
      .addCase(fetchProducts.fulfilled, (state, action) => {
        state.status = 'succeeded';
        state.items = action.payload;
      })
      .addCase(fetchProducts.rejected, (state, action) => {
        state.status = 'failed';
        state.error = action.error.message || 'Something went wrong';
      })
      // Handle Reports Fetch
      .addCase(fetchReportData.pending, (state) => {
        state.status = 'loading';
      })
      .addCase(fetchReportData.fulfilled, (state, action) => {
        state.status = 'succeeded';
        state.items = action.payload;
      })
      .addCase(fetchReportData.rejected, (state, action) => {
        state.status = 'failed';
        state.error = action.error.message || 'Failed to load report';
      });
  },
});

export default inventorySlice.reducer;