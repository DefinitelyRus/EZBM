import React, { useState } from 'react';
import { useTheme } from './theme';
import {
  FilledButton,
  FilledTonalButton,
  OutlinedButton,
  TextButton,
  Fab,
} from './components/buttons';
import {
  FilledTextField,
  OutlinedTextField,
  SearchField,
} from './components/inputs';
import {
  Headline,
  Title,
  Body,
  Label,
} from './components/typography';
import { Checkbox, Radio, Switch, Slider } from './components/controls';
import {
  ElevatedCard,
  FilledCard,
  OutlinedCard,
  Divider,
  Badge,
  Avatar,
} from './components/surfaces';
import {
  CircularProgress,
  LinearProgress,
  Snackbar,
  Dialog,
} from './components/feedback';
import {
  LayoutDashboard,
  Package,
  ShoppingCart,
  Users,
  Settings,
  Bell,
  Moon,
  Sun,
  Plus,
  TrendingUp,
  CheckCircle2,
  DollarSign,
  AlertTriangle,
} from 'lucide-react';
import styles from './SamplePortal.module.css';

export function App() {
  const { resolvedTheme, toggleTheme } = useTheme();

  // Navigation State
  const [activeNav, setActiveNav] = useState('dashboard');

  // Search and Input State
  const [searchQuery, setSearchQuery] = useState('');
  const [productName, setProductName] = useState('');
  const [productPrice, setProductPrice] = useState('');
  const [priceError, setPriceError] = useState(false);

  // Settings & Toggles
  const [autoRestock, setAutoRestock] = useState(true);
  const [notificationsEnabled, setNotificationsEnabled] = useState(true);
  const [shippingMethod, setShippingMethod] = useState<'standard' | 'express'>('standard');
  const [safetyMargin, setSafetyMargin] = useState(35);
  const [termsAgreed, setTermsAgreed] = useState(false);

  // Overlays
  const [newOrderModal, setNewOrderModal] = useState(false);
  const [snackbarOpen, setSnackbarOpen] = useState(false);
  const [snackbarMessage, setSnackbarMessage] = useState('');

  // Sample items
  const recentOrders = [
    { id: '#EZ-1094', customer: 'Sarah Connor', amount: '$340.50', status: 'Completed', avatar: 'SC' },
    { id: '#EZ-1095', customer: 'Alex Murphy', amount: '$120.00', status: 'In Transit', avatar: 'AM' },
    { id: '#EZ-1096', customer: 'Ellen Ripley', amount: '$890.20', status: 'Processing', avatar: 'ER' },
  ];

  const handlePriceChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const val = e.target.value;
    setProductPrice(val);
    setPriceError(isNaN(Number(val)) && val.trim() !== '');
  };

  const handleCreateProduct = () => {
    if (!productName) {
      setSnackbarMessage('Please provide a product title before saving.');
      setSnackbarOpen(true);
      return;
    }
    setSnackbarMessage(`Product "${productName}" saved to inventory.`);
    setSnackbarOpen(true);
    setProductName('');
    setProductPrice('');
    setNewOrderModal(false);
  };

  return (
    <div className={styles.appLayout}>
      {/* 1. Sidebar Navigation */}
      <aside className={styles.sidebar}>
        <div className={styles.brand}>
          <div className={styles.brandLogo}>EZ</div>
          <div>
            <Title size="medium">EZBM Portal</Title>
            <Label size="small" style={{ color: 'var(--md-sys-color-on-surface-variant)' }}>
              Business Suite
            </Label>
          </div>
        </div>

        <nav className={styles.navSection}>
          <button
            type="button"
            className={`${styles.navItem} ${activeNav === 'dashboard' ? styles.navItemActive : ''}`}
            onClick={() => setActiveNav('dashboard')}
          >
            <LayoutDashboard size={20} />
            <span>Dashboard</span>
          </button>
          <button
            type="button"
            className={`${styles.navItem} ${activeNav === 'inventory' ? styles.navItemActive : ''}`}
            onClick={() => setActiveNav('inventory')}
          >
            <Package size={20} />
            <span>Inventory</span>
          </button>
          <button
            type="button"
            className={`${styles.navItem} ${activeNav === 'orders' ? styles.navItemActive : ''}`}
            onClick={() => setActiveNav('orders')}
          >
            <ShoppingCart size={20} />
            <span>Orders</span>
          </button>
          <button
            type="button"
            className={`${styles.navItem} ${activeNav === 'staff' ? styles.navItemActive : ''}`}
            onClick={() => setActiveNav('staff')}
          >
            <Users size={20} />
            <span>Staff</span>
          </button>
          <button
            type="button"
            className={`${styles.navItem} ${activeNav === 'settings' ? styles.navItemActive : ''}`}
            onClick={() => setActiveNav('settings')}
          >
            <Settings size={20} />
            <span>Settings</span>
          </button>
        </nav>

        <Divider />

        <div style={{ display: 'flex', alignItems: 'center', gap: 12 }}>
          <Avatar
            src="https://images.unsplash.com/photo-1494790108377-be9c29b29330?w=100&auto=format&fit=crop&q=80"
            alt="Elena Rostova"
          />
          <div style={{ flex: 1, minWidth: 0 }}>
            <Label size="large" style={{ display: 'block', overflow: 'hidden', textOverflow: 'ellipsis' }}>
              Elena Rostova
            </Label>
            <Label size="small" style={{ color: 'var(--md-sys-color-on-surface-variant)' }}>
              Store Manager
            </Label>
          </div>
        </div>
      </aside>

      {/* 2. Main Content Area */}
      <div className={styles.mainWrapper}>
        {/* Top Bar */}
        <header className={styles.topBar}>
          <SearchField
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
            onClear={() => setSearchQuery('')}
            placeholder="Search orders, inventory or staff..."
          />

          <div className={styles.topBarRight}>
            <Badge value={3}>
              <FilledTonalButton
                icon={<Bell size={18} />}
                onClick={() => {
                  setSnackbarMessage('You have 3 unread business alerts.');
                  setSnackbarOpen(true);
                }}
              >
                Alerts
              </FilledTonalButton>
            </Badge>

            <OutlinedButton
              icon={resolvedTheme === 'light' ? <Moon size={18} /> : <Sun size={18} />}
              onClick={toggleTheme}
            >
              {resolvedTheme === 'light' ? 'Dark' : 'Light'}
            </OutlinedButton>
          </div>
        </header>

        {/* Content View */}
        <main className={styles.content}>
          <div>
            <Headline size="medium">Store Overview</Headline>
            <Body size="medium" style={{ color: 'var(--md-sys-color-on-surface-variant)' }}>
              Live real-time operational status and metrics.
            </Body>
          </div>

          {/* Metric Cards */}
          <div className={styles.metricsGrid}>
            <ElevatedCard className={styles.metricCard}>
              <div className={styles.metricHeader}>
                <Label size="large" style={{ color: 'var(--md-sys-color-on-surface-variant)' }}>
                  Total Revenue
                </Label>
                <DollarSign size={20} color="var(--md-sys-color-primary)" />
              </div>
              <div className={styles.metricValue}>
                <Headline size="small">$48,250</Headline>
                <Label size="small" style={{ color: '#107c41' }}>
                  +12.4%
                </Label>
              </div>
              <LinearProgress value={78} />
              <Label size="small" style={{ color: 'var(--md-sys-color-on-surface-variant)' }}>
                78% towards monthly target
              </Label>
            </ElevatedCard>

            <FilledCard className={styles.metricCard}>
              <div className={styles.metricHeader}>
                <Label size="large" style={{ color: 'var(--md-sys-color-on-surface-variant)' }}>
                  Active Orders
                </Label>
                <TrendingUp size={20} color="var(--md-sys-color-secondary)" />
              </div>
              <div className={styles.metricValue}>
                <Headline size="small">128</Headline>
                <Label size="small" style={{ color: 'var(--md-sys-color-on-surface-variant)' }}>
                  in fulfillment
                </Label>
              </div>
              <div style={{ display: 'flex', alignItems: 'center', gap: 12, marginTop: 4 }}>
                <CircularProgress size={32} strokeWidth={3} value={65} />
                <Label size="medium">65% dispatched today</Label>
              </div>
            </FilledCard>

            <OutlinedCard className={styles.metricCard}>
              <div className={styles.metricHeader}>
                <Label size="large" style={{ color: 'var(--md-sys-color-on-surface-variant)' }}>
                  Inventory Health
                </Label>
                <CheckCircle2 size={20} color="var(--md-sys-color-tertiary)" />
              </div>
              <div className={styles.metricValue}>
                <Headline size="small">94.8%</Headline>
                <Label size="small" style={{ color: '#107c41' }}>Optimal</Label>
              </div>
              <LinearProgress value={95} />
              <Label size="small" style={{ color: 'var(--md-sys-color-on-surface-variant)' }}>
                14 items below reorder threshold
              </Label>
            </OutlinedCard>
          </div>

          {/* Two-column operational section */}
          <div className={styles.workspaceGrid}>
            {/* Recent Orders List Card */}
            <ElevatedCard className={styles.tableCard}>
              <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                <Title size="large">Recent Transactions</Title>
                <TextButton onClick={() => setActiveNav('orders')}>View all</TextButton>
              </div>
              <Divider />

              {recentOrders.map((order) => (
                <div key={order.id} className={styles.orderRow}>
                  <div className={styles.orderInfo}>
                    <Avatar initials={order.avatar} size={36} />
                    <div>
                      <Body size="medium" style={{ fontWeight: 500 }}>
                        {order.customer}
                      </Body>
                      <Label size="small" style={{ color: 'var(--md-sys-color-on-surface-variant)' }}>
                        {order.id}
                      </Label>
                    </div>
                  </div>
                  <div style={{ textAlign: 'right' }}>
                    <Body size="medium" style={{ fontWeight: 600 }}>
                      {order.amount}
                    </Body>
                    <Label size="small" style={{ color: 'var(--md-sys-color-primary)' }}>
                      {order.status}
                    </Label>
                  </div>
                </div>
              ))}
            </ElevatedCard>

            {/* Quick Actions & Store Settings */}
            <OutlinedCard className={styles.settingsCard}>
              <Title size="large">Operational Settings</Title>
              <Divider />

              <div className={styles.controlRow}>
                <div>
                  <Label size="large">Auto-restock threshold</Label>
                  <Body size="small" style={{ color: 'var(--md-sys-color-on-surface-variant)' }}>
                    Trigger POs when stock drops
                  </Body>
                </div>
                <Switch checked={autoRestock} onChange={(e) => setAutoRestock(e.target.checked)} />
              </div>

              <div className={styles.controlRow}>
                <div>
                  <Label size="large">Low-stock notifications</Label>
                  <Body size="small" style={{ color: 'var(--md-sys-color-on-surface-variant)' }}>
                    Notify management via email
                  </Body>
                </div>
                <Switch
                  checked={notificationsEnabled}
                  onChange={(e) => setNotificationsEnabled(e.target.checked)}
                />
              </div>

              <div>
                <Label size="large">Fulfillment Safety Buffer ({safetyMargin}%)</Label>
                <div style={{ marginTop: 8 }}>
                  <Slider
                    value={safetyMargin}
                    min={10}
                    max={60}
                    onChange={(val) => setSafetyMargin(val)}
                  />
                </div>
              </div>

              <Divider />

              <div>
                <Label size="large" style={{ display: 'block', marginBottom: 8 }}>
                  Default Shipping Dispatch
                </Label>
                <div style={{ display: 'flex', flexDirection: 'column', gap: 8 }}>
                  <Radio
                    label="Standard Cargo Dispatch (2-3 business days)"
                    checked={shippingMethod === 'standard'}
                    onChange={() => setShippingMethod('standard')}
                  />
                  <Radio
                    label="Express Courier Fulfillment (Same day)"
                    checked={shippingMethod === 'express'}
                    onChange={() => setShippingMethod('express')}
                  />
                </div>
              </div>
            </OutlinedCard>
          </div>
        </main>

        {/* 3. Floating Action Button for Quick Creation */}
        <div className={styles.fabAnchor}>
          <Fab
            icon={<Plus size={24} />}
            label="New Item"
            onClick={() => setNewOrderModal(true)}
          />
        </div>
      </div>

      {/* 4. Dialog Modal: Add Product to Catalog */}
      <Dialog
        open={newOrderModal}
        onClose={() => setNewOrderModal(false)}
        title="Add Inventory Item"
        actions={
          <>
            <TextButton onClick={() => setNewOrderModal(false)}>Cancel</TextButton>
            <FilledButton onClick={handleCreateProduct}>Create Item</FilledButton>
          </>
        }
      >
        <div style={{ display: 'flex', flexDirection: 'column', gap: 16, marginTop: 8 }}>
          <Body size="medium">
            Register a new product in the store database. All fields adhere to standard M3 inputs.
          </Body>

          <FilledTextField
            label="Product Name"
            placeholder="e.g. Ergonomic Office Desk"
            value={productName}
            onChange={(e) => setProductName(e.target.value)}
            supportingText="Name as displayed to customers"
          />

          <OutlinedTextField
            label="Unit Price ($)"
            placeholder="0.00"
            value={productPrice}
            onChange={handlePriceChange}
            error={priceError}
            supportingText={priceError ? 'Must be a valid numeric amount' : 'Retail selling price'}
          />

          <Checkbox
            label="Confirm catalogue safety verification"
            checked={termsAgreed}
            onChange={(e) => setTermsAgreed(e.target.checked)}
          />

          {priceError && (
            <div style={{ display: 'flex', alignItems: 'center', gap: 8, color: 'var(--md-sys-color-error)' }}>
              <AlertTriangle size={16} />
              <Label size="small">Invalid numeric entry detected in price.</Label>
            </div>
          )}
        </div>
      </Dialog>

      {/* 5. Snackbar Feedback */}
      <Snackbar
        open={snackbarOpen}
        message={snackbarMessage}
        actionLabel="Dismiss"
        onAction={() => setSnackbarOpen(false)}
      />
    </div>
  );
}

export default App;
