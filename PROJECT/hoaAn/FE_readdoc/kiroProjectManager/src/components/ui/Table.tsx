import React from 'react';
import { LoadingSkeleton } from './LoadingSkeleton';

export interface Column<T> {
  key: string;
  header: string;
  render: (item: T) => React.ReactNode;
  className?: string;
}

interface TableProps<T> {
  data: T[];
  columns: Column<T>[];
  onRowClick?: (item: T) => void;
  isLoading?: boolean;
  keyExtractor?: (item: T, index: number) => string | number;
  className?: string;
  emptyMessage?: string;
}

export function Table<T>({
  data,
  columns,
  onRowClick,
  isLoading = false,
  keyExtractor = (_item: T, index: number) => index,
  className = '',
  emptyMessage = 'No data available',
}: TableProps<T>) {
  // Loading state with skeletons
  if (isLoading) {
    return (
      <div className={`w-full ${className}`}>
        {/* Desktop loading skeleton */}
        <div className="hidden sm:block overflow-x-auto">
          <table className="min-w-full divide-y divide-gray-200">
            <thead className="bg-gray-50">
              <tr>
                {columns.map((column) => (
                  <th
                    key={column.key}
                    className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider"
                  >
                    {column.header}
                  </th>
                ))}
              </tr>
            </thead>
            <tbody className="bg-white divide-y divide-gray-200">
              {Array.from({ length: 5 }).map((_, index) => (
                <tr key={index}>
                  {columns.map((column) => (
                    <td key={column.key} className="px-6 py-4 whitespace-nowrap">
                      <LoadingSkeleton variant="text" />
                    </td>
                  ))}
                </tr>
              ))}
            </tbody>
          </table>
        </div>

        {/* Mobile loading skeleton */}
        <div className="sm:hidden space-y-4">
          {Array.from({ length: 3 }).map((_, index) => (
            <div
              key={index}
              className="bg-white rounded-lg shadow p-4 border border-gray-200"
            >
              <LoadingSkeleton variant="text" count={columns.length} />
            </div>
          ))}
        </div>
      </div>
    );
  }

  // Empty state
  if (data.length === 0) {
    return (
      <div className={`w-full ${className}`}>
        <div className="text-center py-12 bg-white rounded-lg border border-gray-200">
          <p className="text-gray-500">{emptyMessage}</p>
        </div>
      </div>
    );
  }

  return (
    <div className={`w-full ${className}`}>
      {/* Desktop table view */}
      <div className="hidden sm:block overflow-x-auto rounded-lg border border-gray-200">
        <table className="min-w-full divide-y divide-gray-200">
          <thead className="bg-gray-50">
            <tr>
              {columns.map((column) => (
                <th
                  key={column.key}
                  className={`px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider ${
                    column.className || ''
                  }`}
                >
                  {column.header}
                </th>
              ))}
            </tr>
          </thead>
          <tbody className="bg-white divide-y divide-gray-200">
            {data.map((item, index) => (
              <tr
                key={keyExtractor(item, index)}
                onClick={() => onRowClick?.(item)}
                className={`${
                  onRowClick
                    ? 'cursor-pointer hover:bg-gray-50:bg-gray-800 transition-colors'
                    : ''
                }`}
              >
                {columns.map((column) => (
                  <td
                    key={column.key}
                    className={`px-6 py-4 whitespace-nowrap text-sm text-gray-900 ${
                      column.className || ''
                    }`}
                  >
                    {column.render(item)}
                  </td>
                ))}
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {/* Mobile card layout fallback */}
      <div className="sm:hidden space-y-4">
        {data.map((item, index) => (
          <div
            key={keyExtractor(item, index)}
            onClick={() => onRowClick?.(item)}
            className={`bg-white rounded-lg shadow p-4 border border-gray-200 ${
              onRowClick
                ? 'cursor-pointer hover:shadow-md transition-shadow'
                : ''
            }`}
          >
            {columns.map((column) => (
              <div key={column.key} className="mb-3 last:mb-0">
                <div className="text-xs font-medium text-gray-500 uppercase tracking-wider mb-1">
                  {column.header}
                </div>
                <div className="text-sm text-gray-900">
                  {column.render(item)}
                </div>
              </div>
            ))}
          </div>
        ))}
      </div>
    </div>
  );
}
