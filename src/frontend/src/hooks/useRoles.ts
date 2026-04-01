import { usePlayUser } from './usePlayUser';

export const useRoles = () => {
    const { user } = usePlayUser();

    const roles = user?.profile.role
        ? Array.isArray(user.profile.role)
            ? user.profile.role
            : [user.profile.role]
        : [];

    return {
        isAdmin:  roles.includes('Admin'),
        isPlayer: roles.includes('Player'),
        roles,
    };
};