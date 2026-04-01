import { useAuth } from 'react-oidc-context';

export const LogoutButton = () => {
    const auth = useAuth();

    const handleLogout = () => {
        auth.signoutRedirect({
            post_logout_redirect_uri: 'http://localhost:3000/authentication/logout-callback',
        });
    };

    return (
        <button onClick={handleLogout}>
            Sign Out
        </button>
    );
};