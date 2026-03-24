import React from 'react';
import logo from './logo.svg';
import './App.css';
// frontend/src/App.tsx
import { CatalogItemList } from './components/CatalogItemsList';
import { InventoryList } from './components/InventoryList';

// Hardcoded for demo — will come from auth in Section 12
const TEST_USER_ID = '00000000-0000-0000-0000-000000000001';

function App() {
  return (
      <div style={{ padding: '2rem', fontFamily: 'sans-serif' }}>
        <h1>Play Economy</h1>
        <CatalogItemList />
        <hr />
        <InventoryList userId={TEST_USER_ID} />
      </div>
  );
}

export default App;
