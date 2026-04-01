import { useAuth } from 'react-oidc-context';
import { PlayEconomyUser } from '../auth/types';

export const usePlayUser = () => {
    const auth = useAuth();
    return {
        ...auth,
        user: auth.user as PlayEconomyUser | null,
    };
};