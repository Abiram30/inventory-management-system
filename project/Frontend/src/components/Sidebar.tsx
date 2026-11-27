import React from 'react';
import { MODULES } from '../types/ims';

interface SidebarProps {
  activeRoute: string;
  onSelectModule: (name: string, route: string) => void;
  onLogout: () => void;
}

export const Sidebar: React.FC<SidebarProps> = ({ activeRoute, onSelectModule, onLogout }) => (
  <aside className="w-64 bg-blue-600 text-white flex flex-col shadow-lg">
    <div className="p-5 flex items-center space-x-3 border-b border-blue-500">
      <span className="font-bold tracking-wide text-xs leading-tight">INVENTORY MANAGEMENT</span>
    </div>
    <nav className="flex-1 px-4 py-4 space-y-1 overflow-y-auto">
      {MODULES.map(mod => (
        <button
          key={mod.route}
          onClick={() => onSelectModule(mod.name, mod.route)}
          className={`w-full text-left block px-4 py-3 rounded-lg text-sm font-medium ${
            activeRoute === mod.route ? 'bg-blue-700 text-white' : 'text-blue-100 hover:bg-blue-500/50'
          }`}
        >
          {mod.name}
        </button>
      ))}
    </nav>
    <div className="p-4 border-t border-blue-500">
      <button
        onClick={onLogout}
        className="w-full flex items-center space-x-3 px-4 py-2.5 rounded-lg text-sm font-medium text-blue-100 hover:bg-blue-500/50 hover:text-white transition-colors"
      >
        <span>Logout</span>
      </button>
    </div>
  </aside>
);