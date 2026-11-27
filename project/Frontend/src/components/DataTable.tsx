import React from 'react';
import { useAppSelector } from '../store/hooks'; 

interface DataTableProps {
  title: string;
  data: Record<string, any>[];
  onDelete: (id: string | number) => void;
}

export const DataTable: React.FC<DataTableProps> = ({ title, data, onDelete }) => {
  const userRole = useAppSelector((state) => state.auth?.userRole || 'Admin');

  const keys = data.length > 0 ? Object.keys(data[0]) : [];

  // Helper to extract clean text from nested relational objects instead of dumping raw JSON
  const formatCellValue = (val: any): string => {
    if (val === null || val === undefined) return '';
    if (Array.isArray(val)) {
      return `${val.length} items`;
    }
    if (typeof val === 'object') {
      // If the nested object has a friendly identifier, pull it out
      if (val.name) return val.name;
      if (val.sku) return val.sku;
      if (val.location) return val.location;
      if (val.title) return val.title;
      // Fallback if it's a generic object without those keys
      return '[Object]';
    }
    return String(val);
  };

  return (
    <div className="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden">
      <div className="px-6 py-4 bg-gray-50 border-b border-gray-200 font-bold text-gray-700">
        {title} Database Table
      </div>
      <div className="overflow-x-auto">
        <table className="w-full text-left border-collapse">
          <thead>
            <tr className="bg-gray-50/70 border-b border-gray-200 text-xs font-semibold text-gray-500 uppercase tracking-wider">
              {keys.length > 0 ? (
                keys.map(k => <th key={k} className="px-6 py-3 uppercase">{k}</th>)
              ) : (
                <th className="px-6 py-3">Status</th>
              )}
              {/* Sticky Actions Header so it never disappears on scroll */}
              <th className="px-6 py-3 text-right sticky right-0 bg-gray-100 shadow-sm z-10">Actions</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-100 text-sm">
            {data.length === 0 ? (
              <tr>
                <td colSpan={(keys.length || 1) + 1} className="px-6 py-6 text-center text-gray-400">
                  No records found in database table.
                </td>
              </tr>
            ) : (
              data.map((row, idx) => (
                <tr key={row.id || idx} className="hover:bg-gray-50/50">
                  {keys.map(k => {
                    const formattedVal = formatCellValue(row[k]);
                    return (
                      <td key={k} className="px-6 py-4 text-gray-700 whitespace-nowrap">
                        {formattedVal}
                      </td>
                    );
                  })}
                  
                  {/* Sticky Actions Column locked to the right side */}
                  <td className="px-6 py-4 text-right whitespace-nowrap sticky right-0 bg-white shadow-sm z-10">
                    {userRole === 'Admin' ? (
                      <button
                        onClick={() => onDelete(row.id)}
                        className="bg-red-50 text-red-600 px-3 py-1 rounded-lg text-xs font-semibold hover:bg-red-100 transition-colors shadow-sm"
                      >
                        Delete
                      </button>
                    ) : (
                      <span className="text-gray-400 text-xs italic bg-gray-50 px-2 py-1 rounded">
                        Read-Only
                      </span>
                    )}
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
};