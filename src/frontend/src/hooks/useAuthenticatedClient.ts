import { useMemo } from 'react';
import { useAuth } from 'react-oidc-context';
import { createAuthenticatedClient } from '../api/axiosClient';

export const useAuthenticatedClient = () => {
    const auth = useAuth();

    return useMemo(
        () => createAuthenticatedClient(() => auth.user?.access_token),
        [auth.user?.access_token]
    );
};