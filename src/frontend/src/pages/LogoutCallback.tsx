import { useNavigate } from 'react-router-dom';
import { useEffect } from 'react';

export const LogoutCallback = () => {
    const navigate = useNavigate();

    useEffect(() => {
        navigate('/');
    }, [navigate]);

    return <p>Logging out...</p>;
};