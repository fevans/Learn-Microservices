import { usePlayUser } from '../hooks/usePlayUser';

export const UserProfile = () => {
    const { user, isAuthenticated } = usePlayUser();

    if (!isAuthenticated || !user) return null;

    const roles = Array.isArray(user.profile.role)
        ? user.profile.role.join(', ')
        : user.profile.role ?? 'None';

    return (
        <div className="profile">
            <h3>Welcome, {user.profile.name}</h3>
            <p>Email: {user.profile.email}</p>
            <p>Roles: {roles}</p>
            <p>Gil:   {user.profile.gil ?? '0'}</p>
        </div>
    );
};