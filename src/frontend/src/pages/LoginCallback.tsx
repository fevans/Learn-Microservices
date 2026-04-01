import { useAuth } from 'react-oidc-context';
import { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';

export const LoginCallback = () => {
    const auth      = useAuth();
    const navigate  = useNavigate();

    useEffect(() => {
        if (!auth.isLoading && !auth.error) {
            navigate('/');
        }
    }, [auth.isLoading, auth.error, navigate]);

    if (auth.isLoading) return <p>Completing login...</p>;
    if (auth.error)     return <p>Login error: {auth.error.message}</p>;

    return null;
};