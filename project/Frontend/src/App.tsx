// File: Frontend/src/App.tsx

import React, { useState, useEffect } from 'react';
import { Sidebar } from './components/Sidebar';
import { DataTable } from './components/DataTable';
import { CreateModal } from './components/CreateModal';
import { useAppSelector, useAppDispatch } from './store/hooks';
import { fetchProducts, fetchReportData } from './store/slices/inventorySlice';
import { loginSuccess, logout } from './store/slices/authSlice';
import { toast } from 'sonner';
import { DashboardCharts } from './components/DashboardCharts';
import { exportToCSV } from './Utils/exportHelper';
import loginLogo from './assets/login.png';


const API_BASE_URL = 'http://localhost:5200';

export default function App() {
  const { isLoggedIn, userRole } = useAppSelector((state) => state.auth);
  const dispatch = useAppDispatch();
  
  const { items: tableData, status: inventoryStatus } = useAppSelector((state) => state.inventory);

  // Local UI-only states
  const [username, setUsername] = useState('Admin_01');
  const [password, setPassword] = useState('hello');
  const [userRoleInput, setUserRoleInput] = useState('Admin');
  const [loginError, setLoginError] = useState('');
  const [metricStatus, setMetricStatus] = useState('Live Connected');
  const [activeRoute, setActiveRoute] = useState('/api/Products');
  const [activeModuleTitle, setActiveModuleTitle] = useState('Products');
  const [searchQuery, setSearchQuery] = useState('');
  const [isModalOpen, setIsModalOpen] = useState(false);
  
  // Modal states for explicit 3-field column schema
  const [sku, setSku] = useState('');
  const [name, setName] = useState('');
  const [category, setCategory] = useState('');
  
  const [activeReportTitle, setActiveReportTitle] = useState<string | null>(null);

  // Fetch initial products when logged in
  useEffect(() => {
    if (isLoggedIn) {
      dispatch(fetchProducts());
      fetchDashboardMetrics();
    }
  }, [isLoggedIn, dispatch]);

  // Robust search filtering logic that handles nested relational objects safely
  const filteredData = (Array.isArray(tableData) ? tableData : []).filter(item => {
    if (!searchQuery) return true;
    const query = searchQuery.toLowerCase();

    return Object.values(item).some(val => {
      if (val === null || val === undefined) return false;
      
      // If it's a nested database object (e.g. product/warehouse), check its keys
      if (typeof val === 'object') {
        return Object.values(val).some(nestedVal => 
          nestedVal !== null && 
          nestedVal !== undefined && 
          String(nestedVal).toLowerCase().includes(query)
        );
      }
      
      // Otherwise check regular text or numbers
      return String(val).toLowerCase().includes(query);
    });
  });

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoginError('');
    const trimmedUser = username.trim();
    const trimmedRole = userRoleInput.trim();

    // Comprehensive Role-to-Username Validation Map
    const expectedRoles: Record<string, string> = {
      "Admin_01": "Admin",
      "Manager_01": "Manager",
      "Coordinator_01": "Coordinator"
    };

    if (expectedRoles[trimmedUser] && expectedRoles[trimmedUser] !== trimmedRole) {
        setLoginError(`The username "${trimmedUser}" does not belong to the "${trimmedRole}" role.`);
        return;
    }
    
    try {
      const res = await fetch(`${API_BASE_URL}/api/Auth/login`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
       body: JSON.stringify({ 
          username: username.trim(), 
          password: password.trim(), 
          role: userRoleInput.trim()
        }),
      });

      if (!res.ok) throw new Error('Invalid credentials or backend login failure.');
     
      const data = await res.json(); 

      dispatch(loginSuccess({
        username: data.username || username,
        userRole: data.role || userRole
      }));
    } catch (err: any) {
      setLoginError(err.message);
    }
  };

  const handleLogout = () => dispatch(logout());

  const handleDeleteRecord = async (id: number | string) => {
    try {
      const res = await fetch(`${API_BASE_URL}${activeRoute}/${id}`, {
        method: 'DELETE',
      });

      if (!res.ok) throw new Error('Failed to delete record from database.');

      toast.success("Record deleted successfully!");

      if (activeRoute === '/api/Products') {
        dispatch(fetchProducts());
      } else {
        dispatch(fetchReportData(activeRoute));
      }
    } catch (err: any) {
      toast.error(err.message || "An error occurred while deleting.");
    }
  };

  const handleSaveRecord = async () => {
    if (!sku.trim() || !name.trim() || !category.trim()) {
      toast.error("Please fill in all fields before saving.");
      return;
    }

    const skuPattern = /^[A-Z]{2}-[A-Z]{2}-\d{3}$/; 
    if (!skuPattern.test(sku)) {
      toast.error("Invalid SKU format. Example format: EL-BP-001");
      return;
    }

    const namePattern = /^[a-zA-Z0-9\s()+-]+$/; 
    if (!namePattern.test(name)) {
      toast.error("Product name contains invalid characters.");
      return;
    }

    const categoryPattern = /^[a-zA-Z\s]+$/;
    if (!categoryPattern.test(category)) {
      toast.error("Category can only contain letters and spaces.");
      return;
    }

    try {
      const res = await fetch(`${API_BASE_URL}${activeRoute}`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ sku, name, category }),
      });

      if (!res.ok) throw new Error('Failed to save record to database.');

      toast.success("Record saved successfully!");
      setIsModalOpen(false);
      setSku('');
      setName('');
      setCategory('');

      if (activeRoute === '/api/Products') {
        dispatch(fetchProducts());
      } else {
        dispatch(fetchReportData(activeRoute));
      }
    } catch (err: any) {
      toast.error(err.message || "An error occurred while saving.");
    }
  };

  const fetchDashboardMetrics = async () => {
    try {
      const res = await fetch(`${API_BASE_URL}/api/Dashboard/metrics`);
      if (res.ok) setMetricStatus('Synced (200 OK)');
    } catch {
      setMetricStatus('Offline');
    }
  };

  const switchModule = (moduleName: string, route: string) => {
    setActiveRoute(route);
    setActiveModuleTitle(moduleName);
    setActiveReportTitle(null);
    if (route === '/api/Products') {
      dispatch(fetchProducts());
    } else {
      dispatch(fetchReportData(route));
    }
  };

  const handleRunReport1 = () => {
    setActiveReportTitle('Low Stock Items in Warehouse 1');
    dispatch(fetchReportData('/api/Reports/low-stock-wh1'));
  };

  const handleRunReport2 = () => {
    setActiveReportTitle('Outflows Only for Warehouse 1');
    dispatch(fetchReportData('/api/Reports/outflows-wh1'));
  };

  const handleRunReport3 = () => {
    setActiveReportTitle('Inflows Only for Warehouse 1');
    dispatch(fetchReportData('/api/Reports/inflows-wh1'));
  };

  // Render Login Screen with Role Selector
  if (!isLoggedIn) {
    return (
      <div className="flex items-center justify-center h-screen bg-blue-900">
        <div className="bg-white p-8 rounded-xl shadow-2xl w-96">
         <div className="text-center mb-6">
            {/* Replace the blue 'IMS' circle div with your image */}
            <img 
              src={loginLogo}
              alt="Inventory Logo" 
              className="w-16 h-16 object-contain mx-auto mb-3" 
            />
            <h2 className="text-2xl font-bold text-gray-800">System Login</h2>
          </div>
          
          <form onSubmit={handleLogin} className="space-y-4">
            
            <div>
              <label className="block text-xs font-semibold text-gray-600 uppercase mb-1">User Role</label>
              <select 
                value={userRoleInput}
                onChange={(e) => setUserRoleInput(e.target.value)}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm bg-white focus:outline-none focus:border-blue-500"
              >
                <option value="Admin">Admin (Full Access)</option>
                <option value="Manager">Manager (View, Reports, Insert)</option>
                <option value="Coordinator">Coordinator (Read-Only / Reports)</option>
              </select>
            </div>
            <div>
              <label className="block text-xs font-semibold text-gray-600 uppercase mb-1">Username</label>
              <input
                type="text"
                value={username}
                onChange={e => setUsername(e.target.value)}
                className="w-full px-4 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:border-blue-500"
              />
            </div>
            <div>
              <label className="block text-xs font-semibold text-gray-600 uppercase mb-1">Password</label>
              <input
                type="password"
                value={password}
                onChange={e => setPassword(e.target.value)}
                className="w-full px-4 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:border-blue-500"
              />
            </div>
            <button
              type="submit"
              className="w-full bg-blue-600 text-white font-semibold py-2.5 rounded-lg hover:bg-blue-700 transition-colors text-sm shadow-md"
            >
              Sign In
            </button>
            {loginError && <p className="text-xs text-red-500 text-center">{loginError}</p>}
          </form>
        </div>
      </div>
    );
  }

  // Render Main Dashboard Layout
  return (
    <div className="flex h-screen overflow-hidden bg-gray-100 text-gray-900">
      <Sidebar activeRoute={activeRoute} onSelectModule={switchModule} onLogout={handleLogout} />

      <div className="flex-1 flex flex-col overflow-hidden">
        <header className="bg-white shadow-sm h-16 flex items-center justify-between px-8 z-10">
          <input
            type="text"
            placeholder="Search records..."
            value={searchQuery}
            onChange={e => setSearchQuery(e.target.value)}
            className="w-96 pl-4 pr-4 py-2 bg-gray-50 border border-gray-200 rounded-lg text-sm focus:outline-none focus:border-blue-500"
          />
          <div className="flex items-center space-x-4">
            <span className="text-xs bg-purple-100 text-purple-800 px-3 py-1 rounded-full font-bold">
              Role: {userRole}
            </span>
            <span className="text-xs bg-blue-100 text-blue-800 px-3 py-1.5 rounded-full font-semibold">
              Authenticated
            </span>
            <span className="text-sm font-semibold text-gray-700">{username}</span>
          </div>
        </header>

        <main className="flex-1 overflow-y-auto p-8 bg-gray-100">
          <div className="max-w-7xl mx-auto space-y-6">
            
            {/* Dashboard 4-Card Grid */}
            <div className="grid grid-cols-1 md:grid-cols-4 gap-6 mb-4">
              <div className="bg-white p-5 rounded-xl shadow-sm border border-gray-100 border-t-4 border-t-blue-500">
                <p className="text-xs font-bold text-gray-400 uppercase tracking-wider">Metrics Status</p>
                <p className="text-xl font-extrabold text-blue-600 mt-1">{metricStatus}</p>
              </div>

              <div 
                onClick={handleRunReport1}
                className="bg-white p-5 rounded-xl shadow-sm border border-gray-100 border-t-4 border-t-amber-500 cursor-pointer hover:shadow-md transition-shadow"
              >
                <p className="text-xs font-bold text-gray-400 uppercase tracking-wider">Report 1</p>
                <p className="text-sm font-bold text-gray-800 mt-1">Low Stock (WH 1)</p>
                <span className="text-xs text-blue-600 font-semibold underline mt-2 inline-block">Run View Query</span>
              </div>

              <div 
                onClick={handleRunReport2}
                className="bg-white p-5 rounded-xl shadow-sm border border-gray-100 border-t-4 border-t-purple-500 cursor-pointer hover:shadow-md transition-shadow"
              >
                <p className="text-xs font-bold text-gray-400 uppercase tracking-wider">Report 2</p>
                <p className="text-sm font-bold text-gray-800 mt-1">Outflows (WH 1)</p>
                <span className="text-xs text-blue-600 font-semibold underline mt-2 inline-block">Run View Query</span>
              </div>

              <div 
                onClick={handleRunReport3}
                className="bg-white p-5 rounded-xl shadow-sm border border-gray-100 border-t-4 border-t-green-500 cursor-pointer hover:shadow-md transition-shadow"
              >
                <p className="text-xs font-bold text-gray-400 uppercase tracking-wider">Report 3</p>
                <p className="text-sm font-bold text-gray-800 mt-1">Inflows (WH 1)</p>
                <span className="text-xs text-blue-600 font-semibold underline mt-2 inline-block">Run View Query</span>
              </div>
            </div>
           {/* activeModuleTitle  === 'Products' && <DashboardCharts productsData={tableData} />*/}
           { activeModuleTitle === 'Products' && !activeReportTitle && <DashboardCharts productsData={tableData} />}
            {/* Title & Conditional Insert Button */}
            <div className="flex justify-between items-center mb-6">
              <h1 className="text-2xl font-bold text-gray-800">
                {activeReportTitle ? `Report: ${activeReportTitle}` : `${activeModuleTitle} Management`}
              </h1>
              
              <div className="flex items-center gap-3">
  {/* Export to CSV Button - Available on all screens */}
  <button
    onClick={() => exportToCSV(filteredData, activeModuleTitle.toLowerCase())}
    className="bg-white border border-gray-300 text-gray-700 px-4 py-2 rounded-lg text-sm font-semibold shadow-sm hover:bg-gray-50 transition-colors flex items-center gap-2"
  >
    📥 Export CSV
  </button>

  {/* Your Existing New Record Button */}
  {userRole !== 'Coordinator' && (
    <button
      onClick={() => setIsModalOpen(true)}
      className="bg-blue-600 text-white px-4 py-2 rounded-lg text-sm font-semibold shadow hover:bg-blue-700 transition-colors"
    >
      + New Record
    </button>
  )}
</div>
            </div>

            {/* Data Table Container - Dynamically Uses filteredData across all modules/reports */}
            {inventoryStatus === 'loading' ? (
              <p className="text-gray-500 text-sm">Loading data from Redux store...</p>
            ) : (
              <DataTable 
                title={activeReportTitle ? activeReportTitle : activeModuleTitle} 
                data={filteredData} 
                onDelete={handleDeleteRecord} 
              />
            )}
           
          </div>
        </main>
      </div>

      <CreateModal
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        onSubmit={handleSaveRecord}
        sku={sku}
        setSku={setSku}
        name={name}
        setName={setName}
        category={category}
        setCategory={setCategory}
      />
    </div>
  );
}