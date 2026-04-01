import { useAuth } from 'react-oidc-context';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { LoginCallback }   from './pages/LoginCallback';
import { LogoutCallback }  from './pages/LogoutCallback';
import { RegisterForm }    from './components/RegisterForm';
import { UserProfile }     from './components/UserProfile';
import { CatalogItemList } from './components/CatalogItemList';
import { InventoryList }   from './components/InventoryList';
import { LogoutButton }    from './components/LogoutButton';

function ProtectedApp() {
    const auth = useAuth();

    if (auth.isLoading)       return <p>Loading...</p>;
    if (!auth.isAuthenticated) return <Navigate to="/register" replace />;
    if (!auth.user)           return <p>Loading user data...</p>;

    return (
        <div style={{ padding: '2rem', fontFamily: 'sans-serif' }}>
            <header style={{ display: 'flex', justifyContent: 'space-between' }}>
                <h1>Play Economy</h1>
                <div>
                    <UserProfile />
                    <LogoutButton />
                </div>
            </header>
            <main>
                <CatalogItemList />
                <hr />
                <InventoryList userId={auth.user!.profile.sub} />
            </main>
        </div>
    );
}

function App() {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/authentication/login-callback"  element={<LoginCallback />} />
                <Route path="/authentication/logout-callback" element={<LogoutCallback />} />
                <Route path="/register"                       element={<RegisterForm />} />
                <Route path="/*"                              element={<ProtectedApp />} />
            </Routes>
        </BrowserRouter>
    );
}

export default App;
