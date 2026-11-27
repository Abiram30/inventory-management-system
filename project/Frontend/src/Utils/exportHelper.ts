// File: Frontend/src/utils/exportHelper.ts
import { toast } from 'sonner';

export const exportToCSV = (data: any[], filename: string) => {
  if (!data || data.length === 0) {
    toast.error('No data available to export');
    return;
  }

  const keys = Object.keys(data[0]);
  const csvContent = [
    keys.join(','),
    ...data.map(row => 
      keys.map(key => {
        let val = row[key];
        if (typeof val === 'object' && val !== null) val = JSON.stringify(val);
        return `"${String(val ?? '').replace(/"/g, '""')}"`;
      }).join(',')
    )
  ].join('\n');

  const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
  const url = URL.createObjectURL(blob);
  const link = document.createElement('a');
  link.setAttribute('href', url);
  link.setAttribute('download', `${filename}_${new Date().toISOString().slice(0, 10)}.csv`);
  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);
  toast.success('CSV exported successfully!');
};